using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeavePortal.Areas.Identity.Data;
using LeavePortal.Helpers;
using LeavePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace LeavePortal.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ModulePermission("Deparments")]
        public IActionResult Index()
        {
            var obj = _context.Department.ToList();
            return View(obj);
        }

        [ModulePermission("Add Department")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                 _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        [ModulePermission("Edit Department")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department =  _context.Department.Find(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }
             _context.Update(department);
             _context.SaveChanges();
            return View("Index");
        }

        [ModulePermission("Delete Department")]
        [HttpPost]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            if (_context.Department == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var department = await _context.Department.FindAsync(id);
            if (department != null)
            {
                _context.Department.Remove(department);
            }
            var users = _context.Users.Where(x => x.DepartmentId == id).ToList();
            foreach (var item in users)
            {
                _context.Users.Remove(item);
                _context.SaveChanges();
            }
            int check= await _context.SaveChangesAsync();

            if (check == 1)
            {
                return new JsonResult("Department Delete Successfully");
            }
            else
            {
                return new JsonResult("System Error! Department is not Delete.");

            }         
        }
    }
}
