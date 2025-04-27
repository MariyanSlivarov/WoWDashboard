namespace WoWDashboard.Models
{
    public class Character
    {
        public string Name { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public int Level { get; set; } // ✅ Level property defined
        public string Race { get; set; } = string.Empty;
        public string Guild { get; set; } = string.Empty;
        public string CharacterClass { get; set; } = string.Empty;
        public List<GearItem> GearItems { get; set; } = new List<GearItem>();
        public double RaiderIoScore { get; set; } // ✅ RaiderIoScore property defined
        public RaidProgression RaidProgression { get; set; } = new RaidProgression();

        public string AvatarUrl {  get; set; } = string.Empty;
    }
}
