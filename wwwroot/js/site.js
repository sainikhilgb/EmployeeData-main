﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getDate() {
    // Get the current date
    const today = new Date();
    // Format the date as YYYY-MM-DD
    const formattedDate = today.toISOString().split('T')[0];
     // Select all elements with the class 'date-field'
     const dateFields = document.querySelectorAll('.date-field');
     // Loop through each date field and set the value
     dateFields.forEach(field => {
         field.value = formattedDate;
     });
}
window.onload = getDate;

function fetchProjectName(projectCode) {
  $.ajax({
      url: `Registration?handler=ProjectName&projectCode=${projectCode}`,
      type: "GET",
      success: function(projectName) {
          $("#ProjectName").val(projectName);
      },
      error: function(error) {
          console.error("Error fetching project name:", error);
      }
  });
}

function fetchGlobalGrade(grade) {
    $.ajax({
        url: `Registration?handler=GlobalGrade&Grade=${grade}`,
        type: "GET",
        success: function(GlobalGrade) {
            $("#GlobalGrade").val(GlobalGrade);
        },
        error: function(error) {
            console.error("Error fetching project name:", error);
        }
    });
  }

  
  function editEmployee(empId) {
    $.ajax({
        url: `Registration/Registration?handler=empId=${empId}`,
        type: "GET",
        success: function(GlobalGrade) {
            $("#GlobalGrade").val(GlobalGrade);
        },
        error: function(error) {
            console.error("Error fetching project name:", error);
        }
    });
    window.location.href = `http://localhost:5165/Registration/Registration?handler=empId=${empId}`;
}

$(document).ready(function () {
    $('#ProjectCode').change(function () {
        var projectCode = $(this).val(); // Get selected project code
        $('#PODName').empty().append('<option value="">--Select POD--</option>'); // Clear previous options

        if (projectCode) {
            $.ajax({
                url: '/Registration/Registration', // The URL of the Razor Page
                data: { handler: 'PODNames', projectCode: projectCode }, // Pass handler and projectCode
                
                success: function (data) {
                    console.log('Data received:', data);
                    // Populate the POD dropdown with the response
                    $.each(data, function (index, podName) {
                        $('#PODName').append('<option value="' + podName + '">' + podName + '</option>');
                    });
                },
                error: function (xhr, status, error) {
                    console.error('AJAX Error: ' + error); // Log any errors for debugging
                }
            });
        }
    });
});

function toggleOtherTextbox() {
    var dropdown = document.getElementById('Certificates');
    var otherCertificate = document.getElementById('OtherCertificate');
    if (dropdown.value === 'Others') {
        otherCertificate.style.display = 'block';
    } else {
        otherCertificate.style.display = 'none';
        // Submit the form if it was visible previously
        if (otherCertificate.style.display === '') {
            document.forms[0].submit(); // Submit the first form on the page
        }
    }
}

function fillAllValues(selectedDropdown) {
    const selectedValue = selectedDropdown.value;
    const row = selectedDropdown.closest('tr');
    const dropdowns = row.querySelectorAll('select');
  
    for (let i = 1; i < dropdowns.length; i++) {
      dropdowns[i].value = selectedValue;
    }
  }

// const monthlyPrice = cor * offshore;

            // document.getElementById("MonthlyPrice").value = monthlyPrice.toFixed(2);
  function calculateMonthlyPrice() {
    const corInput = document.getElementById("COR");
    const cor = parseFloat(corInput.value);
    const loc = document.getElementById("location").value;
   
    $.ajax({
        url: "/Registration/Registration",
        data: { handler: 'MonthlyPrice', COR: cor,location:loc },
        type: "GET", // Or "POST" if needed
        success: function(GlobalGrade) {
            $("#MonthlyPrice").val(GlobalGrade);
        },
    });
            
        }

function openConfirmationModal(endDate, isActive) {
    // Populate modal fields
   
    console.log(isActive,endDate)

    // Populate radio buttons for IsActiveInProject
    if (isActive === "Y") {
        document.getElementById('activeYes').checked = true;
    } else if (isActive === "N") {
        document.getElementById('activeNo').checked = true;
    }

    // Populate the date field for AltriaEnddate
    document.getElementById('AltriaEnddate').value = endDate.ToString("dd-MM-yyy");

}

function openConfirmation(endDate) {


    // Populate the date field for AltriaEnddate
    document.getElementById('EndDate').value = endDate.ToString("dd-MM-yyy");

}
function logout()
{
    window.location.href = "/Logout";

}
 