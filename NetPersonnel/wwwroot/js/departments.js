//Cache departments by ID for easy access during editing
var departmentsById;

//Load departments as soon as the page is fully loaded
document.addEventListener("DOMContentLoaded", async () => {
    await loadDepartments();
});



/*
 Table click handler (event delegation)
 - Clicking on a table row opens edit dialog
 - Clicking the delete button does NOT open edit dialog
*/
document.querySelector("#tblDepartments tbody").addEventListener("click", function (d) {
    var row = d.target.closest("tr");

    // Ignore clicks on delete button or outside of rows
    if (d.target.closest(".btnDelete") || !row) {
        return; 
    }
   
    const departmentId = row.dataset.id;
    openEditingWindow(departmentId);
});

//Opens dialog for adding a new department
document.querySelector("#btnOpenAddDialog").addEventListener("click", function (e) {
    openAddingWindow();
});


//Fetches departments from backend API
async function loadDepartments() {
    const response = await fetch("/api/departments/get");
    const departments = await response.json();
    renderDepartments(departments);
}



/*
 Render department table dynamically
 - Builds departmentsById cache
 - Each row has Delete button
*/
function renderDepartments(departments) {
    departmentsById = {};

    const tbody = document.querySelector("#tblDepartments tbody");
    tbody.innerHTML = "";
    departments.forEach(d => {
        departmentsById[d.id] = d;
        tbody.innerHTML += `
            <tr data-id="${d.id}">
                <td>${d.id}</td>
                <td>${d.name}</td>
                <td><button onclick="deleteDepartment(${d.id})" class="btnDelete">Delete</button></td>
            </tr>`;
    });
}


//Deletes a department after user confirmation
async function deleteDepartment(id) {
    const userConfirmed = confirm("Are you sure you want to delete this department?");

    if (userConfirmed) {
        try {
            const response = await fetch(`/api/departments/delete?id=${id}`, {
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
        await loadDepartments();
    }
}

//Sends new department data to backend
async function addDepartment() {
    const dlg = document.getElementById("dlgAdd");
    const newDepartment =
    {
        name: document.getElementById("addDeptName").value
    }; 
    
    try {
        const response = await fetch("/api/departments/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newDepartment)
        });
       
        if (response.ok) {
            
            //console.log("Saved:", result);
            //alert("Added successfully!");
        } else {
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }

    } catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }


    document.getElementById("dlgAdd").close();
    await loadDepartments();
}


//Updates an existing department
async function editDepartment(id) {
   
    const editedDepartment = {
        id: id,
        name: document.getElementById("editDeptName").value
    }


    try {
        const response = await fetch("/api/departments/edit", {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(editedDepartment)
        });
        if (response.ok) {
            const result = await response.json();
            console.log("Changed:", result);
            //alert("Changed successfully!");
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
    
    await loadDepartments();
   
}

//Opens modal dialog for adding a department
//Dynamically builds dialog content
function openAddingWindow() {
    const dlg = document.getElementById("dlgAdd");


    dlg.innerHTML = `
        <label>Name</label>
        <input id="addDeptName" type="text"/><br>
        <button id="btnAddDepartment">Add</button><button id="btnCancel">Cancel</button>
        `
    dlg.showModal();
    

    document.querySelector("#btnAddDepartment").addEventListener("click", function (e) {
        addDepartment();
    });
    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgAdd").close();
    });


}

//Opens modal dialog for editing an existing department
//Pre-fills fields using cached department data
function openEditingWindow(departmentId) {
    const dep = departmentsById[departmentId];
    const dlg = document.getElementById("dlgEdit");



    dlg.innerHTML = `
        <label>ID</label>
        <input name="id" value="${dep.id}" readonly/><br>
        <label>Name</label>
        <input id="editDeptName" type="text" value="${dep.name}"/><br>
        <button id="btnEditDepartment">Edit</button><button id="btnCancel">Cancel</button>
        `
    dlg.showModal(); 
   

    document.querySelector("#btnEditDepartment").addEventListener("click", function (e) {
        editDepartment(dep.id);
    });
    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgEdit").close();
    });
}
