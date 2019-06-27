using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyApp.ViewModels;

namespace MyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinksController : Controller
    {
        private readonly MyAppContext _appDbContext;
		private readonly UserManager<AppUser> _userManager;

		public LinksController(MyAppContext context, UserManager<AppUser> userManager)
        {
            _appDbContext = context;
			_userManager = userManager;
        }

		// GET: api/links
		[HttpGet]
		[Produces("application/json")]
        public async Task<ActionResult<IEnumerable<UsefulLinks>>> Get()
        {
            return Ok(await _appDbContext.UsefulLinks.ToListAsync());
        }

        // GET: api/Links/5
        [HttpGet("{id}")]
		[Produces("application/json")]
		public async Task<ActionResult<UsefulLinks>> Get(int id)
        {
            var usefulLinks = await _appDbContext.UsefulLinks.FindAsync(id);

            if (usefulLinks == null)
            {
                return BadRequest();
            }

            return Ok(usefulLinks);
        }

		// PUT: api/links/5
		[HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UsefulLinks usefulLinks)
        {
            if (id != usefulLinks.Id)
            {
                return BadRequest();
            }

            _appDbContext.Entry(usefulLinks).State = EntityState.Modified;

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsefulLinksExists(id))
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

		// POST: api/links
		[HttpPost]
        public async Task<ActionResult<UsefulLinks>> Post([FromBody]UsefulLinksViewModel model)
        {
			ClaimsPrincipal currentUser = this.User;
			var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
			AppUser user = await _userManager.FindByNameAsync(currentUserName);

			var link = new UsefulLinks
			{
				Url = model.Url,
				Description = model.Description,
				CatId = model.CatId,
				CreateDate = DateTime.Now,
				CreateUserId = user.Id
			};

			await _appDbContext.UsefulLinks.AddAsync(link);
			await _appDbContext.SaveChangesAsync();

			return new OkObjectResult("Useful Link created");
        }

        // DELETE: api/links/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UsefulLinks>> Delete(int id)
        {
            var usefulLinks = await _appDbContext.UsefulLinks.FindAsync(id);
            if (usefulLinks == null)
            {
                return NotFound();
            }

            _appDbContext.UsefulLinks.Remove(usefulLinks);
            await _appDbContext.SaveChangesAsync();

            return usefulLinks;
        }

        private bool UsefulLinksExists(int id)
        {
            return _appDbContext.UsefulLinks.Any(e => e.Id == id);
        }
    }
}
