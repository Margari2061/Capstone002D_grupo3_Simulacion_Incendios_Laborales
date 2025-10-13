using IncediosWebAPI.Model;
using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;

namespace IncediosWebAPI.Controllers;

[WebAuthorize(AppRoles.Admin)]
public class InicioController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
