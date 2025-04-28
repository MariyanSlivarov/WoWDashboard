namespace WoWDashboard.Models
{
    public class GearItem
    {
        public int Id { get; set; } // <- Primary Key
        public string Slot { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int ItemLevel { get; set; }
        public int ItemId { get; set; }

        // Foreign Key
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;
    }
}
