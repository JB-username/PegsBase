document.addEventListener("DOMContentLoaded", function () {
    const noteTypeSelect = document.getElementById("noteTypeFilter");
    const notesTableBody = document.querySelector("#notesTable tbody");
    const loadingIndicator = document.getElementById("loadingIndicator");

    function loadNotes(noteType = "") {
        loadingIndicator.style.display = "block";
        fetch(`/SurveyNotes/GetNotes?noteType=${noteType}`)
            .then(response => response.text())
            .then(html => {
                notesTableBody.innerHTML = html;
                loadingIndicator.style.display = "none";
            })
            .catch(error => {
                console.error("Error loading notes:", error);
                notesTableBody.innerHTML = "<tr><td colspan='6'>Failed to load notes.</td></tr>";
                loadingIndicator.style.display = "none";
            });
    }

    noteTypeSelect.addEventListener("change", function () {
        loadNotes(this.value);
    });

    // Initial load
    loadNotes(noteTypeSelect.value);
});
