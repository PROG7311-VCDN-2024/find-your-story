using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindYourStoryApp.Controllers
{
    public class ShippingAddressController : Controller
    {
        //Declared a private field to store the DB context of Find Your Story.
        private readonly FindYourStoryDbContext _context;

        //The ShippingAddressController Constructor() takes in an instance of the DB context and assigns it 
        //to the private field, _context.
        public ShippingAddressController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        public IActionResult ShippingAddress()
        {
            return View();
        }



        //Gets the ID of the user currrently logged in.
        public int userLoggedIn()
        {
            //Cull (2016) demonstrates how to use sessions.
            string username = HttpContext.Session.GetString("UserLoggedIn");

            var userID = (from u in _context.Users
                          where u.Username == username
                          select u.UserId).First();

            return userID;
        }
    }
}
//REFERENCE LIST:
//CsharpTutorial. 2023. LINQ Any, 2023 (Version 1.0)
//[Source code] https://www.csharptutorial.net/csharp-linq/linq-any/#:~:text=The%20Any()%20is%20an,Otherwise%2C%20it%20returns%20false%20.&text=In%20this%20syntax%3A,the%20type%20IEnumerable%20.
//(Accessed 6 March 2024).
//Cull, B. 2016. Using Sessions and HttpContext in ASP.NET Core and MVC Core, 23 July 2016 (Version 1.0)
//[Source code] https://bencull.com/blog/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core
//(Accessed 28 February 2024).
