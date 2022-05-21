using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionsBasedProject.Constants;
using PermissionsBasedProject.Models;

namespace PermissionsBasedProject.Controllers
{
    [Authorize(Permissions.Roles.View)]
    public class RolesController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        // GET: RolesController
        [Authorize(Permissions.Roles.View)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // GET: RolesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RolesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController/Create
        [Authorize(Permissions.Roles.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Index" , await _roleManager.Roles.ToListAsync()); 
                }

                if(await _roleManager.RoleExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name","Role already exists");
                    return View("Index", await _roleManager.Roles.ToListAsync());
                }
                await _roleManager.CreateAsync(new IdentityRole { Name = model.Name.Trim() });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }



        [Authorize(Permissions.Roles.Update)]
        // GET: RolesController/ManagePermissions/5
        public async Task<IActionResult> ManagePermissions(string roleId)
        {

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var permissions =  _roleManager.GetClaimsAsync(role).Result.Select(c=>c.Value).ToList();


            var viewModel = new CheckBoxGroupViewModel
            {
                GroupId = role.Id,
                GroupName = role.Name,
                CheckboxList = Permissions.GenerateAllPermissions().Select(per => new CheckBoxViewModel
                {
                    CheckBoxValue = per.ToString(),
                    IsSelected = permissions.Contains(per.ToString())
                }).ToList() 
            };

            return View(viewModel);
        }
        // POST: RolesController/UpdatePermissions
        [Authorize(Permissions.Roles.Update)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePermissions(CheckBoxGroupViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.GroupId);
            if (role == null) return NotFound();
            foreach (var permission in await _roleManager.GetClaimsAsync(role))
            {
                await _roleManager.RemoveClaimAsync(role, permission);
            }


            foreach (var claim in model.CheckboxList.Where(x => x.IsSelected).ToList())
            {
                await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Permissions.PermissionName, claim.CheckBoxValue));
            }

            return RedirectToAction(nameof(Index));


        }

        // GET: RolesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RolesController/Edit/5
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

        // GET: RolesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RolesController/Delete/5
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
