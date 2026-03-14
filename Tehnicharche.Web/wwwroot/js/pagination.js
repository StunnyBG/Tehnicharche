// wwwroot/js/pagination.js
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("filtersForm");
    const pageInput = document.getElementById("Page");

    if (!form || !pageInput) return;

    // Delegate click handler for any .page-btn inside the form
    form.addEventListener("click", function (e) {
        const btn = e.target.closest && e.target.closest(".page-btn");
        if (!btn) return;

        const page = btn.getAttribute("data-page");
        if (!page) return;

        // set and submit
        pageInput.value = page;
        form.submit();
    });

    // When any filter input changes, reset page to 1
    // Keep selector broad (selects, inputs, checkboxes, radios)
    form.addEventListener("change", function (e) {
        // only reset if the change originated from a filter control (not pager)
        const target = e.target;
        if (!target) return;

        // ignore changes on the hidden Page input itself
        if (target.id === "Page") return;

        pageInput.value = 1;
    });
});