# What is this Fork?

This fork updates all of the original Trackable Entities projects to the latest versions of .Net.

The client no longer has a dependency on Newtonsoft, all serialization is done with System.Text.Json.

EF Core [skip navigations](https://learn.microsoft.com/en-us/ef/core/change-tracking/relationship-changes#skip-navigations), used with Many to Many relationships, are supported.

Instead of T4 Templates, an Incremental Generator project is used with the idea that implementors would clone the project and make changes to suit their own needs.

All unit testing has also been migrated over as well to test any changes.


# Trackable Entities for Entity Framework Core

Change-tracking across service boundaries with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) Web API and [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/).

## What is Trackable Entities?

[Trackable Entities](http://trackableentities.github.io/) allows you to mark client-side entities as Added, Modified or Deleted, so that _entire object graphs_ can be sent to a service where changes can be saved with a single round trip to the server and within a single transaction.

## Installation

Trackable Entities for EF Core is available as a NuGet package that can be installed in an ASP.NET Core Web API project that uses Entity Framework Core.

You can use the [Package Manager UI or Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console) in Visual Studio to install the TE package.

```bash
install-package TrackableEntities.EF.Core
```

You can also use the [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) to install the TE package.

```bash
dotnet add package TrackableEntities.EF.Core
```

## Trackable Entities Interfaces

The way Trackable Entities allows change-tracking across service boundaries is by adding a `TrackingState` property to each entity. `TrackingState` is a simple enum with values for `Unchanged`, `Added`, `Modified` and `Deleted`.  The TE library will traverse objects graphs to read this property and inform EF Core of each entity state, so that you can save all the changes at once, wrapping them in a single transaction that will roll back if any individual update fails.

`TrackingState` is a member of the `ITrackable` interface, which also includes a `ModifiedProperties` with the names of all entity properties that have been modified, so that EF Core can perform partial entity updates.

In order for clients to merge changed entities back into client-side object graphs, TE includes an `IMergeable` interface that has an `EntityIdentifier` property for correlating updated entities with original entities on the client.  The reason for returning updated entities to the client is to transmit database-generated values for things like primary keys or concurrency tokens.

Each tracked entity needs to implement `ITrackable`, as well as `IMergeable`, either directly or in a base class. In addition, properties of these interfaces should be decorated with a `[NotMapped]` attribute so that EF Core will not attempt to save these to the database.  For example, a `Product` entity might look like this:

```csharp
public class Product : ITrackable, IMergeable
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
    public List<Territory> Territories { get; set;}
    public Location Location { get; set; }

    [NotMapped]
    public TrackingState TrackingState { get; set; }

    [NotMapped]
    public ICollection<string> ModifiedProperties { get; set; }

    [NotMapped]
    public Guid EntityIdentifier { get; set; }
}
```

Server-side trackable entities can either be writen by hand or generated from an existing database using code-generation techniques. 

## Client Entity Generation

This fork of Trackable Entities has an incremental generator that can create client entities. In your project containing the server-side trackable entities, add a reference to the TackableEntities.IncrementalGenerator project.  Then modify your project file, adding in the following:

```
 <PropertyGroup>
   <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
   <CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
 </PropertyGroup>
 <ItemGroup>
  <Compile Remove="Generated\TrackableEntities.IncrementalGenerator\TrackableEntities.IncrementalGenerator.TrackableEntityGenerator\ClientTrackableEntitiesAttributes.g.cs" />
 </ItemGroup>
```

Also, for the TrackableEntities.IncrementalGenerator Project reference, Add OutputItemType="Analyzer" ReferenceOutputAssembly="false" so it looks like this:
```
   <ProjectReference Include="..\TrackableEntities.IncrementalGenerator\TrackableEntities.IncrementalGenerator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```

I'd suggest creating a client entity project, but you can add the generated client entities directly to your client project.  From this project, create link to the Generated\TrackableEntities.IncrementalGenerate\TrackableEntities.IncrementalGenrater.TrackableEntityGenerator\ClientTrackableEntities.gs file.  Also, edit the project propertes and add USECLIENTENTITIES conditional compilation symbols for both Debug and Release builds.

With this implementation, you also need to create another class file with the following:
```csharp
public partial class ClientBase
{
    protected partial void OnPropertySet(string propertyName, Type propertyType, object? value) { }
}
```

Finally, you can apply attributes to your server side trackable entities to have the client entities created.
```csharp
[TrackableEntity]
public class Product : ITrackable, IMergeable
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? UnitPrice { get; set; }
    [TrackableEntityTrackedProperty]
    public List<Territory> Territories { get; set;}
    [TrackableEntityTrackedProperty]
    public Location Location { get; set; }

    [NotMapped]
    public TrackingState TrackingState { get; set; }

    [NotMapped]
    public ICollection<string> ModifiedProperties { get; set; }

    [NotMapped]
    public Guid EntityIdentifier { get; set; }
}
```

## Usage

For an example of how to use Trackable Entities for EF Core in an ASP.NET Core Web API, have a look at [OrderContoller](https://github.com/TrackableEntities/TrackableEntities.Core.Sample/blob/master/NetCoreSample.Web/Controllers/OrderController.cs) in the sample app, which includes GET, POST, PUT and DELETE actions.  GET actions don't inlude any code that uses Trackable Entities, but the other actions set `TrackingState` before calling `ApplyChanges` on the `DbContext` class and then saving changes.

```csharp
// Set state to added
order.TrackingState = TrackingState.Added;

// Apply changes to context
_context.ApplyChanges(order);

// Persist changes
await _context.SaveChangesAsync();
```

After saving changes, `LoadRelatedEntitiesAsync` is called in order to populate reference properties for foreign keys that have been set. This is required, for example, in order to set the `Customer` property of an `Order` that has been added with a specified `CustomerId`. This way the client can create a new `Order` without populating the `Customer` property, which results in a smaller payload when sending the a new order to the Web API.  Loading related entities then populates the ` Customer` property before returning the added order to the client.

```csharp
// Populate reference properties
await _context.LoadRelatedEntitiesAsync(order);
```

Lastly, you should call `AcceptChanges` to reset `TrackingState` on each entity in the object graph before returning it to the client, so that the client can then make changes to the object and send it back to the Web API for persisting those changes.

```csharp
// Reset tracking state to unchanged
_context.AcceptChanges(order);
```

## Questions and Feedback

If you have any questions about [Trackable Entities](http://trackableentities.github.io/), would like to request features, or discover any bugs, please create an [issue](https://github.com/TrackableEntities/TrackableEntities.Core/issues) on the GitHub repository.  If you wish to [contribute](http://trackableentities.github.io/6-contributing.html) to Trackable Entities, pull [requests](https://help.github.com/articles/about-pull-requests/) are welcome!
