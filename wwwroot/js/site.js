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
        const orderId = this.getAttribute("data-order-id");
        const productId = this.getAttribute("data-product-id");

        // Make sure the orderId and productId are valid
        if (!orderId || !productId) {
          console.error("Invalid data");
          return;
        }

        try {
          const response = await fetch("/Cart/RemoveProduct", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              orderId: parseInt(orderId),
              productId: parseInt(productId),
            }),
          });

          console.log("Response:", response);

          if (!response.ok) {
            console.error("Failed to remove product:", response.statusText);
            return;
          }

          const result = await response.json();
          console.log("Product removed successfully:", result);

          // Remove the product row (parent of the parent of the button)
          const productRow = this.parentElement.parentElement;
          productRow.remove();
          // if there are no more products in the cart, reolad the page
          if (document.querySelectorAll(".cart-row").length == 0) {
            window.location.reload(true);
          }
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
      const orderId = this.getAttribute("data-order-id");
      const productId = this.getAttribute("data-product-id");

      // Make sure the orderId and productId are valid
      if (!orderId || !productId) {
        console.error("Invalid data");
        return;
      }
      try {
        const response = await fetch("/Cart/IncreaseProductQuantity", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            orderId: parseInt(orderId),
            productId: parseInt(productId),
          }),
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

        // Force page reload
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
      const orderId = this.getAttribute("data-order-id");
      const productId = this.getAttribute("data-product-id");

      // Make sure the orderId and productId are valid
      if (!orderId || !productId) {
        console.error("Invalid data");
        return;
      }

      try {
        const response = await fetch("/Cart/DecreaseProductQuantity", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            orderId: parseInt(orderId),
            productId: parseInt(productId),
          }),
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

        // force page reload
        window.location.reload(true);
      } catch (error) {
        console.error("Error:", error);
      }
    });
  });
}
