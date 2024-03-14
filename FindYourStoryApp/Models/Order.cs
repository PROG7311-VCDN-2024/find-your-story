using System;
using System.Collections.Generic;

namespace FindYourStoryApp.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public int ShippingAddressId { get; set; }

    public int TotalAmount { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ShippingAddress ShippingAddress { get; set; } = null!;
}
