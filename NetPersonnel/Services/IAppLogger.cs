namespace NetPersonnel.Services
{
    public interface IAppLogger
    {
        Task LogAsync(int userId, string action, int? objectId, string ipAddress, string details);
    }
}
