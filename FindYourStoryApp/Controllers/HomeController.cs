using System.Diagnostics;
using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FindYourStoryApp.Controllers
{
    public class HomeController : Controller
    {

        //This allows the controller to communicate with the database through the FindYourStoryDbContext object.
        private readonly FindYourStoryDbContext _context;

        //The HomeController Constructor() takes in an instance of ILogger and the DB context and assigns it 
        //to the appropriate private fields.
        public HomeController(FindYourStoryDbContext context)
        {
            _context = context;

        }

        //Index() method is an action method that gets the top 8 books in the Product table, which includes
        //the book cover image and its associated details. These 2 items are added to separate lists for displaying
        //purposes as the book cover image needs to be converted to Base64String to be shown appropriately in the
        //View. Next, ViewBags are used to send necessary data to the View.
        //Finally the necessary View is returned.
        public IActionResult Index()
        {
            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //Gideon (2011) demonstrates how to get the top values in a LINQ query.
            var top5BookCovers = (from p in _context.Products
                                  select p.BookCoverImage)
                                  .Take(8)
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
                                   .Take(8)
                                   .ToList();

            //Declares a List of type string to store the top 8 book cover images and of type object to store the respective
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

            return View("Index");
        }

        //BookDetails() method is an action method that retrieves the product id of the book selected and gets the
        //book cover image and its associated details. These 2 items are added to separate lists for displaying
        //purposes as the book cover image needs to be converted to Base64String to be shown appropriately in the
        //View. Next, ViewBags are used to send necessary data to the View.
        //Finally the necessary View is returned.
        public IActionResult BookDetails(int productID)
        {

            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();


            //TutorialsTeacher (2023) demonstrates how to work with the LINQ WHERE operator.
            var bookImage = from p in _context.Products
                            where p.ProductId == productID
                            select p.BookCoverImage;

            //Microsoft (2023) demonstrates how to work with anonymous types.
            var bookInfo = from p in _context.Products
                           where p.ProductId == productID
                           select new
                           {
                               p.ProductId,
                               p.Title,
                               p.Author,
                               p.Price,
                               p.InStock
                           };

            //Declares a List of type string to store the book cover image and of type object to store the respective
            //details. "object" is being used as bookDetails List holds a subset of the Product class.
            List<string> bookCovers = new List<string>();
            List<object> bookDetails = new List<object>();

            foreach (var book in bookImage)
            {
                //Microsoft (2024) demonstrates how to use the Convert.ToBase64String() method.
                string imgData = Convert.ToBase64String(book);
                bookCovers.Add(imgData);
            }

            foreach (var book in bookInfo)
            {
                bookDetails.Add(book);
            }

            ViewBag.ImageDataList = bookCovers;
            ViewBag.BookDetailList = bookDetails;

            return View("BookDetails");
        }

        //SearchBooks() method is an action method that takes in the search query entered in by the user which is either
        //the title of a book or part of the title of a book.
        //Retrieves the book cover image and its associated details that matches that search query.
        //These 2 items are added to separate lists for displaying purposes as the book cover image needs to be converted
        //to Base64String to be shown appropriately in the View. Next, ViewBags are used to send necessary data to the View.
        //Finally the necessary View is returned.

        public IActionResult SearchBooks(string searchQuery)
        {
            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //TutorialsTeacher (2023) demonstrates how to work with the LINQ WHERE operator.
            var searchBookImage = from p in _context.Products
                                  where p.Title.Contains(searchQuery)
                                  select p.BookCoverImage;

            //Microsoft (2023) demonstrates how to work with anonymous types.
            var searchBookInfo = from p in _context.Products
                                 where p.Title.Contains(searchQuery)
                                 select new
                                 {
                                     p.ProductId,
                                     p.Title,
                                     p.Author,
                                     p.Price,
                                     p.InStock
                                 };

            //Declares a List of type string to store the book cover image and of type object to store the respective
            //details. "object" is being used as bookDetails List holds a subset of the Product class.
            List<string> bookCovers = new List<string>();
            List<object> bookDetails = new List<object>();

            foreach (var book in searchBookImage)
            {
                //Microsoft (2024) demonstrates how to use the Convert.ToBase64String() method.
                string imgData = Convert.ToBase64String(book);
                bookCovers.Add(imgData);
            }

            foreach (var book in searchBookInfo)
            {
                bookDetails.Add(book);
            }

            ViewBag.ImageDataList = bookCovers;
            ViewBag.BookDetailList = bookDetails;

            return View("SearchBooks");
        }

        //AddBookToCart() method is an action method that retrieves the product id of the book selected and the
        //quantity of the book entered by user and first checks if the user has logged in through the session token.
        //Next, adds this book selected with its associated quantity chosen by the user to the Cart table and updates
        //the TotalAmount field accordingly (quantity * amount).
        //Then, finds the product that this user has selected, checks that this product does exist, and updates the InStock
        //field of this product accordingly (decreases the number in stock by quantity of book choosen by user).
        //Finally, redirects to the Home Page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookToCart(int productId, int quantity, int amount)
        {
            //FreeCode Spot (2020) demonstrates using the session token to authorise access to pages.
            var token = HttpContext.Session.GetString("UserLoggedIn");

            if (token != null)
            {
                //Creates an instance of the Cart class.
                Cart cartEntry = new Cart();

                //Assigns the user-selected book to appropriate properties in the Cart table.
                cartEntry.UserId = userLoggedIn();
                cartEntry.ProductId = productId;
                cartEntry.Quantity = quantity;
                cartEntry.TotalAmount = amount * quantity;

                //Adds the cart entry to the Cart table through calling the built-in Add() method.
                //Saves those changes to the database asynchronously.
                //Lastly, assigns a ViewBag the value true to indicate that this item has successfully been added
                //to the Cart table.
                if (ModelState.IsValid)
                {
                    _context.Add(cartEntry);
                    await _context.SaveChangesAsync();
                    ViewBag.ItemAdded = true;

                    //Microsoft (2024) demonstrates the FindAsync() method.
                    var product = await _context.Products.FindAsync(productId);

                    if (product != null)
                    {
                        product.InStock -= quantity;
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }


        //ViewCart() method is an action method that first checks if the user has logged in through the session token.
        //Next gets the book cover image and its associated details for all books that have been added to the user's cart.
        //These 2 items are added to separate lists for displaying purposes as the book cover image needs to be converted to
        //Base64String to be shown appropriately in the View. Next, ViewBags are used to send necessary data to the View.
        //Finally the necessary View is returned.
        public IActionResult ViewCart()
        {
            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //FreeCode Spot (2020) demonstrates using the session token to authorise access to pages.
            var token = HttpContext.Session.GetString("UserLoggedIn");

            if (token != null)
            {
                //Gets all the book cover image entries in the cart for user logged in.
                //Microsoft (2023) demonstrates how to work with anonymous types.
                var userCartImage = (from c in _context.Carts
                                     join p in _context.Products on c.ProductId equals p.ProductId
                                     where c.UserId == userLoggedIn()
                                     select new
                                     {
                                         BookImage = p.BookCoverImage
                                     }).ToList();

                //Gets all the book detail entries in the cart for user logged in.
                //Microsoft (2023) demonstrates how to work with anonymous types.
                var userCart = (from c in _context.Carts
                                join p in _context.Products on c.ProductId equals p.ProductId
                                where c.UserId == userLoggedIn()
                                select new
                                {
                                    BookId = p.ProductId,
                                    BookTitle = p.Title,
                                    BookPrice = c.TotalAmount
                                }).ToList();

                //Gets the total price of all the book items in the cart for user logged in.
                //Dot Net Tutorials (2024) demonstrates the LINQ Sum() method.
                var totalPrice = (from c in _context.Carts
                                  where c.UserId == userLoggedIn()
                                  select c.TotalAmount).Sum();

                List<string> cartImages = new List<string>();
                List<object> cartDisplay = new List<object>();

                foreach (var image in userCartImage)
                {
                    //Microsoft (2024) demonstrates how to use the Convert.ToBase64String() method.
                    string imgData = Convert.ToBase64String(image.BookImage);
                    cartImages.Add(imgData);

                }

                foreach (var item in userCart)
                {
                    cartDisplay.Add(item);
                }

                ViewBag.CartImageDisplay = cartImages;
                ViewBag.CartDetailDisplay = cartDisplay;
                ViewBag.TotalPrice = totalPrice;

                return View("ViewCart");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        //RemoveBookFromCart() method is an action method that retrieves the product id of the book selected and
        //removes this book selected with its associated details from the Cart table for the user.
        //Finally, returns the necessary View.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBookFromCart(int productId)
        {

            //Awaits and assigns the product details in the database to an appropriate variable,
            //but only for the selected product for the user logged in's cart.
            var @book = await (from c in _context.Carts
                               join p in _context.Products on c.ProductId equals p.ProductId
                               where c.UserId == userLoggedIn() && c.ProductId == productId
                               select c).FirstAsync();

            //IF statement checks if @book variable above is not null, and if so, deletes the book item
            //from the cart through using the Remove() method.
            if (@book != null)
            {
                _context.Carts.Remove(@book);
            }

            await _context.SaveChangesAsync();

            //Returning the view directly allows the user to view updates to their carts immediately.
            return RedirectToAction("Index", "Home");
        }

        //Checkout() is an action method that shows the Shipping Address page in which the user enters in their
        //shipping details before continuing to payment.
        public IActionResult Checkout()
        {
            int userID = userLoggedIn();

            //Checks if there any records in the ShippingAddress table for the user currently logged in.
            //CsharpTutorial (2023) demonstrates the LINQ Any() method.
            bool shipAddressExists = _context.ShippingAddresses.Any(s => s.CustomerId == userID);

            //IF statement checks the results of the boolean variable and if true, takes the user straight to the
            //Payment View. ELSE, prompts the user to enter in their shipping address details before continuing.
            if (shipAddressExists == true)
            {
                return RedirectToAction("ProcessPayment", "MockPayment");
            }
            else
            {
                return View("AddShippingAddress");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //AddShippingAddress() is an action method that takes in the necessary parameters for the ShippingAddress table and adds the shipping
        //address for the user logged in to the ShippingAddress table while performing error handling.
        //User email and Role ViewBags are added here so that the user's details appear in menu bar.
        //Once adding is successful, navigates to the Payment View.
        public async Task<IActionResult> AddShippingAddress(string AddressLine1, string AddressLine2, string City, string State, string PostalCode, string Country)
        {

            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //Creates an instance of the ShippingAddress class.
            ShippingAddress shippingAddress = new ShippingAddress();

            //Assigns the user-entered values to appropriate properties in the ShippingAddress table.
            shippingAddress.CustomerId = userLoggedIn();
            shippingAddress.AddressLine1 = AddressLine1;
            shippingAddress.AddressLine2 = AddressLine2;
            shippingAddress.City = City;
            shippingAddress.State = State;
            shippingAddress.PostalCode = PostalCode;
            shippingAddress.Country = Country;

            //Adds the shipping address to the ShippingAddress table through calling the built-in Add() method.
            //Lastly, saves those changes to the database asynchronously.
            _context.Add(shippingAddress);
            await _context.SaveChangesAsync();


            return RedirectToAction("ProcessPayment", "MockPayment");
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


        //Gets the email of the user currently logged in through getting the Firebase User UID of the user and performing
        //a LINQ query to retrieve the matching Email.
        public string userEmail()
        {
            var currentUser = HttpContext.Session.GetString("UserLoggedIn");
            var email = "";

            //IF statement checks if a user has indeed logged in first before getting the user's role.
            if (currentUser != null)
            {
                email = (from u in _context.Users
                         where u.UserId == userLoggedIn()
                         select u.Email).FirstOrDefault();
            }

            return email;

        }

        //Gets the role of the user currently logged in through getting the Firebase User UID of the user and performing
        //a LINQ query to retrieve the matching Role.
        public string userRole()
        {
            var role = "";
            //IF statement checks if a user has indeed logged in first before getting the user's role.
            if (!string.IsNullOrEmpty(ViewBag.UserLoggedIn))
            {
                //Gets the role of the user currently logged in.
                //
                role = (from u in _context.Users
                        join r in _context.Roles on u.RoleId equals r.RoleId
                        where u.UserId == userLoggedIn()
                        select r.RoleType).FirstOrDefault();
            }

            return role;
        }

        //Gets the ID of the user currently logged in through getting the Firebase User UID of the user and performing
        //a LINQ query to retrieve the matching UserId.
        public int userLoggedIn()
        {

            var userID = (from u in _context.Users
                          where u.FirebaseUid == HttpContext.Session.GetString("UserLoggedIn")
                          select u.UserId).First();

            return userID;
        }
    }
}
//REFERENCE LIST:
//Cull, B. 2016. Using Sessions and HttpContext in ASP.NET Core and MVC Core, 23 July 2016 (Version 1.0)
//[Source code] https://bencull.com/blog/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core
//(Accessed 28 February 2024).
//Dot Net Tutorials. 2024. LINQ Sum Method in C#, 2024 (Version 1.0)
//[Source code] https://dotnettutorials.net/lesson/linq-sum-method/
//(Accessed 5 March 2024).
//FreeCode Spot. 2020. How to Integrate Firebase in ASP NET Core MVC, 2020 (Version 1.0)
//[Source code] https://www.freecodespot.com/blog/firebase-in-asp-net-core-mvc/
//(Accessed 8 March 2024).
//Gideon. 2011. LINQ query to select top five. Stack Overflow, 2 February 2011 (Version 1.0)
//[Source code] https://stackoverflow.com/questions/4872946/linq-query-to-select-top-five
//(Accessed 26 January 2024).
//Microsoft. 2023. Anonymous types, 29 November 2023 (Version 1.0)
//[Source code]https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
//(Accessed 26 January 2024).
//Microsoft. 2024. Convert.ToBase64String Method (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.convert.tobase64string?view=net-8.0
//(Accessed 26 January 2024).
//Microsoft. 2024. DbSet.FindAsync Method, 2024 (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset.findasync?view=entity-framework-6.2.0
//(Accessed 10 March 2024).
//TutorialsTeacher. 2023. Filtering Operator - Where (Version 1.0)
//[Source code] https://www.tutorialsteacher.com/linq/linq-filtering-operators-where
//(Accessed 21 November 2023).