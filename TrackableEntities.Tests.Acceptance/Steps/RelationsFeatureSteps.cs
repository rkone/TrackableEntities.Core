using TechTalk.SpecFlow;
using TrackableEntities.Client.Core;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.FamilyModels.Client;
using TrackableEntities.Tests.Acceptance.Helpers;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps;

[Binding]
public class RelationsFeatureSteps : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private HttpClient _client = default!;
    private readonly CustomWebApplicationFactory<Program> _factory = default!;
    private readonly ScenarioContext _scenarioContext;

    public RelationsFeatureSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory)
    {
        _scenarioContext = scenarioContext;
        _factory = factory;
    }

    [BeforeScenario]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [AfterScenario]
    public void TearDown()
    {
        _client.Dispose();
    }

    [Given(@"the following new customer")]
    public void GivenTheFollowingNewCustomer(Table table)
    {
        _scenarioContext.Pending();

        string custId = table.Rows[0]["CustomerId"];
        string custName = table.Rows[0]["CustomerName"];
        var customer = new Customer
        {
            CustomerId = custId,
            CustomerName = custName
        };
        _scenarioContext.Add("Customer", customer);
    }

    [When(@"I submit a POST to create a customer")]
    public void WhenISubmitPostToCreateAnEntity()
    {
        _scenarioContext.Pending();

        var customer = _scenarioContext.Get<Customer>("Customer");
        var changeTracker = new ChangeTrackingCollection<Customer>(true) { customer };
        Customer result = _client.CreateEntity(customer)!;
        changeTracker.MergeChanges(result);
    }

    [When(@"I add a customer setting")]
    public void WhenIAddACustomerSetting()
    {
        _scenarioContext.Pending();

        var customer = _scenarioContext.Get<Customer>("Customer");
        customer.CustomerSetting = new CustomerSetting
        {
            CustomerId = customer.CustomerId,
            Setting = " Test Setting",
            TrackingState = TrackingState.Added // Mark as added
        };
    }

    [When(@"I submit a PUT to update the customer")]
    public void WhenISubmitPutToUpdateTheCustomer()
    {
        _scenarioContext.Pending();

        var customer = _scenarioContext.Get<Customer>("Customer");
        _client.UpdateEntity(customer, customer.CustomerId);
        customer.AcceptChanges();
    }
    
    [Then(@"the request should return the new customer")]
    public void ThenTheRequestShouldReturnTheNewCustomer()
    {
        _scenarioContext.Pending();

        var customer = _scenarioContext.Get<Customer>("Customer");
        var result = _client.GetEntity<Customer, string>(customer.CustomerId);
        Assert.NotNull(result);
        Assert.Equal(customer.CustomerName, result.CustomerName);
        Assert.Equal(customer.CustomerSetting?.Setting, result.CustomerSetting?.Setting);
    }
}
