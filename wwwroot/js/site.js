// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
AOS.init({
  offset: 300,
  once: true,
});
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

          $(productRow).fadeOut(300, function () {
            productRow.remove();
            // Wait other 100ms then reload the page
            setTimeout(() => {
              window.location.reload(true);
            }, 100);
          });
          console.log(
            "Product removed successfully:",
            document.querySelectorAll(".cart-row").length
          );
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

        // Update quantity of the element in the same row but with class cart-quantity-value
        const quantityElement = this.parentElement.querySelector(
          ".cart-quantity-value"
        );
        const quantity = parseInt(quantityElement.textContent);

        $(quantityElement).fadeOut(150, function () {
          quantityElement.textContent = quantity + 1;
          $(this).fadeIn();
          // Wait other 100ms then reload the page
          setTimeout(() => {
            window.location.reload(true);
          }, 100);
        });
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
        // Update quantity of the element in the same row but with class cart-quantity-value
        const quantityElement = this.parentElement.querySelector(
          ".cart-quantity-value"
        );
        const quantity = parseInt(quantityElement.textContent);

        $(quantityElement).fadeOut(150, function () {
          quantityElement.textContent = quantity - 1;
          $(this).fadeIn();
          // Wait other 100ms then reload the page
          setTimeout(() => {
            window.location.reload(true);
          }, 100);
        });
      } catch (error) {
        console.error("Error:", error);
      }
    });
  });
}
