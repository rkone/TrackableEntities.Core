using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using TrackableEntities.Tests.WebApi.Services;

namespace TrackableEntities.Tests.Acceptance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(NorthwindTestDbContext context) : ControllerBase
{
    private readonly NorthwindTestDbContext _context = context;

    // GET api/Employee
    [HttpGet]
    public IAsyncEnumerable<Employee> GetEmployees()
    {
        return _context.Employees
            .Include(o => o.Territories).ThenInclude(t => t.Areas)
            .AsAsyncEnumerable();
    }
    // GET api/Employee?territoryId=ABCD
    [HttpGet("territoryId")]
    public IAsyncEnumerable<Employee> GetEmployees(string territoryId)
    {
        return _context.Employees
            .Include(o => o.Territories).ThenInclude(t => t.Areas)
            .Where(o => o.Territories.Any(t => t.TerritoryId == territoryId))
            .AsAsyncEnumerable();
    }

    // GET api/Employee/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(o => o.Territories).ThenInclude(t => t.Areas)
            .SingleOrDefaultAsync(o => o.EmployeeId == id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    // POST api/Employee
    [HttpPost]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        employee.TrackingState = Common.Core.TrackingState.Added;
        _context.ApplyChanges(employee);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (_context.Employees.Any(c => c.EmployeeId == employee.EmployeeId))
                return Conflict();
            throw;
        }
        _context.AcceptChanges(employee);
        await _context.LoadRelatedEntitiesAsync(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
    }

    // PUT api/Employee/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Employee>> PutEmployee(long id, Employee employee)
    {
        if (id != employee.EmployeeId) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.ApplyChanges(employee);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Employees.Any(c => c.EmployeeId == employee.EmployeeId))
                return NotFound();
            throw;
        }
        _context.AcceptChanges(employee);
        await _context.LoadRelatedEntitiesAsync(employee);
        return Ok(employee);
    }

    // DELETE api/Employee/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees
            .SingleOrDefaultAsync(o => o.EmployeeId == id);
        if (employee == null) return NotFound();
        employee.TrackingState = Common.Core.TrackingState.Deleted;
        _context.ApplyChanges(employee);
        await _context.SaveChangesAsync();
        _context.AcceptChanges(employee);
        return Ok();
    }
}