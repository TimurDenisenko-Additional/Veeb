using Microsoft.AspNetCore.Mvc;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        internal static List<Order> orderDB = [new(0, 0, 0)];
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
            Order order = orderDB.ElementAtOrDefault(id) ?? new();
            if (order.Id == -1)
                return NotFound(new { message = "Tellimust ei leitud" });
            orderDB.RemoveAt(id);
            Reorder();
            return Ok(orderDB);
        }

        //// POST: order/create/username/password/firstname/lastname
        //[HttpPost("create/{kasutajaId}/{toodeId}")]
        //public IActionResult Create(int kasutajaId, int toodeId)
        //{
        //    if (KasutajaController.kasutajaDB.ElementAtOrDefault(kasutajaId) == null)
        //        return NotFound(new { message = "Kasutajat ei leitud" });
        //    else if (ToodeController.toodeDB.ElementAtOrDefault(toodeId) == null)
        //        return NotFound(new { message = "Toodet ei leitud" });
        //    orderDB.Add(new(orderDB.Count, kasutajaId, toodeId));
        //    Reorder();
        //    return Ok(orderDB);
        //}
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
