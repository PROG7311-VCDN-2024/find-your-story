using FindYourStoryApp.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FindYourStoryApp.Controllers
{
    public class AdminUserController : Controller

    {
        //Declared private fields to store the DB context of Find Your Story and the FirebaseAuthProvider.
        //FreeCode Spot (2020) demonstrates declaring the FirebaseAuthProvider.
        private readonly FindYourStoryDbContext _context;
        FirebaseAuthProvider _authProvider;

        public AdminUserController(FindYourStoryDbContext context)
        {
            _context = context;
            //FreeCode Spot (2020) demonstrates initializing the FirebaseAuthProvider and FirebaseConfig with the
            //web API key of our Firebase project.
            _authProvider = new FirebaseAuthProvider(
                new FirebaseConfig("AIzaSyDN5ZLELI3vBMkUQgECLLiDsoVXRPnc0qQ"));
        }

        //Index() method is an action method that based on the role of the user logged in shows different priviledges:
        //If customer (not admin): can only view, and edit their own profile.
        //If admin: can perform all processes for all users registered with Find Your Story.
        // GET: AdminUser
        public async Task<IActionResult> Index()
        {
            var findYourStoryDbContext = _context.Users.Include(u => u.Role);
            return View(await findYourStoryDbContext.ToListAsync());
        }

        // GET: AdminUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: AdminUser/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: AdminUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string FirstName, string LastName, string Email, DateOnly Dob, string Username, string Password, int RoleId)
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
            user.RoleId = RoleId;
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

        // GET: AdminUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // POST: AdminUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Dob,Email,Username,RoleId,FirebaseUid")] Models.User user)
        {

            if (id != user.UserId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: AdminUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
