using Microsoft.AspNetCore.Mvc;
using Veeb.Models;
using Veeb.Models.DB;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static DBContext DB;
        public OrderController(DBContext db)
        {
            DB = db;
        }
        private static void Reorder()
        {
            for (int i = 0; i < DB.Ordered.Count(); i++)
            {
                DB.Ordered.ToList()[i].Id = i;
            }
            DB.SaveChanges();
        }

        // GET: order
        [HttpGet]
        public List<Order> GetOrdered() => [.. DB.Ordered];

        // GET: order/id
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id) => DB.Ordered.ElementAtOrDefault(id) != null ? Ok(DB.Ordered.ElementAtOrDefault(id)) : NotFound(new { message = "Tellimust ei leitud" });

        // DELETE: order/delete/id
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Order order = DB.Ordered.ElementAtOrDefault(id) ?? new();
            if (order.Id == -1)
                return NotFound(new { message = "Tellimust ei leitud" });
            DB.Ordered.ToList().RemoveAt(id);
            Reorder();
            return Ok(DB.Ordered);
        }

        // POST: order/create/username/password/firstname/lastname
        [HttpPost("create/{kasutajaId}/{toodeId}")]
        public IActionResult Create(int kasutajaId, int toodeId)
        {
            if (DB.Kasutajad.ElementAtOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Kasutajat ei leitud" });
            else if (DB.Tooded.ElementAtOrDefault(toodeId) == null)
                return NotFound(new { message = "Toodet ei leitud" });
            DB.Ordered.Add(new(DB.Ordered.Count(), kasutajaId, toodeId));
            Reorder();
            return Ok(DB.Ordered);
        }
        public static void Cleaning(bool isKasutaja, int deletedId)
        {
            foreach (Order order in DB.Ordered)
            {
                if ((isKasutaja && order.KasutajaId == deletedId) || (!isKasutaja && order.ToodeId == deletedId))
                {
                    DB.Ordered.Remove(order);
                }
            }
            Reorder();
        }
        public static void OtherReordering(bool isKasutaja, int reorderId, int newId)
        {
            foreach (Order order in DB.Ordered) 
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
            DB.SaveChanges();
        }
    }
}
