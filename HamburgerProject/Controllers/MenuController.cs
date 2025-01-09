using HamburgerProject.Data;
using HamburgerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HamburgerProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MenuController : Controller
    {
        private readonly HamburgerProjectContext _db;

        public MenuController(HamburgerProjectContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuList = await _db.Menus.AsNoTracking().ToListAsync(); //Task yapmak zorunludur çünkü post metodu da task!
            return View(menuList);
        }
        [HttpGet]
        public IActionResult CreateMenu()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMenu(Menu model)
        {
            ModelState.Remove("Orders");

            if (ModelState.IsValid)
            {
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.Photo.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Photo", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                        return View(model);
                    }

                    if (model.Photo.Length > 2 * 1024 * 1024) // 2 MB sınırı
                    {
                        ModelState.AddModelError("Photo", "File size cannot exceed 2 MB.");
                        return View(model);
                    }

                    var filePath = Path.Combine("wwwroot/images", model.Photo.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }
                    model.ImagePath = $"~/images/{model.Photo.FileName}";
                    await _db.Menus.AddAsync(model);
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "Menu");
                }             
         

            }

            return View(model);


        }
        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            var edit = await _db.Menus.FindAsync(id);

            if (edit == null)
            {
                return NotFound();
            }

            return View(edit);

        }

        [HttpPost]
        public async Task<IActionResult> EditMenu(int id,Menu model)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var editedMenu = await _db.Menus.FindAsync(id);

            if (editedMenu == null)
            {
                return NotFound();
            }

            // Eğer yeni bir fotoğraf yüklendiyse, önceki fotoğrafı silip yenisini kaydediyoruz
            if (model.Photo != null && !string.IsNullOrEmpty(model.Photo.FileName))
            {
                // Mevcut resmin fiziksel dosyasını silmek isterseniz, önceki resmi kaldırabilirsiniz.
   
                    var filePath = Path.Combine("wwwroot/images", model.Photo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }
                editedMenu.ImagePath = $"~/images/{model.Photo.FileName}";

                if (!string.IsNullOrEmpty(model.Name))
                {
                     editedMenu.Name = model.Name;
                }
                if (editedMenu.Price > 0)
                {
                    editedMenu.Price = model.Price;
                }
                if (!string.IsNullOrEmpty(model.Name))
                {
                    editedMenu.Description = model.Description;
                }
            }
            _db.Menus.Update(editedMenu);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index","Menu");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var deleted = await _db.Menus.FindAsync(id);

            if (deleted == null)
            {
                return NotFound();
                
            }
            return View(deleted);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuConfirmed(int id)
        {
            var deletedItem =await  _db.Menus.FindAsync(id);

            if (deletedItem == null)
            {
                return NotFound();                
            }

            _db.Menus.Remove(deletedItem);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index","Menu");

        }
    }
}
