using Microsoft.AspNetCore.Mvc;
using WoWDashboard.Models;
using WoWDashboard.Services;

namespace WoWDashboard.Controllers
{
    public class CharacterController : Controller
    {
        private readonly BlizzardService _blizzardService;
        private readonly RaiderIOService _raiderIOService;

        public CharacterController(BlizzardService blizzardService, RaiderIOService raiderIOService)
        {
            _blizzardService = blizzardService;
            _raiderIOService = raiderIOService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
    }
}
