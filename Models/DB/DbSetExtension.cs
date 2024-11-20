using Microsoft.EntityFrameworkCore;

namespace Veeb.Models.DB
{
    public static class DbSetExtension
    {
        public static T? ElementOrDefault<T>(this DbSet<T> DB, int id) where T : DBModel
        {
            DBModel? model = null;
            if (typeof(T) == typeof(Kasutaja))
                model = DB.First(x => x.Id == id) ?? null;
            else if (typeof(T) == typeof(Order))
                model = DB.First(x => x.Id == id) ?? null;
            else if (typeof(T) == typeof(Toode))
                model = DB.First(x => x.Id == id) ?? null;
            return model as T;
        }
    }
}
