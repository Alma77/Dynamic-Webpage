using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using HW_06.Models;
using HW_06.Services;
using HW_06.Data;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HW_06.Controllers
{
    public class ContentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly string uploadsPath;

        public ContentController(ApplicationDbContext context,
                                 IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
            uploadsPath = Path.Combine(hostingEnvironment.WebRootPath, "img");
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityDataService.Policy_Add)]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Footer,Url,Quote")] CMSPage cmsPage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cmsPage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cmsPage);
        }

        [Authorize(Policy = MyIdentityDataService.Policy_Edit)]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            var view = View(page);
            view.ViewData["images"] = Directory.GetFiles(uploadsPath).Select(f =>
            {
                var file = new FileInfo(f);
                return "webroot/img/" + file.Name;
            });

            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityDataService.Policy_Edit)]
        public async Task<IActionResult> Update(int id,[Bind("Id,Title,Body,Footer,Url,Image,Quote")] CMSPage page, IFormFile image)
        {
            if (id != page.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                        if (file.Length > 0)
                        {
                            var fileName = $"{CMSPage.MakeFriendly(Path.GetFileNameWithoutExtension(file.FileName))}{Path.GetExtension(file.FileName)}";
                            using (var fileStream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            page.Image = fileName;
                        }
                    }

                    var realPage = await _context.Pages.FindAsync(id);

                    realPage.Title = page.Title;
                    realPage.Footer = page.Footer;
                    realPage.Body = page.Body;
                    realPage.Url = page.Url;
                    realPage.Quote = page.Quote;

                    if (page.Image != null)
                    {
                        realPage.Image = page.Image;
                    }

                    _context.Update(realPage);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if(!PageExists(page.Id))
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

            return View(page);
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var file = image;
                if (file.Length > 0)
                {
                    var fileName = $"{CMSPage.MakeFriendly(Path.GetFileNameWithoutExtension(file.FileName))}.{Path.GetExtension(file.FileName)}";
                    using (var fileStream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            return View("Update");
        }

        public bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.Id == id);
        }
    }
}
