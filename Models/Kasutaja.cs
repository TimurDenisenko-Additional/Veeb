using Veeb.Models.DB;

namespace Veeb.Models
{
    public class Kasutaja(int id = -1, string username = "The user is missing", string password = "", string firstName = "", string lastName = "", bool isAdmin = false) : DBModel(id)
    {
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public bool IsAdmin { get; set; } = isAdmin;
    }
}
