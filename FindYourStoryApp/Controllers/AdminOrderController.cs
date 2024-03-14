using FindYourStoryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FindYourStoryApp.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly FindYourStoryDbContext _context;

        public AdminOrderController(FindYourStoryDbContext context)
        {
            _context = context;
        }

        // GET: AdminOrder
        public async Task<IActionResult> Index()
        {
            var findYourStoryDbContext = _context.Orders.Include(o => o.Customer).Include(o => o.ShippingAddress);

            return View(await findYourStoryDbContext.ToListAsync());
        }

        // GET: AdminOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: AdminOrder/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "AddressId", "AddressId");
            return View();
        }

        // POST: AdminOrder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,PaymentStatus,ShippingAddressId,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "UserId", order.CustomerId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "AddressId", "AddressId", order.ShippingAddressId);
            return View(order);
        }

        // GET: AdminOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "UserId", order.CustomerId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "AddressId", "AddressId", order.ShippingAddressId);
            return View(order);
        }

        // POST: AdminOrder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,PaymentStatus,ShippingAddressId,TotalAmount")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "UserId", order.CustomerId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "AddressId", "AddressId", order.ShippingAddressId);
            return View(order);
        }

        // GET: AdminOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: AdminOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
