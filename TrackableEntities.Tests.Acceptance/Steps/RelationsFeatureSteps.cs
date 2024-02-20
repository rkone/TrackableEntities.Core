using TechTalk.SpecFlow;
using TrackableEntities.Client.Core;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.FamilyModels.Client;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.WebApi.Services;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps;

[Binding]
public class RelationsFeatureSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private HttpClient _client = default!;
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private IServiceScope _dbScope = default!;

    [BeforeScenario]
    public void Setup()
    {
        _client = _factory.CreateClient();
        _dbScope = _factory.Services.CreateScope();
    }

    [AfterScenario]
    public void TearDown()
    {
        _client.Dispose();
        _dbScope.Dispose();
    }

    [Given(@"the following new customers")]
    public void GivenTheFollowingNewCustomer(Table table)
    {
        var customers = new List<Customer>();
        foreach (var row in table.Rows)
        {
            string custId = row["CustomerId"];
            string custName = row["CustomerName"];
            customers.Add(new() { CustomerId = custId, CustomerName = custName });
        }
        _scenarioContext.Add("NewCustomers", customers);
    }

    [When(@"I submit POSTs to create customers")]
    public void WhenISubmitPostToCreateCustomers()
    {
        var customers = _scenarioContext.Get<List<Customer>>("NewCustomers");
        var result = new List<Customer>();
        foreach (var customer in customers)
        {
            var customerResult = _client.CreateEntity(customer);
            Assert.NotNull(customerResult);
            customerResult.AcceptChanges();
            result.Add(customerResult);
        }
        _scenarioContext.Add("Customers", result);
    }

    [When(@"I add a customer setting")]
    public void WhenIAddACustomerSetting()
    {
        var customer = _scenarioContext.Get<List<Customer>>("Customers").FirstOrDefault();
        Assert.NotNull(customer);
        var changeTracker = new ChangeTrackingCollection<Customer>(customer);
        customer.CustomerSetting = new CustomerSetting
        {
            CustomerId = customer.CustomerId,
            Setting = "Test Setting",
            TrackingState = TrackingState.Added // Mark as added
        };
        _scenarioContext.Add("ChangeTracker", changeTracker);
    }

    [When(@"I submit a PUT to update the customer")]
    public void WhenISubmitPutToUpdateTheCustomer()
    {
        _scenarioContext.Pending();
        
        var changeTracker = _scenarioContext.Get<ChangeTrackingCollection<Customer>>("ChangeTracker");
        var changedCustomer = changeTracker.GetChanges().SingleOrDefault();
        Assert.NotNull(changedCustomer);
        // changedCustomer.CustomerSetting = null; // web api recognizes Customer when this CustomerSetting isn't set.
        var customerResult = _client.UpdateEntity(changedCustomer, changedCustomer.CustomerId);
        Assert.NotNull(customerResult);
        changeTracker.MergeChanges(customerResult);
        _scenarioContext.Add("CustomerResult", customerResult);
    }

    [Then(@"the request should return the new customer")]
    public void ThenTheRequestShouldReturnTheNewCustomer()
    {
        _scenarioContext.Pending();

        var customer = _scenarioContext.Get<Customer>("CustomerResult");
        var result = _client.GetEntity<Customer, string>(customer.CustomerId);
        Assert.NotNull(result?.CustomerSetting);
        Assert.NotNull(customer?.CustomerSetting);
        Assert.Equal(customer.CustomerName, result.CustomerName);
        Assert.Equal(customer.CustomerSetting.Setting, result.CustomerSetting.Setting);
    }
}