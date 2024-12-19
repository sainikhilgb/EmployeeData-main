using EmployeeData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;


namespace EmployeeData.Pages.Registration
{
    public class Registration : PageModel
    {
        private string employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");
        

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

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
        public List<SelectListItem> CertificationOptions { get; set; }
         public List<SelectListItem> VISATypeOptions { get; set; }

        private Dictionary<string, string> projectCodeToNameMapping = new Dictionary<string, string>();

        private Dictionary<string, string> GradeToGlobalGrade = new Dictionary<string, string>();
         private Dictionary<string, List<string>> ProjectCodeToPODMapping { get; set; } = new Dictionary<string, List<string>>();


                 
        // OnGet to load dropdown options and initialize the form
        public IActionResult OnGet(string empId)
        {
            // Load dropdown options from the dropdown file
            LoadDropdownOptions();
             LoadProjectCodeToPODMapping();

            if (!string.IsNullOrEmpty(empId))
            {
                // Edit existing employee, load data
                Employee = GetEmployeeById(empId);
                if (Employee != null)
                {
                    Employee = Employee;
                    // Populate the POD dropdown if a project code is already selected
                    if (ProjectCodeToPODMapping.ContainsKey(Employee.ProjectCode.ToString()))
                    {
                        PODNameOptions = ProjectCodeToPODMapping[Employee.ProjectCode.ToString()]
                            .Select(pod => new SelectListItem { Value = pod, Text = pod })
                            .ToList();
                    }
                }
                else
                {
                    // Handle the case where the employee is not found
                    return NotFound();
                }
            }

            return Page();
        }

        
        // OnPost to save a new employee record or update an existing one
        public async Task<IActionResult> OnPost()
        {
                if (Employee.Certificates == "Others" && !string.IsNullOrWhiteSpace(Employee.OtherCertificate))
            {
                // Set the custom certificate value if "Others" is selected
                Employee.Certificates = Employee.OtherCertificate;
            }
           
            // Validate model
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                LoadDropdownOptions(); // Reload dropdown options if validation fails
                return Page();
            }

            try
            {
                // Ensure ExcelPackage licensing
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Define the file path
                 employeeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeeData.xlsx");

                // Ensure directory exists
                string directory = Path.GetDirectoryName(employeeFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Check if the file exists, or create a new one
                bool isNewFile = !System.IO.File.Exists(employeeFilePath);
                using var package = new ExcelPackage(new FileInfo(employeeFilePath));

                // Load or create the worksheet
                var worksheet = package.Workbook.Worksheets["Employees"];
            

               var rowCount = worksheet.Dimension?.Rows ?? 6;

                if (string.IsNullOrEmpty(Employee.EmpId)) // If editing, update the existing record
                {
                    int existingRow = GetEmployeeRow(worksheet, Employee.EmpId, Employee.ProjectCode);
                    if (existingRow != -1)
                    {
                        // Update the existing row with new data
                        var employeeData = GetEmployeeData();
                        worksheet.Cells[rowCount, 15].Value = Employee.EmpId; // EmpId
                        worksheet.Cells[rowCount, 14].Value = Employee.GGID;// GGID
                        worksheet.Cells[rowCount, 17].Value = Employee.Resource; // Resource
                        worksheet.Cells[rowCount, 16].Value = Employee.Email; // Email
                        worksheet.Cells[rowCount, 21].Value = Employee.Gender; // Gender
                        worksheet.Cells[rowCount, 127].Value = Employee.DateOfHire.ToString("dd-MM-yyy"); // DateOfHire
                        worksheet.Cells[rowCount, 18].Value = Employee.Grade; // Grade
                        worksheet.Cells[rowCount, 19].Value = Employee.GlobalGrade; // GlobalGrade
                        worksheet.Cells[rowCount, 4].Value = Employee.BU; // BU
                        worksheet.Cells[rowCount, 20].Value = Employee.IsActiveInProject; // IsActiveInProject
                        worksheet.Cells[rowCount, 27].Value = Employee.OverallExp; // OverallExp
                        worksheet.Cells[rowCount, 28].Value = Employee.Skills; // Skills
                        worksheet.Cells[rowCount, 33].Value = Employee.Certificates; // Certificates
                        worksheet.Cells[rowCount, 128].Value = Employee.AltriaStartdate.ToString("dd-MM-yyy"); // AltriaStartdate
                        worksheet.Cells[rowCount, 129].Value = Employee.AltriaEnddate.ToString("dd-MM-yyy"); // AltriaEnddate
                        worksheet.Cells[rowCount, 130].Value = Employee.BGVStatus; // BGVStatus
                        worksheet.Cells[rowCount, 134].Value = Employee.BGVCompletionDate.ToString("dd-MM-yyy"); // Column 133: BGV EndDate
                        worksheet.Cells[rowCount, 131].Value = Employee.VISAStatus; // VISAStatus
                        worksheet.Cells[rowCount, 135].Value = Employee.VISAType; // VISAType

                        worksheet.Cells[rowCount, 1].Value = Employee.Type; // Column 1: Type
                        worksheet.Cells[rowCount, 2].Value = Employee.Tower; // Column 2: Tower
                        worksheet.Cells[rowCount, 3].Value = Employee.ABLGBL; // Column 3: ABLGBL
                        worksheet.Cells[rowCount, 5].Value = Employee.TLName; // Column 5: TLName
                        worksheet.Cells[rowCount, 7].Value = Employee.ProjectCode; // Column 7: ProjectCode
                        worksheet.Cells[rowCount, 8].Value = Employee.ProjectName; // Column 8: ProjectName
                        worksheet.Cells[rowCount, 9].Value = Employee.PONumber; // Column 9: PONumber
                        // Map the Project Code to the corresponding POD Name
                        //worksheet.Cells[existingRow, 10].Value = Employee.PODName; // PODName (in case no mapping exists)
                if (ProjectCodeToPODMapping.ContainsKey(Employee.ProjectCode.ToString()))
                {
                    worksheet.Cells[existingRow, 10].Value = string.Join(",", ProjectCodeToPODMapping[Employee.ProjectCode.ToString()]); // PODName
                }
                else
                {
                    worksheet.Cells[existingRow, 10].Value = Employee.PODName; // PODName (in case no mapping exists)
                }
                        worksheet.Cells[rowCount, 12].Value = Employee.AltriaPODOwner; // Column 12: AltriaPODOwner
                        worksheet.Cells[rowCount, 13].Value = Employee.ALCSDirector; // Column 13: ALCSDirector
                        worksheet.Cells[rowCount, 22].Value = Employee.Location; // Column 22: Location
                        worksheet.Cells[rowCount, 23].Value = Employee.OffshoreCity; // Column 23: OffshoreCity
                        worksheet.Cells[rowCount, 34].Value = Employee.OffshoreBackup; // Column 34: OffshoreBackup
                        worksheet.Cells[rowCount, 35].Value = Employee.Transition; // Column 35: Transition
                        worksheet.Cells[rowCount, 60].Value = Employee.COR; // Column 60: COR
                        worksheet.Cells[rowCount, 62].Value = Employee.Group; // Column 62: Group
                        worksheet.Cells[rowCount, 25].Value = Employee.RoleinPOD; // Column 25: RoleinPOD
                        worksheet.Cells[rowCount, 63].Value = Employee.MonthlyPrice; // Column 63: MonthlyPrice
                        worksheet.Cells[rowCount, 26].Value = Employee.AltriaEXP; // Column 63: MonthlyPrice

                        // Dates: Ensure that start and end dates are formatted correctly
                        worksheet.Cells[rowCount, 132].Value = Employee.StartDate.ToString("dd-MM-yyy"); // Column 131: StartDate
                        worksheet.Cells[rowCount, 133].Value = Employee.EndDate.ToString("dd-MM-yyy"); // Column 132: EndDate

                        
                        worksheet.Cells[rowCount, 36].Value = Employee.January; // Column 36: January
                        worksheet.Cells[rowCount, 37].Value = Employee.February; // Column 37: February
                        worksheet.Cells[rowCount, 38].Value = Employee.March; // Column 38: March
                        worksheet.Cells[rowCount, 39].Value = Employee.April; // Column 39: April
                        worksheet.Cells[rowCount, 40].Value = Employee.May; // Column 40: May
                        worksheet.Cells[rowCount, 41].Value = Employee.June; // Column 41: June
                        worksheet.Cells[rowCount, 42].Value = Employee.July; // Column 42: July
                        worksheet.Cells[rowCount, 43].Value = Employee.August; // Column 43: August
                        worksheet.Cells[rowCount, 44].Value = Employee.September; // Column 44: September
                        worksheet.Cells[rowCount, 45].Value = Employee.October; // Column 45: October
                        worksheet.Cells[rowCount, 46].Value = Employee.November; // Column 46: November
                        worksheet.Cells[rowCount, 47].Value = Employee.December; // Column 47: December

                        
                        return RedirectToPage("/EmployeeList/EmployeeList");
                        }
                            else
                            {
                                // If employee not found for editing, append as new
                                AddEmployeeToExcel(worksheet, rowCount);

                            
                            }
                        }
                        else // If adding a new employee, append as new
                        {
                            AddEmployeeToExcel(worksheet, rowCount);
                        
                        }

                        // Save the changes to the file
                        await package.SaveAsync();

                        // Redirect to employee list
                        return RedirectToPage("/EmployeeList/EmployeeList");
                    }
                    catch (Exception ex)
                    {
                        // Log and display error
                        Console.WriteLine($"Error: {ex.Message}");
                        ModelState.AddModelError("", "An error occurred while processing the request.");
                        LoadDropdownOptions(); // Reload dropdowns
                        return Page();
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
        
       private void LoadProjectCodeToPODMapping()
    {
        // Load the Excel file with project codes and PODs
        var package = new ExcelPackage(new FileInfo(employeeFilePath));
        
            var worksheet = package.Workbook.Worksheets["Dropdown"];  // Assuming the data is in the first sheet
            int rowCount = worksheet.Dimension?.Rows?? 1;

            // Read each row and populate the ProjectCodeToPODMapping dictionary
            for (int row = 2; row <= rowCount; row++)  // Assuming the first row is header
            {
                string projectCode = worksheet.Cells[row, 3].Text;  // Read the Project Code (Column 1)
                string pods = worksheet.Cells[row, 5].Text;  // Read the PODs (Column 2)

                if (!string.IsNullOrEmpty(projectCode) && !string.IsNullOrEmpty(pods))
                {
                    // Split POD names into a list and assign to the project code
                    var podList = new List<string>(pods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    ProjectCodeToPODMapping[projectCode] = podList;
                }
            }
        }

        public JsonResult OnGetPODNames(string projectCode)
        {
            LoadProjectCodeToPODMapping();
            if (ProjectCodeToPODMapping.ContainsKey(projectCode))
            {
                var podNames = ProjectCodeToPODMapping[projectCode];
                return new JsonResult(podNames);
            }else{

            return new JsonResult(new List<string>()); // Return an empty list if project code not found
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
            CertificationOptions = new List<SelectListItem>();
            VISATypeOptions = new List<SelectListItem>();
            

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
                        var certificate = worksheet.Cells[row,11]?.Text?.Trim();
                        var visaType  = worksheet.Cells[row,12]?.Text?.Trim();

                        if (!string.IsNullOrWhiteSpace(grade)) 
                        { 
                            GradeOptions.Add(new SelectListItem { Value = grade, Text = grade });
                            GradeToGlobalGrade.Add(grade, globalgrade);
                        }

                        if (!string.IsNullOrWhiteSpace(bu))
                            BUOptions.Add(new SelectListItem { Value = bu, Text = bu });
                        if (!string.IsNullOrWhiteSpace(bgv))
                            BGVOptions.Add(new SelectListItem { Value = bgv, Text = bgv });

                    
                        if (!string.IsNullOrWhiteSpace(projectcode) && !string.IsNullOrWhiteSpace(projectname))
                        {
                            ProjectCodeOptions.Add(new SelectListItem { Value = projectcode, Text = projectcode });
                             projectCodeToNameMapping.Add(projectcode, projectname);
                        }
                        
                        if (!string.IsNullOrWhiteSpace(PODname))
                        {
                            PODNameOptions.Add(new SelectListItem  { Value = "", Text = "--Select POD--" });

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
                        if (!string.IsNullOrWhiteSpace(certificate))
                        {
                           

                            CertificationOptions.Add(new SelectListItem { Value = certificate, Text = certificate });
                            
                        }
                         if (!string.IsNullOrWhiteSpace(visaType))
                        {
                            VISATypeOptions.Add(new SelectListItem { Value = visaType, Text = visaType });
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

        private Employee GetEmployeeById(string empId)
        {
            var employees = GetAllEmployees();
            return employees.FirstOrDefault(emp => emp.EmpId == empId);
        }


        private int GetEmployeeRow(ExcelWorksheet worksheet, string empId,int projectCode)
        {
            var rowCount = worksheet.Dimension?.Rows ?? 6;
            for (int row = 6; row <= rowCount; row++)
            {
                if (worksheet.Cells[row, 15].Text == empId && worksheet.Cells[row, 7].Text == projectCode.ToString())
                {
                    return row; // Row number where match is found
                }
            }
            return -1;
        }


         private Dictionary<string,object> GetEmployeeData()
        {
            return typeof(Employee).GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(Employee));
        }

        private void AddEmployeeToExcel(ExcelWorksheet worksheet, int row)
        {
            // Find last used row
            int lastRow = worksheet.Dimension?.Rows ?? 5; // Adjust default value based on header rows
            for (int i = lastRow; i >= 6; i--) // Start from bottom, excluding header rows
            {
                if (worksheet.Cells[i, 1].Value != null) // Check if any cell in the row has data
                {
                    lastRow = i;
                    break;
                }
            }

            // Add new row at the end (lastRow + 1)
            row = lastRow + 1;
            worksheet.Cells[row, 15].Value = Employee.EmpId; // EmpId
            worksheet.Cells[row, 14].Value = Employee.GGID;// GGID
            worksheet.Cells[row, 17].Value = Employee.Resource; // Resource
            worksheet.Cells[row, 16].Value = Employee.Email; // Email
            worksheet.Cells[row, 21].Value = Employee.Gender; // Gender
            worksheet.Cells[row, 127].Value = Employee.DateOfHire.ToString("dd-MM-yyy"); // DateOfHire
            worksheet.Cells[row, 18].Value = Employee.Grade; // Grade
            worksheet.Cells[row, 19].Value = Employee.GlobalGrade; // GlobalGrade
            worksheet.Cells[row, 4].Value = Employee.BU; // BU
            worksheet.Cells[row, 20].Value = Employee.IsActiveInProject; // IsActiveInProject
            worksheet.Cells[row, 27].Value = Employee.OverallExp; // OverallExp
            worksheet.Cells[row, 28].Value = Employee.Skills; // Skills
            worksheet.Cells[row, 33].Value = Employee.Certificates; // Certificates
            worksheet.Cells[row, 128].Value = Employee.AltriaStartdate.ToString("dd-MM-yyy"); // AltriaStartdate
            worksheet.Cells[row, 129].Value = Employee.AltriaEnddate.ToString("dd-MM-yyy"); // AltriaEnddate
            worksheet.Cells[row, 130].Value = Employee.BGVStatus; // BGVStatus
            worksheet.Cells[row, 134].Value = Employee.BGVCompletionDate.ToString("dd-MM-yyy"); // BGV Enddate
            worksheet.Cells[row, 131].Value = Employee.VISAStatus; // VISAStatus
            worksheet.Cells[row, 135].Value = Employee.VISAType; // VISAType

            worksheet.Cells[row, 1].Value = Employee.Type; // Column 1: Type
            worksheet.Cells[row, 2].Value = Employee.Tower; // Column 2: Tower
            worksheet.Cells[row, 3].Value = Employee.ABLGBL; // Column 3: ABLGBL
            worksheet.Cells[row, 5].Value = Employee.TLName; // Column 5: TLName
            worksheet.Cells[row, 7].Value = Employee.ProjectCode; // Column 7: ProjectCode
            worksheet.Cells[row, 8].Value = Employee.ProjectName; // Column 8: ProjectName
            worksheet.Cells[row, 9].Value = Employee.PONumber; // Column 9: PONumber
            worksheet.Cells[row, 10].Value = Employee.PODName; // Column 10: PODName
            worksheet.Cells[row, 12].Value = Employee.AltriaPODOwner; // Column 12: AltriaPODOwner
            worksheet.Cells[row, 13].Value = Employee.ALCSDirector; // Column 13: ALCSDirector
            worksheet.Cells[row, 22].Value = Employee.Location; // Column 22: Location
            worksheet.Cells[row, 23].Value = Employee.OffshoreCity; // Column 23: OffshoreCity
            worksheet.Cells[row, 34].Value = Employee.OffshoreBackup; // Column 34: OffshoreBackup
            worksheet.Cells[row, 35].Value = Employee.Transition; // Column 35: Transition
            worksheet.Cells[row, 60].Value = Employee.COR; // Column 60: COR
            worksheet.Cells[row, 62].Value = Employee.Group; // Column 62: Group
            worksheet.Cells[row, 25].Value = Employee.RoleinPOD; // Column 25: RoleinPOD
            worksheet.Cells[row, 63].Value = Employee.MonthlyPrice; // Column 63: MonthlyPrice
            worksheet.Cells[row, 26].Value = Employee.AltriaEXP; // Column 63: Altria Exp

            // Dates: Ensure that start and end dates are formatted correctly
            worksheet.Cells[row, 132].Value = Employee.StartDate.ToString("dd-MM-yyy"); // Column 131: StartDate
            worksheet.Cells[row, 133].Value = Employee.EndDate.ToString("dd-MM-yyy"); // Column 132: EndDate

            worksheet.Cells[row, 36].Value = Employee.January; // Column 36: January
            worksheet.Cells[row, 37].Value = Employee.February; // Column 37: February
            worksheet.Cells[row, 38].Value = Employee.March; // Column 38: March
            worksheet.Cells[row, 39].Value = Employee.April; // Column 39: April
            worksheet.Cells[row, 40].Value = Employee.May; // Column 40: May
            worksheet.Cells[row, 41].Value = Employee.June; // Column 41: June
            worksheet.Cells[row, 42].Value = Employee.July; // Column 42: July
            worksheet.Cells[row, 43].Value = Employee.August; // Column 43: August
            worksheet.Cells[row, 44].Value = Employee.September; // Column 44: September
            worksheet.Cells[row, 45].Value = Employee.October; // Column 45: October
            worksheet.Cells[row, 46].Value = Employee.November; // Column 46: November
            worksheet.Cells[row, 47].Value = Employee.December; // Column 47: December
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

            [HttpGet]
            public IActionResult OnGetProjectName(string projectCode)
            {
            LoadDropdownOptions();
            if (string.IsNullOrWhiteSpace(projectCode))
                return new JsonResult("Invalid Project Code");

            if (projectCodeToNameMapping.TryGetValue(projectCode, out var projectName))
                return new JsonResult(projectName);

            return new JsonResult("Project Code not found");
             }

             [HttpGet]
            public IActionResult OnGetGlobalGrade(string grade)
            {
            LoadDropdownOptions();
            if (string.IsNullOrWhiteSpace(grade))
                return new JsonResult("Invalid Grade");

            if (GradeToGlobalGrade.TryGetValue(grade, out var GlobalGrade))
                return new JsonResult(GlobalGrade);

            return new JsonResult("Global Grade not found");
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