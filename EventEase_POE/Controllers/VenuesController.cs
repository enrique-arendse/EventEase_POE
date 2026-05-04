using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase_POE.Data;
using EventEase_POE.Models;
using EventEase_POE.Service;
using Microsoft.AspNetCore.Http;

namespace EventEase_POE.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;

        public VenuesController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View(new VenueUploadViewModel());
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueUploadViewModel model)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            // Validate image if provided
            if (model.ImageFile != null && !_blobStorageService.IsValidImageFile(model.ImageFile))
            {
                ModelState.AddModelError("ImageFile", "Invalid image file. Please upload a valid image (JPG, PNG, GIF, WebP) up to 5MB.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? imageUrl = null;
                    // Upload image to Azure Blob Storage if provided
                    if (model.ImageFile != null)
                    {
                        imageUrl = await _blobStorageService.UploadFileToBlobAsync(model.ImageFile, model.ImageFile.FileName);
                    }

                    var venue = new Venue
                    {
                        VenueName = model.VenueName,
                        Location = model.Location,
                        Capacity = model.Capacity,
                        ImageUrl = imageUrl
                    };

                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            var model = new VenueUploadViewModel
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Location = venue.Location,
                Capacity = venue.Capacity,
                ImageUrl = venue.ImageUrl
            };

            return View(model);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Delete image from Azure Blob Storage if exists
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    await _blobStorageService.DeleteFileFromBlobAsync(venue.ImageUrl);
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
