using Microsoft.AspNetCore.Mvc;
using Veeb.Models;
using Veeb.Models.DB;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KasutajaController : ControllerBase
    {
        private readonly DBContext DB; 
        private readonly List<Kasutaja> backup = [];
        internal static bool isLogged = false;
        internal static int currentKasutajaId = -1; 
        public KasutajaController(DBContext DB)
        {
            this.DB = DB;
        }
        //private void DeepCopy(List<Kasutaja> from, List<Kasutaja> to)
        //{
        //    if (from.SequenceEqual(to))
        //        return;
        //    if (to.Count > 0)
        //        to.Clear();
        //    from.ForEach(x =>
        //    {
        //        to.Add(new(x.Id, x.Username, x.Password, x.FirstName, x.LastName));
        //    });
        //}
        //private void CreateBackup() => DeepCopy([.. DB.Kasutajad], backup);
        //private void Reorder()
        //{
        //    for (int i = 0; i < DB.Kasutajad.Count(); i++)
        //    {
        //        int oldId = DB.Kasutajad.ToList()[i].Id;
        //        DB.Kasutajad.ToList()[i].Id = i;
        //        OrderController.OtherReordering(true, oldId, i);
        //    }
        //    DB.SaveChanges();
        //}

        // GET: kasutaja
        [HttpGet]
        public List<Kasutaja> GetKasutajad() => [..DB.Kasutajad];

        // GET: kasutaja/id
        [HttpGet("{id}")]
        public IActionResult GetKasutaja(int id) => DB.Kasutajad.ElementOrDefault(id) == null ? BadRequest(new { message = "Kasutajat ei leitud" }) : Ok(DB.Kasutajad.ElementOrDefault(id));

        // DELETE: kasutaja/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Kasutaja kasutaja = await DB.Kasutajad.ElementOrDefault(id) ?? new();
            if (kasutaja.Id == -1)
                return BadRequest(new { message = "Kasutajat ei leitud" });
            DB.Kasutajad.ToList().RemoveAt(id);
            OrderController.Cleaning(DB, true, id);
            await DB.SaveChangesAsync();
            return Ok(DB.Kasutajad);
        }

        // POST: kasutaja/create/username/password/firstname/lastname
        [HttpPost("create/{username}/{password}/{firstname}/{lastname}")]
        public IActionResult Create(string username, string password, string firstname, string lastname)
        {
            if (!DB.Kasutajad.Where(x => x.Username == username).Any())
            {
                DB.Kasutajad.Add(new(0, username, password, firstname, lastname));
                DB.SaveChanges();
                return Ok(DB.Kasutajad);
            }
            return BadRequest(new { message = "Dubleeritud kasutaja" });
        }

        // GET: kasutaja/login/username/password
        [HttpGet("login/{username}/{password}")]
        public IActionResult Login(string username, string password)
        {
            Kasutaja checkingKasutaja = DB.Kasutajad.Where(x => x.Username == username)?.ElementAtOrDefault(0) ?? new();
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
        public async Task<IActionResult> RegisterAsync(string username, string password, string firstname, string lastname)
        {
            if (!DB.Kasutajad.Where(x => x.Username == username).Any())
            {
                Create(username, password, firstname, lastname);
                isLogged = true;
                await DB.SaveChangesAsync();
                currentKasutajaId = DB.Kasutajad.Last().Id;
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
        public IActionResult GetCurrent() => DB.Kasutajad.ElementOrDefault(currentKasutajaId) == null ? NotFound(new { message = "Kasutajat ei leitud" }) : Ok(DB.Kasutajad.ElementOrDefault(currentKasutajaId));

        // GET: kasutaja/is-auth
        [HttpGet("is-auth")]
        public bool IsLogged() => isLogged;

        // GET: kasutaja/is-admin
        [HttpGet("is-admin")]
        public async Task<bool> IsAdmin() => (await DB.Kasutajad.ElementOrDefault(currentKasutajaId) ?? new()).IsAdmin;

        //// POST: kasutaja/backup
        //[HttpPost("backup")]
        //public List<Kasutaja> Backup()
        //{
        //    if (backup.Count > 0)
        //        DeepCopy(backup,[.. DB.Kasutajad]);
        //    DB.SaveChanges();
        //    return [.. DB.Kasutajad];
        //}
    }
}
