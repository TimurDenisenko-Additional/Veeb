using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Veeb.Migrations;
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
        //private static void Reorder()
        //{
        //    for (int i = 0; i < DB.Ordered.Count(); i++)
        //    {
        //        DB.Ordered.ToList()[i].Id = i;
        //    }
        //    DB.SaveChanges();
        //}s

        // GET: order
        [HttpGet]
        public List<Order> GetOrdered() => [.. DB.Ordered];

        // GET: order/id
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id) => DB.Ordered.ElementOrDefault(id) != null ? Ok(DB.Ordered.ElementOrDefault(id)) : NotFound(new { message = "Tellimust ei leitud" });

        // DELETE: order/delete/id
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Order order = DB.Ordered.ElementOrDefault(id) ?? new();
            if (order.Id == -1)
                return NotFound(new { message = "Tellimust ei leitud" });
            DB.Ordered.ToList().RemoveAt(id);
            return Ok(DB.Ordered);
        }

        // POST: order/create/kasutajaId/toodeId
        [HttpPost("create/{kasutajaId}/{toodeId}")]
        public IActionResult Create(int kasutajaId, int toodeId)
        {
            if (DB.Kasutajad.ElementOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Kasutajat ei leitud" });
            else if (DB.Tooded.ElementOrDefault(toodeId) == null)
                return NotFound(new { message = "Toodet ei leitud" });
            DB.Ordered.Add(new(DB.Ordered.Count(), kasutajaId, toodeId));
            DB.SaveChanges();
            return Ok(DB.Ordered);
        }

        // POST: order/buy/toodeId
        [HttpPost("buy/{toodeId}")]
        public IActionResult AddToCard(int toodeId) => KasutajaController.currentKasutajaId != -1 ? Create(KasutajaController.currentKasutajaId, toodeId) : NotFound(new {message = "Sa ei ole sisse logitud"});

        // GET: order/userCart/kasutajaId
        [HttpGet("userCart/{kasutajaId}")]
        public IActionResult UserCart(int kasutajaId)
        {
            if (DB.Kasutajad.ElementOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Sa ei ole sisse logitud" });
            return Ok(DB.Ordered.Where(x => x.KasutajaId == kasutajaId));
        }

        // GET: order/userCartSum/kasutajaId
        [HttpGet("userCartSum/{kasutajaId}")]
        public IActionResult UserCartSum(int kasutajaId)
        {
            if (DB.Kasutajad.ElementOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Sa ei ole sisse logitud" });
            return Ok(userCartSum(kasutajaId));
        }

        private double userCartSum(int kasutajaId)
        {
            int orderId = DB.Ordered.Where(x => x.KasutajaId == kasutajaId).FirstOrDefault()?.Id ?? -1;
            return DB.Tooded.Where(x => x.Id == orderId)?.Sum(x =>  x.Price) ?? 0;
        }

        [HttpGet("MakePayment/{kasutajaId}")]
        public async Task<IActionResult> MakePayment(int kasutajaId)
        {
            double amount = userCartSum(kasutajaId);
            if (amount == 0)
                return BadRequest("Cart is empty");
            string json = JsonSerializer.Serialize(new
            {
                api_username = "e36eb40f5ec87fa2",
                account_name = "EUR3D1",
                amount,
                order_reference = Math.Ceiling(new Random().NextDouble() * 999999),
                nonce = $"a9b7f7e7as{DateTime.Now}{new Random().NextDouble() * 999999}",
                timestamp = DateTime.Now,
                customer_url = "https://maksmine.web.app/makse"
            });

            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "ZTM2ZWI0MGY1ZWM4N2ZhMjo3YjkxYTNiOWUxYjc0NTI0YzJlOWZjMjgyZjhhYzhjZA==");
            HttpResponseMessage response = await client.PostAsync("https://igw-demo.every-pay.com/api/v4/payments/oneoff", new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode )
                return BadRequest("Payment failed.");
            string responseContent = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
            JsonElement paymentLink = jsonDoc.RootElement.GetProperty("payment_link");
            return Ok(paymentLink);
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
        }
        //public static void OtherReordering(bool isKasutaja, int reorderId, int newId)
        //{
        //    foreach (Order order in DBContext.DB.Ordered) 
        //    {
        //        if (isKasutaja && order.KasutajaId == reorderId)
        //        {
        //            order.KasutajaId = newId;
        //        }
        //        else if (!isKasutaja && order.ToodeId == reorderId)
        //        {
        //            order.ToodeId = newId;
        //        }
        //    }
        //    DBContext.DB.SaveChanges();
        //}
    }
}
