namespace Veeb.Models
{
    public class Order(int id = -1, int kasutajaId = -1, int toodeId = -1)
    {
        public int Id { get; set; } = id;
        public int KasutajaId { get; set; } = kasutajaId;
        public int ToodeId { get; set; } = toodeId;
    }
}
