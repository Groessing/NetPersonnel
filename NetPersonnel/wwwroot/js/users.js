
/*
 Runs when page is fully loaded
 - Loads all users from backend API
*/
document.addEventListener("DOMContentLoaded", async () => {
    await loadUsers();
});

//Opens the Add User dialog
document.querySelector("#btnOpenAddDialog").addEventListener("click", function (e) {
    openAddingWindow();
});

//Fetch all users from backend and render table
async function loadUsers() {
    const response = await fetch("/api/users/get");
    const users = await response.json();

    renderUsers(users);
}


/*
 Render users table dynamically
 - Builds usersById cache
 - Includes Active/Inactive dropdown and Delete button
 - Handles change events on the Active/Inactive dropdown
*/
function renderUsers(users) {

    const tbody = document.querySelector("#tblUsers tbody");
    tbody.innerHTML = "";
    users.forEach(u => {

        tbody.innerHTML += `
            <tr data-id="${u.id}">
                <td>${u.id}</td>
                <td>${u.username}</td>
                <td>${u.role.name}</td>
                <td>${u.employee.id}</td>
                <td>${u.createdOn}</td>
                <td><select class="selIsActive" data-user-id="${u.id}"">
                <option value="true"${u.isActive === true ? 'selected' : ''}>Active</option>
                <option value="false"${u.isActive === false ? 'selected' : ''}>Inactive</option>
                </select></td>
                <td><button onclick="deleteUser(${u.id})">Delete</button></td>
            </tr>`;
    });

    document.addEventListener('change', function (e) {
        if (!e.target.classList.contains('selIsActive')) return;

        const select = e.target;
        const userId = select.dataset.userId;
        const isActive = select.value;

        editActivityStatus(userId, isActive);
    });
}



//Sends PATCH request to update a user's Active/Inactive status
async function editActivityStatus(userId, isActive) {
    try {
        const response = await fetch(`/api/users/edit?userId=${userId}&isActive=${isActive}`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
        });
        if (response.ok) {
            //const text = await response.json();
            //const result = text ? JSON.parse(text) : null;
            //console.log("Changed:", result);
        } else {
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }

    }
    catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
}

//Deletes a user after confirmation
async function deleteUser(id) {
    const userConfirmed = confirm("Are you sure you want to delete this user?");

    if (userConfirmed) {
        try {
            const response = await fetch(`/api/users/delete?id=${id}`, {
                method: "DELETE"
            });

            if (response.status === 204) {
                //console.log("Saved:", result);
            } else {
                console.error("Server error:", response.status);
                alert("Error while saving. " + response.status);
            }

        } catch (error) {
            console.error("Request failed:", error);
            alert("Failed to connect to server." + error);
        }
        await loadUsers();
    }
}

/*
 Retrieves user input values from Add User dialog
 - Returns user object suitable for POST request
*/
function getUserValue() {
    const user =
    {
        username: document.getElementsByName("username")[0].value,
        password: document.getElementsByName("password")[0].value,
        employeeId: parseInt(document.getElementsByName("employeeId")[0].value),
        roleId: parseInt(document.getElementById("roles").value)
    };
    return user;
}

/*
 Sends POST request to add a new user
 - Closes dialog and refreshes users table
*/
async function addUser() {
    
    const newUser = getUserValue();
    try {
        const response = await fetch("/api/users/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newUser)
        });

        if (response.ok) {

            //console.log("Saved:", result);
            //alert("Added successfully!");
        } else {
            console.log(newUser);
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }

    } catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
    document.getElementById("dlgAdd").close();



    await loadUsers();
}

/*
 Opens the Add User modal dialog
 - Includes fields for Username, Password, Confirm Password, Role, Employee ID
 - Validates password confirmation before submitting
*/
function openAddingWindow() {
    const dlg = document.getElementById("dlgAdd");


    dlg.innerHTML = `
        <label>Username</label>
        <input name="username" type="text"/><br>
        <label>Password</label>
        <input name="password" type="password"/><br>
        <label>Confirm Password</label>
        <input name="confirmPassword" type="password"/><br>
        <label>Role</label>
        <select id="roles">
        <option value="1">Admin</option>
        <option value="2">HR</option>
        <option value="3">Manager</option>
        <option value="4">Employee</option>
        </select><br>
        <label>Employee ID</label>
        <input name="employeeId" type="text"/><br>
        <button id="btnAddUser">Add</button>
        <button id="btnCancel">Cancel</button>
        `
    dlg.showModal();



    document.querySelector("#btnAddUser").addEventListener("click", function (e) {
        if (document.getElementsByName("password")[0].value === document.getElementsByName("confirmPassword")[0].value) {
            addUser();
        }
        else {
            alert("Password doesn't match");
        }
    });
    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgAdd").close();
    });


}


