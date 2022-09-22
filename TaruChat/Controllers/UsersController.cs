using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaruChat.Data;
using TaruChat.Models;

namespace TaruChat.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ChatContext _context;

        public UsersController(ChatContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["ClassSortParm"] = sortOrder == "Class" ? "class_desc" : "Class";
            ViewData["CurrentFilter"] = searchString;


            var users = from s in _context.Users
                           select s;

            //Searching
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(c => c.Name.Contains(searchString));
            }

            //Sorting
            switch (sortOrder)
            {
                case "id_desc":
                    users = users.OrderByDescending(c => c.ID);
                    break;
                case "Name":
                    users = users.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    users = users.OrderByDescending(c => c.Name);
                    break;
                case "Email":
                    users = users.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(c => c.Email);
                    break;
                case "Class":
                    users = users.OrderBy(c => c.ClassID);
                    break;
                case "class_desc":
                    users = users.OrderByDescending(c => c.ClassID);
                    break;
                default:
                    users = users.OrderBy(c => c.ID);
                    break;
            }

            return View(await users.AsNoTracking().ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Class)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["ClassID"] = new SelectList(_context.Classes, "ID", "ID");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Role,Password,Email,Gender,ProfilePic,Status,ClassID")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassID"] = new SelectList(_context.Classes, "ID", "ID", user.ClassID);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["ClassID"] = new SelectList(_context.Classes, "ID", "ID", user.ClassID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Name,Password,Role,Email,Gender,ProfilePic,Status,ClassID")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            ViewData["ClassID"] = new SelectList(_context.Classes, "ID", "ID", user.ClassID);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Class)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        //Hash
        PasswordHasher ph = new PasswordHasher();
        private string HashPassword(string password)
        {
            return ph.HashPassword(password);
        }

        private bool VerifyPassword(string hash, string password)
        {
            return ph.VerifyHashedPassword(hash, password)
                   == PasswordVerificationResult.Success;
        }

    }
}
