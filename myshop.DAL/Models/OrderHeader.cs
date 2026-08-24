using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using myshop.Entities.Models.Interfaces;

namespace myshop.Entities.Models;

public class OrderHeader : ISoftDeletable, IAuditableEntity
{
    public int Id { get; set; }

    public int ApplicationUserId { get; set; }
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ShippingDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }

    public string? TrackingNumber { get; set; }
    public string? TrakcingNumber
    {
        get => TrackingNumber;
        set => TrackingNumber = value;
    }
    public string? Carrier { get; set; }

    public DateTime? PaymentDate { get; set; }

    // Payment / Session Properties
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }

    // Delivery / Customer Data
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    [ValidateNever]
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
