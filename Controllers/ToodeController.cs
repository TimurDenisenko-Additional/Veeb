using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veeb.Migrations;
using Veeb.Models;
using Veeb.Models.DB;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ToodeController : ControllerBase
    {
        private readonly DBContext DB;
        private static readonly List<Toode> backup = [];
        public ToodeController(DBContext db) 
        {
            DB = db;
        }

        //private void DeepCopy(List<Toode> from, List<Toode> to)
        //{
        //    if (from.SequenceEqual(to))
        //        return;
        //    if (to.Count > 0)
        //        to.Clear();
        //    from.ForEach(x =>
        //    {
        //        to.Add(new Toode(x.Id, x.Name, x.Price, x.IsActive));
        //    });
        //}
        //private void CreateBackup() => DeepCopy([.. DB.Tooded], backup);
        //private void Reorder()
        //{
        //    for (int i = 0; i < DB.Tooded.Count(); i++)
        //    {
        //        int oldId = DB.Tooded.ToList()[i].Id;
        //        DB.Tooded.ToList()[i].Id = i;
        //        OrderController.OtherReordering(true, oldId, i);
        //    }
        //    DB.SaveChanges();
        //}

        // GET: toode
        [HttpGet]
        public List<Toode> GetTooded() => [.. DB.Tooded];

        // GET: toode/getActiveTooded
        [HttpGet("getActiveTooded")]
        public List<Toode> GetActiveTooded() => [.. DB.Tooded.Where(x => x.IsActive)];

        // GET: toode/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToode(int id) => await DB.Tooded.ElementOrDefault(id) == null ? NotFound(new { message = "Toodet ei leitud" }) : Ok(await DB.Tooded.ElementOrDefault(id));

        // GET: toode/suurenda-hinda/id/price
        [HttpPatch("suurenda-hinda/{id}/{price}")]
        public async Task<List<Toode>> SuurendaHinda(int id, float price)
        {
            Toode toode = await DB.Tooded.ElementOrDefault(id) ?? new Toode();
            if (toode.Id != -1)
                toode.Price += price;
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/change-active/id
        [HttpPatch("change-active/{id}")]
        public async Task<List<Toode>> ChangeActive(int id)
        {
            Toode toode = await DB.Tooded.ElementOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return [.. DB.Tooded];
            toode.IsActive = !toode.IsActive;
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/change-name/id
        [HttpPatch("change-name/{id}/{newName}")]
        public async Task<List<Toode>> ChangeName(int id, string newName)
        {
            Toode toode = await DB.Tooded.ElementOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return [.. DB.Tooded];
            toode.Name = newName;
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/multiply-price/id/factor
        [HttpPatch("multiply-price/{id}/{factor}")]
        public async Task<List<Toode>> MultiplyPrice(int id, int factor)
        {
            Toode toode = await DB.Tooded.ElementOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return [.. DB.Tooded];
            toode.Price = Math.Round(toode.Price * factor, 2);
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Toode toode = await DB.Tooded.ElementOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return NotFound(new { message = "Toodet ei leitud" });
            DB.Tooded.ToList().RemoveAt(id);
            OrderController.Cleaning(DB, false, id);
            DB.SaveChanges();
            return Ok(DB.Tooded.ToList());
        }

        // GET: toode/create/name/price/state
        [HttpPost("create/{name}/{price}/{state}")]
        public List<Toode> Create(string name, double price, bool state)
        {
            DB.Tooded.Add(new Toode(0, name, price, state));
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        //Another way to add an element
        // GET: toode/create?id=1&nimi=Koola&hind=1.5&aktiivne=true
        //[HttpGet("create")] 
        //public List<Toode> Create([FromQuery] int id, [FromQuery] string nimi, [FromQuery] double hind, [FromQuery] bool aktiivne)
        //{
        //    CreateBackup();
        //    DB.Tooded.ToList().Add(new Toode(id, nimi, hind, aktiivne));
        //    return DB.Tooded.ToList();
        //}

        // GET: toode/rate/1.5
        [HttpPatch("rate/{rate}")]
        public List<Toode> ChangeRate(double rate)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            DB.Tooded = DB.Tooded.ToList().Select(x =>
            {
                x.Price = Math.Round(x.Price * rate, 2);
                return x;
            }) as DbSet<Toode>;
#pragma warning restore CS8601 // Possible null reference assignment.
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/clear
        [HttpDelete("clear")]
        public List<Toode> ClearTable()
        {
            DB.Tooded.ToList().ForEach(x => OrderController.Cleaning(DB, false, x.Id));
            DB.Tooded.ToList().Clear();
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        // GET: toode/state-manage/true
        [HttpPatch("state-manage/{state}")]
        public List<Toode> StateManage(bool state)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            DB.Tooded = DB.Tooded.ToList().Select(x =>
            {
                x.IsActive = state;
                return x;
            }) as DbSet<Toode>;
#pragma warning restore CS8601 // Possible null reference assignment.
            DB.SaveChanges();
            return [.. DB.Tooded];
        }

        //// GET: toode/backup
        //[HttpPost("backup")]
        //public List<Toode> Backup()
        //{
        //    if (backup.Count > 0)
        //        DeepCopy(backup, [.. DB.Tooded]);
        //    DB.SaveChanges();
        //    return [.. DB.Tooded];
        //}

        //GET: toode/max-price
        [HttpGet("max-price")]
        public string MaxPrice()
        {
            Toode toode = new();
            foreach (Toode item in DB.Tooded.ToList())
            {
                if (item.Price > toode.Price)
                    toode = item;
            }
            return $"Kõrgeim hind on tootel '{toode.Name}' indeksiga {toode.Id}, selle hind on {toode.Price}.";
        }
    }
}
