using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AutoMapper;
using FluentValidation.AspNetCore;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using MyApp.Auth;
using MyApp.Models;
using MyApp.Helpers;
using MyApp.Extensions;

namespace MyApp
{
	public class Startup
	{
		private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH";
		private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<MyAppContext>(
				options =>
					options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
					x => x.MigrationsAssembly("MyApp")
				)
			);

			services.AddIdentity<AppUser, AppRole>()
				.AddEntityFrameworkStores<MyAppContext>()
				.AddDefaultTokenProviders();

			services.AddSingleton<IJwtFactory, JwtFactory>();

			services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

			// jwt wire up
			// Get options from app settings
			var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

			// Configure JwtIssuerOptions
			services.Configure<JwtIssuerOptions>(options =>
			{
				options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
				options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
			});

			var issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

				ValidateAudience = true,
				ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _signingKey,

				RequireExpirationTime = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

			}).AddJwtBearer(configureOptions =>
			{
				configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
				configureOptions.TokenValidationParameters = tokenValidationParameters;
				configureOptions.SaveToken = true;
			});

			// api user claim policy
			services.AddAuthorization(options =>
			{
				options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
			});

			// add identity
			var builder = services.AddIdentityCore<IdentityUser>(o =>
			{
				// configure identity options
				o.Password.RequireDigit = false;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 6;
			});
			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
			builder.AddDefaultTokenProviders();

			services.AddAutoMapper();
			services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseExceptionHandler(
				builder =>
				{
					builder.Run(
						async context =>
						{
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

							var error = context.Features.Get<IExceptionHandlerFeature>();
							if (error != null)
							{
								context.Response.AddApplicationError(error.Error.Message);
								await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
							}
						});
				});

			app.UseAuthentication();
			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
