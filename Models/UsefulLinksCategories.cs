using System;
using System.Collections.Generic;

namespace MyApp.Models
{
    public class UsefulLinksCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? CreateUserId { get; set; }
    }
}
