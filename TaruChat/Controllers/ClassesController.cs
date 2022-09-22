using TaruChat.Models.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaruChat.Data;
using TaruChat.Models;
using Microsoft.AspNetCore.Authorization;

namespace TaruChat.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private readonly ChatContext _context;

        public ClassesController(ChatContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["TitleSortParm"] = sortOrder =="Title" ? "title_desc" : "Title";
            ViewData["CurrentFilter"] = searchString;

            var classes = from c in _context.Classes
                          select c;

            //Searching
            if (!String.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(c => c.Title.Contains(searchString));
            }

            //Sorting
            switch (sortOrder)
            {
                case "id_desc":
                    classes = classes.OrderByDescending(c => c.ID);
                    break;
                case "Title":
                    classes = classes.OrderBy(c => c.Title);
                    break;
                case "title_desc":
                    classes = classes.OrderByDescending(c => c.Title);
                    break;
                default:
                    classes = classes.OrderBy(c => c.ID);
                    break;
            }
            return View(await classes.AsNoTracking().ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Assigns)
                .ThenInclude(e => e.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            var classes = new Class();
            classes.Assigns = new List<Assign>();
            PopulateAssignedSubjectData(classes);
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ID,Title")] Class @class, string[] selectedSubjects)
        {
            if (selectedSubjects != null)
            {
                @class.Assigns = new List<Assign>();
                foreach (var subject in selectedSubjects)
                {
                    var subjectToAdd = new Assign { ClassID = @class.ID, SubjectID = subject };
                    @class.Assigns.Add(subjectToAdd);
                }
            }

                if (ModelState.IsValid)
                {
                    _context.Add(@class);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            PopulateAssignedSubjectData(@class);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(i => i.Assigns).ThenInclude(i => i.Subject)
                .AsTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@class == null)
            {
                return NotFound();
            }
            PopulateAssignedSubjectData(@class);
            return View(@class);
        }

        private void PopulateAssignedSubjectData(Class @class)
        {
            var allSubjects = _context.Subjects;
            var classSubjects = new HashSet<string>(@class.Assigns.Select(c => c.SubjectID));
            var viewModel = new List<AssignedSubjectData>();
            foreach (var subject in allSubjects)
            {
                viewModel.Add(new AssignedSubjectData
                {
                    SubjectID = subject.ID,
                    Title = subject.Title,
                    Assigned = classSubjects.Contains(subject.ID)
              });
            }
            ViewData["Subjects"] = viewModel;
         }


       
        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Title")] Class @class)
        {
            if (id != @class.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.ID))
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
            return View(@class);
        }*/

        //Customize Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, string[] selectedSubjects)
        {
            if (id == null)
            {
                return NotFound();
            }
            var classToUpdate = await _context.Classes
                .Include(i => i.Assigns)
                .ThenInclude(i => i.Subject)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (await TryUpdateModelAsync<Class>(
                classToUpdate,
                "",
                c => c.Title))
                UpdateClassSubjects(selectedSubjects, classToUpdate);
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
            UpdateClassSubjects(selectedSubjects, classToUpdate);
            PopulateAssignedSubjectData(classToUpdate);
            return View(classToUpdate);
        }

        private void UpdateClassSubjects(string[] selectedSubjects, Class classToUpdate)
        {
            if (selectedSubjects == null)
            {
                classToUpdate.Assigns = new List<Assign>();
                return;
            }

            var selectedSubjectsHS = new HashSet<string>(selectedSubjects);
            var classSubjects = new HashSet<string>
                (classToUpdate.Assigns.Select(c => c.Subject.ID));
            foreach (var subject in _context.Subjects)
            {
                if (selectedSubjectsHS.Contains(subject.ID.ToString()))
                {
                    if (!classSubjects.Contains(subject.ID))
                    {
                        classToUpdate.Assigns.Add(new Assign { ClassID = classToUpdate.ID, SubjectID = subject.ID });
                    }
                }
                else
                {

                    if (classSubjects.Contains(subject.ID))
                    {
                        Assign subjectToRemove = classToUpdate.Assigns.FirstOrDefault(i => i.SubjectID == subject.ID);
                        _context.Remove(subjectToRemove);
                    }
                }
            }
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Class @class = await _context.Classes
                .Include(c => c.Assigns)
                .SingleAsync(c => c.ID == id);


            _context.Classes.Remove(@class);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(string id)
        {
            return _context.Classes.Any(e => e.ID == id);
        }
    }
}
