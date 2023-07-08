using System;
using System.Collections.Generic;

namespace Business.Models
{
    public class ProductCategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        // Navigation property
        public ICollection<int> ProductIds { get; set; }
    }
}
