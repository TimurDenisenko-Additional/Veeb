using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KasutajaController : ControllerBase
    {
        private static List<Kasutaja> kasutajaDB = [new(0, "Admin", "1234", "Adam", "Adamson"), new(1, "Goldik", "1234", "Goldar", "Lusa")];
        private static readonly List<Kasutaja> backup = [];
        private static bool isLogged = false;
        private static int currentKasutajaId = -1;

        private static void DeepCopy(List<Kasutaja> from, List<Kasutaja> to)
        {
            if (from.SequenceEqual(to))
                return;
            if (to.Count > 0)
                to.Clear();
            from.ForEach(x =>
            {
                to.Add(new(x.Id, x.Username, x.Password, x.FirstName, x.LastName));
            });
        }
        private static void CreateBackup() => DeepCopy(kasutajaDB, backup);
        private static void Reorder()
        {
            for (int i = 0; i < kasutajaDB.Count; i++)
            {
                kasutajaDB[i].Id = i;
            }
        }

        // GET: kasutaja
        [HttpGet]
        public List<Kasutaja> GetKasutajad() => kasutajaDB;

        // GET: kasutaja/id
        [HttpGet("{id}")]
        public Kasutaja GetKasutaja(int id) => kasutajaDB.ElementAtOrDefault(id) ?? new();

        // GET: kasutaja/delete/id
        [HttpDelete("delete/{id}")]
        public List<Kasutaja> Delete(int id)
        {
            CreateBackup();
            Kasutaja kasutaja = kasutajaDB.ElementAtOrDefault(id) ?? new();
            if (kasutaja.Id == -1)
                return [];
            kasutajaDB.RemoveAt(id);
            Reorder();
            return kasutajaDB;
        }

        // GET: kasutaja/create/username/password/firstname/lastname
        [HttpPost("create/{username}/{password}/{firstname}/{lastname}")]
        public List<Kasutaja> Create(string username, string password, string firstname, string lastname)
        {
            CreateBackup();
            kasutajaDB.Add(new(kasutajaDB.Count, username, password, firstname, lastname));
            Reorder();
            return kasutajaDB;
        }

        // GET: kasutaja/login/username/password
        [HttpGet("login/{username}/{password}")]
        public string Login(string username, string password)
        {
            Kasutaja checkingKasutaja = kasutajaDB.Where(x => x.Username == username)?.ElementAtOrDefault(0) ?? new();
            if (checkingKasutaja.Password == password)
            {
                isLogged = true;
                currentKasutajaId = checkingKasutaja.Id;
                return "Edu! Ole sisse logitud";
            }
            else
                return "Ebaõnnestumine! Midagi on valesti";
        }

        // GET: kasutaja/logout
        [HttpGet("logout")]
        public string Logout()
        {
            if (isLogged)
            {
                isLogged = false;
                currentKasutajaId = -1;
                return "Ole välja logitud";
            }
            else
                return "Sa ei ole sisse logitud";
        }

        // GET: kasutaja/get-current
        [HttpGet("get-current")]
        public Kasutaja GetCurrent() => GetKasutaja(currentKasutajaId);
    }
}
