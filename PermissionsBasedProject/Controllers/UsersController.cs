using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionsBasedProject.Constants;
using PermissionsBasedProject.Models;

namespace PermissionsBasedProject.Controllers
{
    [Authorize(Permissions.Users.View)]

    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;    
        private readonly SignInManager<IdentityUser> _signInManager;    

        public UsersController(UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser>signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        // GET: UsersController
        [Authorize(Permissions.Users.View)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userManager.Users.Select(x => new UserViewModel { Id = x.Id, Email = x.Email, UserName = x.UserName, Roles = _userManager.GetRolesAsync(x).Result }).ToListAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            
        }

        // GET: UsersController/ManageRoles/5
        [Authorize(Permissions.Users.Update)]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new CheckBoxGroupViewModel
            {
                GroupId = user.Id, 
                GroupName = user.UserName,
                CheckboxList = roles.Select(role => new CheckBoxViewModel
                {
                    CheckBoxValue = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList() 
            };

            return View(viewModel);
        }

        // POST: UsersController/UpdateRoles
        [Authorize(Permissions.Users.Update)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(CheckBoxGroupViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.GroupId);
            if(user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, model.CheckboxList.Where(r=>r.IsSelected).Select(r=>r.CheckBoxValue));

            return RedirectToAction(nameof(Index));


        }


        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
