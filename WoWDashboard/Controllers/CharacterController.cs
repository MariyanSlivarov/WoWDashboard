using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> SavedCharacters()
        {
            var characters = await _context.Characters.ToListAsync();
            return View(characters); 
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

        [HttpGet]
        public async Task<IActionResult> SavedCharacterDetails(int id)
        {
           
            var character = await _context.Characters
                .Include(c => c.GearItems) 
                .Include(c => c.RaidProgression)
                .FirstOrDefaultAsync(c => c.Id == id);  

            if (character == null)
            {
                return NotFound();
            }

            return View("Details", character);
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
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }
     
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(SavedCharacters));
        }
    }
}
