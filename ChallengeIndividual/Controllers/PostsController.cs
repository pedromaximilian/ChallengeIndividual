﻿using System;
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
            var dataContext = _context.Posts.Include(p => p.Category);
            return View(await dataContext.ToListAsync());
        }


        // GET: Posts
        public async Task<IActionResult> List()
        {
            var dataContext = _context.Posts.Include(p => p.Category);
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
                string uniqueFileName = UploadedFile(post);

                Post _post = new Post
                {
                    Title = post.Title,
                    Article = post.Article,
                    CategoryId = post.CategoryId,
                    Image = uniqueFileName
                };

                _context.Add(_post);
                await _context.SaveChangesAsync();
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
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostViewModel post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        private string UploadedFile(PostViewModel model)
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
    }
}