using NetPersonnel.Data;
using NetPersonnel.Models;

namespace NetPersonnel.Services
{
    public class AppLogger : IAppLogger
    {
        ApplicationDBContext _db;
        public AppLogger(ApplicationDBContext db)
        {
            _db = db;

        }

        //Stores the action to database
        public async Task LogAsync(int userId, string action, int? objectId, string ipAddress, string details)
        {
            _db.Logs.Add(new Log
            {
                UserId = userId,
                Action = action,
                ObjectId = objectId,
                TimeStamp = DateTime.UtcNow,
                IPAddress = ipAddress,
                Details = details
            });

            await _db.SaveChangesAsync();
        }
    }
}
