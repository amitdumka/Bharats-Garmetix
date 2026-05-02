using Garmetix.Models.Bases;



namespace Garmetix.Models
{
    public class AppClient : CEntity
    {
        // public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public Guid CompanyId { get; set; }
        public string? ClientSecret { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceToken { get; set; }
        public string? IpAddress { get; set; }
    }
}