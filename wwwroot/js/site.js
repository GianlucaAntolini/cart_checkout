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
if (currentPage == "Cart") {
  document.addEventListener("DOMContentLoaded", function () {
    const removeProductButtons = document.querySelectorAll(
      ".remove-product-btn"
    );

    removeProductButtons.forEach((button) => {
      button.addEventListener("click", async function () {
        const orderProductId = this.getAttribute("data-order-product-id");

        // Make sure the orderProductId is valid
        if (!orderProductId) {
          console.error("Invalid OrderProductId");
          return;
        }

        try {
          const response = await fetch("/Cart/RemoveProduct", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({ orderProductId: parseInt(orderProductId) }),
          });

          console.log("Response:", response);

          if (!response.ok) {
            console.error("Failed to remove product:", response.statusText);
            return;
          }

          const result = await response.json();
          console.log("Product removed successfully:", result);

          // Optionally, you can refresh the page or update the UI to reflect changes
          // force page reload
          window.location.reload(true);
        } catch (error) {
          console.error("Error:", error);
        }
      });
    });
  });

  const increaseProductQuantityButtons = document.querySelectorAll(
    ".cart-quantity-plus"
  );
  increaseProductQuantityButtons.forEach((button) => {
    button.addEventListener("click", async function () {
      const orderProductId = this.getAttribute("data-order-product-id");

      // Make sure the orderProductId is valid
      if (!orderProductId) {
        console.error("Invalid OrderProductId");
        return;
      }

      try {
        const response = await fetch("/Cart/IncreaseProductQuantity", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ orderProductId: parseInt(orderProductId) }),
        });

        console.log("Response:", response);

        if (!response.ok) {
          console.error(
            "Failed to increase product quantity:",
            response.statusText
          );
          return;
        }

        const result = await response.json();
        console.log("Product quantity increased successfully:", result);

        // Optionally, you can refresh the page or update the UI to reflect changes
        // force page reload
        window.location.reload(true);
      } catch (error) {
        console.error("Error:", error);
      }
    });
  });

  const decreaseProductQuantityButtons = document.querySelectorAll(
    ".cart-quantity-minus"
  );

  decreaseProductQuantityButtons.forEach((button) => {
    button.addEventListener("click", async function () {
      const orderProductId = this.getAttribute("data-order-product-id");

      // Make sure the orderProductId is valid
      if (!orderProductId) {
        console.error("Invalid OrderProductId");
        return;
      }

      try {
        const response = await fetch("/Cart/DecreaseProductQuantity", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ orderProductId: parseInt(orderProductId) }),
        });

        console.log("Response:", response);

        if (!response.ok) {
          console.error(
            "Failed to decrease product quantity:",
            response.statusText
          );
          return;
        }

        const result = await response.json();
        console.log("Product quantity decreased successfully:", result);

        // Optionally, you can refresh the page or update the UI to reflect changes
        // force page reload
        window.location.reload(true);
      } catch (error) {
        console.error("Error:", error);
      }
    });
  });
}
