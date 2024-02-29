using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindYourStoryApp.Controllers
{
    public class CartController : Controller
    {

        //This allows the controller to communicate with the database through the FindYourStoryDbContext object.
        private readonly FindYourStoryDbContext _context;

        //The CartController Constructor() takes in an instance of the DB context and assigns it to the
        //appropriate private field.
        public CartController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        //Index() method displays the full cart of all the book that the user currently logged in has added to their cart,
        //which displays, the title of the book added, along with the quantity of that book.
        public IActionResult Cart()
        {

            //Gets the ID of the user currrently logged in through the session.
            string username = HttpContext.Session.GetString("UserLoggedIn");

            var userID = (from u in _context.Users
                          where u.Username == username
                          select u.UserId).First();

            //Calculates the total quantity amount of each book that has been added to the cart through using GROUP BY and SUM.
            var bookItemQuantity = (from c in _context.Carts
                                    join p in _context.Products on c.ProductId equals p.ProductId
                                    group c by new { c.ProductId, p.Title } into bookOrder
                                    select new FullCart
                                    {
                                        BookProductId = bookOrder.Key.ProductId,
                                        BookTitle = bookOrder.Key.Title,
                                        TotalQuantity = bookOrder.Sum(c => c.Quantity)
                                    }).ToList();


            List<FullCart> fullCarts = bookItemQuantity;


            //Assigns this now summed-up full cart to a ViewBag.
            ViewBag.FullCart = fullCarts;

            return View();
        }
    }


    class FullCart
    {
        public int BookProductId { get; set; }
        public string BookTitle { get; set; }
        public int? TotalQuantity { get; set; }
    }
}
