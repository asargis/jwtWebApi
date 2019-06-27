using Microsoft.AspNetCore.Identity;

namespace MyApp.Models
{
	public class AppRole : IdentityRole<int>
	{
		public AppRole(string name)
			: base(name)
		{
		}
	}
}
