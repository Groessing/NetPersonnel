document.addEventListener("DOMContentLoaded", async () => {
    await loadLogs();
});

async function loadLogs() {
    const response = await fetch("/api/logs/get");
    const logs = await response.json();

    renderLogs(logs);
}

function renderLogs(logs) {

    const tbody = document.querySelector("#tblLogs tbody");
    tbody.innerHTML = "";
    logs.forEach(l => {
        let timeStamp = formatTimeStamp(l.timeStamp);
        
        tbody.innerHTML += `
            <tr data-id="${l.id}">
                <td>${l.id}</td>
                <td>${l.userId}</td>
                <td>${l.action}</td>
                <td>${timeStamp}</td>     
                <td>${l.objectId}</td>     
                <td>${l.ipAddress}</td>     
                <td>${l.details}</td>     
            </tr>`;
    });
}



function formatTimeStamp(timeStamp) {
    const d = new Date(timeStamp);

    return d.toISOString().slice(0, 10) + " at " + d.toTimeString().slice(0, 8);
}