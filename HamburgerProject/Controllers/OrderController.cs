using HamburgerProject.Areas.Identity.Data;
using HamburgerProject.Data;
using HamburgerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Security.Claims;

namespace HamburgerProject.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly HamburgerProjectContext _db;
       

        public OrderController(HamburgerProjectContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

       
        public IActionResult Index()
        {
            return View();
        }

      
        [HttpGet]
        public IActionResult CreateOrder()
        {
            ViewBag.Menus = _db.Menus.AsNoTracking().ToList();
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(Order model, List<int> orderExtraIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Menus = _db.Menus.AsNoTracking().ToList();
                ViewBag.OrderExtras = _db.OrderExtras.AsNoTracking().ToList();
                return View(model);
            }

            var menu = await _db.Menus.FirstOrDefaultAsync(x => x.Id == model.MenuId);

            if (menu == null)
            {

                ModelState.AddModelError("", "Selected Menu Does not exist!");
                ViewBag.OrderExtras = _db.OrderExtras.ToList();
                ViewBag.Menus = _db.Menus.ToList();
                return View(model);
            }

            decimal totalPrice;

            model.OrderDate = DateTime.Now;
            totalPrice = model.Quantity * menu.Price;

            List<OrderExtra> orderExtras = new List<OrderExtra>();

            if (orderExtraIds != null && orderExtraIds.Any())
            {
                var extras = await _db.Extras.Where(x => orderExtraIds.Contains(x.Id)).ToListAsync();

                foreach (var extra in extras)
                {
                    model.OrderExtras.Add(new OrderExtra
                    {                         
                        Extra = extra,
                        OrderId = model.Id,                        

                    });

                    totalPrice += extra.Price + model.Quantity;
                }
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            model.ApplicationUserId = userId;

            model.TotalPrice = totalPrice;
            model.OrderDate = DateTime.Now;

            _db.Orders.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

            
        }
        
        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            
            var order = await _db.Orders.Include(x=>x.Menu).Include(oe=>oe.OrderExtras).FirstOrDefaultAsync(x=>x.Id==id);         
           

            if (order == null)
            {                        
                TempData["ErrorMessage"] = "The order you are trying to edit is not exist!";
                return RedirectToAction("Index");
            }

            ViewBag.Menus = await _db.Menus.AsNoTracking().ToListAsync();
            ViewBag.Orders = await _db.Orders.AsNoTracking().ToListAsync();

            return View(order);


        }
        [HttpPost]
        
        public async Task<IActionResult> EditOrder(Order model,int id,List<int> orderExtraIds)
        {
            if (ModelState.IsValid)
            {
                var order = await _db.Orders.Include(oe => oe.OrderExtras).FirstOrDefaultAsync(eo => eo.Id == model.Id);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "The order you are trying to edit is not exist!";
                    return RedirectToAction("Index");
                }
                order.OrderDate = DateTime.Now;
                order.Quantity = model.Quantity;
                order.TotalPrice = model.TotalPrice;
                order.MenuId = model.MenuId;
                order.OrderExtras.Clear();



                if (orderExtraIds != null && orderExtraIds.Any())
                {
                    var selectedExtras = await _db.Extras.Where(d => orderExtraIds.Contains(d.Id)).ToListAsync();
                    foreach (var extra in selectedExtras)
                    {
                        order.OrderExtras.Add(new OrderExtra
                        {
                            Extra = extra
                        });
                        order.TotalPrice = extra.Price * model.Quantity;
                    }
                }
                _db.Update(order);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "There were errors in the form. Please check the input!";
            ViewBag.Menus = await _db.Menus.ToListAsync();
            ViewBag.OrderExtras = await _db.OrderExtras.ToListAsync();
            return View(model);

        }
        [HttpGet]
        
        public async Task<IActionResult> DeleteOrder(int id)
        {

            var deletedItem = await _db.Orders.Include(m => m.Menu).FirstOrDefaultAsync(x => x.Id == id);
                
                if (deletedItem == null)
                {
                    TempData["ErrorMessage"] = "There is no order you trying to delete!";
                    return RedirectToAction("Index");
                }                
   
            return View(deletedItem);
        }
        
        [HttpPost]        
        public async Task<IActionResult> DeleteOrderConfirmed(int id)
        {
            var order = await _db.Orders.FindAsync(id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "The order you are trying to cancel is not exist!";
                return RedirectToAction("Index");
            }
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order Deleted Successfully!";

            return RedirectToAction("Index");
        }
    }
}
