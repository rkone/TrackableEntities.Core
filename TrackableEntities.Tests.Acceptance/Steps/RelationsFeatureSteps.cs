using TechTalk.SpecFlow;
using TrackableEntities.Client.Core;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.FamilyModels.Client;
using TrackableEntities.Tests.Acceptance.Helpers;
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
            customers.Add(new() { 
                CustomerId = custId, 
                CustomerName = custName,
                TrackingState = TrackingState.Added
            });
        }
        _scenarioContext["Customers"] =  customers;
    }

    [Given(@"the following new employees")]
    public void GivenTheFollowingNewEmployees(Table table)
    {
        var employees = new List<Employee>();
        foreach (var row in table.Rows)
        {
            string employeeId = row["EmployeeId"];
            if (!int.TryParse(employeeId, out int id)) continue;
            string lastName = row["LastName"];
            string firstName = row["FirstName"];           
            employees.Add(new()
            {
                EmployeeId = id,
                LastName = lastName,
                FirstName = firstName,
                TrackingState = TrackingState.Added
            });
        }
        _scenarioContext["Employees"] = employees;
    }

    [Given(@"the following new territories")]
    public void GivenTheFollowingNewTerritories(Table table)
    {
        var territories = new List<Territory>();
        foreach (var row in table.Rows)
        {
            string territoryId = row["TerritoryId"];
            string territoryDescription = row["TerritoryDescription"];
            territories.Add(new() {
                TerritoryId = territoryId, 
                TerritoryDescription = territoryDescription, 
                TrackingState = TrackingState.Added 
            });
        }
        _scenarioContext["Territories"] = territories;
    }

    [When("I GET the employee by id (.*)")]
    public void WhenIGetTheEmployeeById(int employeeId)
    {
        var employee = _client.GetEntity<Employee, int>(employeeId);
        Assert.NotNull(employee);
        _scenarioContext["Employees"] = new List<Employee>() { employee };
    }

    [When(@"I GET the territory by id ""(.*)""")]
    public void WhenIGETTheTerritoryById(string p0)
    {
        var territory = _client.GetEntity<Territory, string>(p0);
        Assert.NotNull(territory);
        _scenarioContext["Territories"] = new List<Territory>() { territory };
    }

    [When(@"I submit POSTs to create customers")]
    public void WhenISubmitPostToCreateCustomers()
    {
        var customers = _scenarioContext.Get<List<Customer>>("Customers");
        var result = new List<Customer>();
        foreach (var customer in customers)
        {
            var customerResult = _client.CreateEntity(customer);
            Assert.NotNull(customerResult);
            customerResult.AcceptChanges();
            result.Add(customerResult);
        }
        _scenarioContext["Customers"] = result;
    }

    [When(@"I submit POSTs to create employees")]
    public void WhenISubmitPostToCreateEmployeess()
    {
        var employees = _scenarioContext.Get<List<Employee>>("Employees");
        var result = new List<Employee>();
        foreach (var employee in employees)
        {
            var employeeResult = _client.CreateEntity(employee);
            Assert.NotNull(employeeResult);
            employeeResult.AcceptChanges();
            result.Add(employeeResult);
        }
        _scenarioContext["Employees"] = result;
    }

    [When(@"I submit POSTs to create territories")]
    public void WhenISubmitPostToCreateTerritories()
    {
        var territories = _scenarioContext.Get<List<Territory>>("Territories");
        var result = new List<Territory>();
        foreach (var territory in territories)
        {
            var territoryResult = _client.CreateEntity(territory);
            Assert.NotNull(territoryResult);
            territoryResult.AcceptChanges();
            result.Add(territoryResult);
        }
        _scenarioContext["Territories"] = result;        
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
            TrackingState = TrackingState.Added 
        };
        _scenarioContext["ChangeTracker"] = changeTracker;
    }

    [When(@"I add territory ""(.*)"" to employee (.*)")]
    public void WhenIAddTerritoryToEmployee(string territoryId, int employeeId)
    {
        var employee = _scenarioContext.Get<List<Employee>>("Employees").FirstOrDefault(e => e.EmployeeId == employeeId);
        var territory = _scenarioContext.Get<List<Territory>>("Territories").FirstOrDefault(t => t.TerritoryId == territoryId);
        Assert.NotNull(employee);
        Assert.NotNull(territory);
        if (!_scenarioContext.TryGetValue("ChangeTracker", out ChangeTrackingCollection<Employee> changeTracker))
            changeTracker = new ChangeTrackingCollection<Employee>(employee);
        employee.Territories.Add(territory);
        _scenarioContext["ChangeTracker"] = changeTracker;
        _scenarioContext["TerritoryResult"] = territory;
        _scenarioContext["EmployeeResult"] = employee;
    }

    [When(@"I remove territory ""(.*)"" from employee (.*)")]
    public void WhenIRemoveTerritoryFromEmployee(string territoryId, int employeeId)
    {
        var employee = _scenarioContext.Get<List<Employee>>("Employees").FirstOrDefault(e => e.EmployeeId == employeeId);
        Assert.NotNull(employee);
        var territory = employee.Territories.FirstOrDefault(t => t.TerritoryId == territoryId);
        Assert.NotNull(territory);
        if (!_scenarioContext.TryGetValue("ChangeTracker", out ChangeTrackingCollection<Employee> changeTracker))
            changeTracker = new ChangeTrackingCollection<Employee>(employee);
        employee.Territories.Remove(territory);
        _scenarioContext["ChangeTracker"] = changeTracker;
        _scenarioContext["TerritoryResult"] = territory;
        _scenarioContext["EmployeeResult"] = employee;
    }

    [When(@"I submit a PUT to update the customer")]
    public void WhenISubmitPutToUpdateTheCustomer()
    {
        var changeTracker = _scenarioContext.Get<ChangeTrackingCollection<Customer>>("ChangeTracker");
        Assert.NotNull(changeTracker);
        var changedCustomer = changeTracker.GetChanges().SingleOrDefault();
        Assert.NotNull(changedCustomer);
        var customerResult = _client.UpdateEntity(changedCustomer, changedCustomer.CustomerId);
        Assert.NotNull(customerResult);
        changeTracker.MergeChanges(customerResult);
        _scenarioContext["CustomerResult"] = customerResult;
    }

    [When(@"I submit a PUT to update the employee")]
    public void WhenISubmitPutToUpdateTheEmployee()
    {
        var changeTracker = _scenarioContext.Get<ChangeTrackingCollection<Employee>>("ChangeTracker");
        Assert.NotNull(changeTracker);
        var changedEmployee = changeTracker.GetChanges().SingleOrDefault();
        Assert.NotNull(changedEmployee);
        var employeeResult = _client.UpdateEntity(changedEmployee, changedEmployee.EmployeeId);
        Assert.NotNull(employeeResult);
        changeTracker.MergeChanges(employeeResult);
        _scenarioContext["EmployeeResult"] = employeeResult;
    }
    
    [When(@"I modify territory ""(.*)"" from employee (.*) to have description ""(.*)""")]
    public void WhenIModifyTerritoryFromEmployeeToHaveDescription(string territoryId, int employeeId, string territoryDescription)
    {
        var employee = _scenarioContext.Get<List<Employee>>("Employees").FirstOrDefault(e => e.EmployeeId == employeeId);
        Assert.NotNull(employee);
        if (!_scenarioContext.TryGetValue("ChangeTracker", out ChangeTrackingCollection<Employee> changeTracker))
            changeTracker = new ChangeTrackingCollection<Employee>(employee);
        var territory = employee.Territories.FirstOrDefault(t => t.TerritoryId == territoryId);
        Assert.NotNull(territory);
        territory.TerritoryDescription = territoryDescription;
        _scenarioContext["ChangeTracker"] = changeTracker;
        _scenarioContext["TerritoryResult"] = territory;
        _scenarioContext["EmployeeResult"] = employee;
    }

    [When(@"I GET employee (.*) to the results")]
    public void WhenIGetEmployeeToTheResults(int employeeId)
    {
        var employee = _client.GetEntity<Employee, int>(employeeId);
        Assert.NotNull(employee);
        _scenarioContext["EmployeeResult"] = employee;
    }

    [Then(@"the request should return the new customer")]
    public void ThenTheRequestShouldReturnTheNewCustomer()
    {
        var customer = _scenarioContext.Get<Customer>("CustomerResult");
        var result = _client.GetEntity<Customer, string>(customer.CustomerId);

        Assert.NotNull(result?.CustomerSetting);
        Assert.NotNull(customer?.CustomerSetting);
        Assert.Equal(customer.CustomerName, result.CustomerName);
        Assert.Equal(customer.CustomerSetting.Setting, result.CustomerSetting.Setting);
    }

    [Then("the employee should have the territory")]
    public void ThenTheEmployeeShouldHaveTheTerritory()
    {
        var employee = _scenarioContext.Get<Employee>("EmployeeResult");
        var territory = _scenarioContext.Get<Territory>("TerritoryResult");
        Assert.NotNull(employee);
        Assert.NotNull(territory);
        var result = employee.Territories.FirstOrDefault(t => t.TerritoryId == territory.TerritoryId);
        Assert.NotNull(result);
        Assert.Equal(territory.TerritoryDescription, result.TerritoryDescription);
    }

    [Then(@"the employee should not have the territory")]
    public void ThenTheEmployeeShouldNotHaveTheTerritory()
    {
        var employee = _scenarioContext.Get<Employee>("EmployeeResult");
        var territory = _scenarioContext.Get<Territory>("TerritoryResult");
        Assert.NotNull(employee);
        Assert.NotNull(territory);
        Assert.DoesNotContain(territory, employee.Territories);
    }

    [Then(@"the employee should have territory ""(.*)"" with description ""(.*)""")]
    public void ThenTheEmployeeShouldHaveTerritoryWithDescription(string territoryId, string territoryDescription)
    {
        var employee = _scenarioContext.Get<Employee>("EmployeeResult");
        var territory = employee.Territories.FirstOrDefault(t => t.TerritoryId == territoryId);
        Assert.NotNull(territory);
        Assert.Equal(territoryDescription, territory.TerritoryDescription);
    }
}