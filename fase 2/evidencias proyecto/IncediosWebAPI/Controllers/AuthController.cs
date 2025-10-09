using Microsoft.AspNetCore.Mvc;

namespace IncediosWebAPI.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
