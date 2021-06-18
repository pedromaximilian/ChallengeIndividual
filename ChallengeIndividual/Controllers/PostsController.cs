using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChallengeIndividual.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ChallengeIndividual.Models.ViewModels;

namespace ChallengeIndividual.Controllers
{
    public class PostsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostsController(DataContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _environment = enviroment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Posts.Include(p => p.Category).Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt);
            return View(await dataContext.ToListAsync());
        }


        // GET: Posts
        public async Task<IActionResult> List()
        {
            var dataContext = _context.Posts.Include(p => p.Category).Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt);
            return View(await dataContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadFile(post);

                Post _post = new Post
                {
                    Title = post.Title,
                    Article = post.Article,
                    CategoryId = post.CategoryId,
                    Image = uniqueFileName,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Add(_post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post Created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.DeletedAt != null)
            {
                TempData["Error"] = "Post not found";
                return RedirectToAction(nameof(Index));
            }
            PostViewModel _post = new PostViewModel
            {
                Title = post.Title,
                Article = post.Article,
                CategoryId = post.CategoryId,
                CreatedAt = DateTime.UtcNow,
            };
            if (post.Image !=null)
            {
                ViewBag.Image = post.Image;
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(_post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostViewModel post)
        {
            if (id != post.Id )
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var _post = await _context.Posts.FindAsync(id);
                if (_post == null || _post.DeletedAt != null)
                {
                    return NotFound();
                }


                _post.Title = post.Title;
                _post.Article = post.Article;
                _post.CategoryId = post.CategoryId;
                if (post.Image != null)
                {
                    string uniqueFileName = UploadFile(post);
                    _post.Image = uniqueFileName;
                }
                
                try
                {
                    _context.Update(_post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Success"] = "Post edited successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Post deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Search(string query)
        {
            var postList = _context.Posts.Include(p => p.Category).Where(x => x.DeletedAt == null).OrderByDescending(x => x.CreatedAt).ToList();

            if (query != null)
            {
                postList = postList.Where(x => x.Title.ToLower().Contains(query.ToLower()) || x.Article.ToLower().Contains(query.ToLower())).OrderByDescending(x => x.CreatedAt).ToList(); 
            }

            if (postList.Count > 0)
            {
                return View("List", postList);
            }
            else 
            {
                ViewBag.ErrorBusqueda = "No se encontraron resultados";
                return View("List", null);
            }
        }

        // Upload files
        private string UploadFile(PostViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // Delete files
        public bool DeleteFile(string imageName)
        {
            try
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var uniqueFileName = imageName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

    }
}
