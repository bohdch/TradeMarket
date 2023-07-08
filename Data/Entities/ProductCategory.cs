using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string CategoryName { get; set; }

        // Naviagation property
        public ICollection<Product> Products { get; set; }
    }
}
