using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaruChat.Data;
using TaruChat.Models;
using TaruChat.Models.ChatViewModels;


namespace TaruChat.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChatsController : Controller
    {
        private readonly ChatContext _context;

        public ChatsController(ChatContext context)
        {
            _context = context;
        }

        // GET: Chats
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chats.ToListAsync());
        }

        // GET: Chats/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // GET: Chats/Create
        public IActionResult Create()
        {
            var chats = new Chat();
            chats.Enrollments = new List<Enrollment>();
            PopulateEnrolledUserData(chats);
            return View();
        }

        // POST: Chats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,CreatedAt,UpdatedAt,DeletedAt,ProfilePic")] Chat chat, string[] selectedUsers)
        {
            if (selectedUsers != null)
            {
                chat.Enrollments = new List<Enrollment>();
                foreach (var user in selectedUsers)
                {
                    var userToAdd = new Enrollment { ChatID = chat.ID, UserID = user };
                    chat.Enrollments.Add(userToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(chat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateEnrolledUserData(chat);
            return View(chat);
        }

        // GET: Chats/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .Include(i => i.Enrollments).ThenInclude(i => i.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (chat == null)
            {
                return NotFound();
            }

            PopulateEnrolledUserData(chat);
            return View(chat);
        }

        // POST: Chats/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, string[] selectedUsers)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatToUpdate = await _context.Chats
                .Include(i => i.Enrollments)
                .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (await TryUpdateModelAsync<Chat>(
                chatToUpdate,
                "",
                c => c.Title,
                c => c.CreatedAt,
                c => c.UpdatedAt,
                c => c.DeletedAt,
                c => c.ProfilePic))
                UpdateChatUsers(selectedUsers, chatToUpdate);

            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateChatUsers(selectedUsers, chatToUpdate);
            PopulateEnrolledUserData(chatToUpdate);
            return View(chatToUpdate);
        }

        private void PopulateEnrolledUserData(Chat chat)
        {
            var allUsers = _context.Users;
            var chatUsers= new HashSet<string>(chat.Enrollments.Select(c => c.UserID));
            var viewModel = new List<EnrolledUserData>();
            foreach (var user in allUsers)
            {
                viewModel.Add(new EnrolledUserData
                {
                    UserID= user.ID,
                    Name = user.Name,
                    Enrolled = chatUsers.Contains(user.ID)
                });
            }
            ViewData["Users"] = viewModel;
        }

        private void UpdateChatUsers(string[] selectedUsers, Chat chatToUpdate)
        {
            if (selectedUsers == null)
            {
                chatToUpdate.Enrollments = new List<Enrollment>();
                return;
            }

            var selectedUsersHS = new HashSet<string>(selectedUsers);
            var chatUsers = new HashSet<string>
                (chatToUpdate.Enrollments.Select(c => c.User.ID));
            foreach (var user in _context.Users)
            {
                if (selectedUsersHS.Contains(user.ID.ToString()))
                {
                    if (!chatUsers.Contains(user.ID))
                    {
                        chatToUpdate.Enrollments.Add(new Enrollment { ChatID = chatToUpdate.ID, UserID = user.ID });
                    }
                }
                else
                {

                    if (chatUsers.Contains(user.ID))
                    {
                        Enrollment userToRemove = chatToUpdate.Enrollments.FirstOrDefault(i => i.UserID == user.ID);
                        _context.Remove(userToRemove);
                    }
                }
            }
        }

        // GET: Chats/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .FirstOrDefaultAsync(m => m.ID == id);
            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // POST: Chats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chat = await _context.Chats.FindAsync(id);
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatExists(string id)
        {
            return _context.Chats.Any(e => e.ID == id);
        }
    }
}
