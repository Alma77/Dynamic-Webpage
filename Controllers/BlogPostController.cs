using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HW_06.Data;
using HW_06.Models;
using HW_06.Services;
using System.Text;
using PusherServer;
using System.Web;

namespace HW_06.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Comments)
                .ToArrayAsync();

            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }


            return View(blogPost);
        }

        public async Task<IActionResult> Comment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id) ;

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityDataService.Policy_Add)]
        public async Task<IActionResult> Comment(int postId, string userName, string commentBody)
        {
            var comment = new Comment();

            var post = await _context.BlogPosts.FindAsync(postId);
            comment.BlogPost = post;
            comment.Body = commentBody;
            comment.UserName = userName;

            if(ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(comment);
        }

        public IActionResult Post()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityDataService.Policy_Add)]
        public async Task<IActionResult> Post([Bind("Id,Title,Body,TimePosted")] BlogPost blogPost)
        {
            if (ModelState.IsValid) {
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blogPost);
        }

        //How does the controller connect the two edit actions
        [Authorize(Policy = MyIdentityDataService.Policy_Edit)]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyIdentityDataService.Policy_Edit)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,TimePosted")] BlogPost blogPost)
        {

            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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

            return View(blogPost);
        }

        [Authorize(Policy = MyIdentityDataService.Policy_Delete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Why not use .FindAsync(id);
            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}