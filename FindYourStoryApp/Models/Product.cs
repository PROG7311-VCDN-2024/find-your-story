using System;
using System.Collections.Generic;

namespace FindYourStoryApp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public byte[] BookCoverImage { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int Price { get; set; }

    public int InStock { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
