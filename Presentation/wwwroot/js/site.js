function toggleDropdown() {
    var dropdown = document.getElementById('account-dropdown');
    dropdown.classList.toggle('show');
    dropdown.classList.toggle('hide');
}

function toggleModal() {
    var modal = document.getElementById('add-project-modal');
    modal.classList.toggle('hide');
}

function closeModal() {
    var modal = document.getElementById('add-project-modal');
    modal.classList.add('hide');
}

function openEditModal(projectId) {
    const modal = document.getElementById('edit-project-modal');
    modal.classList.remove('hide');
    modal.classList.add('show');
    populateEditForm(projectId);

    setTimeout(async () => {
        const textarea = document.getElementById('edit-project-description');

        if (!window.editProjectDescriptionQuill) {
            window.editProjectDescriptionQuill = new Quill('#edit-project-description-wysiwyg-editor', {
                modules: {
                    toolbar: '#edit-project-description-wysiwyg-toolbar'
                },
                theme: 'snow',
                placeholder: 'Type something...'
            });

            window.editProjectDescriptionQuill.on('text-change', function () {
                textarea.value = window.editProjectDescriptionQuill.root.innerHTML;
            });
        }

        await populateEditForm(projectId);

    }, 100);
}


async function populateEditForm(projectId) {
    const response = await fetch(`/api/projects/${projectId}`);
    if (!response.ok) {
        alert("Kunde inte hämta projektdata.");
        return;
    }

    const data = await response.json();

    document.getElementById("edit-project-id").value = data.id;
    document.getElementById("edit-project-name").value = data.projectName;
    document.getElementById("edit-client-name").value = data.clientName;
    document.getElementById("edit-project-description").value = data.description;

    const today = new Date();
    const nextWeek = new Date();
    nextWeek.setDate(today.getDate() + 7);

    document.getElementById("edit-start-date").value =
        data.startDate && data.startDate !== '0001-01-01T00:00:00'
            ? data.startDate.substring(0, 10)
            : today.toISOString().split("T")[0];

    document.getElementById("edit-end-date").value =
        data.endDate
            ? data.endDate.substring(0, 10)
            : nextWeek.toISOString().split("T")[0];

    document.getElementById("edit-budget").value = data.budget;
    document.getElementById("edit-status-id").value = data.statusId;

    if (window.editProjectDescriptionQuill) {
        window.editProjectDescriptionQuill.root.innerHTML = data.description;
    }
}


function toggleEdit(projectId) {
    const dropdown = document.getElementById(`project-dropdown-${projectId}`);
    if (!dropdown) {
        console.warn(`Dropdown hittades inte för id: ${projectId}`);
        return;
    }
    dropdown.classList.toggle('hide');
    dropdown.classList.toggle('show');
}

function closeAllModals() {
    const modals = document.querySelectorAll('.modal');
    modals.forEach(modal => modal.classList.add('hide'));
}

function closeEditModal() {
    const modal = document.getElementById('edit-project-modal');
    if (modal) {
        modal.classList.add('hide');
        modal.classList.remove('show');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const addProjectDescriptionTextarea = document.getElementById('add-project-description');
    const addQuillContainer = document.getElementById('add-project-description-wysiwyg-editor');
    if (addProjectDescriptionTextarea && addQuillContainer) {
        const addProjectDescriptionQuill = new Quill('#add-project-description-wysiwyg-editor', {
            modules: {
                toolbar: '#add-project-description-wysiwyg-toolbar'
            },
            theme: 'snow',
            placeholder: 'Type something'
        });

        addProjectDescriptionQuill.on('text-change', function () {
            addProjectDescriptionTextarea.value = addProjectDescriptionQuill.root.innerHTML;
        });
    }

    const editProjectDescriptionTextarea = document.getElementById('edit-project-description');
    const editQuillContainer = document.getElementById('edit-project-description-wysiwyg-editor');
    if (editProjectDescriptionTextarea && editQuillContainer) {
        const editProjectDescriptionQuill = new Quill('#edit-project-description-wysiwyg-editor', {
            modules: {
                toolbar: '#edit-project-description-wysiwyg-toolbar'
            },
            theme: 'snow',
            placeholder: 'Type something'
        });

        editProjectDescriptionQuill.on('text-change', function () {
            editProjectDescriptionTextarea.value = editProjectDescriptionQuill.root.innerHTML;
        });
    }
});


document.addEventListener('click', function (e) {
    const dropdown = document.getElementById('account-dropdown');
    const button = document.querySelector('.btn-account');
    const modal = document.getElementById('add-project-modal');
    const isDropdown = e.target.closest('.dropdown-action');
    const isEditButton = e.target.closest('.btn-edit');
    const isDropdownContent = e.target.closest('.project-edit-dropdown');

    if (dropdown && !dropdown.contains(e.target) && !button?.contains(e.target) && !isDropdown) {
        dropdown.classList.remove('show');
        dropdown.classList.add('hide');
    }

    if (
        modal &&
        !modal.contains(e.target) &&
        !e.target.closest('.btn-close') &&
        !e.target.closest('.btn-add-project')
    ) {
        closeModal();
    }

    if (!isEditButton && !isDropdownContent) {
        const dropdowns = document.querySelectorAll('.project-edit-dropdown');
        dropdowns.forEach(dropdown => {
            dropdown.classList.add('hide');
            dropdown.classList.remove('show');
        });
    }
});

function copyAddProjectDescription() {
    const quillEditor = document.querySelector("#add-project-description-wysiwyg-editor .ql-editor");
    const hiddenTextarea = document.getElementById('add-project-description');

    if (quillEditor && hiddenTextarea) {
        hiddenTextarea.value = quillEditor.innerHTML;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const editForm = document.getElementById("edit-project-form");
    if (editForm) {
        editForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            if (!validateEditProjectForm()) {
                return;
            }

            const formData = {
                Id: document.getElementById("edit-project-id").value,
                ProjectName: document.getElementById("edit-project-name").value,
                ClientName: document.getElementById("edit-client-name").value,
                Description: document.getElementById("edit-project-description").value,
                StartDate: document.getElementById("edit-start-date").value,
                EndDate: document.getElementById("edit-end-date").value,
                Budget: document.getElementById("edit-budget").value,
                StatusId: parseInt(document.getElementById("edit-status-id").value)
            };

            const response = await fetch(`/admin/projects/edit/${formData.Id}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                location.reload();
            } else {
                alert("Could not update.");
            }
        });
    }
});



async function deleteProject(projectId) {
    if (!confirm("Are you sure you want to delete this project?")) return;

    const response = await fetch(`/admin/projects/delete`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ id: projectId })
    });

    if (response.ok) {
        location.reload();
    } else {
        alert("Could not delete project.");
    }
}


/* ============================
       VALIDATION SIGNUP
============================ */

function validateFullName() {
    const input = document.getElementById("signup-fullname");
    const error = document.getElementById("signup-fullname-error");
    const value = input.value.trim();

    const isValid = /^[A-Za-zÅÄÖåäö]+(?:\s+[A-Za-zÅÄÖåäö]+)+$/.test(value);

    if (!value) {
        error.textContent = "Full name is required.";
        return false;
    } else if (!isValid) {
        error.textContent = "Please enter your full name.";
        return false;
    }

    error.textContent = "";
    return true;
}

/*
   Taget av ChatGPT 4o - Denna funktion validerar användarens email i registreringsformuläret. Den kontrollerar att email inte är tom, 
   har korrekt format & inte redan finns registrerad i databasen.
   Funktionen är asynkron och visar ett felmeddelande i formuläret om något är fel, den returnerar sedan true om valideringen lyckas, annars
   returneras false.
*/
async function validateEmail() {
    const input = document.getElementById("signup-email");
    const error = document.getElementById("signup-email-error");
    const value = input.value.trim();

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!value) {
        error.textContent = "Email is required.";
        return false;
    } else if (!emailRegex.test(value)) {
        error.textContent = "Enter a valid email address.";
        return false;
    }

    try {
        const response = await fetch(`/api/check-email?email=${encodeURIComponent(value)}`);
        if (!response.ok) throw new Error("Could not validate email.");

        const result = await response.json();
        if (result.exists) {
            error.textContent = "This email is already registered.";
            return false;
        }
    } catch (err) {
        console.error(err);
        error.textContent = "Could not verify email. Try again.";
        return false;
    }

    error.textContent = "";
    return true;
}

function validatePassword() {
    const input = document.getElementById("signup-password");
    const error = document.getElementById("signup-password-error");
    const value = input.value.trim();

    const isValid = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$/.test(value);

    if (!value) {
        error.textContent = "Password is required.";
        return false;
    } else if (!isValid) {
        error.textContent = "Password must be at least 8 characters, include uppercase, lowercase, and a digit.";
        return false;
    }

    error.textContent = "";
    return true;
}

function validateConfirmPassword() {
    const password = document.getElementById("signup-password").value.trim();
    const confirmPassword = document.getElementById("signup-confirm-password").value.trim();
    const error = document.getElementById("signup-confirm-password-error");

    if (!confirmPassword) {
        error.textContent = "Please confirm your password.";
        return false;
    } else if (password !== confirmPassword) {
        error.textContent = "Passwords do not match.";
        return false;
    }

    error.textContent = "";
    return true;
}

/*
   Taget av ChatGPT 4o - 
*/
function validateTerms() {
    console.log("Validating checkbox...");
    const checkbox = document.getElementById("signup-terms");
    const error = document.getElementById("signup-terms-error");

    if (!checkbox.checked) {
        error.textContent = "You must accept the terms and conditions.";
        error.style.display = "block";
        return false;
    }

    error.textContent = "";
    error.style.display = "none";
    return true;
}

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("signup-form");
    if (!form) return;

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const isNameValid = validateFullName();
        const isEmailValid = await validateEmail();
        const isPasswordValid = validatePassword();
        const isConfirmPasswordValid = validateConfirmPassword();
        const isTermsValid = validateTerms();

        if (isNameValid && isEmailValid && isPasswordValid && isConfirmPasswordValid && isTermsValid) {
            form.submit();
        }
    });
});

/* ============================
       VALIDATION LOGIN
============================ */

function validateLoginEmail() {
    const input = document.getElementById("login-email");
    const error = document.getElementById("login-email-error");
    const value = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!value) {
        error.textContent = "Email is required.";
        return false;
    } else if (!emailRegex.test(value)) {
        error.textContent = "Enter a valid email address.";
        return false;
    }

    error.textContent = "";
    return true;
}

function validateLoginPassword() {
    const input = document.getElementById("login-password");
    const error = document.getElementById("login-password-error");
    const value = input.value.trim();

    if (!value) {
        error.textContent = "Password is required.";
        return false;
    }

    error.textContent = "";
    return true;
}

/*
   Taget av ChatGPT 4o - 
*/
document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.querySelector("form[asp-action='Login']");
    if (loginForm) {
        loginForm.addEventListener("submit", function (e) {
            const isEmailValid = validateLoginEmail();
            const isPasswordValid = validateLoginPassword();

            if (!isEmailValid || !isPasswordValid) {
                e.preventDefault();
            }
        });
    }
});


/* ============================
  VALIDATION ADD PROJECT FORM
============================ */
/*
   Taget av ChatGPT 4o - Denna funktion validerar fälten i Add Project formuläret innan det skickas. Funktionen kontrollerar att ProjectName & 
   ClientName är ifyllt, description ska inte vara tom, att man valt ett startdatum och att budget ska vara ett positivt tal. 
   Returnerar true om alla fält är giltiga, annars false – vilket förhindrar att formuläret skickas.
*/
function validateAddProjectForm() {
    copyAddProjectDescription();

    const nameInput = document.querySelector('[name="ProjectName"]');
    const clientInput = document.querySelector('[name="ClientName"]');
    const descriptionInput = document.querySelector('[name="Description"]');
    const startDateInput = document.getElementById("add-start-date");
    const endDateInput = document.getElementById("add-end-date");
    const budgetInput = document.querySelector('[name="Budget"]');

    let isValid = true;

    clearErrors();

    if (!nameInput.value.trim()) {
        showError(nameInput, "Project name is required.");
        isValid = false;
    }

    if (!clientInput.value.trim()) {
        showError(clientInput, "Client name is required.");
        isValid = false;
    }

    if (!descriptionInput.value.trim() || descriptionInput.value.trim() === "<p><br></p>") {
        showError(document.getElementById("add-project-description-wysiwyg-editor"), "Description is required.");
        isValid = false;
    }

    if (!startDateInput.value) {
        showError(startDateInput, "Start date is required.");
        isValid = false;
    }

    if (budgetInput.value && parseFloat(budgetInput.value) < 0) {
        showError(budgetInput, "Budget must be a positive number.");
        isValid = false;
    }

    return isValid;
}

function showError(input, message) {
    const error = document.createElement("div");
    error.classList.add("field-error");
    error.textContent = message;
    input.classList.add("error");
    input.closest(".form-group")?.appendChild(error);
}

function clearErrors() {
    document.querySelectorAll(".field-error").forEach(e => e.remove());
    document.querySelectorAll(".form-input.error").forEach(e => e.classList.remove("error"));
}

/* ============================
 VALIDATION EDIT PROJECT FORM
============================ */

function validateEditProjectForm() {
    const nameInput = document.getElementById("edit-project-name");
    const clientInput = document.getElementById("edit-client-name");
    const descriptionInput = document.getElementById("edit-project-description");
    const startDateInput = document.getElementById("edit-start-date");
    const endDateInput = document.getElementById("edit-end-date");
    const budgetInput = document.getElementById("edit-budget");

    let isValid = true;
    clearErrors();

    if (!nameInput.value.trim()) {
        showError(nameInput, "Project name is required.");
        isValid = false;
    }

    if (!clientInput.value.trim()) {
        showError(clientInput, "Client name is required.");
        isValid = false;
    }

    if (!descriptionInput.value.trim() || descriptionInput.value.trim() === "<p><br></p>") {
        showError(document.getElementById("edit-project-description-wysiwyg-editor"), "Description is required.");
        isValid = false;
    }

    if (!startDateInput.value) {
        showError(startDateInput, "Start date is required.");
        isValid = false;
    }

    if (budgetInput.value && parseFloat(budgetInput.value) < 0) {
        showError(budgetInput, "Budget must be a positive number.");
        isValid = false;
    }

    return isValid;
}