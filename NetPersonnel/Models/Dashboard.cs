namespace NetPersonnel.Models
{
    public class Dashboard
    {
        public int? EmployeesCount { get; set; }           //For Manager, HR

        public int? PendingVacationRequestsCount { get; set; }     //For Manager, HR, Employee

        public int? SickLeavesLast30DaysCount { get; set; }        //For Manager, HR

        public int? DocumentsCount { get; set; }              //For Manager, HR, Employee

        public int? DepartmentsCount { get; set; }            //For HR, Admin

        public int? ActiveUsersCount { get; set; }                 //For Admin

    }
}
