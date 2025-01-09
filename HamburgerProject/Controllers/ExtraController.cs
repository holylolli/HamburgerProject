using HamburgerProject.Data;
using HamburgerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace HamburgerProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ExtraController : Controller
    {
        private readonly HamburgerProjectContext _db;

        public ExtraController(HamburgerProjectContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var getAll = await _db.Extras.AsNoTracking().ToListAsync();
            return View(getAll);
        }
        [HttpGet]
        public async Task<IActionResult> CreateExtra()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateExtra(Extra model)
        {
            ModelState.Remove("OrderExtras");

            if (ModelState.IsValid)
            {
               await _db.Extras.AddAsync(model);
               await _db.SaveChangesAsync();
               return RedirectToAction("Index","Extra");
            }
            return View(model);
            
        }
        [HttpGet]
        public async Task<IActionResult> EditExtra(int id)
        {
            
            var editedExtra = await _db.Extras.FindAsync(id);

            if (editedExtra == null)
            {
                return NotFound();    
            }
            return View(editedExtra);
        }
        [HttpPost]
       
        public async Task<IActionResult> EditExtra(int id,Extra model)
        {
            ModelState.Remove("OrderExtras");
            var  editedExtra = await _db.Extras.FindAsync(id);

            if (editedExtra == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                editedExtra.Name = model.Name;
            }
            if (editedExtra.Price > 0)
            {
                editedExtra.Price = model.Price;
            }
            if (ModelState.IsValid)
            {
                _db.Extras.Update(editedExtra);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Extra");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteExtra(int id)
        {
            var deletedItem = await _db.Extras.FindAsync(id);

            if (deletedItem == null)
            {
                return NotFound();
            }
            return View(deletedItem);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExtraConfirmed(int id)
        {           
            var deletedExtra = await _db.Extras.FindAsync(id);

            if (deletedExtra == null)
            {
                return NotFound();

            }

            _db.Extras.Remove(deletedExtra);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index","Extra");
        }
       
    }

}
