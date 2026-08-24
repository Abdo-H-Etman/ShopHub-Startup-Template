using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.Entities.Models.Interfaces;

namespace myshop.Entities.Models;

public class Category : ISoftDeletable, IAuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedTime
    {
        get => CreatedAt;
        set => CreatedAt = value;
    }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    [ValidateNever]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
