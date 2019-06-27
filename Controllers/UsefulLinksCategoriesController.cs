using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly MyAppContext _context;

        public CategoriesController(MyAppContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsefulLinksCategories>>> GetUsefulLinksCategories()
        {
            return await _context.UsefulLinksCategories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsefulLinksCategories>> GetUsefulLinksCategories(int id)
        {
            var usefulLinksCategories = await _context.UsefulLinksCategories.FindAsync(id);

            if (usefulLinksCategories == null)
            {
                return NotFound();
            }

            return usefulLinksCategories;
        }

        // PATCH: api/Categories/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutUsefulLinksCategories(int id, UsefulLinksCategories usefulLinksCategories)
        {
            if (id != usefulLinksCategories.Id)
            {
                return BadRequest();
            }

            _context.Entry(usefulLinksCategories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsefulLinksCategoriesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<UsefulLinksCategories>> PostUsefulLinksCategories(UsefulLinksCategories usefulLinksCategories)
        {
            _context.UsefulLinksCategories.Add(usefulLinksCategories);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsefulLinksCategories", new { id = usefulLinksCategories.Id }, usefulLinksCategories);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UsefulLinksCategories>> DeleteUsefulLinksCategories(int id)
        {
            var usefulLinksCategories = await _context.UsefulLinksCategories.FindAsync(id);
            if (usefulLinksCategories == null)
            {
                return NotFound();
            }

            _context.UsefulLinksCategories.Remove(usefulLinksCategories);
            await _context.SaveChangesAsync();

            return usefulLinksCategories;
        }

        private bool UsefulLinksCategoriesExists(int id)
        {
            return _context.UsefulLinksCategories.Any(e => e.Id == id);
        }
    }
}
