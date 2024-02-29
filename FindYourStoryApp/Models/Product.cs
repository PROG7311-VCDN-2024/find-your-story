using System;
using System.Collections.Generic;

namespace FindYourStoryApp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public byte[]? BookCoverImage { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public decimal Price { get; set; }

    public bool InStock { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
