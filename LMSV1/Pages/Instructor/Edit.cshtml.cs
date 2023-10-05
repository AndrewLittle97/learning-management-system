﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMSV1.Models;

namespace LMSV1.Pages.Instructor
{
    public class EditModel : PageModel
    {
        private readonly Data.LMSV1Context _context;

        [BindProperty]
        public Course Course { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedDays { get; set; } = new List<int>();

        public EditModel(Data.LMSV1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course =  await _context.Courses.FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            Course = course;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Convert SelectedDays to DaysOfWeek enum and assign to Course.MeetDays
            Course.MeetDays = SelectedDays.Aggregate(DaysOfWeek.None, (current, day) => current | (DaysOfWeek)day);

            _context.Attach(Course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(Course.CourseID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./CourseManager");
        }

        private bool CourseExists(int id)
        {
          return (_context.Courses?.Any(e => e.CourseID == id)).GetValueOrDefault();
        }
    }
}
