using Garmetix.Models.Enums;

namespace Garmetix.Core.Sessions
{
    public class CurrentSession
    {
        public bool IsAutoLoginEnabled { get; set; }
        public Guid? CompanyId { get; set; }
        public string? UserName { get; set; }
        public string? CompanyDatabaseFileName { get; set; }
        public DateTime LoginTime { get; set; }
        public LoginRole UserRole { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
    }
}