using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetFinal_GuyllaumePaulChristiane.Models;
using ProjetFinal_GuyllaumePaulChristiane.ViewModel.User;

namespace ProjetFinal_GuyllaumePaulChristiane.Areas.Identity.Pages.Account.Manage
{
    public class AffichageModel(UserManager<User> userManager) : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;

        [BindProperty]
        public AffichageViewModel Input { get; set; } = new AffichageViewModel();
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new AffichageViewModel
            {
                nbDVDParPage = user.nbDVDParPage
            };
            ViewData["ActivePage"] = "Affichage";

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.nbDVDParPage = Input.nbDVDParPage;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                StatusMessage = "Your profile has been updated.";
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
