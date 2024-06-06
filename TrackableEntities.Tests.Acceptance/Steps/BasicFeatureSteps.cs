using TechTalk.SpecFlow;
using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core.Tests.FamilyModels.Client;
using TrackableEntities.Tests.Acceptance.Helpers;
using TrackableEntities.Tests.WebApi.Services;
using Xunit;

namespace TrackableEntities.Tests.Acceptance.Steps;

[Binding]
public class BasicFeatureSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
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

    [Given(@"the following customers")]
    public void GivenTheFollowingCustomers(Table table)
    {
        var custIds = new List<string>();
        var context = _dbScope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
        foreach (var row in table.Rows)
        {
            string custId = row["CustomerId"];
            string custName = row["CustomerName"];
            context.EnsureTestCustomer(custId, custName);
            custIds.Add(custId);
        }
        _scenarioContext.Add("CustIds", custIds);
    }

    [Given(@"the following customer orders")]
    public void GivenTheFollowingOrders(Table table)
    {
        var orders = new List<Order>();
        var context = _dbScope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
        foreach (var row in table.Rows)
        {
            string custId = row["CustomerId"];
            context.EnsureTestCustomer(custId, "Test Customer " + custId);
            var order = context.EnsureTestOrder(custId);            
            orders.Add(order.ToClientEntity()!);
        }
        _scenarioContext.Add("CustOrders", orders);
    }

    [Given(@"the following new customer orders")]
    public void GivenTheFollowingNewCustomerOrders(Table table)
    {
        var orders = new List<Order>();
        var context = _dbScope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
        foreach (var row in table.Rows)
        {
            string custId = row["CustomerId"];
            context.EnsureTestCustomer(custId, "Test Customer " + custId);
            int[] productIds = context.CreateTestProducts();
            _scenarioContext.Add("ProductIds", productIds);
            var clientOrder = EntityExtensions.CreateNewOrder(custId, productIds);
            orders.Add(clientOrder);
        }        
        _scenarioContext.Add("NewCustOrders", orders);
    }

    [Given(@"the following existing customer orders")]
    public void GivenTheFollowingExistingCustomerOrders(Table table)
    {
        var orders = new List<Order>();
        var context = _dbScope.ServiceProvider.GetRequiredService<NorthwindTestDbContext>();
        foreach (var row in table.Rows)
        {
            string custId = row["CustomerId"];
            context.EnsureTestCustomer(custId, "Test Customer " + custId);
            int[] productIds = context.CreateTestProducts();
            _scenarioContext.Add("ProductIds", productIds);
            var order = context.CreateTestOrder(custId, productIds);
            orders.Add(order.ToClientEntity()!);
        }
        _scenarioContext.Add("ExistingCustOrders", orders);
    }

    [Given(@"the order is modified")]
    public void GivenTheOrderIsModified()
    {
        var order = _scenarioContext.Get<List<Order>>("ExistingCustOrders").First();
        var changeTracker = new ChangeTrackingCollection<Order>(order);
        _scenarioContext.Add("DeletedDetail", order.OrderDetails[1]);
        int[] productIds = _scenarioContext.Get<int[]>("ProductIds");
        var addedDetail = new OrderDetail
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
        var order = _scenarioContext.Get<List<Order>>("ExistingCustOrders").First();
        var changeTracker = new ChangeTrackingCollection<Order>(order);
        int[] productIds = _scenarioContext.Get<int[]>("ProductIds");
        var addedDetail1 = new OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = productIds[3],
            Quantity = 15,
            UnitPrice = 30
        };
        var addedDetail2 = new OrderDetail
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
        _scenarioContext.Add("CustomersResult", _client.GetEntities<Customer>());
    }

    [When(@"I submit a GET request for customer orders")]
    public void WhenISubmitGetRequestForCustomerOrders()
    {
        var order = _scenarioContext.Get<List<Order>>("CustOrders").First();
        _scenarioContext.Add("CustomerOrdersResult", _client.GetEntitiesByKey<Order, string>("customerId", order?.CustomerId!));
    }

    [When(@"I submit a GET request for an order")]
    public void WhenISubmitGetRequestForAnOrder()
    {
        var order = _scenarioContext.Get<List<Order>>("CustOrders").First();
        var orderResult = _client.GetEntity<Order, int>(order.OrderId);
        Assert.NotNull(orderResult);
        var result = new List<Order> { orderResult };
        _scenarioContext.Add("CustomerOrdersResult", result);
    }

    [When(@"I submit a POST to create an order")]
    public void WhenISubmitPostToCreateAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<Order>>("NewCustOrders").First();
        var orderResult = _client.CreateEntity(clientOrder);
        Assert.NotNull(orderResult);
        var result = new List<Order> { orderResult };
        _scenarioContext.Add("CustomerOrdersResult", result);
    }

    [When(@"I submit a PUT to modify an order")]
    public void WhenISubmitPutToModifyAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<Order>>("ExistingCustOrders").First();        
        var changeTracker = _scenarioContext.Get<ChangeTrackingCollection<Order>>("ChangeTracker");
        var clonedOrder = changeTracker.Clone()[0];
        _scenarioContext["ExistingCustOrders"] = new List<Order> { clonedOrder };
        var changedOrder = changeTracker.GetChanges().SingleOrDefault();
        Assert.NotNull(changedOrder);
        var orderResult = _client.UpdateEntity(changedOrder, changedOrder.OrderId);
        Assert.NotNull(orderResult);
        changeTracker.MergeChanges(orderResult);
        _scenarioContext.Add("CustomerOrdersResult", new List<Order> { clientOrder });
    }

    [When(@"I submit a DELETE to delete an order")]
    public void WhenISubmitDeleteToDeleteAnOrder()
    {
        var clientOrder = _scenarioContext.Get<List<Order>>("ExistingCustOrders").First();
        _client.DeleteEntity<Order, int>(clientOrder.OrderId);
    }

    [Then(@"the request should return the customers")]
    public void ThenTheRequestShouldReturnTheCustomers()
    {
        var custId1 = _scenarioContext.Get<List<string>>("CustIds")[0];
        var custId2 = _scenarioContext.Get<List<string>>("CustIds")[1];
        var result = _scenarioContext.Get<List<Customer>>("CustomersResult");
        Assert.Contains(result, c => c.CustomerId == custId1);
        Assert.Contains(result, c => c.CustomerId == custId2);
    }

    [Then(@"the request should return the orders")]
    public void ThenTheRequestShouldReturnTheOrders()
    {
        var result = _scenarioContext.Get<List<Order>>("CustomerOrdersResult");
        var orders = _scenarioContext.Get<List<Order>>("CustOrders");
        foreach (var order in orders)
        {
            Assert.Contains(result, o => o.OrderId == order.OrderId);
        }
    }

    [Then(@"the request should return the new order")]
    public void ThenTheRequestShouldReturnTheNewOrders()
    {
        var result = _scenarioContext.Get<List<Order>>("CustomerOrdersResult").Single();
        Assert.True(result.OrderId > 0);
    }

    [Then(@"the request should return the modified order")]
    public void ThenTheRequestShouldReturnTheModifiedOrder()
    {
        var modifiedOrder = _scenarioContext.Get<List<Order>>("ExistingCustOrders").Single();
        var updatedOrder = _scenarioContext.Get<List<Order>>("CustomerOrdersResult").Single();
        var addedDetail = _scenarioContext.Get<OrderDetail>("AddedDetail");
        var deletedDetail = _scenarioContext.Get<OrderDetail>("DeletedDetail");

        Assert.Equal(modifiedOrder.OrderDate, updatedOrder.OrderDate);
        Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail.ProductId);
        Assert.DoesNotContain(updatedOrder.OrderDetails, d => d.ProductId == deletedDetail.ProductId);
    }

    [Then(@"the request should return the added order details")]
    public void ThenTheRequestShouldReturnTheAddedOrderDetails()
    {
        var modifiedOrder = _scenarioContext.Get<List<Order>>("ExistingCustOrders").Single();
        var updatedOrder = _scenarioContext.Get<List<Order>>("CustomerOrdersResult").Single();
        var addedDetail1 = _scenarioContext.Get<OrderDetail>("AddedDetail1");
        var addedDetail2 = _scenarioContext.Get<OrderDetail>("AddedDetail2");

        Assert.Equal(modifiedOrder.OrderDetails[0].UnitPrice, updatedOrder.OrderDetails[0].UnitPrice);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail1.ProductId);
        Assert.Contains(updatedOrder.OrderDetails, d => d.ProductId == addedDetail2.ProductId);
    }

    [Then(@"the order details should have an OrderDetailId > (.*)")]
    public void ThenTheOrderDetailsShouldHaveAnOrderDetailId(int p0)
    {
        var addedDetail1 = _scenarioContext.Get<OrderDetail>("AddedDetail1");
        var addedDetail2 = _scenarioContext.Get<OrderDetail>("AddedDetail2");
        Assert.NotEqual(0, addedDetail1.OrderDetailId);
        Assert.NotEqual(0, addedDetail2.OrderDetailId);
    }

    [Then(@"the order should be deleted")]
    public void ThenTheOrderShouldBeDeleted()
    {
        var clientOrder = _scenarioContext.Get<List<Order>>("ExistingCustOrders").First();
        string request = "api/Order/" + clientOrder.OrderId;
        var response = _client.GetAsync(request).Result;
        Assert.False(response.IsSuccessStatusCode);
    }
}
