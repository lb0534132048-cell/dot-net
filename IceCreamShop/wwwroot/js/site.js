const uri = '/icecream';
let iceCreams = [];

async function request(url, options) {
    const response = await fetch(url, options);
    if (!response.ok) {
        const detail = await response.text();
        throw new Error(detail || `Request failed with status ${response.status}.`);
    }
    return response;
}

function showMessage(message) {
    document.getElementById('message').textContent = message;
}

async function getItems() {
    try {
        const response = await request(uri);
        iceCreams = await response.json();
        displayItems(iceCreams);
        showMessage('');
    } catch (error) {
        showMessage(`Unable to load flavors: ${error.message}`);
    }
}

async function addItem(event) {
    event.preventDefault();
    const nameInput = document.getElementById('add-name');
    const item = {
        name: nameInput.value.trim(),
        isDairyFree: document.getElementById('add-isDairyFree').checked
    };

    try {
        await request(uri, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(item)
        });
        nameInput.value = '';
        document.getElementById('add-isDairyFree').checked = false;
        await getItems();
    } catch (error) {
        showMessage(`Unable to add flavor: ${error.message}`);
    }
}

async function deleteItem(id) {
    try {
        await request(`${uri}/${id}`, { method: 'DELETE' });
        await getItems();
    } catch (error) {
        showMessage(`Unable to delete flavor: ${error.message}`);
    }
}

function displayEditForm(id) {
    const item = iceCreams.find(flavor => flavor.id === id);
    if (!item) {
        showMessage('That flavor is no longer on the menu. Refresh the list and try again.');
        return;
    }

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-isDairyFree').checked = item.isDairyFree;
    document.getElementById('editForm').hidden = false;
    document.getElementById('edit-name').focus();
}

async function updateItem(event) {
    event.preventDefault();
    const id = Number(document.getElementById('edit-id').value);
    const item = {
        id,
        name: document.getElementById('edit-name').value.trim(),
        isDairyFree: document.getElementById('edit-isDairyFree').checked
    };

    try {
        await request(`${uri}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(item)
        });
        closeInput();
        await getItems();
    } catch (error) {
        showMessage(`Unable to update flavor: ${error.message}`);
    }
}

function closeInput() {
    document.getElementById('editForm').hidden = true;
}

function displayItems(data) {
    const tbody = document.getElementById('ice-creams');
    tbody.replaceChildren();

    const count = data.length;
    document.getElementById('counter').textContent =
        `${count} ${count === 1 ? 'flavor' : 'flavors'} on the menu`;

    for (const item of data) {
        const row = tbody.insertRow();
        const dairyFreeCell = row.insertCell();
        const checkbox = document.createElement('input');
        checkbox.type = 'checkbox';
        checkbox.disabled = true;
        checkbox.checked = item.isDairyFree;
        checkbox.setAttribute('aria-label', `${item.name} is dairy-free`);
        dairyFreeCell.appendChild(checkbox);

        row.insertCell().textContent = item.name;

        const actions = row.insertCell();
        const editButton = document.createElement('button');
        editButton.type = 'button';
        editButton.textContent = 'Edit';
        editButton.addEventListener('click', () => displayEditForm(item.id));

        const deleteButton = document.createElement('button');
        deleteButton.type = 'button';
        deleteButton.className = 'secondary';
        deleteButton.textContent = 'Delete';
        deleteButton.addEventListener('click', () => deleteItem(item.id));

        actions.append(editButton, deleteButton);
    }

    iceCreams = data;
}

document.getElementById('add-form').addEventListener('submit', addItem);
document.getElementById('edit-form').addEventListener('submit', updateItem);
document.getElementById('close-edit').addEventListener('click', closeInput);
getItems();
