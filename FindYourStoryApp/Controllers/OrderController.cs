using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindYourStoryApp.Controllers
{
    public class OrderController : Controller
    {

        //Declared a private field to store the DB context of Find Your Story.
        private readonly FindYourStoryDbContext _context;

        //The OrderController Constructor() takes in an instance of the DB context and assigns it 
        //to the private field, _context.
        public OrderController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        //ViewOrder() method is an action method that gets the associated details for all the orders that have
        //been added by the user logged in. Next, a ViewBag is used to send necessary order data to the View.
        //User email and Role ViewBags are added here so that the user's details appear in menu bar.
        //Finally the necessary View is returned.
        public IActionResult ViewOrder()
        {
            //Assigns the user currently logged in's email and role to ViewBags.
            ViewBag.UserLoggedIn = userEmail();
            ViewBag.UserRole = userRole();

            //FreeCode Spot (2020) demonstrates using the session token to authorise access to pages.
            var token = HttpContext.Session.GetString("UserLoggedIn");

            if (token != null)
            {

                //Gets all the order detail entries for user logged in.
                //Microsoft (2023) demonstrates how to work with anonymous types.
                var userOrders = (from o in _context.Orders
                                  join s in _context.ShippingAddresses on o.ShippingAddressId equals s.AddressId
                                  where o.CustomerId == userLoggedIn()
                                  select new
                                  {
                                      OrderID = o.OrderId,
                                      OrderDateAndTime = o.OrderDate,
                                      OrderAmount = o.TotalAmount,
                                      OrderStatus = o.PaymentStatus,
                                      OrderAddress = $"{s.AddressLine1};{s.AddressLine2};{s.City};{s.Country}"
                                  }).ToList();

                ViewBag.OrderDetailDisplay = userOrders;
                return View("ViewOrder");
            }
            else
            {
                return RedirectToAction("Login", "User");
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
//Cull, B. 2016. Using Sessions and HttpContext in ASP.NET Core and MVC Core, 23 July 2016 (Version 1.0)
//[Source code] https://bencull.com/blog/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core
//(Accessed 28 February 2024).
