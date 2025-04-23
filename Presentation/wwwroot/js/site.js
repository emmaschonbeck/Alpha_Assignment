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

    setTimeout(() => {
        if (!window.editProjectDescriptionQuill) {
            const textarea = document.getElementById('edit-project-description');
            window.editProjectDescriptionQuill = new Quill('#edit-project-description-wysiwyg-editor', {
                modules: {
                    toolbar: '#edit-project-description-wysiwyg-toolbar'
                },
                theme: 'snow',
                placeholder: 'Type something...'
            });

            editProjectDescriptionQuill.on('text-change', function () {
                textarea.value = editProjectDescriptionQuill.root.innerHTML;
            });
        }
    }, 100);
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

    const editProjectDescriptionTextarea = document.getElementById('edit-project-description');
    if (editProjectDescriptionTextarea) {
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
