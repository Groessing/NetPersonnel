// Stores the current user's role (Employee, Manager, HR)
var role;

//Cache dictionary for vacation requests keyed by ID
let vacReqById = {};       
// Runs when the DOM is fully loaded

document.addEventListener("DOMContentLoaded", async () => {
    role = document.body.dataset.role;
    await loadVacationRequests();

    //Employees can add a new Vacation Request
    if (role == "Employee") {
        document.querySelector("#btnOpenAddDialog").addEventListener("click", function (e) {
        openAddingWindow();
        });
    }
});

//Global listener for status dropdowns (approve/decline)
document.addEventListener("change", function (e) {
    if (!e.target.classList.contains("selStatus")) return;

    const vacationId = e.target.dataset.vacationId;
    const statusId = e.target.value;

    editVacationRequest(vacationId, statusId);
});

//Manager/HR can change vacation request status directly in the table
if (role === "Manager" || role === "HR") {
    document.querySelector("#tblVacationRequests tbody").addEventListener("change", function (e) {
        if (!e.target.classList.contains("selStatus")) return;

        const select = e.target;
        const row = select.closest("tr");

        const requestId = row.dataset.id;
        const newStatusId = select.value;

        console.log("VacationRequestId:", requestId);
        console.log("New Status:", newStatusId);


        updateVacationStatus(requestId, newStatusId);
    });

   
}



//Fetches all vacation requests from backend
async function loadVacationRequests() {
    const response = await fetch(`/api/vacationrequests/get`);
    const vacRequests = await response.json();
    console.log(vacRequests);
    renderVacationRequests(vacRequests.value);
}

/*
 Renders the vacation requests table
 - HR and Manager can see and edit status (dropdown)
 - Employee sees only the current status
*/
function renderVacationRequests(vacRequests) {
    const tbody = document.querySelector("#tblVacationRequests tbody");
    console.log(vacRequests.length);
    tbody.innerHTML = "";
    vacRequests.forEach(v => {
        vacReqById[v.id] = v;
        let row = `
           <tr data-id="${v.id}">
                <td>${v.id}</td>
                <td>${v.employee.id}</td>
                <td>${v.fromDate}</td>
                <td>${v.toDate}</td>`;

      
        //Manager/HR can edit status via dropdown
        if (role === "HR" || role === "Manager") {
            row += `
            <td>
                <select class="selStatus" data-vacation-id="${v.id}">`;

            //Pending status is only selectable if still pending
            if (v.statusId === 1) {
                row += `<option value="1"${v.statusId === 1 ? 'selected' : ''}>Pending</option>`;
            }
                    
            row += `
                    <option value="2"${v.statusId === 2 ? 'selected' : ''}>Approve</option>
                    <option value="3"${v.statusId === 3 ? 'selected' : ''}>Decline</option>
                </select>
            </td>`;
        }

        //Employee can only view status
        if (role === "Employee") {
            row += `<td>${v.status.approvalStatus}</td>`;
        }
        row += `</tr>`;
        tbody.innerHTML += row;


        
    });
}


//Adds a new vacation request(Employee only)
async function addVacationRequest() {
    const newVacReq = getVacationRequestValue();
    console.log(newVacReq);
    try {
        const response = await fetch("/api/vacationrequests/add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newVacReq)
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

    loadVacationRequests();
}

//Opens the Add Vacation Request modal
function openAddingWindow() {
    const dlg = document.getElementById("dlgAdd");

    dlg.innerHTML = `
        <label>From</label>
        <input name="fromDate" type="date"/><br>
        <label>To</label>
        <input name="toDate" type="date"/><br>
        <button id="btnAddVacationRequest">Add</button><button id="btnCancel">Cancel</button>
        `
    dlg.showModal();


    document.querySelector("#btnAddVacationRequest").addEventListener("click", function (e) {
        addVacationRequest();
        
    });

    document.querySelector("#btnCancel").addEventListener("click", function (e) {
        document.getElementById("dlgAdd").close();
       
    });


}



/*
 Approve or decline a vacation request (Manager/HR only)
 - vacReqId: ID of vacation request
 - statusId: 2 = Approve, 3 = Decline
*/
async function editVacationRequest(vacReqId, statusId) {
    console.log(vacReqId);
    console.log(statusId);
    try {
        const response = await fetch(`/api/vacationrequests/edit?requestId=${vacReqId}&statusId=${statusId}`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
        });
        if (response.ok) {
        } else {
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status);
        }

    }
    catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
   


    await loadVacationRequests();
}

