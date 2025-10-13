using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;

namespace IncediosWebAPI.Controllers;

[JWTAuthorize]
public class PartidaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
