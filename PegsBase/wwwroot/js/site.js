
function showToast(message, type = 'success') {
    const toastEl = document.getElementById('appToast');
    const toastBody = document.getElementById('toast-body');

    // Update content and style
    toastBody.textContent = message;
    toastEl.className = 'toast align-items-center text-white border-0 bg-' + type;

    const toast = new bootstrap.Toast(toastEl, {delay: 4000 });
    toast.show();
}

function showSpinner() {
    document.getElementById('loadingSpinner').style.display = 'block';
}

function hideSpinner() {
    document.getElementById('loadingSpinner').style.display = 'none';
}

function loadDataFromDb() {
    showSpinner();

    $.ajax({
        url: '/Peg/GetRecentPegs',
        method: 'GET',
        success: function (data) {
            // Do something with data (e.g., update DOM)
            showToast('Data loaded!', 'success');
        },
        error: function () {
            showToast('Error loading data.', 'danger');
        },
        complete: function () {
            hideSpinner();
        }
    });
}
