// Holds the current user's role (HR, Employee, Manager, Admin)
// This value is read from a data-attribute on the <body> tag
var role;

// Holds all numeric values displayed on dashboard cards
// Example: employees count, documents count, etc
var counts;


/*
 Maps each user role to the dashboard cards they are allowed to see
 Each card has:
 - title: text shown to the user
 - key: property name used to read data from `counts`
*/
const roleCards = {
    HR: [
        { title: "All Employees", key: "employees" },
        { title: "Sick Leaves in last 30 Days", key: "sickLeavesLast30" },
        { title: "Pending Vacation Requests", key: "pendingVacationRequests" },
        { title: "All Departments", key: "departments" },
        { title: "All Documents", key: "documents" }
    ],
    Employee: [
        { title: "Sick Leaves in last 30 Days", key: "sickLeavesLast30" },
        { title: "Pending Vacation Requests", key: "pendingVacationRequests" },
        { title: "Documents", key: "documents" }
    ],
    Manager: [
        { title: "Employees Count", key: "employees" },
        { title: "Sick Leaves Last 30 Days", key: "sickLeavesLast30" },
        { title: "Pending Vacation Requests", key: "pendingVacationRequests" },
        { title: "Documents", key: "documents" }
    ],
    Admin: [
        { title: "All Employees", key: "employees" },
        { title: "All Active Users", key: "activeUsers" },
        { title: "All Departments", key: "departments" }
    ]
};

/*
 Runs when the HTML document is fully loaded
 Reads the user's role from the body element and loads dashboard data
*/
document.addEventListener("DOMContentLoaded", async () => {
    role = document.body.dataset.role;

    loadDashboard();
});


/*
 Fetches dashboard data from backend API
 and triggers rendering after data is loaded
*/
async function loadDashboard() {
    const response = await fetch(`/api/dashboard/get`);
    const dashboard = await response.json();
    setValues(dashboard);
    renderDashboard();
}

/*
 Creates and displays dashboard cards dynamically
 depending on the current user's role
*/
function renderDashboard() {
    const dashboard = document.getElementById('dashboard');
    dashboard.innerHTML = '';

    // Loop through the cards for the selected role
    roleCards[role].forEach(card => {
        const div = document.createElement('div');
        div.className = 'card';
        div.innerHTML = `<h3>${card.title}</h3><p>${counts[card.key]}</p>`;
        dashboard.appendChild(div);
    });
}

/*
 Maps backend response fields to frontend keys used by dashboard cards.
 This creates a single source of truth for displayed values.
*/
function setValues(dashboard) {
    counts = {
        employees: dashboard.employeesCount,
        sickLeavesLast30: dashboard.sickLeavesLast30DaysCount,
        pendingVacationRequests: dashboard.pendingVacationRequestsCount,
        departments: dashboard.departmentsCount,
        documents: dashboard.documentsCount,
        activeUsers: dashboard.activeUsersCount
    };
}




