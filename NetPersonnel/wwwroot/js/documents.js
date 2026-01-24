// Stores current user's role (HR, Employee, etc.)
var role;

// Stores document objects indexed by document ID for quick lookup
var documentsById;

// Stores current user's ID (used to check permissions)
var userId;

/*
 Runs when page is fully loaded
 - Reads role and userId from <body> data attributes
 - Loads documents from backend
*/
document.addEventListener("DOMContentLoaded", async () => {
    role = document.body.dataset.role;
    userId = Number(document.body.dataset.userid);

    

    await loadDocuments();

});



//Opens the "Upload Document" dialog
document.querySelector("#btnOpenUploadDialog").addEventListener("click", function (e) {
    openUploadingWindow();
});


/*
 Builds a FormData object for uploading a document
 - Includes file, filename, document type
 - If role is HR, can optionally attach to an EmployeeId
*/
function getDocumentValue() {
    const fileInput = document.getElementsByName("fileInput")[0];
    const file = fileInput.files[0];
    const filename = file.name;
    const extension = filename.split('.')[1];   //The extension of the uploaded file will be taken


    const formData = new FormData();
    formData.append("File", file);
    formData.append("Filename", document.getElementsByName("filename")[0].value + `.${extension}`); //Filename = input + ".extension"
    formData.append("DocumentTypeId", parseInt(document.getElementById("selDocTypes").value));



    // If HR is uploading, optionally attach to a specific Employee
    if (role === "HR") {
        const employeeElem = document.getElementsByName("employeeID")[0];
        if (employeeElem && employeeElem.value) {
            formData.append("EmployeeId", parseInt(employeeElem.value));
        }
       
    }

    
    return formData;
}


//Fetch documents from backend and render them in the table
async function loadDocuments() {
    const response = await fetch(`/api/documents/get`);
    const documents = await response.json();
    renderDocuments(documents.value);
}


/*
 Render the documents table dynamically
 - Builds documentsById lookup
 - Adds "Download" button to every document
 - Adds "Delete" button only if user is HR or document owner
*/
function renderDocuments(documents) {
    documentsById = {};
    const tbody = document.querySelector("#tblDocuments tbody");
    tbody.innerHTML = "";
    documents.forEach(d => {
        documentsById[d.id] = d;
        let timeStamp = formatTimeStamp(d.uploadedAt)
        let row = `
           <tr data-id="${d.id}">
                <td>${d.id}</td>
                <td>${d.employeeId}</td>
                <td>${d.filename}</td>
                <td>${d.documentType.name}</td>
                <td>${d.uploadedBy}</td>
                <td>${timeStamp}</td>
                <td><button onclick="downloadDocument(${d.id})">Download</button></td>`;
               
        // Only HR or document owner can delete
        if (role === "HR" || d.uploadedBy === userId) {
            row += `
            <td>
                <button onclick="deleteDocument(${d.id})">Delete</button>
            </td>`;
        }
        
        else {
            row += `<td></td>`; // empty cell if no permission
        }
        row += `</tr>`;

        //tbody.insertAdjacentHTML("beforeend", row);
        tbody.innerHTML += row;
    });
   
}

//Format timestamp into YYYY-MM-DD at HH:MM:SS
function formatTimeStamp(timeStamp) {
    const d = new Date(timeStamp);

    return d.toISOString().slice(0, 10) + " at " + d.toTimeString().slice(0, 8);
}


//Uploads a document using FormData
async function uploadDocument() {
    const newDoc = getDocumentValue();
    try {
        const response = await fetch("/api/documents/upload", {
            method: "POST",
            body: newDoc
        });
        if (response.ok) {
            //const result = await response.json();
            //console.log("Saved:", result);
            //alert("Added successfully!");
        } else {
            const errorText = await response.text();
            console.error("Server error:", response.status);
            alert("Error while saving. " + response.status + " " + errorText);
        }
    }


    catch (error) {
        console.error("Request failed:", error);
        alert("Failed to connect to server." + error);
    }
    document.getElementById("dlgAdd").close();

    loadDocuments();
}

/*
 Opens the "Upload Document" dialog
 - Dynamically builds dialog HTML
 - Loads available document types based on role
*/
function openUploadingWindow() {
    const dlg = document.getElementById("dlgAdd");

    dlg.innerHTML = `
        <label>Name</label>
        <input name="filename" type="text"/><br>
        <label>Type</label>
        <select id="selDocTypes"></select><br>
        `
    if (role === "HR") {
        dlg.innerHTML += `
        <label>Employee ID</label>
        <input name="employeeId" type="text"/><br>`;
    }
    dlg.innerHTML += `<label>Upload file</label>
                      <input type="file" name="fileInput"/><br>
                      <button id="btnUploadDocument">Upload</button>`
    dlg.showModal();

    loadDocumentTypes();
    document.querySelector("#btnUploadDocument").addEventListener("click", function (e) {
        uploadDocument();
    });
}

/*
 Load document types into the select element
 - HR has additional types
 - Employee sees only standard types
*/
function loadDocumentTypes() {
    const selectBody = document.querySelector("#selDocTypes");
    selectBody.innerHTML = "";

    //HR has a few more types to select
    //This types cannot be selected by Employee
    if (role === "HR") {
        selectBody.innerHTML += `
            <option value="1">Employee Contract</option>
            <option value="3">Insurance</option>
            <option value="7">Job Offer Letter</option>
            <option value="4">Tax Form</option>;`
    }

    //These types can be selected by both HR and Employee
    selectBody.innerHTML += `
            <option value="2">Certificate</option>
            <option value="5">Medical Certificate</option>
            <option value="6">Resume</option>
            <option value="4">Tax Form</option>
            <option value="8">Other</option>`;
}


//Download document by opening API endpoint in browser
function downloadDocument(id) {
    window.location.href = `/api/documents/download?id=${id}`;
}

//Delete a document after user confirmation
//Only allowed if user is HR or document owner
async function deleteDocument(id) {
    const userConfirmed = confirm("Are you sure you want to delete this document?");

    if (userConfirmed) {
        try {
            const response = await fetch(`/api/documents/delete?id=${id}`, {
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
        await loadDocuments();
    }
}
