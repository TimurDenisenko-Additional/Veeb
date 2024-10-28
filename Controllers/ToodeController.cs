using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Xml.Linq;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ToodeController : ControllerBase
    {
        internal static List<Toode> toodeDB = [new(0, "Koola", 1.5, true), new(1, "Jäätis", 5, true)];
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
        private static void Reorder()
        {
            for (int i = 0; i < toodeDB.Count; i++)
            {
                toodeDB[i].Id = i;
            }
        }

        // GET: toode
        [HttpGet]
        public List<Toode> GetTooded() => toodeDB;

        // GET: toode/getActiveTooded
        [HttpGet("getActiveTooded")]
        public List<Toode> GetActiveTooded() => toodeDB.Where(x => x.IsActive).ToList();

        // GET: toode/id
        [HttpGet("{id}")]
        public Toode GetToode(int id) => toodeDB.ElementAtOrDefault(id) ?? new Toode();

        // GET: toode/suurenda-hinda/id/price
        [HttpPatch("suurenda-hinda/{id}/{price}")]
        public List<Toode> SuurendaHinda(int id, float price)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id != -1)
                toode.Price += price;
            return toodeDB;
        }

        // GET: toode/change-active/id
        [HttpPatch("change-active/{id}")]
        public List<Toode> ChangeActive(int id)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toodeDB;
            toode.IsActive = !toode.IsActive;
            return toodeDB;
        }

        // GET: toode/change-name/id
        [HttpPatch("change-name/{id}/{newName}")]
        public List<Toode> ChangeName(int id, string newName)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toodeDB;
            toode.Name = newName;
            return toodeDB;
        }

        // GET: toode/multiply-price/id/factor
        [HttpPatch("multiply-price/{id}/{factor}")]
        public List<Toode> MultiplyPrice(int id, int factor)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return toodeDB;
            toode.Price = Math.Round(toode.Price * factor, 2); 
            return toodeDB;
        }

        // GET: toode/delete/id
        [HttpDelete("delete/{id}")]
        public List<Toode> Delete(int id)
        {
            CreateBackup();
            Toode toode = toodeDB.ElementAtOrDefault(id) ?? new Toode();
            if (toode.Id == -1)
                return [];
            toodeDB.RemoveAt(id);
            Reorder();
            return toodeDB;
        }

        // GET: toode/create/name/price/state
        [HttpPost("create/{name}/{price}/{state}")]
        public List<Toode> Create(string name, double price, bool state)
        {
            CreateBackup();
            toodeDB.Add(new Toode(toodeDB.Count, name, price, state));
            Reorder();
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
        [HttpPatch("rate/{rate}")]
        public List<Toode> ChangeRate(double rate)
        {
            CreateBackup();
            toodeDB = toodeDB.Select(x =>
            {
                x.Price = Math.Round(x.Price * rate, 2);
                return x;
            }).ToList();
            return toodeDB;
        }

        // GET: toode/clear
        [HttpDelete("clear")]
        public List<Toode> ClearTable()
        {
            CreateBackup();
            toodeDB.Clear();
            return toodeDB;
        }

        // GET: toode/state-manage/true
        [HttpPatch("state-manage/{state}")]
        public List<Toode> StateManage(bool state)
        {
            CreateBackup();
            toodeDB = toodeDB.Select(x =>
            {
                x.IsActive = state;
                return x;
            }).ToList();
            return toodeDB;
        }

        // GET: toode/backup
        [HttpPost("backup")]
        public List<Toode> Backup()
        {
            if (backup.Count > 0)
                DeepCopy(backup, toodeDB);
            return toodeDB;
        }

        //GET: toode/max-price
        [HttpGet("max-price")]
        public string MaxPrice()
        {
            Toode toode = new();
            foreach (Toode item in toodeDB)
            {
                if (item.Price > toode.Price)
                    toode = item;
            }
            return $"Kõrgeim hind on tootel '{toode.Name}' indeksiga {toode.Id}, selle hind on {toode.Price}.";
        }
    }
}
