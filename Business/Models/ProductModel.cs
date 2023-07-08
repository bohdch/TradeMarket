﻿using System;
using System.Collections.Generic;

namespace Business.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        // Navigation property
        public ICollection<int> ReceiptDetailIds { get; set; }
    }
}
