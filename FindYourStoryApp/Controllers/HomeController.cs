using System.Diagnostics;
using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindYourStoryApp.Controllers
{
    public class HomeController : Controller
    {
        //Declared private fields to store the log information and the DB context of Find Your Story.
        private readonly ILogger<HomeController> _logger;

        //This allows the controller to communicate with the database through the FindYourStoryDbContext object.
        private readonly FindYourStoryDbContext _context;

        //The HomeController Constructor() takes in an instance of ILogger and the DB context and assigns it 
        //to the appropriate private fields.
        public HomeController(ILogger<HomeController> logger, FindYourStoryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Index() method is an action method that makes use of TempData to retrieve the boolean value for the password
        //entered by the user, after successful login and the username of the user currently logged in so that these values
        //can be assigned to the appropriate ViewBag in order to be used in the View.
        //Next, retrieves the top 5 book cover images from the Product table, and their respective details using LINQ queries.
        //Then converts the images to Base64String, adds this image data to a List, as well as adds the repsective details
        //to a separate List. And assigns these list to appropriate ViewBags in order to be used in the View.
        //Lastly, the View is returned.
        public IActionResult Index()
        {
            //Gets the username of the user currently logged in, through using the session.
            ViewBag.UserLoggedIn = HttpContext.Session.GetString("UserLoggedIn");

            //Gideon (2011) demonstrates how to get the top values in a LINQ query.
            var top5BookCovers = (from p in _context.Products
                                  select p.BookCoverImage)
                                  .Take(5)
                                  .ToList();

            //Microsoft (2023) demonstrates how to work with anonymous types.
            var top5BookDetails = (from p in _context.Products
                                   select new
                                   {
                                       p.ProductId,
                                       p.Title,
                                       p.Author,
                                       p.Price,
                                       p.InStock
                                   })
                                   .Take(5)
                                   .ToList();

            //Declares a List of type string to store the top 5 book cover images and of type object to store the respective
            //details. "object" is being used as bookDetails List holds a subset of the Product class.
            List<string> bookCovers = new List<string>();
            List<object> bookDetails = new List<object>();

            foreach (var book in top5BookCovers)
            {
                //Microsoft (2024) demonstrates how to use the Convert.ToBase64String() method.
                string imgData = Convert.ToBase64String(book);
                bookCovers.Add(imgData);
            }

            foreach (var book in top5BookDetails)
            {
                bookDetails.Add(book);
            }

            ViewBag.ImageDataList = bookCovers;
            ViewBag.BookDetailList = bookDetails;

            return View();
        }


        //Index() method adds a book selected by the user to the Cart table.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int ProductId)
        {
            //Creates an instance of the Cart class.
            Cart cartEntry = new Cart();

            //Gets the ID of the user currrently logged in.
            string username = HttpContext.Session.GetString("UserLoggedIn");

            var userID = (from u in _context.Users
                          where u.Username == username
                          select u.UserId).First();

            //Assigns the user-selected book to appropriate properties in the Cart table.
            cartEntry.UserId = userID;
            cartEntry.ProductId = ProductId;
            //Only 1 book item will be added.
            cartEntry.Quantity = 1;


            //Adds the cart entry to the Cart table through calling the built-in Add() method.
            //Assigns a ViewBag the value true to indicate that this item has successfully been added to the Cart table.
            //Lastly, saves those changes to the database asynchronously.
            if (ModelState.IsValid)
            {
                _context.Add(cartEntry);
                await _context.SaveChangesAsync();
                ViewBag.ItemAdded = true;
            }


            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
//REFERENCE LIST:
//Gideon. 2011. LINQ query to select top five. Stack Overflow, 2 February 2011 (Version 1.0)
//[Source code] https://stackoverflow.com/questions/4872946/linq-query-to-select-top-five
//(Accessed 26 January 2024).
//Microsoft. 2023. Anonymous types, 29 November 2023 (Version 1.0)
//[Source code]https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
//(Accessed 26 January 2024).
//Microsoft. 2024. Convert.ToBase64String Method (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.convert.tobase64string?view=net-8.0
//(Accessed 26 January 2024).