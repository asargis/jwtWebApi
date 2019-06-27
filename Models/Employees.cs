using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models
{
    public partial class Employees
    {
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int? Id { get; set; }
        public string Fio { get; set; }
        public string MobPhone { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CreateUserId { get; set; }
	}
}
