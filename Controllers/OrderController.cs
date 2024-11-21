using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Veeb.Models;
using Veeb.Models.DB;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private DBContext DB;
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
        public async Task<IActionResult> GetOrder(int id) => await DB.Ordered.ElementOrDefault(id) != null ? Ok(await DB.Ordered.ElementOrDefault(id)) : NotFound(new { message = "Tellimust ei leitud" });

        // DELETE: order/delete/id
        [HttpDelete("delete/{id}")]
        public async Task <IActionResult> Delete(int id)
        {
            Order order = await DB.Ordered.ElementOrDefault(id) ?? new();
            if (order.Id == -1 || !DB.Ordered.Any(x => x.Id == id))
                return NotFound(new { message = "Tellimust ei leitud" });
            DB.Ordered.Remove(order);
            return Ok(DB.Ordered);
        }

        // POST: order/create/kasutajaId/toodeId
        [HttpPost("create/{kasutajaId}/{toodeId}")]
        public async Task<IActionResult> Create(int kasutajaId, int toodeId)
        {
            if (await DB.Kasutajad.ElementOrDefault(kasutajaId) == null)
                return NotFound(new { message = "Kasutajat ei leitud" });
            else if (await DB.Tooded.ElementOrDefault(toodeId) == null)
                return NotFound(new { message = "Toodet ei leitud" });
            DB.Ordered.Add(new(0, kasutajaId, toodeId));
            DB.SaveChanges();
            return Ok(DB.Ordered);
        }

        // POST: order/buy/toodeId
        [HttpPost("buy/{toodeId}")]
        public async Task<IActionResult> AddToCart(int toodeId)
        {
            if (await DB.Kasutajad.ElementOrDefault(KasutajaController.currentKasutajaId) == null)
                return NotFound(new { message = "Sa ei ole sisse logitud" });
            else if (await DB.Tooded.ElementOrDefault(toodeId) == null)
                return NotFound(new { message = "Toodet ei leitud" });
            DB.Ordered.Add(new(0, KasutajaController.currentKasutajaId, toodeId));
            DB.SaveChanges();
            return Ok(new { message = "Toode lisatud ostukorvi" });
        }

        // GET: order/userCart
        [HttpGet("userCart")]
        public async Task<IActionResult> UserCart()
        {
            if (await DB.Kasutajad.ElementOrDefault(KasutajaController.currentKasutajaId) == null)
                return NotFound(new { message = "Sa ei ole sisse logitud" });
            Toode?[] tooded = await userCart();
            return Ok(tooded);
        }

        // GET: order/userCartSum/kasutajaId
        [HttpGet("userCartSum")]
        public async Task<IActionResult> UserCartSumAsync()
        {
            if (await DB.Kasutajad.ElementOrDefault(KasutajaController.currentKasutajaId) == null)
                return NotFound(new { message = "Sa ei ole sisse logitud" });
            return Ok(await userCartSum());
        }

        private async Task<Toode?[]> userCart()
        {
            return await DB.Ordered.Where(x => x.KasutajaId == KasutajaController.currentKasutajaId).Join(
            DB.Tooded,
            order => order.ToodeId,
            toode => toode.Id,
            (order, toode) => toode
            ).ToAsyncEnumerable().ToArrayAsync();
        }
        private async Task<double> userCartSum()
        {
            Toode?[] tooded = await userCart();
            return tooded.Sum(x => x.Price);
        }

        [HttpGet("MakePayment")]
        public async Task<IActionResult> MakePayment()
        {
            double amount = await userCartSum();
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

        public static void Cleaning(DBContext DB, bool isKasutaja, int deletedId)
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
