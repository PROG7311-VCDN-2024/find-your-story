using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindYourStoryApp.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly FindYourStoryDbContext _context;

        public AdminProductController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        // GET: AdminProduct
        //Index() method is an action method that converts the book cover image to Base64 String and uses a ViewModel:
        //ProductViewModel to display each image along with its associated data in the table view.
        //Laslty, returns this new ProductViewModel to the View.
        public async Task<IActionResult> Index()
        {

            //Microsoft (2023) demonstrates how to work with anonymous types.
            var productDetails = (from p in _context.Products
                                  select new ProductViewModel
                                  {
                                      ProductId = p.ProductId,
                                      //Microsoft (2024) demonstrates how to use the Convert.ToBase64String() method.
                                      Base64BookCover = Convert.ToBase64String(p.BookCoverImage),
                                      Title = p.Title,
                                      Author = p.Author,
                                      Price = p.Price,
                                      InStock = p.InStock
                                  })
                                   .ToList();

            return View(productDetails);
        }

        // GET: AdminProduct/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: AdminProduct/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Title,Author,Price,InStock")] Product product, IFormFile imageUpload)
        {

            //Milish (2023) demonstrates how to add images via upload.
            //IF statement checks that an image file has been uploaded and has content (there is indeed an image).
            if (imageUpload != null && imageUpload.Length > 0)
            {
                //Creates a MemoryStream Object to store the contents of the image file. 
                //Copies the content of the image file to the MemoryStream object through using the CopyToAsync() method.
                //Finally, sets the BookCoverImage field to a byte array of the uploaded image through using the ToArray() method
                //so that this image can be stored in the database.
                using (var memoryStream = new MemoryStream())
                {
                    await imageUpload.CopyToAsync(memoryStream);
                    product.BookCoverImage = memoryStream.ToArray();
                }
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: AdminProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: AdminProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,BookCoverImage,Title,Author,Price,InStock")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: AdminProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
//REFERENCE LIST:
//Microsoft. 2023. Anonymous types, 29 November 2023 (Version 1.0)
//[Source code]https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
//(Accessed 26 January 2024).
//Microsoft. 2024. Convert.ToBase64String Method (Version 1.0)
//[Source code] https://learn.microsoft.com/en-us/dotnet/api/system.convert.tobase64string?view=net-8.0
//(Accessed 26 January 2024).\
//Milish, H. 2023. Uploading Files in ASP.NET Core 6 MVC. Devart, 24 March 2023 (Version 1.0)
//[Source code] https://blog.devart.com/uploading-files-in-asp-net-core-6-mvc.html
//(Accessed 12 March 2024).