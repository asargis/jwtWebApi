using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;

namespace MyApp.ViewModels
{
	public class ChangeRoleViewModel
	{
		public string UserId { get; set; }
		public List<AppRole> AllRoles { get; set; }
		public IList<string> UserRoles { get; set; }
		public ChangeRoleViewModel()
		{
			AllRoles = new List<AppRole>();
			UserRoles = new List<string>();
		}
	}
}
