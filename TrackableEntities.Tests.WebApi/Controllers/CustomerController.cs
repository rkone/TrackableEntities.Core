using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using TrackableEntities.Tests.WebApi.Services;

namespace TrackableEntities.Tests.Acceptance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly NorthwindTestDbContext _context;
    public CustomerController(NorthwindTestDbContext context)
    {
        _context = context;
    }

    // GET api/Customer
    [HttpGet]
    public IAsyncEnumerable<Customer> GetCustomers()
    {
        return _context.Customers.AsAsyncEnumerable();
    }

    // GET api/Customer/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(string id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    // POST api/Customer
    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        customer.TrackingState = Common.Core.TrackingState.Added;
        _context.ApplyChanges(customer);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (_context.Customers.Any(c => c.CustomerId == customer.CustomerId))
                return Conflict();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(customer);
        //customer.AcceptChanges();
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
    }

    // PUT api/Customer
    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> PutCustomer(string id, Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        customer.TrackingState = Common.Core.TrackingState.Modified;
        _context.ApplyChanges(customer);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Customers.Any(c => c.CustomerId == customer.CustomerId))
                return NotFound();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(customer);
        //customer.AcceptChanges();
        return Ok(customer);
    }

    // DELETE api/Customer/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(string id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        customer.TrackingState = Common.Core.TrackingState.Deleted;
        _context.ApplyChanges(customer);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
