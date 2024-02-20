using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using TrackableEntities.Tests.WebApi.Services;

namespace TrackableEntities.Tests.Acceptance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(NorthwindTestDbContext context) : ControllerBase
{
    private readonly NorthwindTestDbContext _context = context;

    // GET api/Order
    [HttpGet]
    public IAsyncEnumerable<Order> GetOrders()
    {
        return _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
            .AsAsyncEnumerable();
    }
    // GET api/Order?customerId=ABCD
    [HttpGet("customerId:string")]
    public IAsyncEnumerable<Order> GetOrders(string customerId)
    {
        return _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
            .Where(o => o.CustomerId == customerId)
            .AsAsyncEnumerable();
    }

    // GET api/Order/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
            .SingleOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST api/Order
    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(Order order)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(order.CustomerId)) return BadRequest(ModelState);
        order.TrackingState = Common.Core.TrackingState.Added;
        _context.ApplyChanges(order);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (_context.Orders.Any(c => c.OrderId == order.OrderId))
                return Conflict();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(order);
        //order.AcceptChanges();
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    // PUT api/Order/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Order>> PutOrder(long id, Order order)
    {
        if (id != order.OrderId) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        order.TrackingState = Common.Core.TrackingState.Modified;
        _context.ApplyChanges(order);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Orders.Any(c => c.OrderId == order.OrderId))
                return NotFound();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(order);
        //customer.AcceptChanges();
        return Ok(order);
    }

    // DELETE api/Order/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .SingleOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();
        order.TrackingState = Common.Core.TrackingState.Deleted;
        _context.ApplyChanges(order);
        await _context.SaveChangesAsync();
        return Ok();
    }
}