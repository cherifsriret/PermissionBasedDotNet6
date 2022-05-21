using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PermissionsBasedProject.Constants;

namespace PermissionsBasedProject.Controllers
{
    public class ProductsController : Controller
    {
        [Authorize(Permissions.Products.View)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
