using TechTalk.SpecFlow;
using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.WebApi.Services;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps;

[Binding]
public class BasicFeatureSteps : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private HttpClient _client = default!;
    private readonly CustomWebApplicationFactory<Program> _factory = default!;
    private readonly ScenarioContext _scenarioContext;

    public BasicFeatureSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory)
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

    [Given(@"the following customers")]
    public void GivenTheFollowingCustomers(Table table)
    {
        var custIds = new List<string>();
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                string custName = row["CustomerName"];
                dbContext.EnsureTestCustomer(custId, custName);
                custIds.Add(custId);
            }
        }
        _scenarioContext.Add("CustIds", custIds);
    }

    [Given(@"the following customer orders")]
    public void GivenTheFollowingOrders(Table table)
    {
        var orders = new List<Order>();
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                dbContext.EnsureTestCustomer(custId, "Test Customer " + custId);
                var order = dbContext.EnsureTestOrder(custId);
                orders.Add(order);
            }
        }
        _scenarioContext.Add("CustOrders", orders);
    }

    [Given(@"the following new customer orders")]
    public void GivenTheFollowingNewCustomerOrders(Table table)
    {
        var orders = new List<EF.Core.Tests.FamilyModels.Client.Order>();
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                int[] productIds = dbContext.CreateTestProducts();
                _scenarioContext.Add("ProductIds", productIds);
                var clientOrder = EntityExtensions.CreateNewOrder(custId, productIds);
                orders.Add(clientOrder);
            }
        }
        _scenarioContext.Add("NewCustOrders", orders);
    }

    [Given(@"the following existing customer orders")]
    public void GivenTheFollowingExistingCustomerOrders(Table table)
    {
        var orders = new List<EF.Core.Tests.FamilyModels.Client.Order>();
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
            foreach (var row in table.Rows)
            {
                string custId = row["CustomerId"];
                dbContext.EnsureTestCustomer(custId, "Test Customer " + custId);
                int[] productIds = dbContext.CreateTestProducts();
                _scenarioContext.Add("ProductIds", productIds);
                var order = dbContext.CreateTestOrder(custId, productIds);
                orders.Add(order.ToClientEntity()!);
            }
        }
        _scenarioContext.Add("ExistingCustOrders", orders);
    }

    [Given(@"the order is modified")]
    public void GivenTheOrderIsModified()
    {
        var order = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").First();
        var changeTracker = new ChangeTrackingCollection<EF.Core.Tests.FamilyModels.Client.Order>(order);
        _scenarioContext.Add("DeletedDetail", order.OrderDetails[1]);
        int[] productIds = _scenarioContext.Get<int[]>("ProductIds");
        var addedDetail = new EF.Core.Tests.FamilyModels.Client.OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = productIds[3],
            Quantity = 15,
            UnitPrice = 30
        };
        _scenarioContext.Add("AddedDetail", addedDetail);
        order.OrderDate = order.OrderDate.AddDays(1);
        order.OrderDetails[0].UnitPrice++;
        order.OrderDetails.RemoveAt(1);
        order.OrderDetails.Add(addedDetail);
        _scenarioContext.Add("ChangeTracker", changeTracker);
    }

    [Given(@"order details are added")]
    public void GivenOrderDetailsAreAdded()
    {
        var order = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").First();
        var changeTracker = new ChangeTrackingCollection<EF.Core.Tests.FamilyModels.Client.Order>(order);
        int[] productIds = _scenarioContext.Get<int[]>("ProductIds");
        var addedDetail1 = new EF.Core.Tests.FamilyModels.Client.OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = productIds[3],
            Quantity = 15,
            UnitPrice = 30
        };
        var addedDetail2 = new EF.Core.Tests.FamilyModels.Client.OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = productIds[4],
            Quantity = 20,
            UnitPrice = 40
        };
        _scenarioContext.Add("AddedDetail1", addedDetail1);
        _scenarioContext.Add("AddedDetail2", addedDetail2);
        order.OrderDetails.Add(addedDetail1);
        order.OrderDetails.Add(addedDetail2);
        _scenarioContext.Add("ChangeTracker", changeTracker);
    }

    [When(@"I submit a GET request for customers")]
    public void WhenISubmitGetRequestForCustomers()
    {
        _scenarioContext.Add("CustomersResult", _client.GetEntities<EF.Core.Tests.FamilyModels.Client.Customer>());
    }

    [When(@"I submit a GET request for customer orders")]
    public void WhenISubmitGetRequestForCustomerOrders()
    {        
        var order = _scenarioContext.Get<List<Order>>("CustOrders").First();
        _scenarioContext.Add("CustomerOrdersResult", _client.GetEntitiesByKey<EF.Core.Tests.FamilyModels.Client.Order, string>("customerId", order?.CustomerId!));        
    }

    [When(@"I submit a GET request for an order")]
    public void WhenISubmitGetRequestForAnOrder()
    {
        var order = _scenarioContext.Get<List<Order>>("CustOrders").First();
        var orderResult = _client.GetEntity<EF.Core.Tests.FamilyModels.Client.Order, int>(order.OrderId);
        Assert.NotNull(orderResult);
        var result = new List<EF.Core.Tests.FamilyModels.Client.Order> { orderResult };
        _scenarioContext.Add("CustomerOrdersResult", result);
    }

    [When(@"I submit a POST to create an order")]
    public void WhenISubmitPostToCreateAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("NewCustOrders").First();
        var orderResult = _client.CreateEntity(clientOrder);
        Assert.NotNull(orderResult);
        var result = new List<EF.Core.Tests.FamilyModels.Client.Order> { orderResult };
        _scenarioContext.Add("CustomerOrdersResult", result);
    }

    [When(@"I submit a PUT to modify an order")]
    public void WhenISubmitPutToModifyAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").First();
        var changeTracker = _scenarioContext.Get<ChangeTrackingCollection<EF.Core.Tests.FamilyModels.Client.Order>>("ChangeTracker");
        var clonedOrder = changeTracker.Clone()[0];
        _scenarioContext["ExistingCustOrders"] = new List<EF.Core.Tests.FamilyModels.Client.Order> { clonedOrder };
        var changedOrder = changeTracker.GetChanges().SingleOrDefault();
        Assert.NotNull(changedOrder);
        var orderResult = _client.UpdateEntity(changedOrder, changedOrder.OrderId);
        Assert.NotNull(orderResult);
        changeTracker.MergeChanges(orderResult);
        _scenarioContext.Add("CustomerOrdersResult", new List<EF.Core.Tests.FamilyModels.Client.Order>{ clientOrder});
    }

    [When(@"I submit a DELETE to delete an order")]
    public void WhenISubmitDeleteToDeleteAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").First();
        _client.DeleteEntity<Order, int>(clientOrder.OrderId);
    }
    
    [Then(@"the request should return the customers")]
    public void ThenTheRequestShouldReturnTheCustomers()
    {
        var custId1 = _scenarioContext.Get<List<string>>("CustIds")[0];
        var custId2 = _scenarioContext.Get<List<string>>("CustIds")[1];
        var result = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Customer>>("CustomersResult");
        Assert.Contains(result, c => c.CustomerId == custId1);
        Assert.Contains(result, c => c.CustomerId == custId2);
    }

    [Then(@"the request should return the orders")]
    public void ThenTheRequestShouldReturnTheOrders()
    {
        var result = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("CustomerOrdersResult");
        var orders = _scenarioContext.Get<List<Order>>("CustOrders");
        foreach (var order in orders)
        {
            Assert.Contains(result, o => o.OrderId == order.OrderId);
        }
    }

    [Then(@"the request should return the new order")]
    public void ThenTheRequestShouldReturnTheNewOrders()
    {
        var result = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("CustomerOrdersResult").Single();
        Assert.True(result.OrderId > 0);
    }

    [Then(@"the request should return the modified order")]
    public void ThenTheRequestShouldReturnTheModifiedOrder()
    {
        var modifiedOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").Single();
        var updatedOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("CustomerOrdersResult").Single();
        var addedDetail = _scenarioContext.Get<EF.Core.Tests.FamilyModels.Client.OrderDetail>("AddedDetail");
        var deletedDetail = _scenarioContext.Get<EF.Core.Tests.FamilyModels.Client.OrderDetail>("DeletedDetail");

        Assert.Equal(modifiedOrder.OrderDate, updatedOrder.OrderDate);
        Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail.ProductId);
        Assert.DoesNotContain(updatedOrder.OrderDetails, d => d.ProductId == deletedDetail.ProductId);
    }

    [Then(@"the request should return the added order details")]
    public void ThenTheRequestShouldReturnTheAddedOrderDetails()
    {
        var modifiedOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").Single();
        var updatedOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("CustomerOrdersResult").Single();
        var addedDetail1 = _scenarioContext.Get<EF.Core.Tests.FamilyModels.Client.OrderDetail>("AddedDetail1");
        var addedDetail2 = _scenarioContext.Get<EF.Core.Tests.FamilyModels.Client.OrderDetail>("AddedDetail2");

        Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail1.ProductId);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail2.ProductId);
    }
    
    [Then(@"the order should be deleted")]
    public void ThenTheOrderShouldBeDeleted()
    {
        var clientOrder = _scenarioContext.Get<List<EF.Core.Tests.FamilyModels.Client.Order>>("ExistingCustOrders").First();
        string request = "api/Order/" + clientOrder.OrderId;
        var response = _client.GetAsync(request).Result;
        Assert.False(response.IsSuccessStatusCode);
    }
}
