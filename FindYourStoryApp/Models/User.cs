using System;
using System.Collections.Generic;

namespace FindYourStoryApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public int RoleId { get; set; }

    public string FirebaseUid { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
}
