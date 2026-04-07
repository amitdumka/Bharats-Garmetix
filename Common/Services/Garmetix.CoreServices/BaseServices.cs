using Garmetix.Databases.Services;

namespace Garmetix.CoreServices
{
    public class BaseServices
    {
        public static DatabaseService DbService =>  DatabaseService.Instance;
        public static Databases.DatabaseContext Db =>   DatabaseService.Instance.LocalDB;

    }
}