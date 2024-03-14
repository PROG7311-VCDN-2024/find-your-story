using FindYourStoryApp.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FindYourStoryApp.Controllers
{
    public class UserController : Controller
    {
        //Declared private fields to store the DB context of Find Your Story and the FirebaseAuthProvider.
        //FreeCode Spot (2020) demonstrates declaring the FirebaseAuthProvider.
        private readonly FindYourStoryDbContext _context;
        FirebaseAuthProvider _authProvider;

        //Declared private fields to store the log information and the DB context of Find Your Story.
        private readonly ILogger<UserController> _logger;


        //The UserController Constructor() takes in an instance of the DB context and the FirebaseAuthProvider and
        //assigns them to appropriate private fields.
        public UserController(FindYourStoryDbContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;

            //Microsoft (2023) demonstrates how to use Configuration.
            string apiKey = configuration["Firebase:APIKey"];

            //FreeCode Spot (2020) demonstrates initializing the FirebaseAuthProvider and FirebaseConfig with the
            //web API key of our Firebase project.
            _authProvider = new FirebaseAuthProvider(
                new FirebaseConfig(apiKey));
            ;
        }


        //Register() is an action method that returns the relevant Register View to the user.
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Register() is an action method that takes in the necessary parameters for the User table, peforms Firebase authentication
        //to register the user, and adds the user to the User table while performing error handling.
        //Once registration is successful, navigates to the Home View.
        public async Task<IActionResult> Register(string FirstName, string LastName, string Email, DateOnly Dob, string Username, string Password)
        {
            //////////////////////////////////////FIREBASE AUTHENTICATION////////////////////////////////////////////////////
            //FreeCode Spot (2020) demonstrates registering a user for Firebase Authentication.
            //TRY...CATCH block handles any Firebase exceptions that occur during registration and login processes.
            try
            {
                //Registers the user in Firebase with their email and password. 
                await _authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);

                //Logs the user in through Firebase with their email and password.
                var authoriseFB = await _authProvider.SignInWithEmailAndPasswordAsync(Email, Password);

                //Stores the User UID returned by Firebase Authentication services after a successful login,
                //in the token variable.
                string token = authoriseFB.User.LocalId;


                //IF statment first checks if there indeed is a session token first, and if so,
                //saves the token in a session variable through calling the SetString() method.
                if (token != null)
                {
                    HttpContext.Session.SetString("UserLoggedIn", token);
                }
            }
            catch (FirebaseAuthException ex)
            {
                //Deserializes the Firebase Exception ResponseData into the FirebaseError object.
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);

                //Gets the error message and assigns it to the ViewBag.
                ViewBag.RegError = firebaseEx.error.message;
            }

            //////////////////////////////////////ADD USER TO DATABASE////////////////////////////////////////////////////
            //Creates an instance of the User class.
            Models.User user = new Models.User();

            //Assigns the user-entered values to appropriate properties in the User table.
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.Dob = Dob;
            user.Username = Username;
            //All users are set to have a role of id 2:customer, as only admin can make users admin.
            user.RoleId = 2;
            user.FirebaseUid = HttpContext.Session.GetString("UserLoggedIn");


            //Adds the user to the User table through calling the built-in Add() method.
            //Lastly, saves those changes to the database asynchronously.
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //Login() is an action method that takes in the username and password parameters inputted by user and
        //verifies the username and password while performing error handling through Firebase Authentication.
        public async Task<IActionResult> Login(string Email, string Password)
        {

            //TRY...CATCH block handles any Firebase exceptions that occur during login processes.
            //The catch clause performs the same operations as stated before in registration.
            //FreeCode Spot (2020) demonstrates logging in a user for Firebase Authentication.
            try
            {
                //Logs in an existing user through Firebase with their email and password.
                var authoriseFB = await _authProvider.SignInWithEmailAndPasswordAsync(Email, Password);

                //Stores the User UID returned by Firebase Authentication services after a successful login
                //in the token variable.
                string token = authoriseFB.User.LocalId;

                //IF statment first checks if there indeed is a session token first, and if so,
                //saves the token in a session variable through calling the SetString() method, and
                //redirects to the Home Page.
                if (token != null)
                {
                    HttpContext.Session.SetString("UserLoggedIn", token);

                    return RedirectToAction("Index", "Home");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ViewBag.LoginError = firebaseEx.error.message;
                //Calls the getFalseLogin() method to write false login attempts to textfile.
                getFalseLogin(Email);
            }

            return View();
        }

        //getFalseLogin() method takes in the email of the user attempting to login, and through using a TRY..CATCH clause,
        //makes use of a StreamWriter to write the false login attempts to a textfile, which includes the email of the user and
        //the date and time on which this attempt occurred.
        //If an error occurs, the exception message will be logged using the LogError() method.
        private void getFalseLogin(string email)
        {
            //Wells (2019) demonstrates how to write and append to a textfile.
            try
            {
                using (StreamWriter writer = new StreamWriter("FalseLoginAttempts.txt", append: true))
                {
                    writer.WriteLine($"A False Login Occurred By {email} On {DateTime.Now}");
                }

            }
            //Microsoft (2024) demonstrates the LogError() method.
            catch (Exception ex)
            {
                _logger.LogError($"An error occured logging a false login: {ex.Message}");
            }
        }

        //UserProfile() is an action method that first retrieves the id of the user currently logged in.
        //Next, gets the details of that user through using the FindAsync() method and finally returns the
        //necessary View.
        //Error handling is performed where necessary.
        public async Task<IActionResult> UserProfile()
        {
            int id = userLoggedIn();

            if (id == null)
            {
                return NotFound();
            }

            //Microsoft (2024) demonstrates the FindAsync() method.
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            };

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile([Bind("UserId,FirstName,LastName,Dob,Email,Username,RoleId,FirebaseUid")] Models.User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            ViewBag.Alert = "User Updated!";
            return View("UserProfile");

        }

        //Logout() method is an action method that logs out user be removing the session token and
        //redirects the user back to the original Home page.
        public IActionResult Logout()
        {
            //FreeCode Spot (2020) demonstrates logging out a user.
            HttpContext.Session.Remove("UserLoggedIn");
            return RedirectToAction("Index", "Home");
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
//REFERNCE LIST:
//Code Maze. 2022. How to secure passwords with BCrypt.NET, 19 December 2022 (Version 2.0)
//[Source code] https://code-maze.com/dotnet-secure-passwords-bcrypt/
//(Accessed 25 February 2024).
//Cull, B. 2016. Using Sessions and HttpContext in ASP.NET Core and MVC Core, 23 July 2016 (Version 1.0)
//[Source code] https://bencull.com/blog/using-sessions-and-httpcontext-in-aspnetcore-and-mvc-core
//(Accessed 28 February 2024).
//FreeCode Spot. 2020. How to Integrate Firebase in ASP NET Core MVC, 2020 (Version 1.0)
//[Source code] https://www.freecodespot.com/blog/firebase-in-asp-net-core-mvc/
//(Accessed 8 March 2024).
//Microsoft. 2023. Configuration in ASP.NET Core, 9 November 2023 (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0
//(Accessed 12 March 2024).
//Microsoft. 2024. DbSet.FindAsync Method, 2024 (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbset.findasync?view=entity-framework-6.2.0
//(Accessed 10 March 2024).
//Microsoft. 2024. LoggerExtensions.LogError Method, 2024 (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions.logerror?view=dotnet-plat-ext-8.0
//(Accessed 29 February 2024)
//TutorialsTeacher. 2023. Filtering Operator - Where (Version 1.0)
//[Source code] https://www.tutorialsteacher.com/linq/linq-filtering-operators-where
//(Accessed 25 February 2024).
//TutorialsTeacher. 2023. Projection Operators: Select, SelectMany (Version 1.0)
//[Source code] https://www.tutorialsteacher.com/linq/linq-projection-operators
//(Accessed 25 February 2024).
//Wells, B. 2019. Write to Text File C#, 17 June 2019 (Version 1.0)
//[Source code] https://wellsb.com/csharp/beginners/write-to-text-file-csharp
//(Accessed 29 February 2024)