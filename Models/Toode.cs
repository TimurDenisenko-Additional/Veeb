namespace Veeb.Models
{
    public class Toode(int id = -1, string name = "The object is missing", double price = 0, bool isActive = false)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
        public double Price { get; set; } = price;
        public bool IsActive { get; set; } = isActive;
    }
}
