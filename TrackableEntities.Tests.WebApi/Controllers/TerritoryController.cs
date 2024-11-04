using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using TrackableEntities.Client.Core;
using TrackableEntities.EF.Core;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using TrackableEntities.Tests.WebApi.Services;

namespace TrackableEntities.Tests.Acceptance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TerritoryController(NorthwindTestDbContext context) : ControllerBase
{
    private readonly NorthwindTestDbContext _context = context;

    // GET api/Territory
    [HttpGet]
    public IAsyncEnumerable<Territory> GetTerritories()
    {
        return _context.Territories
            .Include(t => t.Areas)
            .AsAsyncEnumerable();
    }    

    // GET api/Territory/ABCD
    [HttpGet("{id}")]
    public async Task<ActionResult<Territory>> GetTerritory(string id)
    {
        var order = await _context.Territories
            .Include(t => t.Areas)
            .SingleOrDefaultAsync(o => o.TerritoryId == id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST api/Territory
    [HttpPost]
    public async Task<ActionResult<Territory>> PostTerritory(Territory territory)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        territory.TrackingState = Common.Core.TrackingState.Added;
        _context.ApplyChanges(territory);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (_context.Territories.Any(c => c.TerritoryId == territory.TerritoryId))
                return Conflict();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(territory);
        //order.AcceptChanges();
        return CreatedAtAction(nameof(GetTerritory), new { id = territory.TerritoryId }, territory);
    }

    // PUT api/Territory/ABCD
    [HttpPut("{id}")]
    public async Task<ActionResult<Territory>> PutTerritory(string id, Territory territory)
    {
        if (id != territory.TerritoryId) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.ApplyChanges(territory);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Territories.Any(c => c.TerritoryId == territory.TerritoryId))
                return NotFound();
            throw;
        }

        await _context.LoadRelatedEntitiesAsync(territory);
        //customer.AcceptChanges();
        return Ok(territory);
    }

    // DELETE api/Territory/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTerritory(string id)
    {
        var territory = await _context.Territories
            .SingleOrDefaultAsync(o => o.TerritoryId == id);
        if (territory == null) return NotFound();
        territory.TrackingState = Common.Core.TrackingState.Deleted;
        _context.ApplyChanges(territory);
        await _context.SaveChangesAsync();
        return Ok();
    }
}