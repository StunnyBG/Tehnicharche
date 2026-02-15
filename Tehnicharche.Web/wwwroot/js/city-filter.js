document.addEventListener("DOMContentLoaded", function () {

    const regionSelect = document.querySelector('[name="RegionId"]');
    const citySelect = document.getElementById("CitySelect");

    if (!regionSelect || !citySelect) return;

    function filterCities() {
        const regionId = regionSelect.value;

        Array.from(citySelect.options).forEach(option => {
            const optionRegion = option.getAttribute("data-region");

            if (!optionRegion || option.value === "") {
                option.hidden = false;
                return;
            }

            option.hidden = optionRegion !== regionId;
        });

        if (citySelect.selectedOptions.length &&
            citySelect.selectedOptions[0].hidden) {
            citySelect.value = "";
        }
    }

    regionSelect.addEventListener("change", filterCities);
    filterCities();
});
