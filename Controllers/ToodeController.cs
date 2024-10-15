using Microsoft.AspNetCore.Mvc;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ToodeController : ControllerBase
    {
        private static List<Toode> toodeDB = [new(1, "Koola", 1.5, true), new(2, "Jäätis", 5, true)];
        private static readonly List<Toode> backup = [];

        private static void DeepCopy(List<Toode> from, List<Toode> to)
        {
            if (from.SequenceEqual(to))
                return;
            if (to.Count > 0)
                to.Clear();
            from.ForEach(x =>
            {
                to.Add(new Toode(x.Id, x.Name, x.Price, x.IsActive));
            });
        }
        private static void CreateBackup() => DeepCopy(toodeDB, backup);

        // GET: toode
        [HttpGet]
        public List<Toode> GetTooded() => toodeDB;

        // GET: toode/id
        [HttpGet("{id}")]
        public Toode GetToode(int id) => toodeDB.ElementAtOrDefault(id) ?? new Toode();

        // GET: toode/suurenda-hinda/id
        [HttpGet("suurenda-hinda/{id}")]
        public Toode SuurendaHinda(int id)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toode;
            toode.Price++;
            return toode;
        }

        // GET: toode/change-active/id
        [HttpGet("change-active/{id}")]
        public Toode ChangeActive(int id)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toode;
            toode.IsActive = !toode.IsActive;
            return toode;
        }

        // GET: toode/change-name/id
        [HttpGet("change-name/{id}/{newName}")]
        public Toode ChangeName(int id, string newName)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toode;
            toode.Name = newName;
            return toode;
        }

        // GET: toode/multiply-price/id/factor
        [HttpGet("multiply-price/{id}/{factor}")]
        public Toode MultiplyPrice(int id, int factor)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toode;
            toode.Price *= factor;
            return toode;
        }

        // GET: toode/delete/id
        [HttpGet("delete/{id}")]
        public string Delete(int id)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return "The object missing";
            toodeDB.RemoveAt(id);
            return $"Toode: \"{toode.Name} {toode.Price}$\" was deleted!";
        }

        // GET: toode/create/id/name/price/state
        [HttpGet("create/{id}/{name}/{price}/{state}")]
        public List<Toode> Create(int id, string name, double price, bool state)
        {
            CreateBackup();
            toodeDB.Add(new Toode(id, name, price, state));
            return toodeDB;
        }

        //Another way to add an element
        // GET: toode/create?id=1&nimi=Koola&hind=1.5&aktiivne=true
        //[HttpGet("create")] 
        //public List<Toode> Create([FromQuery] int id, [FromQuery] string nimi, [FromQuery] double hind, [FromQuery] bool aktiivne)
        //{
        //    CreateBackup();
        //    toodeDB.Add(new Toode(id, nimi, hind, aktiivne));
        //    return toodeDB;
        //}

        // GET: toode/rate/1.5
        [HttpGet("rate/{rate}")]
        public List<Toode> ChangeRate(double rate)
        {
            CreateBackup();
            toodeDB = toodeDB.Select(x =>
            {
                x.Price *= rate;
                return x;
            }).ToList();
            return toodeDB;
        }

        // GET: toode/clear
        [HttpGet("clear")]
        public string ClearTable()
        {
            CreateBackup();
            toodeDB.Clear();
            return "The table has been cleared";
        }

        // GET: toode/state-manage/true
        [HttpGet("state-manage")]
        public string StateManage(bool state)
        {
            CreateBackup();
            toodeDB = toodeDB.Select(x =>
            {
                x.IsActive = state;
                return x;
            }).ToList();
            return $"All items have been {(state ? "enabled" : "disabled")}";
        }

        // GET: toode/backup
        [HttpGet("backup")]
        public List<Toode> Backup()
        {
            if (backup.Count > 0)
                DeepCopy(backup, toodeDB);
            return toodeDB;
        }

        //GET: toode/max-price
        [HttpGet("max-price")]
        public Toode MaxPrice()
        {
            Toode toode = new();
            foreach (Toode item in toodeDB)
            {
                if (item.Price > toode.Price)
                    toode = item;
            }
            return toode;
        }
    }
}
