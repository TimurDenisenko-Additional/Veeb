using Microsoft.AspNetCore.Mvc;
using Veeb.Models;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static List<Order> orderDB = [new(0, 0, 0)];
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
        public Order GetOrder(int id) => orderDB.ElementAtOrDefault(id) ?? new();

        // DELETE: order/delete/id
        [HttpDelete("delete/{id}")]
        public List<Order> Delete(int id)
        {
            CreateBackup();
            Order order = orderDB.ElementAtOrDefault(id) ?? new();
            if (order.Id == -1)
                return [];
            orderDB.RemoveAt(id);
            Reorder();
            return orderDB;
        }

        // POST: order/create/username/password/firstname/lastname
        [HttpPost("create/{kasutajaId}/{toodeId}")]
        public List<Order> Create(int kId, int tId)
        {
            CreateBackup();
            orderDB.Add(new(orderDB.Count, kId, tId));
            Reorder();
            return orderDB;
        }
    }
}
