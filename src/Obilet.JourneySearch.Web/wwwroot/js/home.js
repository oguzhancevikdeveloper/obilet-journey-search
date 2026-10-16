document.addEventListener("DOMContentLoaded", () => {
    const originSelect = document.getElementById("originId");
    const destinationSelect = document.getElementById("destinationId");
    const departureDate = document.getElementById("departureDate");
    const validationMessage = document.getElementById("validationMessage");

    function formatDate(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");

        return `${year}-${month}-${day}`;
    }

    function validateForm() {
        const today = formatDate(new Date());

        if (originSelect.value === destinationSelect.value) {
            validationMessage.textContent =
                "Kalkış ve varış noktaları aynı olamaz.";
            validationMessage.classList.remove("d-none");
            return false;
        }

        if (!departureDate.value || departureDate.value < today) {
            validationMessage.textContent =
                "Gidiş tarihi bugün veya daha ileri bir tarih olmalıdır.";
            validationMessage.classList.remove("d-none");
            return false;
        }

        validationMessage.textContent = "";
        validationMessage.classList.add("d-none");
        return true;
    }

    document.getElementById("swapLocations").addEventListener("click", () => {
        const originValue = originSelect.value;

        originSelect.value = destinationSelect.value;
        destinationSelect.value = originValue;

        validateForm();
    });

    document.getElementById("todayButton").addEventListener("click", () => {
        departureDate.value = formatDate(new Date());
        validateForm();
    });

    document.getElementById("tomorrowButton").addEventListener("click", () => {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);

        departureDate.value = formatDate(tomorrow);
        validateForm();
    });

    originSelect.addEventListener("change", validateForm);
    destinationSelect.addEventListener("change", validateForm);
    departureDate.addEventListener("change", validateForm);

    validateForm();
});