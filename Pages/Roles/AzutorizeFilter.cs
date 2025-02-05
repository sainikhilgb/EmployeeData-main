using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeData.Pages.Roles;

public class AzutorizeFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Session.GetString("Username") == null)
        {
            context.Result = new RedirectToPageResult("/Roles/Login"); 
        }
    }
}
