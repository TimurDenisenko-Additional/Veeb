using Microsoft.AspNetCore.Mvc;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KasutajaController : ControllerBase
    {
        internal static List<Kasutaja> kasutajaDB = [new(0, "Admin", "1234", "Adam", "Adamson", true), new(1, "Goldik", "1234", "Goldar", "Lusa")];
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
                int oldId = kasutajaDB[i].Id;
                kasutajaDB[i].Id = i;
                OrderController.OtherReordering(true, oldId, i);
            }
        }

        // GET: kasutaja
        [HttpGet]
        public List<Kasutaja> GetKasutajad() => kasutajaDB;

        // GET: kasutaja/id
        [HttpGet("{id}")]
        public IActionResult GetKasutaja(int id) => kasutajaDB.ElementAtOrDefault(id) == null ? BadRequest(new { message = "Kasutajat ei leitud" }) : Ok(kasutajaDB.ElementAtOrDefault(id));

        // DELETE: kasutaja/delete/id
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            CreateBackup();
            Kasutaja kasutaja = kasutajaDB.ElementAtOrDefault(id) ?? new();
            if (kasutaja.Id == -1)
                return BadRequest(new { message = "Kasutajat ei leitud" });
            kasutajaDB.RemoveAt(id);
            OrderController.Cleaning(true, id);
            Reorder();
            return Ok(kasutajaDB);
        }

        // POST: kasutaja/create/username/password/firstname/lastname
        [HttpPost("create/{username}/{password}/{firstname}/{lastname}")]
        public IActionResult Create(string username, string password, string firstname, string lastname)
        {
            if (!kasutajaDB.Where(x => x.Username == username).Any())
            {
                CreateBackup();
                kasutajaDB.Add(new(kasutajaDB.Count, username, password, firstname, lastname));
                Reorder();
                return Ok(kasutajaDB);
            }
            return BadRequest(new { message = "Dubleeritud kasutaja" });
        }

        // GET: kasutaja/login/username/password
        [HttpGet("login/{username}/{password}")]
        public IActionResult Login(string username, string password)
        {
            Kasutaja checkingKasutaja = kasutajaDB.Where(x => x.Username == username)?.ElementAtOrDefault(0) ?? new();
            if (checkingKasutaja.Password == password)
            {
                isLogged = true;
                currentKasutajaId = checkingKasutaja.Id;
                return Ok(true);
            }
            else
            {
                return BadRequest("Vale parool või kasutajanimi");
            }
        }

        // POST: kasutaja/register/username/password/firstname/lastname
        [HttpPost("register/{username}/{password}/{firstname}/{lastname}")]
        public IActionResult Register(string username, string password, string firstname, string lastname)
        {
            if (!kasutajaDB.Where(x => x.Username == username).Any())
            {
                Create(username, password, firstname, lastname);
                isLogged = true;
                currentKasutajaId = kasutajaDB.Count;
                return Ok(isLogged);
            }
            else
            {
                isLogged = false;
                currentKasutajaId = -1;
                return BadRequest(new { message = "Dubleeritud kasutaja" });
            }
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
        public IActionResult GetCurrent() => kasutajaDB.ElementAtOrDefault(currentKasutajaId) == null ? NotFound(new { message = "Kasutajat ei leitud" }) : Ok(kasutajaDB.ElementAtOrDefault(currentKasutajaId));

        // GET: kasutaja/is-auth
        [HttpGet("is-auth")]
        public bool IsLogged() => isLogged;

        // GET: kasutaja/is-admin
        [HttpGet("is-admin")]
        public bool IsAdmin() => (kasutajaDB.ElementAtOrDefault(currentKasutajaId) ?? new()).IsAdmin;

        // POST: kasutaja/backup
        [HttpPost("backup")]
        public List<Kasutaja> Backup()
        {
            if (backup.Count > 0)
                DeepCopy(backup, kasutajaDB);
            return kasutajaDB;
        }
    }
}
