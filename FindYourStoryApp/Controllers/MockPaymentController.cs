using System.Globalization;
using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindYourStoryApp.Controllers
{
    public class MockPaymentController : Controller
    {
        //Declared a private field to store the DB context of Find Your Story.
        private readonly FindYourStoryDbContext _context;

        //The OrderController Constructor() takes in an instance of the DB context and assigns it 
        //to the private field, _context.
        public MockPaymentController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        //ProcessPayment() method is an action method that first retrieves the total cart amount for
        //the user currently logged in and through using object initialization, sets the Amount value of 
        //the payment to that total cart amount.
        //User email and Role ViewBags are added here so that the user's details appear in menu bar.
        //Lastly passes this specific mockPayment with this specific total cart amount to the View.
        public IActionResult ProcessPayment()
        {
            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //Gets the total price of all the book items in the cart for user logged in.
            //Dot Net Tutorials (2024) demonstrates the LINQ Sum() method.
            var totalPrice = (from c in _context.Carts
                              where c.UserId == userLoggedIn()
                              select c.TotalAmount).Sum();


            var mockPayment = new MockPayment
            {
                Amount = totalPrice
            };

            return View("ProcessPayment", mockPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //ProcessPayment() is an action method that takes in the necessary parameters for the MockPayment class and performs
        //error handling before showing the PaymentSuccessful View.
        //Lastly, passes an error message to the View and redirects to the ProcessPayment View.
        public IActionResult ProcessPayment(string CardNumber, string ExpiryDate, string CVVDigits, decimal Amount)
        {

            if (ModelState.IsValid)
            {
                if (ValidExpiryDate(ExpiryDate) == true)
                {

                    return View("PaymentSuccessful");
                }

            }

            ViewBag.ErrorMessage = "Payment was not successful. Please check payment details.";
            return RedirectToAction("ProcessPayment", "MockPayment");

        }

        //ValidExpiryDate() method makes sure that the expiry date entered in by user is in the correct "MM/YY" format.
        public bool ValidExpiryDate(string expDate)
        {
            //Microsoft (2024) demonstrates the DateTime.TryParseExact() method.
            DateTime validDate;
            return DateTime.TryParseExact(expDate, "MM/yy", null, DateTimeStyles.None, out validDate); ;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //CheckOrders() is an action method that adds the order to the Order table after successful payment has been made and
        //after a cart has been selected and checked out.
        //Next removes all book items from the user currently logged in's cart through using the RemoveAllFromCart() method.
        //Lastly, returns the Order Page.
        public async Task<IActionResult> CheckOrders()
        {
            //Creates an instance of the Order class and MockPayment class.
            Order order = new Order();

            //Gets the total price of all the book items in the cart for user logged in.
            //Dot Net Tutorials (2024) demonstrates the LINQ Sum() method.
            var totalPrice = (from c in _context.Carts
                              where c.UserId == userLoggedIn()
                              select c.TotalAmount).Sum();

            //Gets the shipping address ID for the user currently logged in.
            var shipAddressId = (from s in _context.ShippingAddresses
                                 where s.CustomerId == userLoggedIn()
                                 select s.AddressId).FirstOrDefault();

            //Assigns the necessary order details to appropriate properties in the Order table.
            order.CustomerId = userLoggedIn();
            order.OrderDate = DateTime.Now;
            order.TotalAmount = totalPrice;
            //Assigning payment to successful as doing a mock payment so will always be successful.
            order.PaymentStatus = "Successful";
            order.ShippingAddressId = shipAddressId;

            //Adds the order entry to the Order table through calling the built-in Add() method.
            //Saves those changes to the database asynchronously.
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
            }

            await RemoveAllFromCart();

            return RedirectToAction("ViewOrder", "Order");
        }



        //RemoveAllFromCart() method is an action method that retrieves a list of all the books that have been added to the
        //user currently logged in's cart and removes all these entries (in essence this cart) so that the user's cart is
        //refreshed and a new cart with a new order can be made.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task RemoveAllFromCart()
        {

            //Awaits and assigns the product details in the database to an appropriate variable,
            //but only for the selected product for the user logged in's cart.
            var allBooks = await (from c in _context.Carts
                                  where c.UserId == userLoggedIn()
                                  select c).ToListAsync();

            //IF statement checks if the allBooks variable holds any books, and if so, uses a
            //FOREACH LOOP to delete each book from the cart for the user logged in through using
            //the Remove() method and saves the changes to the database.
            if (allBooks.Any())
            {
                foreach (var book in allBooks)
                {
                    _context.Carts.Remove(book);
                }
                await _context.SaveChangesAsync();
            }
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
//Dot Net Tutorials. 2024. LINQ Sum Method in C#, 2024 (Version 1.0)
//[Source code] https://dotnettutorials.net/lesson/linq-sum-method/
//(Accessed 5 March 2024).
//Microsoft. 2024. DateTime.TryParseExact Method, 2024 (Version 1.0).
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tryparseexact?view=net-8.0
//(Accessed 5 March 2024).