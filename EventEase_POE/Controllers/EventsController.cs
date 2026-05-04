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
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;

        public EventsController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            return View(new EventUploadViewModel());
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventUploadViewModel model)
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
                    if (model.ImageFile != null)
                    {
                        imageUrl = await _blobStorageService.UploadFileToBlobAsync(model.ImageFile, model.ImageFile.FileName);
                    }

                    var @event = new Event
                    {
                        EventName = model.EventName,
                        EventDate = model.EventDate,
                        Description = model.Description,
                        ImageUrl = imageUrl
                    };

                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event created successfully!";
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

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var model = new EventUploadViewModel
            {
                EventId = @event.EventId,
                EventName = @event.EventName,
                EventDate = @event.EventDate,
                Description = @event.Description,
                ImageUrl = @event.ImageUrl
            };

            return View(model);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventUploadViewModel model)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            if (id != model.EventId)
            {
                return NotFound();
            }

            // Validate image if provided
            if (model.ImageFile != null && !_blobStorageService.IsValidImageFile(model.ImageFile))
            {
                ModelState.AddModelError("ImageFile", "Invalid image file. Please upload a valid image (JPG, PNG, GIF, WebP) up to 5MB.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var @event = await _context.Events.FindAsync(id);
                    if (@event == null)
                    {
                        return NotFound();
                    }

                    @event.EventName = model.EventName;
                    @event.EventDate = model.EventDate;
                    @event.Description = model.Description;

                    // Handle image upload/update
                    if (model.ImageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(@event.ImageUrl))
                        {
                            await _blobStorageService.DeleteFileFromBlobAsync(@event.ImageUrl);
                        }
                        // Upload new image
                        @event.ImageUrl = await _blobStorageService.UploadFileToBlobAsync(model.ImageFile, model.ImageFile.FileName);
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(model.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            // Check if event has active bookings
            var hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has active bookings. Please remove associated bookings first.";
                return View(@event);
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                // Delete image from Azure Blob Storage if exists
                if (!string.IsNullOrEmpty(@event.ImageUrl))
                {
                    await _blobStorageService.DeleteFileFromBlobAsync(@event.ImageUrl);
                }

                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
