
document.addEventListener("DOMContentLoaded", () => {
    const originSearch = document.getElementById("originSearch");
    const originId = document.getElementById("originId");
    const originSuggestions = document.getElementById("originSuggestions");

    const destinationSearch = document.getElementById("destinationSearch");
    const destinationId = document.getElementById("destinationId");
    const destinationSuggestions = document.getElementById("destinationSuggestions");

    const departureDate = document.getElementById("departureDate");
    const validationMessage = document.getElementById("validationMessage");


    const storageKey = "obilet-last-search";

function saveSearch() {
    if (!originId.value || !destinationId.value || !departureDate.value) {
        return;
    }

    localStorage.setItem(storageKey, JSON.stringify({
        originId: originId.value,
        originName: originSearch.value,
        destinationId: destinationId.value,
        destinationName: destinationSearch.value,
        departureDate: departureDate.value
    }));
}



function restoreSearch() {
    const savedSearch = localStorage.getItem(storageKey);

    if (!savedSearch) {
        return;
    }

    try {
        const search = JSON.parse(savedSearch);

        if (search.originId && search.originName) {
            originId.value = search.originId;
            originSearch.value = search.originName;
        }

        if (search.destinationId && search.destinationName) {
            destinationId.value = search.destinationId;
            destinationSearch.value = search.destinationName;
        }

        if (search.departureDate) {
            departureDate.value = search.departureDate;
        }
    } catch (error) {
        console.error("Kayıtlı arama okunamadı:", error);
        localStorage.removeItem(storageKey);
    }
}



    function formatDate(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");

        return `${year}-${month}-${day}`;
    }

    function validateForm() {
        const today = formatDate(new Date());

if (!originId.value || !destinationId.value) {
    validationMessage.classList.add("d-none");
    return false;
}
if (!originId.value || !destinationId.value) {
    return false;
}

        if (originId.value === destinationId.value) {
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

    function setupAutocomplete(searchInput, hiddenId, suggestions) {
        let searchTimeout;
        let requestController;

searchInput.addEventListener("focus", () => {
    suggestions.style.display = "none";
});

        searchInput.addEventListener("input", () => {
            hiddenId.value = "";
            suggestions.replaceChildren();

            const keyword = searchInput.value.trim();

            clearTimeout(searchTimeout);

            if (requestController) {
                requestController.abort();
            }

            if (keyword.length < 2) {
                suggestions.style.display = "none";
                validateForm();
                return;
            }

            searchTimeout = setTimeout(async () => {
                const controller = new AbortController();
                requestController = controller;

                try {
                    const response = await fetch(
                        `/Home/SearchLocations?keyword=${encodeURIComponent(keyword)}`,
                        { signal: controller.signal }
                    );

                    if (!response.ok) {
                        throw new Error("Lokasyon araması başarısız.");
                    }

                    const locations = await response.json();

                    // Daha yeni bir istek başladıysa eski sonucu gösterme.
                    if (controller !== requestController) {
                        return;
                    }

                    suggestions.replaceChildren();

                    if (locations.length === 0) {
                        const emptyItem = document.createElement("div");
                        emptyItem.className = "list-group-item text-muted";
                        emptyItem.textContent = "Lokasyon bulunamadı.";
                        suggestions.appendChild(emptyItem);
                    } else {
                        locations.forEach(location => {
                            const item = document.createElement("button");
                            item.type = "button";
                            item.className = "list-group-item list-group-item-action";
                            item.textContent = location.longName || location.name;

item.addEventListener("click", () => {
    searchInput.value = location.name;
    hiddenId.value = location.id;

    suggestions.style.display = "none";
    validateForm();
});

                            suggestions.appendChild(item);
                        });
                    }

                    suggestions.style.display = "block";
                } catch (error) {
                    if (error.name === "AbortError") {
                        return;
                    }

                    console.error("Lokasyon arama hatası:", error);
                    suggestions.style.display = "none";
                }
            }, 300);
        });

        document.addEventListener("click", event => {
            if (!searchInput.contains(event.target) &&
                !suggestions.contains(event.target)) {
                suggestions.style.display = "none";
            }
        });
    }

    setupAutocomplete(originSearch, originId, originSuggestions);
    setupAutocomplete(destinationSearch, destinationId, destinationSuggestions);

    document.getElementById("swapLocations").addEventListener("click", () => {
        const originValue = originId.value;
        const destinationValue = destinationId.value;
        const originText = originSearch.value;
        const destinationText = destinationSearch.value;

        originId.value = destinationValue;
        destinationId.value = originValue;

        originSearch.value = destinationText;
        destinationSearch.value = originText;

        originSuggestions.style.display = "none";
        destinationSuggestions.style.display = "none";

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

    departureDate.addEventListener("change", validateForm);

document.querySelector("form").addEventListener("submit", event => {
    const originValid = originId.value.trim() !== "";
    const destinationValid = destinationId.value.trim() !== "";

    if (!originValid || !destinationValid) {
        event.preventDefault();

        if (!originValid && !destinationValid) {
            validationMessage.textContent =
                "Lütfen kalkış ve varış noktalarını seçin.";
        } else if (!originValid) {
            validationMessage.textContent =
                "Lütfen kalkış noktasını seçin.";
        } else {
            validationMessage.textContent =
                "Lütfen varış noktasını seçin.";
        }

        validationMessage.classList.remove("d-none");
        return;
    }

    if (!validateForm()) {
        event.preventDefault();
         return;
    }
    saveSearch();
});

restoreSearch();
});
