using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeData.Models 
{
    public class Employee 
    {
        [Key]
        [Required(ErrorMessage = "Please enter EmployeeId")]
        public string EmpId {get; set;}
        [Required(ErrorMessage = "Please enter GGID")]
        public int GGID {get; set;}

        [Required(ErrorMessage = "Please enter your first name")]
        [StringLength(50)]
        public string Resource {get; set;}

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress]  
        public string Email {get; set;}
        [Required(ErrorMessage = "Please Select your Grade")]
        public string Grade {get; set;}
        public string GlobalGrade {get; set;}
        
        [Required(ErrorMessage = "Please enter the date of hire")]
        [DataType(DataType.Date)]
        [DisplayName("Date Of Hire")]
         public DateTime DateOfHire {get; set;}
    
        [Required(ErrorMessage = "Select Yes for tagging/working in project")]
        public string IsActiveInProject { get; set; }
        [Required(ErrorMessage = "Please Select the BU")]
         public string BU {get; set;}

        public string Gender { get; set; }

        public decimal OverallExp {get; set;}
        public string Skills {get; set;}
        public string Certificates {get; set;}
    
        public string? OtherCertificate {get; set;}

        [Required(ErrorMessage = "Please enter the Altria Start date")]
        [DataType(DataType.Date)]
        [DisplayName("Altria Start date")]
         public DateTime AltriaStartdate {get; set;}

        [Required(ErrorMessage = "Please enter the Altria End date")]
        [DataType(DataType.Date)]
        [DisplayName("Altria End date")]
         public DateTime AltriaEnddate {get; set;}

         [Required(ErrorMessage = "Please select BGV Status")]
         public string BGVStatus {get; set;}
         
         [Required(ErrorMessage = "Please enter the BGV Completion Date")]
        [DataType(DataType.Date)]
        [DisplayName("BGV Completion Date")]
         public DateTime BGVCompletionDate {get; set;}

         [Required(ErrorMessage = "Please select VISA Availability")]
         public string VISAStatus {get; set;}

         [Required(ErrorMessage = "Please select VISA Type")]
         public string VISAType {get; set;}

         [DisplayName("Project Code")]
         public int ProjectCode {get; set;}

        [DisplayName("Project name")]
        public string? ProjectName {get; set;}
        
        public int PONumber {get;set;}
        [Required(ErrorMessage = "Please Select the POD name")]
        [DisplayName("POD name")]
        public string PODName {get; set;}
        [Required(ErrorMessage = "Please Enter the Altria POD name")]
        public string AltriaPODOwner {get; set;}
        [Required(ErrorMessage = "Please Enter the ALCS Director")]
        public string ALCSDirector {get; set;}
        [Required(ErrorMessage = "Please Select the Type")]
        
        public string Type {get;set;}
        [Required(ErrorMessage = "Please Select the Tower")]
        public string Tower {get;set;}
        [Required(ErrorMessage = "Please Select the ABL or GBL")]
        public string? ABLGBL {get;set;}
        [Required(ErrorMessage = "Please Select the POD name")]
        public string TLName {get;set;}

        [Required(ErrorMessage = "Please select Onshore or Offshore")]
        public string Location {get; set;}
        [DisplayName("Offshore City")]
        public string OffshoreCity {get; set;}
        [DisplayName("Offshore Backup")]
        public string OffshoreBackup {get; set;}

        public string Transition {get; set;}
        [DisplayName("Altria EXP")]
        public decimal AltriaEXP {get; set;}
        
        [Required(ErrorMessage = "Please enter the Project Start date")]
        [DataType(DataType.Date)]
        [DisplayName("Start date")]
         public DateTime StartDate {get; set;}

        [Required(ErrorMessage = "Please enter the Project end date")]
        [DataType(DataType.Date)]
        [DisplayName("End date")]
         public DateTime EndDate {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal January {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal February {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal March {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal April {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal May {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal June {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal July {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal August {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal September {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal October {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal November {get; set;}
         [Range(0,1,ErrorMessage ="Value Should be in between 0-1")]
         public decimal December {get; set;}

         public string COR {get; set;}

         public string Group {get; set;}
         [DisplayName("Monthly Price")]
        public decimal MonthlyPrice {get; set;} 
       
        [DisplayName("Role in POD")]
        public string RoleinPOD {get; set;}



       
        
    }
}