using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Helpers;
using MyApp.Models;
using MyApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace MyApp.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Policy = "ApiUser")]
	[Authorize(Roles = "admin")]
	public class AdminController : Controller
	{
		private readonly MyAppContext _appDbContext;
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<AppRole> _roleManager;
		private readonly IMapper _mapper;

		public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, MyAppContext appDbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_appDbContext = appDbContext;
		}

		// POST api/employee/add
		[HttpPost("employees/add")]
		public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userIdentity = _mapper.Map<AppUser>(model);

			var result = await _userManager.CreateAsync(userIdentity, model.Password);

			if (!result.Succeeded)
				return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

			await _appDbContext.Employees.AddAsync(new Employees
			{
				Fio = model.LastName + ' ' + model.FirstName,
				MobPhone = model.Phone,
				CreateDate = DateTime.Now,
				CreateUserId = userIdentity.Id
			});
			await _appDbContext.SaveChangesAsync();

			return new OkObjectResult("Account created");
		}

		[Produces("application/json")]
		[HttpGet("employees/all")]
		public async Task<IActionResult> Get()
		{
			try
			{
				var employees = await _appDbContext.Employees.ToListAsync();
				return Ok(employees);

			}
			catch
			{
				return BadRequest();
			}
		}

		[Produces("application/json")]
		[HttpGet("employees/{id}")]
		public async Task<IActionResult> Get(int? id)
		{
			try
			{
				var employee = await _appDbContext.Employees.FindAsync(id);
				return Ok(employee);
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpPost("roles/add")]
		public async Task<IActionResult> Post([FromBody]AddRoleViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result = await _roleManager.CreateAsync(new AppRole(model.Name));
			if (result.Succeeded)
			{
				return new OkObjectResult("Role created");
			}
			else
			{
				return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
			}
		}

		[HttpPost("user/roles")]
		public async Task<IActionResult> Post([FromBody]UserRolesViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			AppUser user = await _userManager.FindByIdAsync(model.UserId);

			if (user != null)
			{
				var result = await _userManager.AddToRolesAsync(user, model.UserRoles);
				if (!result.Succeeded)
				{
					return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
				}
				else
				{
					return new OkObjectResult("Roles added");
				}

			}
			else
			{
				return BadRequest("User is not found");
			}
		}

		/*		[HttpPost("roles/edit/")]
				public async Task<IActionResult> Post([FromBody]ChangeRoleViewModel model)
				{
					if (!ModelState.IsValid)
					{
						return BadRequest(ModelState);
					}

					AppUser user = await _userManager.FindByIdAsync(model.UserId);

					//var userRoles = await _userManager.GetRolesAsync(user);
					//var allRoles = await _roleManager.Roles.ToListAsync();

					var result = await _userManager.AddToRolesAsync(user, model.Roles);

					if (!result.Succeeded)
					{
						return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
					}

					return new OkObjectResult("success");

				}*/


	}
}