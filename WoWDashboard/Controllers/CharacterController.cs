using Microsoft.AspNetCore.Mvc;
using WoWDashboard.Data;
using WoWDashboard.Models;
using WoWDashboard.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WoWDashboard.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlizzardService _blizzardService;
        private readonly RaiderIOService _raiderIOService;

        public CharacterController(ApplicationDbContext context, BlizzardService blizzardService, RaiderIOService raiderIOService)
        {
            _blizzardService = blizzardService;
            _raiderIOService = raiderIOService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GoToIndex()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Details(string name, string realm, string region)
        {
            var character = await _blizzardService.GetCharacterInfoAsync(name, realm, region);

            if (character == null)
            {
                return View("Error");
            }

            var (score, progression) = await _raiderIOService.GetRaiderIoProfileAsync(name, realm, region);
            character.RaiderIoScore = score;
            character.RaidProgression = progression;

            var equipedItems = await _blizzardService.GetCharacterEquipmentAsync(name, realm, region);
            character.GearItems = equipedItems;

            var avatarUrl = await _blizzardService.GetCharacterAvatartAsync(name, realm, region);
            character.AvatarUrl = avatarUrl;

            return View(character);
        }


        [HttpPost]
        public async Task<IActionResult> SaveCharacter(string name, string realm, string region)
        {
            var character = await _blizzardService.GetCharacterInfoAsync(name, realm, region);
            var (score, progression) = await _raiderIOService.GetRaiderIoProfileAsync(name, realm, region); 
            var equipedItems = await _blizzardService.GetCharacterEquipmentAsync(name, realm, region);
            var avatarUrl = await _blizzardService.GetCharacterAvatartAsync(name, realm, region);
            character.AvatarUrl = avatarUrl;
            character.GearItems = equipedItems;
            character.RaiderIoScore = score;
            character.RaidProgression = progression;
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return View("Details", character);
        }
    }
}
