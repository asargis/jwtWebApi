using System;
using System.Collections.Generic;

namespace MyApp.Models
{
    public partial class UsefulLinks
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int CatId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
    }
}
