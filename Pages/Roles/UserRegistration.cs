using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace EmployeeData.Pages.Roles
{
    public class UserRegistration : PageModel
{
     [BindProperty]
    public Users users{ get; set; } = new Users();

    private string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

    public void OnGet()
    {
        
    }

    public IActionResult OnPost(string userName, string password, string role)
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
                 string encryptedPassword = EncriptPassword.Encrypt(password);
                    if (!string.IsNullOrEmpty(userName)) // If editing, update the existing record
                    {
                         int row = GetRow(worksheet, userName);
                        if (row != -1)
                        {
                       
                            // Assuming the EndDate column is the 5th column (adjust index if needed)
                            worksheet.Cells[row, 1].Value = userName;
                            worksheet.Cells[row, 2].Value = encryptedPassword;
                            worksheet.Cells[row, 3].Value = role;
                        }
                        else{
                           int lastRow = worksheet.Dimension?.Rows ?? 2; // Adjust default value based on header rows
                            for (int i = lastRow; i >= 2; i--) // Start from bottom, excluding header rows
                            {
                                if (worksheet.Cells[i, 1].Value != null) // Check if any cell in the row has data
                                {
                                    lastRow = i;
                                    break;
                                }
                            }

                            // Add new row at the end (lastRow + 1)
                            int newrow = lastRow + 1;
                            worksheet.Cells[newrow, 1].Value = userName;
                            worksheet.Cells[newrow, 2].Value = encryptedPassword;
                            worksheet.Cells[newrow, 3].Value = role;
                        }
                        
                    }
                    // Save the changes to the file
                     package.SaveAsync();

                // Redirect to Login Page
                return Page();
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

        private int GetRow(ExcelWorksheet worksheet, string userName)
        {
            if(worksheet!=null)
            {

            var rowCount =worksheet?.Dimension?.Rows ?? 3;
            for (int row = 2; row <= rowCount; row++)
            {
                if (worksheet.Cells[row, 1].Text == userName)
                {
                    return row; // Row number where match is found
                }
            }
            }
            return -1;
        }

    
}

}

