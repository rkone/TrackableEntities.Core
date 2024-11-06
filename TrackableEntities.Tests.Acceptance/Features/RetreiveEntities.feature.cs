﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace TrackableEntities.Tests.Acceptance.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class RetreiveEntitiesFeature : object, Xunit.IClassFixture<RetreiveEntitiesFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "RetreiveEntities.feature"
#line hidden
        
        public RetreiveEntitiesFeature(RetreiveEntitiesFeature.FixtureData fixtureData, TrackableEntities_Tests_Acceptance_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Retreive Entities", "\tIn order to retrieve entities\r\n\tAs a Web API client\r\n\tI want to retrieve entitie" +
                    "s from the database", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Retreive Customers")]
        [Xunit.TraitAttribute("FeatureTitle", "Retreive Entities")]
        [Xunit.TraitAttribute("Description", "Retreive Customers")]
        [Xunit.TraitAttribute("Category", "retrieve_entities")]
        public void RetreiveCustomers()
        {
            string[] tagsOfScenario = new string[] {
                    "retrieve_entities"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retreive Customers", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "CustomerId",
                            "CustomerName"});
                table4.AddRow(new string[] {
                            "ABCD",
                            "Test Customer ABCD"});
                table4.AddRow(new string[] {
                            "EFGH",
                            "Test Customer EFGH"});
#line 8
 testRunner.Given("the following customers", ((string)(null)), table4, "Given ");
#line hidden
#line 12
 testRunner.When("I submit a GET request for customers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 13
 testRunner.Then("the request should return the customers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Retreive Customer Orders")]
        [Xunit.TraitAttribute("FeatureTitle", "Retreive Entities")]
        [Xunit.TraitAttribute("Description", "Retreive Customer Orders")]
        [Xunit.TraitAttribute("Category", "retrieve_entities")]
        public void RetreiveCustomerOrders()
        {
            string[] tagsOfScenario = new string[] {
                    "retrieve_entities"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retreive Customer Orders", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 16
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "CustomerId",
                            "CustomerName"});
                table5.AddRow(new string[] {
                            "ABCD",
                            "Test Customer ABCD"});
#line 17
 testRunner.Given("the following customers", ((string)(null)), table5, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "CustomerId"});
                table6.AddRow(new string[] {
                            "ABCD"});
#line 20
 testRunner.And("the following customer orders", ((string)(null)), table6, "And ");
#line hidden
#line 23
 testRunner.When("I submit a GET request for customer orders", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 24
 testRunner.Then("the request should return the orders", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Retreive Order")]
        [Xunit.TraitAttribute("FeatureTitle", "Retreive Entities")]
        [Xunit.TraitAttribute("Description", "Retreive Order")]
        [Xunit.TraitAttribute("Category", "retrieve_entities")]
        public void RetreiveOrder()
        {
            string[] tagsOfScenario = new string[] {
                    "retrieve_entities"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retreive Order", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 27
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "CustomerId",
                            "CustomerName"});
                table7.AddRow(new string[] {
                            "ABCD",
                            "Test Customer ABCD"});
#line 28
 testRunner.Given("the following customers", ((string)(null)), table7, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "CustomerId"});
                table8.AddRow(new string[] {
                            "ABCD"});
#line 31
 testRunner.And("the following customer orders", ((string)(null)), table8, "And ");
#line hidden
#line 34
 testRunner.When("I submit a GET request for an order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 35
 testRunner.Then("the request should return the orders", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                RetreiveEntitiesFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                RetreiveEntitiesFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
