using Microsoft.AspNetCore.Mvc;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        internal static List<Order> orderDB = [new(0, 0, 0)];
        private static readonly List<Order> backup = [];
        private static void DeepCopy(List<Order> from, List<Order> to)
        {
            if (from.SequenceEqual(to))
                return;
            if (to.Count > 0)
                to.Clear();
            from.ForEach(x =>
            {
                to.Add(new(x.Id, x.KasutajaId, x.ToodeId));
            });
        }
        private static void CreateBackup() => DeepCopy(orderDB, backup);
        private static void Reorder()
        {
            for (int i = 0; i < orderDB.Count; i++)
            {
                orderDB[i].Id = i;
            }
        }

        // GET: order
        [HttpGet]
        public List<Order> GetOrdered() => orderDB;

        // GET: order/id
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id) => orderDB.ElementAtOrDefault(id) != null ? Ok(orderDB.ElementAtOrDefault(id)) : NotFound(new { message = "Tellimust ei leitud" });

        // DELETE: order/delete/id
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            CreateBackup();
            Order order = orderDB.ElementAtOrDefault(id) ?? new();
            if (order.Id == -1)
                return NotFound(new { message = "Tellimust ei leitud" });
            orderDB.RemoveAt(id);
            Reorder();
            return Ok(orderDB);
        }

        // POST: order/create/username/password/firstname/lastname
        [HttpPost("create/{kasutajaId}/{toodeId}")]
        public IActionResult Create(int kasutajaId, int toodeId)
        {
            if (KasutajaController.kasutajaDB.ElementAtOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Kasutajat ei leitud" });
            else if (ToodeController.toodeDB.ElementAtOrDefault(toodeId) == null)
                return NotFound(new { message = "Toodet ei leitud" });
            CreateBackup();
            orderDB.Add(new(orderDB.Count, kasutajaId, toodeId));
            Reorder();
            return Ok(orderDB);
        }

        // GET: order/backup
        [HttpPost("backup")]
        public List<Order> Backup()
        {
            if (backup.Count > 0)
                DeepCopy(backup, orderDB);
            return orderDB;
        }

        public static void Cleaning(bool isKasutaja, int deletedId)
        {
            foreach (Order order in orderDB.ToList())
            {
                if ((isKasutaja && order.KasutajaId == deletedId) || (!isKasutaja && order.ToodeId == deletedId))
                {
                    orderDB.Remove(order);
                }
            }
            Reorder();
        }
        public static void OtherReordering(bool isKasutaja, int reorderId, int newId)
        {
            foreach (Order order in orderDB.ToList()) 
            {
                if (isKasutaja && order.KasutajaId == reorderId)
                {
                    order.KasutajaId = newId;
                }
                else if (!isKasutaja && order.ToodeId == reorderId)
                {
                    order.ToodeId = newId;
                }
            }
        }
    }
}
