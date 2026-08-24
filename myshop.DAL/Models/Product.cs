using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.Entities.Models.Interfaces;

namespace myshop.Entities.Models;

public class Product : ISoftDeletable, IAuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Img { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    [ValidateNever]
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    [ValidateNever]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ValidateNever]
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
