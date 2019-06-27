using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MyApp.Auth;
using MyApp.Helpers;
using MyApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyApp.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace MyApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IJwtFactory _jwtFactory;
		private readonly JwtIssuerOptions _jwtOptions;

		public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
		{
			_userManager = userManager;
			_jwtFactory = jwtFactory;
			_jwtOptions = jwtOptions.Value;
		}

		// POST api/auth/login
		[HttpPost("login")]
		public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
		{
			
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
			if (identity == null)
			{
				return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
			}

			var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
			return new OkObjectResult(jwt);
		}

		private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
			{
				return await Task.FromResult<ClaimsIdentity>(null);
			}

			var userToVerify = await _userManager.FindByNameAsync(userName);

			if (userToVerify == null)
			{
				return await Task.FromResult<ClaimsIdentity>(null);
			}

			if (await _userManager.CheckPasswordAsync(userToVerify, password))
			{
				return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, Convert.ToString(userToVerify.Id)));
			}

			return await Task.FromResult<ClaimsIdentity>(null);
		}
	}
}