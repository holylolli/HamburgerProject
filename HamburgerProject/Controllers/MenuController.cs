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
            _db = db;
        }

       
        public IActionResult Index()
        {
            var menuList = _db.Menus.ToListAsync();
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
            {
                if (ModelState.IsValid)
                {
                    string? photoUrl = null;

                    // Fotoğraf var mı kontrolü
                    if (model.Photo != null)
                    {
                        // Dosya yolunu oluşturuyoruz (wwwroot dizinine kaydedilecek dosya yolu)
                        var filePath = Path.Combine("wwwroot/images", model.Photo.FileName);

                        // Fotoğrafı kaydediyoruz
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Photo.CopyToAsync(stream);  // Dosyayı kopyalıyoruz
                        }

                        // Fotoğrafın yolunu model.ImagePath'a atıyoruz
                        photoUrl = $"/images/{model.Photo.FileName}";
                        model.ImagePath = photoUrl;  // Fotoğraf yolunu modele ekliyoruz
                    }

                    // Modeli veri tabanına kaydediyoruz
                    await _db.Menus.AddAsync(model);
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            var edit = _db.Menus.FindAsync(id);

            if (edit == null)
            {
                return NotFound();
            }

            return View(edit);

        }

        [HttpPost]
        public async Task<IActionResult> EditMenu(int id,Menu model,IFormFile photo)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var editedMenu = await _db.Menus.FindAsync(id);

            if (editedMenu == null)
            {
                return NotFound();
            }

            // Eğer yeni bir fotoğraf yüklendiyse, önceki fotoğrafı silip yenisini kaydediyoruz
            if (photo != null && photo.Length > 0)
            {
                // Mevcut resmin fiziksel dosyasını silmek isterseniz, önceki resmi kaldırabilirsiniz.
                if (!string.IsNullOrEmpty(editedMenu.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", editedMenu.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath); // Eski resmi sil
                    }
                }

                // Yeni resmi kaydet
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                // Yeni resmi kaydetme işlemi
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Yeni resim yolu modelde güncelleniyor
                editedMenu.ImagePath = "/images/" + fileName;
            }

            // Menü bilgilerini güncelleme
            editedMenu.Name = model.Name;
            editedMenu.Description = model.Description;
            editedMenu.Price = model.Price;

            // Menü veritabanında güncelleniyor
            _db.Menus.Update(editedMenu);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");


        }
        [HttpGet]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var deleted = _db.Menus.FindAsync(id);

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

            var deletedItem = await _db.Menus.FindAsync(id);

            if (deletedItem == null)
            {
                return NotFound();                
            }
            
            _db.Menus.Remove(deletedItem);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }
    }
}
