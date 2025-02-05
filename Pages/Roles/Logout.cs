using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeData.Pages.Roles{


[Route("Logout")]
[ApiController]
public class Logout : Controller
{
    [HttpGet]
     public IActionResult OnGet()
    {
            // Ensure you are accessing Session from the HttpContext property of PageModel
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToPage("/Roles/Login"); // Redirect to the login page
    }
}
}
