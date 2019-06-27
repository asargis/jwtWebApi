using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyApp.ViewModels;

namespace MyApp.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Policy = "ApiUser")]
	[Authorize(Roles = "admin")]
	public class CatalogsController : Controller
	{
		private readonly MyAppContext _appDbContext;
		private readonly UserManager<AppUser> _userManager;

		public CatalogsController(MyAppContext appDbContext, UserManager<AppUser> userManager)
		{
			_userManager = userManager;
			_appDbContext = appDbContext;
		}

		// POST: api/catalogs
		[HttpPost]
		public async Task<IActionResult> Post([FromBody]UsefulLinksCategoriesViewModel model)
		{
			ClaimsPrincipal currentUser = this.User;
			var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
			AppUser user = await _userManager.FindByNameAsync(currentUserName);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			await _appDbContext.UsefulLinksCategories.AddAsync(new UsefulLinksCategories
			{
				Name = model.Name,
				IsActive = model.IsActive,
				CreateUserId = user.Id
			});
			await _appDbContext.SaveChangesAsync();

			return new OkObjectResult("Catalog created");
		}

		// GET: api/catalogs/5
		[HttpGet("{id}")]
		[Produces("application/json")]
		public async Task<ActionResult<UsefulLinksCategories>> Get(int id)
		{
			var catalog = await _appDbContext.UsefulLinksCategories.FindAsync(id);

			if (catalog == null)
			{
				return BadRequest();
			}

			return Ok(catalog);
		}

		// PUT: api/catalogs/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromBody]UsefulLinksCategories model)
		{
			if (id != model.Id)
			{
				return BadRequest();
			}

			_appDbContext.Entry(model).State = EntityState.Modified;

			try
			{
				await _appDbContext.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UsefulLinksCategoriesExists(id))
				{
					return BadRequest();
				}
				else
				{
					throw;
				}
			}

			return new OkObjectResult("Catalog updated");
		}

		// GET: api/catalogs/
		[HttpGet]
		[Produces("application/json")]
		public async Task<IActionResult> Get()
		{
			try
			{
				var categories = await _appDbContext.UsefulLinksCategories.ToListAsync();
				return Ok(categories);

			}
			catch
			{
				return BadRequest();
			}
		}


		// DELETE: api/catalogs/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<UsefulLinksCategories>> DeleteUsefulLinksCategories(int id)
		{
			var usefulLinksCategories = await _appDbContext.UsefulLinksCategories.FindAsync(id);
			if (usefulLinksCategories == null)
			{
				return BadRequest();
			}

			_appDbContext.UsefulLinksCategories.Remove(usefulLinksCategories);
			await _appDbContext.SaveChangesAsync();

			return usefulLinksCategories;
		}


		private bool UsefulLinksCategoriesExists(int id)
		{
			return _appDbContext.UsefulLinksCategories.Any(e => e.Id == id);
		}
	}
}