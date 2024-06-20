// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var currentPage = location.pathname.split("/")[1];
if (currentPage == "UserInfo") {
  document.addEventListener("DOMContentLoaded", function () {
    var invoiceCheckbox = document.getElementById("invoice-checkbox");
    var fiscalTaxField = document.getElementById("fiscal-tax-field");
    var fiscalCodeField = document.getElementById("fiscal-code-field");

    function toggleFiscalFields() {
      if (invoiceCheckbox.checked) {
        fiscalTaxField.style.display = "block";
        fiscalCodeField.style.display = "block";
      } else {
        fiscalTaxField.style.display = "none";
        fiscalCodeField.style.display = "none";
      }
    }

    invoiceCheckbox.addEventListener("change", toggleFiscalFields);

    // Initial check
    toggleFiscalFields();
  });
}
