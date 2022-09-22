using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaruChat.Data;
using TaruChat.Models;

namespace TaruChat.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ChatContext _context;

        public MessagesController(ChatContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MessageType,Word,AttachmentURL,CreatedAt,ChatID,UserID")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.CreatedAt = DateTime.Now;
                message.UserID = User.Identity.Name;
                _context.Add(message);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MessageType,Word,AttachmentURL,CreatedAt,ChatID,UserID")] Message message)
        {
            if (id != message.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.ID))
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
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendImage(string Word, string ChatID)
        {
            var chat = await _context.Chats.FindAsync(ChatID);

            if (chat != null)
            {
                var message = new Message
                {
                    MessageType = "Image",
                    Word = Word,
                    AttachmentURL = "",
                    CreatedAt = DateTime.Now,
                    ChatID = ChatID,
                    UserID = User.Identity.Name
                };

                _context.Add(message);
                await _context.SaveChangesAsync();
                return Json(new { result = "Success", message = "Stored image successful." });
            }
            return Json(new { result = "Success", message = "Stored image successful." });

        }

        [HttpPost]
        public async Task<IActionResult> SendAttachment(string Word, string ChatID)
        {
            var chat = await _context.Chats.FindAsync(ChatID);

            if (chat != null)
            {
                var message = new Message
                {
                    MessageType = "Document",
                    Word = Word,
                    AttachmentURL = "",
                    CreatedAt = DateTime.Now,
                    ChatID = ChatID,
                    UserID = User.Identity.Name
                };

                _context.Add(message);
                await _context.SaveChangesAsync();
                return Json(new { result = "Success", message = "Stored document successful." });
            }
            return Json(new { result = "Success", message = "Stored document successful." });

        }

        [HttpPost]
        public async Task<IActionResult> SendAudio(string Word, string ChatID)
        {
            var chat = await _context.Chats.FindAsync(ChatID);

            if (chat != null)
            {
                var message = new Message
                {
                    MessageType = "Audio",
                    Word = Word,
                    AttachmentURL = "",
                    CreatedAt = DateTime.Now,
                    ChatID = ChatID,
                    UserID = User.Identity.Name
                };

                _context.Add(message);
                await _context.SaveChangesAsync();
                return Json(new { result = "Success", message = "Stored audio successful." });
            }
            return Json(new { result = "Success", message = "Stored audio successful." });

        }
        [HttpPost]
        public async Task<IActionResult> SendCamVideo(string Word, string ChatID)
        {
            var chat = await _context.Chats.FindAsync(ChatID);

            if (chat != null)
            {
                var message = new Message
                {
                    MessageType = "CamVideo",
                    Word = Word,
                    AttachmentURL = "",
                    CreatedAt = DateTime.Now,
                    ChatID = ChatID,
                    UserID = User.Identity.Name
                };

                _context.Add(message);
                await _context.SaveChangesAsync();
                return Json(new { result = "Success", message = "Stored Video successful." });
            }
            return Json(new { result = "Success", message = "Stored Video successful." });

        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.ID == id);
        }
    }
}
