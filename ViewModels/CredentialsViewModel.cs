using MyApp.ViewModels.Validations;
using FluentValidation.AspNetCore;

namespace MyApp.ViewModels
{
	public class CredentialsViewModel
	{
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}