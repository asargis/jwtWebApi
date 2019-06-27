using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.ViewModels
{
	public class UsefulLinksViewModel
	{
		public string Url { get; set; }
		public string Description { get; set; }
		public int CatId { get; set; }
		public bool IsActive { get; set; }
	}
}
