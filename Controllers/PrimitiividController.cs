using Microsoft.AspNetCore.Mvc;

namespace Veeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrimitiividController : ControllerBase
    {
        // GET: primitiivid/hello
        [HttpGet("hello")]
        public string Hello() => "Hello world at " + DateTime.Now;

        // GET: primitiivid/hello/mari
        [HttpGet("hello/{nimi}")]
        public string Hello(string nimi) => "Hello " + nimi;

        // GET: primitiivid/addition/5/6
        [HttpGet("addition/{nr1}/{nr2}")]
        public int Addition(int nr1, int nr2) => nr1 + nr2;

        // GET: primitiivid/multiplication/5/6
        [HttpGet("multiplication/{nr1}/{nr2}")]
        public int Multiplication(int nr1, int nr2) => nr1 * nr2;

        // GET: primitiivid/do-logs/5
        [HttpGet("do-logs/{arv}")]
        public void DoLogs(int arv)
        {
            for (int i = 0; i < arv; i++)
            {
                Console.WriteLine("See on logi nr " + i);
            }
        }

        // GET: primitiivid/random/1/100
        [HttpGet("random/{nr1}/{nr2}")]
        public int Random(int nr1, int nr2) => nr1 < nr2 ? new Random().Next(nr1, nr2) : new Random().Next(nr2, nr1);

        // GET: primitiivid/age/2006-09-21
        [HttpGet("age/{date}")]
        public int Age(DateOnly date) =>
            DateTime.Now.Year - date.Year - (date.Month < DateTime.Now.Month || date.Month == DateTime.Now.Month && date.Day <= DateTime.Now.Day ? 0 : 1);
    }
}
