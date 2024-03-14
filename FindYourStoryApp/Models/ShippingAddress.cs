using System;
using System.Collections.Generic;

namespace FindYourStoryApp.Models;

public partial class ShippingAddress
{
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string AddressLine2 { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
