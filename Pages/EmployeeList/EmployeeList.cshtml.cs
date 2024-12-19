using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeData.Pages.EmployeeList
{
    public class EmployeeList : PageModel
    {
        [BindProperty]
        public Employee employees { get; set; } = new Employee();
        
        [BindProperty]

        public List<SelectListItem> GradeOptions { get; set; }
         public List<SelectListItem> GlobalGradeOptions { get; set; }
        public List<SelectListItem> BUOptions { get; set; }
        public List<SelectListItem> BGVOptions { get; set; }
        public List<SelectListItem> ProjectCodeOptions { get; set; }
        public List<SelectListItem> ProjectNameOptions { get; set; }
        public List<SelectListItem> PODNameOptions { get; set; }
        public List<SelectListItem> OffShoreCityOptions { get; set; }
        public List<SelectListItem> TypeOptions { get; set; }
        public List<SelectListItem> TowerOptions { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
       
        private readonly string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

        private Dictionary<string, string> projectCodeToNameMapping = new Dictionary<string, string>();

    


        // Method to load Employee and Project details from Excel
             public string Message { get; set; }
       public void OnGet()
{
    Employees = GetAllEmployees(); // Fetch all employees initially (used later for filtering)
    LoadDropdownOptions();
    LoadEmployeeData();

    // If there's a search term, apply the filter to search across all rows
    if (!string.IsNullOrEmpty(SearchTerm))
    {
        // Filter employees based on EmpId, Email, or ProjectCode
        var filteredEmployees = Employees
            .Where(e => e.EmpId.ToString().Contains(SearchTerm) 
                     || e.Email.Contains(SearchTerm) 
                     || e.ProjectCode.ToString().Contains(SearchTerm))
            .ToList();

        if (filteredEmployees.Any())
        {
            Employees = filteredEmployees; // Update the list with filtered employees
        }
        else
        {
            Message = "No employee data is available for the provided search term.";
            Employees = new List<Employee>(); // Empty list when no matches are found
        }
    }
    else
    {
        // Display only the top 20 employees when no search term is provided
        Employees = Employees.Take(20).ToList();
    }
}

        

         

        private void LoadDropdownOptions()
        {
            GradeOptions = new List<SelectListItem>();
            GlobalGradeOptions = new List<SelectListItem>();
            BUOptions = new List<SelectListItem>();
            BGVOptions = new List<SelectListItem>();
            ProjectCodeOptions = new List<SelectListItem>();
            ProjectNameOptions = new List<SelectListItem>();
            PODNameOptions = new List<SelectListItem>();
            OffShoreCityOptions = new List<SelectListItem>();
            TypeOptions = new List<SelectListItem>();
            TowerOptions = new List<SelectListItem>();


            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Dropdown"]; // Ensure this matches your worksheet name
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var grade = worksheet.Cells[row, 1]?.Text?.Trim();
                        var bu = worksheet.Cells[row, 2]?.Text?.Trim();
                        var projectcode = worksheet.Cells[row, 3]?.Text?.Trim();
                        var projectname = worksheet.Cells[row, 4]?.Text?.Trim();
                        var PODname = worksheet.Cells[row, 5]?.Text?.Trim();
                        var Offshore = worksheet.Cells[row, 6]?.Text?.Trim();
                        var type = worksheet.Cells[row, 7]?.Text?.Trim();
                        var tower = worksheet.Cells[row, 8]?.Text?.Trim();
                        var globalgrade = worksheet.Cells[row, 9]?.Text?.Trim();
                        var bgv = worksheet.Cells[row, 10]?.Text?.Trim();

                        
                        if (!string.IsNullOrWhiteSpace(projectcode) && !string.IsNullOrWhiteSpace(projectname))
                        {
                            ProjectCodeOptions.Add(new SelectListItem { Value = projectcode, Text = projectcode });
                            projectCodeToNameMapping.Add(projectcode, projectname);
                        }
                        if (!string.IsNullOrWhiteSpace(PODname))
                        {
                            PODNameOptions.Add(new SelectListItem { Value = PODname, Text = PODname });
                        }
                        if (!string.IsNullOrWhiteSpace(Offshore))
                        {
                            OffShoreCityOptions.Add(new SelectListItem { Value = Offshore, Text = Offshore });
                        }
                        if (!string.IsNullOrWhiteSpace(type))
                        {
                            TypeOptions.Add(new SelectListItem { Value = type, Text = type });
                        }
                        if (!string.IsNullOrWhiteSpace(tower))
                        {
                            TowerOptions.Add(new SelectListItem { Value = tower, Text = tower });
                        }

                        if (!string.IsNullOrWhiteSpace(grade)){ 

                            GradeOptions.Add(new SelectListItem { Value = grade, Text = grade });
                        }

                        if (!string.IsNullOrWhiteSpace(bu))
                            BUOptions.Add(new SelectListItem { Value = bu, Text = bu });
                        if (!string.IsNullOrWhiteSpace(bgv))
                            BGVOptions.Add(new SelectListItem { Value = bgv, Text = bgv });

                         if (!string.IsNullOrWhiteSpace(globalgrade)){ 

                            GlobalGradeOptions.Add(new SelectListItem { Value = globalgrade, Text = globalgrade });
                        }
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Worksheet 'Dropdown' not found in the dropdown file.");
                }
            }
            else
            {
                ModelState.AddModelError("", $"Dropdown file not found at {employeeFilePath}.");
            }
        }


       
        private void LoadEmployeeData()
        {
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Employees"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension?.Rows ?? 6;

                    // Loop through the rows and add employees
                    for (int row = 6; row <= rowCount; row++)
                    {
                        var employee = new Employee
                        {
                            EmpId = worksheet.Cells[row, 15].Text,
                            GGID = ParseInt(worksheet.Cells[row, 14].Text),
                            Resource = worksheet.Cells[row, 17].Text,
                            Email = worksheet.Cells[row, 16].Text,
                            Gender = worksheet.Cells[row, 21].Text,
                            DateOfHire = ParseDate(worksheet.Cells[row, 127].Text),
                            Grade = worksheet.Cells[row, 18].Text,
                            GlobalGrade = worksheet.Cells[row, 19].Text,
                            BU = worksheet.Cells[row, 4].Text,
                            IsActiveInProject = worksheet.Cells[row, 20].Text,
                            OverallExp = ParseInt(worksheet.Cells[row, 27].Text),
                            Skills = worksheet.Cells[row, 28].Text,
                            Certificates = worksheet.Cells[row, 33].Text,
                            AltriaStartdate = ParseDate(worksheet.Cells[row, 128].Text),
                            AltriaEnddate = ParseDate(worksheet.Cells[row, 129].Text),
                            BGVStatus = worksheet.Cells[row, 130].Text,
                            BGVCompletionDate = ParseDate(worksheet.Cells[row, 134].Text),
                            VISAStatus = worksheet.Cells[row, 131].Text,
                            VISAType = worksheet.Cells[row, 135].Text,

                            ProjectCode = ParseInt(worksheet.Cells[row, 7].Text),
                            ProjectName = worksheet.Cells[row, 8].Text,
                            PONumber = ParseInt(worksheet.Cells[row, 9].Text),
                            PODName = worksheet.Cells[row, 10].Text,
                            StartDate = ParseDate(worksheet.Cells[row, 132].Text),
                            EndDate = ParseDate(worksheet.Cells[row, 133].Text),
                            Location = worksheet.Cells[row, 22].Text,
                            OffshoreCity = worksheet.Cells[row, 23].Text,
                            OffshoreBackup = worksheet.Cells[row, 34].Text,
                            AltriaPODOwner = worksheet.Cells[row, 12].Text, 
                            ALCSDirector = worksheet.Cells[row, 13].Text,
                            Type = worksheet.Cells[row, 1].Text,
                            Tower = worksheet.Cells[row, 2].Text,
                            ABLGBL = worksheet.Cells[row, 3].Text,
                            TLName = worksheet.Cells[row, 5].Text,
                            Transition = worksheet.Cells[row, 35].Text,
                            COR = worksheet.Cells[row, 60].Text,
                            Group = worksheet.Cells[row, 62].Text,
                            RoleinPOD = worksheet.Cells[row, 25].Text,
                            MonthlyPrice = ParseDecimal(worksheet.Cells[row, 63].Text),
                            January = ParseDecimal(worksheet.Cells[row,36].Text),
                            February = ParseDecimal(worksheet.Cells[row,37].Text),
                            March = ParseDecimal(worksheet.Cells[row,38].Text),
                            April = ParseDecimal(worksheet.Cells[row,39].Text),
                            May = ParseDecimal(worksheet.Cells[row,40].Text),
                            June = ParseDecimal(worksheet.Cells[row,41].Text),
                            July = ParseDecimal(worksheet.Cells[row,42].Text),
                            August = ParseDecimal(worksheet.Cells[row,43].Text),
                            September = ParseDecimal(worksheet.Cells[row,44].Text),
                            October = ParseDecimal(worksheet.Cells[row,45].Text),
                            November = ParseDecimal(worksheet.Cells[row,46].Text),
                            December =ParseDecimal(worksheet.Cells[row,47].Text),
                        
                    };

                        Employees.Add(employee);
                    }
                }
            }
        }

        private List<Employee> GetAllEmployees()
        {
            // This should read from the Excel file and return a list of all employees
            // For simplicity, assuming a method that loads all employees
            var employees = new List<Employee>();
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(employeeFilePath));

                var worksheet = package.Workbook.Worksheets["Employees"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension?.Rows ?? 6;
                    for (int row = 6; row <= rowCount; row++)
                    {
                        var emp = new Employee
                        {
                            EmpId = worksheet.Cells[row, 15].Text,
                            GGID = ParseInt(worksheet.Cells[row, 14].Text),
                            Resource = worksheet.Cells[row, 17].Text,
                            Email = worksheet.Cells[row, 16].Text,
                            Gender = worksheet.Cells[row, 21].Text,
                            DateOfHire = ParseDate(worksheet.Cells[row, 127].Text),
                            Grade = worksheet.Cells[row, 18].Text,
                            GlobalGrade = worksheet.Cells[row, 19].Text,
                            BU = worksheet.Cells[row, 4].Text,
                            IsActiveInProject = worksheet.Cells[row, 20].Text,
                            OverallExp = ParseDecimal(worksheet.Cells[row, 27].Text),
                            Skills = worksheet.Cells[row, 28].Text,
                            Certificates = worksheet.Cells[row, 33].Text,
                            AltriaStartdate = ParseDate(worksheet.Cells[row, 128].Text),
                            AltriaEnddate = ParseDate(worksheet.Cells[row, 129].Text),
                            BGVStatus = worksheet.Cells[row, 130].Text,
                            BGVCompletionDate = ParseDate(worksheet.Cells[row, 134].Text),
                            VISAStatus = worksheet.Cells[row, 131].Text,
                            VISAType = worksheet.Cells[row, 135].Text,

                            ProjectCode = ParseInt(worksheet.Cells[row, 7].Text),
                            ProjectName = worksheet.Cells[row, 8].Text,
                            PONumber = ParseInt(worksheet.Cells[row, 9].Text),
                            PODName = worksheet.Cells[row, 10].Text,
                            StartDate = ParseDate(worksheet.Cells[row, 132].Text),
                            EndDate = ParseDate(worksheet.Cells[row, 133].Text),
                            Location = worksheet.Cells[row, 22].Text,
                            OffshoreCity = worksheet.Cells[row, 23].Text,
                            OffshoreBackup = worksheet.Cells[row, 34].Text,
                            AltriaPODOwner = worksheet.Cells[row, 12].Text,
                            ALCSDirector = worksheet.Cells[row, 13].Text,
                            Type = worksheet.Cells[row, 1].Text,
                            Tower = worksheet.Cells[row, 2].Text,
                            ABLGBL = worksheet.Cells[row, 3].Text,
                            TLName = worksheet.Cells[row, 5].Text,
                            Transition = worksheet.Cells[row, 35].Text,
                            Group = worksheet.Cells[row, 62].Text,
                            RoleinPOD = worksheet.Cells[row, 25].Text,
                            MonthlyPrice = ParseDecimal(worksheet.Cells[row, 63].Text),
                            AltriaEXP = ParseDecimal(worksheet.Cells[row, 26].Text),
                            COR = worksheet.Cells[row, 60].Text,
                            January = ParseDecimal(worksheet.Cells[row, 36].Text),
                            February = ParseDecimal(worksheet.Cells[row, 37].Text),
                            March = ParseDecimal(worksheet.Cells[row, 38].Text),
                            April = ParseDecimal(worksheet.Cells[row, 39].Text),
                            May = ParseDecimal(worksheet.Cells[row, 40].Text),
                            June = ParseDecimal(worksheet.Cells[row, 41].Text),
                            July = ParseDecimal(worksheet.Cells[row, 42].Text),
                            August = ParseDecimal(worksheet.Cells[row, 43].Text),
                            September = ParseDecimal(worksheet.Cells[row, 44].Text),
                            October = ParseDecimal(worksheet.Cells[row, 45].Text),
                            November = ParseDecimal(worksheet.Cells[row, 46].Text),
                            December = ParseDecimal(worksheet.Cells[row, 47].Text),
                        
                        };
                        employees.Add(emp);
                    }
                }
            }
            return employees;
        }
            
        

// Method to delete an employee
[HttpDelete]
public IActionResult OnPostDelete(string empId)
{
    // Remove employee from the in-memory list
    var employeeToDelete = Employees.FirstOrDefault(e => e.EmpId == empId)?? new Employee();
    if (employeeToDelete != null)
    {
        Employees.Remove(employeeToDelete);

        // Update the Excel file
        DeleteEmployeeFromExcel(empId);

        // Reload data to reflect changes
        LoadEmployeeData();
    }

    // Redirect back to the same page after deletion
    return RedirectToPage();
}



        // Method to delete an employee from the Excel file
        private void DeleteEmployeeFromExcel(string empId)
        {
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(employeeFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets["Employees"];
                    if (worksheet != null)
                    {
                    var rowCount = worksheet.Dimension?.Rows ?? 6;
                        for (int row = 6; row <= rowCount; row++) // Skip header
                        {
                            string currentEmpId = worksheet.Cells[row, 15].Text;
                            if (currentEmpId == empId)
                            {
                                worksheet.DeleteRow(row);
                                break;
                            }
                        }

                        // Save changes to the Excel file
                        package.Save();
                    }
                }
            }
        }


// Method to delete an project
[HttpDelete]
public IActionResult OnPostProjectDelete(int projectCode)
{
    // Remove employee from the in-memory list
    var employeeToDelete = Employees.FirstOrDefault(e => e.ProjectCode == projectCode)?? new Employee();
    if (employeeToDelete != null)
    {
        Employees.Remove(employeeToDelete);

        // Update the Excel file
        DeleteProjectFromExcel(projectCode);

        // Reload data to reflect changes
     LoadEmployeeData();
    }

    // Redirect back to the same page after deletion
    return RedirectToPage();
}



        // Method to delete an employee from the Excel file
        private void DeleteProjectFromExcel(int projectCode)
        {
            if (System.IO.File.Exists(employeeFilePath))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(employeeFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets["Employees"];
                    if (worksheet != null)
                    {
                    var rowCount = worksheet.Dimension?.Rows ?? 6;
                        for (int row = 6; row <= rowCount; row++) // Skip header
                        {
                            int currentProjectCode = ParseInt(worksheet.Cells[row, 7].Text);
                            if (currentProjectCode == projectCode)
                            {
                                worksheet.DeleteRow(row);
                                break;
                            }
                        }

                        // Save changes to the Excel file
                        package.Save();
                    }
                }
            }
        }
       
        

              private DateTime ParseDate(string dateString)
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    return date;
                }
                return DateTime.MinValue; // Default value for invalid or missing dates
            }

        private int ParseInt(string numberString)
        {
            if (int.TryParse(numberString, out var number))
            {
                return number;
            }
            return 0; // Default value for invalid or missing numbers
        }

        private decimal ParseDecimal(string numberString)
        {
            if (decimal.TryParse(numberString, out var number))
            {
                return number;
            }
            return 0; // Default value for invalid or missing numbers
        }

        
    }
}
