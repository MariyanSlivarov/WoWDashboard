using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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

        public async Task<IActionResult> SavedCharacters(string searchTerm)
        {
            var characters = from c in _context.Characters
                             select c;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var terms = searchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var term in terms)
                {
                    characters = characters.Where(c =>
                        c.Name.ToLower().Contains(term) ||
                        c.Realm.ToLower().Contains(term) ||
                        c.CharacterClass.ToLower().Contains(term) ||
                        c.Level.ToString() == term
                    );
                }

                ViewData["SearchTerm"] = searchTerm;
            }

            return View(await characters.ToListAsync());
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
        [ValidateAntiForgeryToken]
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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFinal(int id, string name, string realm, string characterClass, string race, int level, string region, string guild, int raiderIoScore, string avatarUrl)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(character.OriginalName))
            {
                character.OriginalName = character.Name;
                character.OriginalRealm = character.Realm;
                character.OriginalRegion = character.Region;
            }
            character.Name = name;
            character.Realm = realm;
            character.CharacterClass = characterClass;
            character.Race = race;
            character.Level = level;
            character.Region = region;
            character.Guild = guild;
            character.RaiderIoScore = raiderIoScore;
            character.AvatarUrl = avatarUrl;

            _context.Update(character);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SavedCharacters));
        }
        public async Task<IActionResult> UpdateCharacter(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            var updatedCharacter = await _blizzardService.GetCharacterInfoAsync(character.OriginalName, character.OriginalRealm, character.OriginalRegion);
            var updatedAvatarUrl = await _blizzardService.GetCharacterAvatartAsync(character.OriginalName, character.OriginalRealm, character.OriginalRegion);
            var (score, progression) = await _raiderIOService.GetRaiderIoProfileAsync(character.OriginalName, character.OriginalRealm, character.OriginalRegion);

            if (updatedCharacter == null)
            {
                TempData["ErrorMessage"] = "Failed to fetch updated character data from Blizzard API.";
                return RedirectToAction("SavedCharacters");
            }

            character.Realm = updatedCharacter.Realm;
            character.Region = updatedCharacter.Region;
            character.RaiderIoScore = score;
            character.AvatarUrl = updatedAvatarUrl;
            character.Name = updatedCharacter.Name;
            character.Level = updatedCharacter.Level;
            character.Guild = updatedCharacter.Guild;
            character.Race = updatedCharacter.Race;
            character.CharacterClass = updatedCharacter.CharacterClass;

            _context.Update(character);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{character.Name}'s information was updated.";
            return RedirectToAction("SavedCharacters");
        }
    }
}
