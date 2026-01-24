//Cache employees by ID for easy access during editing
var employeesById 

//Current user's role (HR, Admin, Employee, etc.)
var role;

/*
Runs when page is fully loaded
 - Reads role from <body> data attributes
 - Loads employees from backend
*/
document.addEventListener("DOMContentLoaded", async () => {
    role = document.body.dataset.role;

    loadEmployees();
});


/*
 Event delegation for clicks in the employees table
 - Clicking a row opens the edit dialog
 - Clicking [Delete] button does not open edit dialog
*/
document.querySelector("#tblEmployees tbody").addEventListener("click", function (e) {
    var row = e.target.closest("tr");

    //Prevents from opening the editing window when [Delete] is clicked
    if (e.target.closest(".btnDelete") || !row) {
        return;
    }
   
    console.log("Test");
    const employeeId = row.dataset.id;
    openEditingWindow(employeeId);
});


//Opens dialog for adding a new employee
document.querySelector("#btnOpenAddDialog").addEventListener("click", function (e) {
    openAddingWindow();
});



/*
 Sends a POST request to add a new employee
 - Reads data from add dialog fields
 - Closes dialog and refreshes table on success
*/
async function addEmployee() {
    const deptId = parseInt(document.querySelector("#addSelDepartments")?.value ?? "-1", 10);
    const newEmployee =
    {
        firstName: document.getElementById("addFirstname").value,
        lastName: document.getElementById("addLastname").value,
        departmentId: deptId,
        jobTitle: document.getElementById("addJobtitle").value,
        email: document.getElementById("addEmail").value,
        phone: document.getElementById("addPhone").value,
        hireDate: document.getElementById("addHiredate").value,
        birthday: document.getElementById("addBirthday").value
    };

    try {
        const response = await fetch("/api/employees/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newEmployee)
        });
        if (response.ok) {
            const result = await response.json();
            //console.log("Saved:", result);
        } else {
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }

    } catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
    document.getElementById("dlgAdd").close();


    await loadEmployees();
}


/*
 Sends a PUT request to edit an existing employee
 - Reads data from edit dialog fields
 - Closes dialog and refreshes table on success
*/
async function editEmployee(id) {
    const deptId = parseInt(document.querySelector("#editSelDepartments")?.value ?? "-1", 10);
    const editedEmployee =
    {
        id: id,
        firstName: document.getElementById("editFirstname").value,
        lastName: document.getElementById("editLastname").value,
        departmentId: deptId,
        jobTitle: document.getElementById("editJobtitle").value,
        email: document.getElementById("editEmail").value,
        phone: document.getElementById("editPhone").value,
        hireDate: document.getElementById("editHiredate").value,
        birthday: document.getElementById("editBirthday").value
    };
    try {
        const response = await fetch("/api/employees/edit", {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(editedEmployee)
        });
        if (response.ok) {
            const result = await response.json();
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
    document.getElementById("dlgEdit").close();
    

    await loadEmployees();
}

/*
 Opens modal dialog for adding an employee
 - Dynamically builds HTML fields
 - HR/Admin users can select department
*/
function openAddingWindow() {
    const dlg = document.getElementById("dlgAdd");

    dlg.innerHTML = `
        <label>First Name</label>
        <input id="addFirstname" type="text"/><br>
        <label>Last Name</label>
        <input id="addLastname" type="text"/><br>
        `;
    if (role === "HR" || role === "Admin") {

        dlg.innerHTML += `
        <label>Department</label>
        <select id="addSelDepartments"></select><br>`;
        }
    dlg.innerHTML += `
        <label>Job Title</label>
        <input id="addJobtitle" type="text"/><br>
        <label>Email</label>
        <input id="addEmail" type="text"/><br>
        <label>Phone</label>
        <input id="addPhone" type="text"/><br>
        <label>Hire Date</label>
        <input id="addHiredate" type="date"/><br>
        <label>Birthday</label>
        <input id="addBirthday" type="date"/><br>
        <button id="btnAddEmployee">Add</button><button id="btnCancel">Cancel</button>
        `;
    dlg.showModal();
    if (role === "HR" || role === "Admin") {
        loadDepartments("add");
    }

    document.querySelector("#btnAddEmployee").addEventListener("click", function (e) {
        addEmployee();
    });

    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgAdd").close();
    });


}


/*
 Opens modal dialog for editing an employee
 - Dynamically builds HTML fields
 - HR/Admin users can edit department
*/
function openEditingWindow(empId) {
    const emp = employeesById[empId];
    const dlg = document.getElementById("dlgEdit");

    

    dlg.innerHTML = `
        <label>ID</label>
        <input name="id" type="text" value="${emp.id}" readonly/><br>
        <label>First Name</label>
        <input id="editFirstname" type="text" value="${emp.firstName}"/><br>
        <label>Last Name</label>
        <input id="editLastname" type="text" value="${emp.lastName}"/><br>
        `;
    if (role === "HR" || role === "Admin") {
        dlg.innerHTML += `
        <label>Department</label>
        <select id="editSelDepartments"></select><br>`;
    }
    dlg.innerHTML += `
        <label>Job Title</label>
        <input id="editJobtitle" type="text" value="${emp.jobTitle}"/><br>
        <label>Email</label>
        <input id="editEmail" type="text" value="${emp.email}"/><br>
        <label>Phone</label>
        <input id="editPhone" type="text" value="${emp.phone}"/><br>
        <label>Hire Date</label>
        <input id="editHiredate" type="text" value="${emp.hireDate}"/><br>
        <label>Birthday</label>
        <input id="editBirthday" type="text" value="${emp.birthday}"/><br>
        <button id="btnEditEmployee">Edit</button><button id="btnCancel">Cancel</button>
        `;
    dlg.showModal(); // fixed, cannot resize

    if (role === "HR" || role === "Admin") {
        loadDepartments("edit");
    }
        

    document.querySelector("#btnEditEmployee").addEventListener("click", function (e) {
        editEmployee(emp.id);
    });

    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgEdit").close();
    });
}

/*
 Loads department options into a select element
 - status = "add" or "edit" determines which select element
*/
async function loadDepartments(status) {
    const response = await fetch("/api/departments/get");
    const departments = await response.json();
    var selectBody;


    if (status === "add") selectBody = document.querySelector("#addSelDepartments");
    else selectBody = document.querySelector("#editSelDepartments");


    selectBody.innerHTML = "";
    departments.forEach(d => {
        selectBody.innerHTML += `
            <option value="${d.id}">${d.name}</option>
            `;
    });
}

//Fetch all employees from backend
async function loadEmployees() {
    const response = await fetch(`/api/employees/get`);
    const employees = await response.json();
    renderEmployees(employees);
}


/*
 Render employee table dynamically
 - Builds employeesById cache
 - Each row has Delete button
*/
function renderEmployees(employees) {
    employeesById = {};
    const tbody = document.querySelector("#tblEmployees tbody");
    tbody.innerHTML = "";
    employees.forEach(e => {
        employeesById[e.id] = e;
        tbody.innerHTML += `
            <tr data-id="${e.id}">
                <td>${e.id}</td>
                <td>${e.firstName}</td>
                <td>${e.lastName}</td>
                <td>${e.department.name}</td>
                <td>${e.jobTitle}</td>
                <td>${e.email}</td>
                <td>${e.phone}</td>
                <td>${e.hireDate}</td>
                <td>${e.birthday}</td>
                <td><button onclick="deleteEmployee(${e.id})" class="btnDelete">Delete</button></td>
            </tr>`;
    });
}


//Deletes an employee after confirmation
async function deleteEmployee(id)
{
    const userConfirmed = confirm("Are you sure you want to delete this employee?");
    
    if (userConfirmed) {
        try {
            const response = await fetch(`/api/employees/delete?id=${id}`, {
                method: "DELETE"
            });

            if (response.status === 204) {
                console.log("Deleted successfully");
            } else {
                console.error("Server error:", response.status);
                alert("Error while saving. " + response.status);
            }

        } catch (error) {
            console.error("Request failed:", error);
            alert("Failed to connect to server." + error);
        }
        await loadEmployees();
    }
}


