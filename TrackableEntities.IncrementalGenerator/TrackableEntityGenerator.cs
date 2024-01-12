using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace TrackableEntities.IncrementalGenerator;

[Generator(LanguageNames.CSharp)]
public class TrackableEntityGenerator : IIncrementalGenerator
{
    private const string attributeText = @"#nullable enable
using System;
[AttributeUsage(AttributeTargets.Class)]
[System.Diagnostics.Conditional(""TrackableEntityGenerator_DEBUG"")]
internal sealed class TrackableEntityAttribute : Attribute
{
    public string[]? UsingDirectives { get; }  
    public TrackableEntityAttribute(params string[] usingDirectives) 
        => this.UsingDirectives = usingDirectives;
}
[AttributeUsage(AttributeTargets.Property)]
[System.Diagnostics.Conditional(""TrackableEntityGenerator_DEBUG"")]
internal sealed class TrackableEntityTrackedPropertyAttribute : Attribute
{
    public bool IsManyToMany { get; }
    public TrackableEntityTrackedPropertyAttribute(bool isManyToMany = false)
        => this.IsManyToMany = isManyToMany;
}
[AttributeUsage(AttributeTargets.Property)]
[System.Diagnostics.Conditional(""TrackableEntityGenerator_DEBUG"")]
internal sealed class TrackableEntityPropertyIgnoreAttribute : Attribute
{
    public TrackableEntityPropertyIgnoreAttribute()
    {
    }
}
[AttributeUsage(AttributeTargets.Class)]
[System.Diagnostics.Conditional(""TrackableEntityGenerator_DEBUG"")]
internal sealed class TrackableEntityCopyAttribute : Attribute
{
    public string[]? UsingDirectives { get; }  
    public TrackableEntityCopyAttribute(params string[] usingDirectives)
        => this.UsingDirectives = usingDirectives;
}";
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    // determine the namespace the class/enum/struct is declared in, if any
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
                potentialNamespaceParent is not NamespaceDeclarationSyntax
                && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, string attributeName)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }
                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the attributeName attribute?
                if (fullName == attributeName)
                {
                    // return the enum
                    return classDeclarationSyntax;
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (!Debugger.IsAttached)
        {
            //Debugger.Launch();
        }

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ClientTrackableEntitiesAttributes.g.cs", SourceText.From(attributeText, Encoding.UTF8)));

        IncrementalValuesProvider<ClassDeclarationSyntax> trackedClassDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select classes with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx, "TrackableEntityAttribute")) // select the class with the [TrackableEntity] attribute
            .Where(static m => m is not null)!; // filter out attributed classes that we don't care about

        IncrementalValuesProvider<ClassDeclarationSyntax> unTrackedClassDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select classes with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx, "TrackableEntityCopyAttribute")) // select the class with the [TrackableEntityCopy] attribute
            .Where(static m => m is not null)!;

        // Combine the selected classes with the `Compilation`
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndTrackedClasses
            = context.CompilationProvider.Combine(trackedClassDeclarations.Collect());

        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndUnTrackedClasses
            = context.CompilationProvider.Combine(unTrackedClassDeclarations.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(compilationAndTrackedClasses,
            static (spc, source) => ExecuteTrackedGeneration(source.Item1, source.Item2, spc));

        context.RegisterSourceOutput(compilationAndUnTrackedClasses,
            static (spc, source) => ExecuteUnTrackedGeneration(source.Item1, source.Item2, spc));
    }

    private static void ExecuteTrackedGeneration(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }
        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();

        // Convert each ClassDeclarationSyntax to a ClientEntityToGenerate
        var baseNameSpace = GetNamespace(distinctClasses.First());
        //var useShared = distinctClasses.Any(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.ArgumentList?.Arguments.Count > 0 && a.ArgumentList.Arguments[0].ToString() == "true")));
        var usings = GetUsingDirectives("TrackableEntity", distinctClasses);
        List<ClientEntityToGenerate> entitiesToGenerate = GetTypesToGenerate(compilation, distinctClasses, usings.Contains("Newtonsoft.Json"), usings.Contains("System.Text.Json.Serialization"), context.CancellationToken);

        // If there were errors in the ClassDeclarationSyntax, we won't create an
        // ClientEntityToGenerate for it, so make sure we have something to generate
        if (entitiesToGenerate.Count > 0)
        {
            StringBuilder stringBuilder = new();
            // generate the source code and add it to the output
            foreach (var entity in entitiesToGenerate)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                stringBuilder.Append(GenerateClientEntity(entity));
            }
            string result = AddTrackedHeaders(stringBuilder.ToString(), baseNameSpace, usings);
            context.AddSource("ClientTrackableEntities.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    //compile all using directives for the attribute
    private static IEnumerable<string> GetUsingDirectives(string attributeName, IEnumerable<ClassDeclarationSyntax> source)
    {
        List<string> usings = new();
        foreach (var classSyntax in source)
        {
            foreach (var attributeList in classSyntax.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (attribute.Name.ToString() == attributeName && attribute.ArgumentList is not null)
                    {
                        foreach (var argument in attribute.ArgumentList.Arguments)
                        {
                            usings.Add(argument.ToString().Replace("\"", ""));
                        }
                    }
                }
            }
        }
        return usings.Distinct();
    }

    private static void ExecuteUnTrackedGeneration(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }
        IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();
        var baseNameSpace = GetNamespace(distinctClasses.First());
        var usings = GetUsingDirectives("TrackableEntityCopy", distinctClasses);
        List<ClientEntityToGenerate> entitiesToGenerate = GetTypesToGenerate(compilation, distinctClasses, false, false, context.CancellationToken);

        if (entitiesToGenerate.Count > 0)
        {
            StringBuilder stringBuilder = new();
            foreach (var entity in entitiesToGenerate)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                stringBuilder.Append(CopyClientEntity(entity));
            }
            string result = AddUnTrackedHeaders(stringBuilder.ToString(), baseNameSpace, usings);
            context.AddSource("ClientUnTrackedEntities.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static List<ClientEntityToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classDeclarationSyntaxes, bool useNewtonsoftJson, bool useSytemTextJson, CancellationToken cancellationToken)
    {
        var entities = new List<ClientEntityToGenerate>();
        INamedTypeSymbol? classAttribute = compilation.GetTypeByMetadataName("TrackableEntityAttribute");
        if (classAttribute is null) return entities;

        foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol) continue;

            var className = classDeclarationSyntax.Identifier.Text;
            var modelOverride = false;
            var properties = new List<ClientEntityProperty>();
            foreach (var member in classDeclarationSyntax.Members)
            {
                if (member is not PropertyDeclarationSyntax property) continue;
                if (property.Identifier.Text is "TrackingState" or "ModifiedProperties" or "EntityIdentifier") continue;
                var tracked = false;
                var manyToMany = false;
                if (property.AttributeLists.Count > 0)
                {
                    var ignored = false;
                    foreach (var attribute in property.AttributeLists)
                    {
                        if (!attribute.Attributes.Any()) continue;
                        var name = attribute.Attributes[0].Name.ToString();
                        if (name == "TrackableEntityPropertyIgnore")
                            ignored = true;
                        if (name == "TrackableEntityTrackedProperty")
                        {
                            tracked = true;
                            if (attribute.Attributes[0].ArgumentList?.Arguments.ToString() == "true")
                                manyToMany = true;
                        }
                    }
                    if (ignored) continue;
                }
                modelOverride |= tracked;
                var setter = (property.AccessorList?.Accessors.Count ?? 0) == 2;
                var genericType = property.Type as GenericNameSyntax;
                var collection = false;
                var baseType = property.Type.ToString().TrimEnd('?');
                if (genericType is not null)
                {
                    collection = new[] { "List", "ICollection", "IEnumerable" }.Contains(genericType.Identifier.Text);
                    baseType = genericType.TypeArgumentList.Arguments[0].ToString();
                }
                var initializer = property.Initializer?.Value.ToFullString();
                bool nullable = collection ? !tracked : property.Type is NullableTypeSyntax;
                var jsonIgnored = genericType is not null || baseType is not "DateTime" && property.Type is not PredefinedTypeSyntax && property.Type is NullableTypeSyntax pType && pType.ElementType is not PredefinedTypeSyntax;

                properties.Add(new(property.Identifier.Text, baseType, nullable, initializer, collection, tracked, setter, useNewtonsoftJson, useSytemTextJson, jsonIgnored, manyToMany));
            }
            entities.Add(new ClientEntityToGenerate(className, modelOverride, properties));
        }

        return entities;
    }

    private static string AddTrackedHeaders(string body, string hostNamespace, IEnumerable<string> namespaces)
    {
        StringBuilder res = new();
        res.Append(@"#nullable enable
#if USECLIENTENTITIES
using TrackableEntities.Client.Core;
");

        foreach (var ns in namespaces ?? Array.Empty<string>())
        {
            res.AppendLine($"using {ns};");
        }
        res.Append($@"
//Instructions: This file is auto-generated. Find the source in the {hostNamespace} project under:
//Generated\TrackableEntities.IncrementalGenerator\TrackableEntities.IncrementalGenerator.TrackableEntityGenerator\ClientTrackableEntities.g.cs

{(hostNamespace == string.Empty ? string.Empty : $"namespace {hostNamespace}.Client;")}
public partial class ClientBase : EntityBase 
{{
    protected partial void OnPropertySet(string propertyName, Type propertyType, object? value);
}}
public partial interface IClientBase {{}}
{body}
#endif");
        return res.ToString();
    }


    private static string AddUnTrackedHeaders(string body, string hostNamespace, IEnumerable<string> namespaces)
    {
        StringBuilder res = new();
        res.Append(@"#nullable enable
#if USECLIENTENTITIES
");
        foreach (var ns in namespaces ?? Array.Empty<string>())
        {
            res.AppendLine($"using {ns};");
        }
        res.Append($@"
//Instructions: This file is auto-generated. Find the source in the {hostNamespace} project under:
//Generated\TrackableEntities.IncrementalGenerator\TrackableEntities.IncrementalGenerator.TrackableEntityGenerator\ClientUnTrackedEntities.g.cs

{(hostNamespace == string.Empty ? string.Empty : $"namespace {hostNamespace}.Client;")}
{body}
#endif
");
        return res.ToString();
    }

    private static StringBuilder GenerateClientEntity(ClientEntityToGenerate entity)
    {
        var manyToManyProperties = new List<string>();
        var sourcebuilder = new StringBuilder(@$"public partial class {entity.ClassName} : ClientBase, IClientBase
{{
");
        foreach (var prop in entity.Properties)
        {
            var n = prop.Nullable ? "?" : string.Empty;
            if (prop.Tracked)
            {
                if (prop.Collection)
                {
                    sourcebuilder.AppendLine($@"    private ChangeTrackingCollection<{prop.BaseType}> _{prop.Name} = new();
    public ChangeTrackingCollection<{prop.BaseType}> {prop.Name}
    {{ 
        get => _{prop.Name}; 
        set
        {{");

                    if (prop.ManyToMany)
                    {
                        sourcebuilder.AppendLine($@"            value.Parent = this;");
                        manyToManyProperties.Add(prop.Name);
                    }
                    sourcebuilder.AppendLine($@"            if (Equals(_{prop.Name}, value)) return;
            _{prop.Name} = value;
            NotifyPropertyChanged();
        }}
    }}");
                }
                else
                {
                    sourcebuilder.AppendLine($@"    private ChangeTrackingCollection<{prop.BaseType}>? {prop.Name}ChangeTracker {{ get; set; }}
    private {prop.BaseType}? _{prop.Name};
    public {prop.BaseType}? {prop.Name} 
    {{ 
        get => _{prop.Name};
        set
        {{
            if (Equals(value, _{prop.Name})) return;
            _{prop.Name} = value;
            {prop.Name}ChangeTracker = _{prop.Name} == null ? null : new ChangeTrackingCollection<{prop.BaseType}> {{ _{prop.Name} }};
            NotifyPropertyChanged();
            OnPropertySet(nameof({prop.Name}), typeof({prop.BaseType}), value);
        }}
    }}");
                }
            }
            else
            {
                if ((prop.JsonIgnored || !prop.Setter) && (prop.UseNewtonsoftJson || prop.UseSystemTextJson))
                {
                    if (prop.UseNewtonsoftJson && prop.UseSystemTextJson)
                    {
                        sourcebuilder.AppendLine("    [System.Text.Json.Serialization.JsonIgnore]");
                        sourcebuilder.AppendLine("    [Newtonsoft.Json.JsonIgnore]");
                    }
                    else
                        sourcebuilder.AppendLine("    [JsonIgnore]");
                }
                if (prop.Collection)
                    sourcebuilder.AppendLine($"    public ICollection<{prop.BaseType}>? {prop.Name} {{ get; {(prop.Setter ? "set; " : string.Empty)}}}");
                else
                {
                    sourcebuilder.AppendLine($@"    public {prop.BaseType}{n} {prop.Name}
    {{ 
        get => _{prop.Name};{(prop.Setter ? $@"
        set
        {{
            if (Equals(_{prop.Name}, value)) return;
            _{prop.Name} = value;
            NotifyPropertyChanged();
            OnPropertySet(nameof({prop.Name}), typeof({prop.BaseType}), value);
        }}" : string.Empty)}
    }}
    private {prop.BaseType}{n} _{prop.Name}{(prop.Initializer is null ? string.Empty : $" = {prop.Initializer}")};");
                }
            }
        }
        if (manyToManyProperties.Count > 0)
        {
            sourcebuilder.AppendLine($@"    public {entity.ClassName}() 
    {{");
            foreach (var prop in manyToManyProperties)
            {
                sourcebuilder.AppendLine($"        _{prop}.Parent = this;");
            }
            sourcebuilder.AppendLine("    }");
        }
        sourcebuilder.AppendLine("}");
        return sourcebuilder;
    }

    private static StringBuilder CopyClientEntity(ClientEntityToGenerate entity)
    {
        var sourcebuilder = new StringBuilder(@$"
public partial class {entity.ClassName}
{{
");
        foreach (var prop in entity.Properties)
        {
            var n = prop.Nullable ? "?" : string.Empty;
            if (prop.Collection)
                sourcebuilder.AppendLine($"    public ICollection<{prop.BaseType}>? {prop.Name} {{ get; {(prop.Setter ? "set; " : string.Empty)}}}");
            else
                sourcebuilder.AppendLine($@"    public {prop.BaseType}{n} {prop.Name} {{ get; {(prop.Setter ? "set; " : string.Empty)}}}{(prop.Initializer is null ? string.Empty : $" = {prop.Initializer};")}");
        }
        sourcebuilder.AppendLine("}");
        return sourcebuilder;
    }
}
