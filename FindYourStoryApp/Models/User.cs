using System.ComponentModel.DataAnnotations;

namespace FindYourStoryApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    [EmailAddress]
    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
