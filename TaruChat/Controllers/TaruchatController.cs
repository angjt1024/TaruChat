using Microsoft.AspNetCore.Mvc;
using TaruChat.Data;
using TaruChat.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaruChat.Models.ChatViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace TaruChat.Controllers
{
    [Authorize(Roles = "User")]
    public class TaruchatController : Controller
    {
        private readonly ChatContext _context;

        public TaruchatController(ChatContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string id)
        {
            var user = await _context.Users
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Chat)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == User.Identity.Name);

            var viewModel = new MessageViewModel();


            bool valid = false;

            if(id != null)
            {
                var chat = await _context.Chats
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

                foreach (var item in user.Enrollments)
                {
                    if (item.ChatID == id)
                    {
                        valid = true;
                        viewModel.Chat = chat;
                        break;
                    }
                }

                if (valid == false || chat == null)
                {
                    return NotFound();
                }
            }
            ViewData["Testing"] = "Chat Code";

            

            if (user == null)
            {
                return NotFound();
            }

            viewModel.Messages = await _context.Messages
                .Include(i => i.Chat)
                    .ThenInclude(i => i.Enrollments)
                        .ThenInclude(i => i.User)
                .AsNoTracking()
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
            viewModel.Message = new Message();
            viewModel.User = user;
            return View(viewModel);

        }

        async static Task FromMic(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

    }
}
