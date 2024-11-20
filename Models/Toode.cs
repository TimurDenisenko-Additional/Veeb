
using Veeb.Models.DB;

namespace Veeb.Models
{
    public class Toode(int id = -1, string name = "", double price = 0, bool isActive = false) : DBModel(id)
    {
        public string Name { get; set; } = name;
        public double Price { get; set; } = price;
        public bool IsActive { get; set; } = isActive;
    }
}
