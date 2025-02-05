using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeData.Pages.Roles;

public class BasePageModel : PageModel
{
    public string Username { get; set; }
    public string Role { get; set; }

    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        Username = HttpContext.Session.GetString("Username");
        Role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(Username))
        {
            // Redirect to login page if session is not valid
            context.Result = new RedirectToPageResult("/Roles/Login");
        }

        base.OnPageHandlerExecuting(context);

    }
    protected bool UserHasRole(string requiredRole)
    {
        return Role == requiredRole;
    }
}
