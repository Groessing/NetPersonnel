//Current user's role (HR, Admin, Employee, etc.)
var role;

//Current employee ID (for employees, to limit to their own sick leaves)
var currentEmpId;

//Cache sickleaves by ID for easy access during editing
var sickLeavesById;


/*
Runs when page is fully loaded
 - Reads role and currentEmpId from <body> data attributes
 - Loads sick leaves from backend
*/
document.addEventListener("DOMContentLoaded", async () => {
    role = document.body.dataset.role;
    currentEmpId = document.body.dataset.employeeid;

    await loadSickLeaves();


});


/*
 Event delegation for clicks in the employees table
 - Clicking a row opens the edit dialog
*/
document.querySelector("#tblSickLeaves tbody").addEventListener("click", function (e) {
    const row = e.target.closest("tr");

    if (!row) return;

    const sickLeaveId = row.dataset.id;
    openEditingWindow(sickLeaveId);
});



//Opens the "Add Sick Leave" dialog
document.querySelector("#btnOpenAddDialog").addEventListener("click", function (e) {
    openAddingWindow();
});


//Fetch all sick leaves from backend and render table
async function loadSickLeaves() {
    const response = await fetch(`/api/sickleaves/get`);
    const sickLeaves = await response.json();
    renderSickLeaves(sickLeaves.value);
}

//Render sick leaves table dynamically
//Builds sickLeavesById cache for fast access during editing
function renderSickLeaves(sickLeaves) {
    sickLeavesById = {};
    const tbody = document.querySelector("#tblSickLeaves tbody");
    tbody.innerHTML = "";
    sickLeaves.forEach(s => {
        sickLeavesById[s.id] = s;
        tbody.innerHTML += `
           <tr data-id="${s.id}">
                <td>${s.id}</td>
                <td>${s.employee.id}</td>
                <td>${s.fromDate}</td>
                <td>${s.toDate}</td>
                <td>${s.info}</td>
            </tr>`;
    });
}



/*
 Sends POST request to add a new sick leave
 - Uses either HR-entered employee ID or current employee
 - Closes dialog and reloads table
*/
async function addSickLeave() {
    //const newSickLeave = getSickLeaveValue();
    const employeeElem = document.getElementById("addEmployeeID");
    const empId = Number(employeeElem ? employeeElem.value : currentEmpId);
    const newSickLeave =
    {
        employeeId: empId,
        fromDate: document.getElementById("addFromDate").value,
        toDate: document.getElementById("addToDate").value,
        info: document.getElementById("addInfo").value
    };


    try {
        const response = await fetch("/api/sickleaves/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newSickLeave)
        });
        if (response.ok) {
            const result = await response.json();
            //console.log("Saved:", result);
            //alert("Added successfully!");
        } else {
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }
    }
        

    catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
    document.getElementById("dlgAdd").close();

    loadSickLeaves("my");
}



/*
 Opens modal dialog for adding a sick leave
 - HR can input employee ID
 - Regular employees use currentEmpId
*/
function openAddingWindow() {
    const dlg = document.getElementById("dlgAdd");
    dlg.innerHTML = "";
    //HR has to enter the employee's ID
    if (role === "HR") {
        dlg.innerHTML = `
        <label>Employee ID</label>
        <input id="addEmployeeID"/><br>
        `;
    }
    dlg.innerHTML += `
        <label>From</label>
        <input id="addFromDate" type="date"/><br>
        <label>To</label>
        <input id="addToDate" type="date"/><br>
        <label>Info</label>
        <input id="addInfo" type="text"/><br>
        <button id="btnAddSickLeave">Add</button><button id="btnCancel">Cancel</button>
        `;
    dlg.showModal();
   

    document.querySelector("#btnAddSickLeave").addEventListener("click", function (e) {
        addSickLeave();
    });

    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgAdd").close();
    });

  
}

/*
 Opens modal dialog for editing an existing sick leave
 - HR can change employee ID
*/
function openEditingWindow(sickLeaveId) {
    const sickLeave = sickLeavesById[sickLeaveId];
    const dlg = document.getElementById("dlgEdit");
    dlg.innerHTML = "";
    //HR has to enter the employee's ID
    if (role === "HR") {
        dlg.innerHTML = `
        <label>Employee ID</label>
        <input id="editEmployeeID" value="${sickLeave.employeeId}"/><br>
        `;
    }
    dlg.innerHTML += `
        <label>From</label>
        <input id="editFromDate" type="date" value="${sickLeave.fromDate}"/><br>
        <label>To</label>
        <input id="editToDate" type="date" value="${sickLeave.toDate}"/><br>
        <label>Info</label>
        <input id="editInfo" type="text" value="${sickLeave.info}"/><br>
        <button id="btnEditSickLeave">Edit</button><button id="btnCancel">Cancel</button>
        `;
    dlg.showModal();
    document.querySelector("#btnEditSickLeave").addEventListener("click", function (e) {
        editSickLeave(sickLeave.id);
    });

    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgEdit").close();
    });

}


/*
 Sends PUT request to edit a sick leave
 - Uses either HR-entered employee ID or current employee
 - Closes dialog and reloads table
*/

async function editSickLeave(id) {
    const employeeElem = document.getElementById("addEmployeeID");
    const empId = Number(employeeElem ? employeeElem.value : currentEmpId);
    const editedSickLeave =
    {
        id: id,
        employeeId: empId,
        fromDate: document.getElementById("editFromDate").value,
        toDate: document.getElementById("editToDate").value,
        info: document.getElementById("editInfo").value
    };

    try {
        const response = await fetch("/api/sickleaves/edit", {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(editedSickLeave)
        });
        if (response.ok) {
            const result = await response.json();
            //console.log("Changed:", result);
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


    await loadSickLeaves();
}