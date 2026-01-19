# NetPersonnel
This project is an ASP.NET backend-focused application built to demonstrate my skills as a backend developer, with emphasis on API design, role-based authorization, database interaction, and testing.

The primary focus of the project is the backend and database layer. A small JavaScript frontend is included to demonstrate API consumption. Each page uses its own JavaScript file (e.g., employees.js, vacationrequests.js) to handle CRUD operations by calling the corresponding API endpoints.

## User Roles:
The application supports the following roles, each with different responsibilities and permissions:
- Admin:
  - Creates users, employees, and departments
  - Views system logs
- HR:
  - Manages employees and departments
  - Handles sick leaves and vacation requests
- Manager:
  - Manages employees, sick leaves, and vacation requests within their own department
- Employee:
  - Creates vacation requests
  - Submits sick leave records

# Features:
- User management (Admin, HR, Manager)
- Department management (Admin, HR)
- Employee management (Admin, HR, Manager)
- Vacation request creation (Employee)
- Vacation request approval or rejection (HR, Manager)
- Sick leave creation and editing (Employee, HR)
- System activity logging (Admin)

## Security-Features:
- Password hashing using Rfc2898 with a unique salt
- Role-based authorization for all API endpoints
- Unauthorized actions return Forbid() from Task<IActionResult>
- API endpoints are accessible only to users with the required permissions

## Tests:
The project includes both unit tests and integration tests to ensure correctness and security.
- Unit Tests: 
  - Test creation, editing, and deletion of entities (employees, users, etc.)
  - Controllers are tested by calling Task<IActionResult> methods directly
  - Veifies correct responses such as OK, Forbid, and NotFound
- Integration Tests:
  - Test API endpoints through HTTP requests
  - Validate authentication and authorization behavior
  - Ensure users can only access or modify data they are permitted to (e.g., employees can only access their own records or global records)

