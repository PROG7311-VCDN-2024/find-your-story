using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FindYourStoryApp.Controllers
{
    public class UserController : Controller
    {
        //Declared a private field to store the DB context of Find Your Story.
        private readonly FindYourStoryDbContext _context;

        //The UserController Constructor() takes in an instance of the DB context and assigns it 
        //to the private field, _context.
        public UserController(FindYourStoryDbContext context)
        {
            _context = context;
        }


        //Register() is an action method that returns the relevant Register View to the user.
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Regsiter() is an action method that takes in the necessary parameters for the User table and adds the user to
        //the User table while performing error handling.
        //Once registration is successful, navigates to the Login View.
        public async Task<IActionResult> Register(string FirstName, string LastName, string Email, DateOnly Dob, string Username, string PasswordHash)
        {
            //Creates an instance of the User class.
            User user = new User();

            //Assigns the user-entered values to appropriate properties in the User table.
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.Dob = Dob;
            user.Username = Username;
            //Hashes the password for security purposes using BCrypt.
            //Code Maze (2022) demonstrates how to hash passwords using Bcrypt.
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordHash);

            //Adds the user to the User table through calling the built-in Add() method.
            //Lastly, saves those changes to the database asynchronously.
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Login", "User");
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //Login() is an action method that takes in the username and password parameters inputted by user and
        //verifies the username and password while performing error handling.
        public async Task<IActionResult> Login(string Username, string PasswordHash)
        {

            //Makes use of flags to keep track of whether the username and password is correct or not.
            bool usernameCorrect = false, passwordCorrect = false;
            //Holds the username of the user currently logged in.
            string userLoggedIn;

            //TutorialsTeacher (2023) demonstrates how to work with the LINQ SELECT operator.
            var DBUsernames = from u in _context.Users select u.Username;

            //FOREACH LOOP to loop through all the usernames in the database and first checks if the username passed in exists in 
            //the database. 
            //Next, checks if the password passed in matches this user's password in the database, and if so, logs in the user
            //and takes them to the Home page.
            foreach (var u in DBUsernames)
            {
                if (Username.Equals(u))
                {

                    usernameCorrect = true;

                    //TutorialsTeacher (2023) demonstrates how to work with the LINQ WHERE operator.
                    var DBPassword = from p in _context.Users where p.Username == Username select p.PasswordHash;

                    //Makes use of the Verify() method in the Bcrypt class to verify that the password passed in matches the password stored
                    //in the database.
                    //Also, makes use of the First() method to retrieve the password as a string and because only one row will be retrieved.
                    //Lastly, the necessary private variables are assigned their appropriate values.
                    //Code Maze (2022) demonstrates how to verify hashed passwords using Bcrypt.
                    if (BCrypt.Net.BCrypt.Verify(PasswordHash, DBPassword.First()))
                    {
                        passwordCorrect = true;
                        userLoggedIn = u;

                    }
                    else
                    {
                        passwordCorrect = false;

                    }//end of IF..ELSE STATEMENT.
                }
                else
                {
                    usernameCorrect = false;
                }//end of IF..ELSE STATEMENT.

            }//end of FOREACH LOOP.

            //Logs in the user by taking them to the Home page, or displays an appropriate error message to the user
            //based on the values of the 2 boolean variables.
            if (passwordCorrect)
            {
                //Stores the username of the user who has successfully logged in, in the session.
                HttpContext.Session.SetString("UserLoggedIn", Username);
                return RedirectToAction("Index", "Home");

            }
            else if (usernameCorrect && passwordCorrect == false)
            {
                ViewBag.Message = "The password is incorrect. Please try again.";
                return View();
            }
            else
            {
                ViewBag.Message = "The credentials are incorrect. Please try again.";
                return View();
            }
        }



    }
}
//REFERNCE LIST:
//Code Maze. 2022. How to secure passwords with BCrypt.NET, 19 December 2022 (Version 2.0)
//[Source code] https://code-maze.com/dotnet-secure-passwords-bcrypt/
//(Accessed 25 February 2024).
//TutorialsTeacher. 2023. Filtering Operator - Where (Version 1.0)
//[Source code] https://www.tutorialsteacher.com/linq/linq-filtering-operators-where
//(Accessed 25 February 2024).
//TutorialsTeacher. 2023. Projection Operators: Select, SelectMany (Version 1.0)
//[Source code] https://www.tutorialsteacher.com/linq/linq-projection-operators
//(Accessed 25 February 2024).