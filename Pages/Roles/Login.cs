
using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace EmployeeData.Pages.Roles
{

public class Login : PageModel
{
    
    public string ErrorMessage { get; set; }
    public Users users{ get; set; } = new Users();

    public List<Users> allUsers { get; set;}

     private string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

    public void OnGet()
    {

    }
    public IActionResult OnPostLogin(string userName, string password)
    {
        
        // Validate model
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                return Page();
            }
           if (!System.IO.File.Exists(employeeFilePath))
            {
                ModelState.AddModelError("", $"Excel file not found at {employeeFilePath}.");
                ViewData["ErrorMessage"]= $"Excel file not found.";
                return Page(); // Handle or log the case where file doesn't exist
            }

             // Ensure directory exists
                string directory = Path.GetDirectoryName(employeeFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                 // Ensure ExcelPackage licensing
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Check if the file exists, or create a new one
                bool isNewFile = !System.IO.File.Exists(employeeFilePath);
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                // Load or create the worksheet
                var worksheet = package.Workbook.Worksheets["Roles"];

            try
            {
                // Check if the username exists in the list
                var user = GetAllUsers();
               var correct= user.FirstOrDefault(x=>x.UserName == userName);
                

                if (correct == null)
                {
                     // Store the error message in TempData
                    ViewData["ErrorMessage"] = "Username not found. Please check your input.";

                    // Redirect back to the login page
                    return RedirectToPage("/Roles/Login");
                }
               string encryptedPassword = EncriptPassword.Encrypt(password);

                // Check if the password matches
                if (correct.Password != encryptedPassword)
                {
                    // Store the error message in TempData
                    ViewData["ErrorMessage"] = "Incorrect password. Please try again.";

                    // Redirect back to the login page
                    return RedirectToPage("/Roles/Login");
                   
                }
                // Assume successful login if the username matches (password validation can be added here)
                HttpContext.Session.SetString("Username",correct.UserName);
                HttpContext.Session.SetString("Role", correct.Roles);

                // Redirect to the EmployeeList page if both username and password match
                return RedirectToPage("/EmployeeList/EmployeeList");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return a general error response
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
    }

    private void LogModelErrors()
        {
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Any())
                {
                    Console.WriteLine($"Key: {state.Key}");
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }
        }


        private List<Users> GetAllUsers()
        {
           var users = new List<Users>();
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Roles"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension?.Rows ?? 2;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var user = new Users
                        {
                            UserName = worksheet.Cells[row, 1].Text,
                            Password = worksheet.Cells[row, 2].Text,
                            Roles = worksheet.Cells[row, 3].Text

                        };
                        users.Add(user);
                    }
                }
            }
            return users;
        }


}
}


