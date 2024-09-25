using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetFinal_GuyllaumePaulChristiane.Data;
using ProjetFinal_GuyllaumePaulChristiane.Models;
using Microsoft.AspNetCore.Identity;
using ProjetFinal_GuyllaumePaulChristiane.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ProjetFinal_GuyllaumePaulChristiane.ViewModel.DVDs;
using ProjetFinal_GuyllaumePaulChristiane.Tasks;


namespace ProjetFinal_GuyllaumePaulChristiane.Controllers
{
    [Authorize] // Cette attribute assure que seuls les utilisateurs authentifiés peuvent accéder à ces actions
    public class DVDsController : Controller
    {
        private readonly ProjetFinal_GPC_DBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly EmailSender _emailSender;

        public DVDsController(ProjetFinal_GPC_DBContext context, UserManager<User> userManager, EmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // Méthode helper pour obtenir la liste des catégories
        private static IEnumerable<SelectListItem> GetCategoriesList()
        {
            return Enum.GetValues(typeof(Categorie))
                .Cast<Categorie>()
                .Select(c => new SelectListItem
                {
                    Value = c.ToString(),
                    Text = c.GetType()
                            .GetMember(c.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            ?.GetName() ?? c.ToString()
                });
        }
        // GET: DVDs
        public async Task<IActionResult> Index(string[] sortedBy,string recherche, int? pageNoParam)
        {
            //Initial values
            int pageNo = pageNoParam ?? 1;
            int moviesPerPages = _userManager.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.nbDVDParPage).FirstOrDefault(); //Needs to be selected with Personalisation

            //Collection en Query for dynamic sorting
            var query = _context.DVDs.AsQueryable();

            // Filter by search term (e.g., searching by DVD title or other fields)
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                query = query.Where(d => d.TitreFrancais.Contains(recherche) || d.TitreOriginal.Contains(recherche)); // Adjust this to search on different fields if necessary
            }

            query = query.Where(d => d.VisibleATous || d.UtilisateurEmprunteur == User.Identity.Name);

            if (sortedBy != null && sortedBy.Any())
            {
                // Apply dynamic sorting
                bool isFirstSort = true;
                
                // Always sort by "User" first if it's in the sortedBy list
                if (sortedBy.Contains("User"))
                {
                    query = isFirstSort ? query.OrderBy(d => d.UtilisateurEmprunteur) : ((IOrderedQueryable<DVD>)query).ThenBy(d => d.UtilisateurEmprunteur);
                    isFirstSort = false;
                }

                // Then sort by "Titre" if it's in the sortedBy list
                if (sortedBy.Contains("Titre"))
                {
                    query = isFirstSort ? query.OrderBy(d => d.TitreFrancais) : ((IOrderedQueryable<DVD>)query).ThenBy(d => d.TitreFrancais);
                    isFirstSort = false;
                }
            }
            else //if no option selected or first time on page, orderby titre
            {
                query = query.OrderBy(d => d.TitreFrancais);
            }

            // Store the selected sort options in ViewData
            ViewData["SelectedSortOptions"] = sortedBy ?? [];

            //LINQ query 
            var totalResults = await query.ToListAsync();
            var pagedResults = await query
                .Skip((pageNo - 1) * moviesPerPages) //OFFSET
                .Take(moviesPerPages) //LIMIT
                .ToListAsync();
            var totalPages = (int)Math.Ceiling(totalResults.Count() / (double)moviesPerPages);
            if (totalPages == 0) totalPages = 1;

            var viewModel = new DVDViewModel { DVDs = pagedResults, TotalPages = totalPages, currentPage = pageNo, isResearched = !string.IsNullOrWhiteSpace(recherche) };


            return View(viewModel);
        }
        // GET: DVDs/IndexConnectedUser
        public async Task<IActionResult> IndexConnectedUser(int? pageNoParam)
        {
            //Initial values
            int pageNo = pageNoParam ?? 1;
            int moviesPerPages = _userManager.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.nbDVDParPage).FirstOrDefault(); //Needs to be selected with Personalisation

            //Collection en Query for dynamic sorting
            var query = _context.DVDs.AsQueryable();

            //Add userConnected restriction !!!
            query = query.Where(d => User.Identity.Name == d.UtilisateurEmprunteur);

            //OrderBy default
            query = query.OrderBy(d => d.TitreFrancais);

            //LINQ query 
            var totalResults = await query.ToListAsync();

            var pagedResults = await query
                .Skip((pageNo - 1) * moviesPerPages) //OFFSET
                .Take(moviesPerPages) //LIMIT
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalResults.Count() / (double)moviesPerPages);
            if (totalPages == 0) totalPages = 1;
            var viewModel = new DVDViewModel { DVDs = pagedResults, TotalPages = totalPages, currentPage = pageNo };

            return View(viewModel);
        }
        // GET: DVDs/IndexSelectedUser
        public async Task<IActionResult> IndexSelectedUser(string userId, int? pageNoParam)
        {
            //Initial values
            int pageNo = pageNoParam ?? 1;
            int moviesPerPages = _userManager.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.nbDVDParPage).FirstOrDefault(); //Needs to be selected with Personalisation

            //Collection en Query for dynamic sorting
            var query = _context.DVDs.AsQueryable();

            //Add userConnected
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); 
            }
            query = query.Where(d => user.UserName == d.UtilisateurEmprunteur && d.VisibleATous);

            //OrderBy default
            query = query.OrderBy(d => d.TitreFrancais);

            //LINQ query 
            var totalResults = await query.ToListAsync();

            var pagedResults = await query
                .Skip((pageNo - 1) * moviesPerPages) //OFFSET
                .Take(moviesPerPages) //LIMIT
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalResults.Count() / (double)moviesPerPages);
            if (totalPages == 0) totalPages = 1;
            var viewModel = new SelectedUserViewModel { DVDs = pagedResults, TotalPages = totalPages, currentPage = pageNo, SelectedUserId = userId, UserName = user.UserName};

            return View(viewModel);
        }
        // GET: DVDs/IndexSelectUser
        public async Task<IActionResult> IndexSelectUser()
        {
            // Retrieve all users except connected user
            var users = await _userManager.Users
                .Where(u => u.UserName != User.Identity.Name)
                .ToListAsync();

            // Prepare the ViewModel
            var model = new SelectUserViewModel
            {
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id, // User's ID
                    Text = u.UserName // User's username or email
                }).ToList()
            };

            return View(model);
        }
        // POST: DVDs/IndexSelectUser
        [HttpPost]
        public IActionResult ProcessSelectedUser(SelectUserViewModel model)
        {
            if (model.SelectedUserId != null)
            {
                var selectedUserId = model.SelectedUserId;

                return RedirectToAction("IndexSelectedUser", new { userId = selectedUserId });
            }

            // If the model is invalid, return to the form
            return View("IndexSelectUser", model);
        }

        // GET: DVDs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVD = await _context.DVDs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dVD == null)
            {
                return NotFound();
            }

            return View(dVD);
        }

        // GET: DVDs/Create
        public IActionResult Create()
        {
            var proprietaire = _userManager.GetUserAsync(User).Result;
            ViewBag.UserName = proprietaire?.UserName;

            ViewBag.Categories = GetCategoriesList();
            
            ViewBag.Formats = new SelectList(new List<string> { "Normal", "Panoramique", "Blu-Ray" });

            ViewBag.Langues = new SelectList(new List<string> { "anglais", "français", "espagnol" });

            ViewBag.STitres = new SelectList(new List<string> { "anglais", "français", "espagnol" });

            return View();
        }

        // POST: DVDs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TitreFrancais,TitreOriginal,AnneeSortie,Categorie,DerniereMiseAJour,DerniereMiseAJourPar,DescriptionSupplements,Duree,EstOriginal,Format,Langue,NombreDisques,NomProducteur,NomRealisateur,NomsActeursPrincipaux,ResumeFilm,SousTitres,UtilisateurProprietaire,UtilisateurEmprunteur,VersionEtendue,VisibleATous")] DVD dVD, IFormFile imagePochette)
        {
            var proprietaire = _userManager.GetUserAsync(User).Result;
            ViewBag.UserName = proprietaire?.UserName;

            //vérifier si le titre existe déjà pour un DVD
            var dvdExistant = await _context.DVDs.FirstOrDefaultAsync(d => d.TitreFrancais == dVD.TitreFrancais);
            if (dvdExistant != null)
            {
                ModelState.AddModelError("TitreFrancais", "Le titre existe déjà.");

                return View(dVD);
            }
                ModelState.Remove("imagePochette");
            if (ModelState.IsValid)
            {
                
                if (imagePochette != null && imagePochette.Length > 0)
                {
                   
                    var image = imagePochette;
                    var extension = Path.GetExtension(image.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "-" + dVD.TitreFrancais + extension;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ImagePochette");
                
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    dVD.ImagePochette = Path.Combine("\\ImagePochette", uniqueFileName);
                    Console.WriteLine(dVD.ImagePochette);

                    
                }
                
                dVD.DerniereMiseAJour = DateTime.Now;
                dVD.DerniereMiseAJourPar = User?.Identity?.Name?? string.Empty;
                dVD.UtilisateurProprietaire = proprietaire?.UserName;

                _context.Add(dVD);
                await _context.SaveChangesAsync();

                //Envoie de courriel
                var userToNotify = await _userManager.Users.Where(u => u.courrielOnDVDCreate).Select(u => u.Email).ToListAsync();
                if ( userToNotify != null && userToNotify.Count > 0 )
                {
                    await _emailSender.notifyDVD(userToNotify, "Un Nouveau DVD est disponible", dVD);
                }
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.Categories = GetCategoriesList();
            ViewBag.Formats = new SelectList(new List<string> { "Normal", "Panoramique", "Blu-Ray" });
            ViewBag.Langues = new SelectList(new List<string> { "anglais", "français", "espagnol" });
            ViewBag.STitres = new SelectList(new List<string> { "anglais", "français", "espagnol" });
            return View(dVD);
        }

        // GET: DVDs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVD = await _context.DVDs.FindAsync(id);
            if (dVD == null)
            {
                return NotFound();
            }

            var proprietaire = _userManager.GetUserAsync(User).Result;
            ViewBag.UserName = proprietaire?.UserName;

            ViewBag.Formats = new SelectList(new List<string> { "Normal", "Panoramique", "Blu-Ray" });

            ViewBag.Langues = new SelectList(new List<string> { "anglais", "français", "espagnol" });

            ViewBag.STitres = new SelectList(new List<string> { "anglais", "français", "espagnol" });

            ViewBag.Categories = GetCategoriesList();

            return View(dVD);
        }

        // POST: DVDs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TitreFrancais,TitreOriginal,AnneeSortie,Categorie,DerniereMiseAJour,DerniereMiseAJourPar,DescriptionSupplements,Duree,EstOriginal,Format,Langue,NombreDisques,NomProducteur,NomRealisateur,NomsActeursPrincipaux,ResumeFilm,SousTitres,UtilisateurProprietaire,UtilisateurEmprunteur,VersionEtendue,VisibleATous")] DVD dVD, IFormFile imagePochette)
        {
            if (id != dVD.Id)
            {
                return NotFound();
            }

            ModelState.Remove("imagePochette");

            var dVDFromDB = await _context.DVDs.FindAsync(dVD.Id);
            if (dVDFromDB == null)
            {
                return NotFound();
            }

            //vérifier si le titre existe déjà pour un DVD
            var dvdExistant = await _context.DVDs.FirstOrDefaultAsync(d => d.TitreFrancais == dVD.TitreFrancais && d.Id != dVD.Id);
            if (dvdExistant != null)
            {
                ModelState.AddModelError("TitreFrancais", "Le titre existe déjà.");

                return View(dVD);
            }

            if (ModelState.IsValid)
            {

                if (imagePochette == null)
                {
                    dVD.ImagePochette = dVDFromDB.ImagePochette;
                }
                else
                {

                    var image = imagePochette;
                    var extension = Path.GetExtension(image.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "-" + dVD.TitreFrancais + extension;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ImagePochette");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    dVD.ImagePochette = Path.Combine("\\ImagePochette", uniqueFileName);
                    Console.WriteLine(dVD.ImagePochette);
                }

                try
                {
                    dVD.DerniereMiseAJour = DateTime.Now;
                    if(User.Identity?.Name != null)
                    {
                        dVD.DerniereMiseAJourPar = User.Identity.Name;
                    }

                    _context.Entry(dVDFromDB).CurrentValues.SetValues(dVD);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDExists(dVD.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

             

                return RedirectToAction(nameof(Index));
            }

            var proprietaire = _userManager.GetUserAsync(User).Result;
            ViewBag.UserName = proprietaire?.UserName;
            ViewBag.Formats = new SelectList(new List<string> { "Normal", "Panoramique", "Blu-Ray" });
            ViewBag.Langues = new SelectList(new List<string> { "anglais", "français", "espagnol" });
            ViewBag.STitres = new SelectList(new List<string> { "anglais", "français", "espagnol" });
            ViewBag.Categories = GetCategoriesList();

            return View(dVD);
        }

        // GET: DVDs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVD = await _context.DVDs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dVD == null)
            {
                return NotFound();
            }

            return View(dVD);
        }

        // POST: DVDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVD = await _context.DVDs.FindAsync(id);
            if (dVD != null)
            {
                _context.DVDs.Remove(dVD);
                //Envoie de courriel
                var userToNotify = await _userManager.Users.Where(u => u.courrielOnDVDDelete).Select(u => u.Email).ToListAsync();
                if (userToNotify != null && userToNotify.Count > 0)
                {
                    await _emailSender.notifyDVD(userToNotify, "Un DVD n'est plus disponible", dVD);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DVDExists(int id)
        {
            return _context.DVDs.Any(e => e.Id == id);
        }

        // GET: DVDs/AppropriationConfirmation/5
        [HttpGet]
        public async Task<IActionResult> AppropriationConfirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dvd = await _context.DVDs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dvd == null)
            {
                return NotFound();
            }

            return View(dvd);
        }

        // POST: DVDs/Appropriation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Appropriation(int id)
        {
            var dvd = await _context.DVDs.FindAsync(id);
            if (dvd == null)
            {
                return NotFound();
            }

            dvd.UtilisateurProprietaire = User.Identity.Name;
            dvd.UtilisateurEmprunteur = User.Identity.Name;
            dvd.DerniereMiseAJour = DateTime.Now;
            dvd.DerniereMiseAJourPar = User.Identity.Name;

            _context.Update(dvd);
            await _context.SaveChangesAsync();

            //Envoie de courriel
            var userToNotify = await _userManager.Users.Where(u => u.courrielOnAppropriation).Select(u => u.Email).ToListAsync();
            if (userToNotify != null && userToNotify.Count > 0)
            {
                await _emailSender.notifyDVD(userToNotify, "Un DVD a été attribuer a " + dvd.UtilisateurEmprunteur, dvd);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DVDs/Contact/5&youremail@email.com id et username optionnel si contact général
        public async Task<IActionResult> Contact(int? id, string? username, string? statusMessage)
        {
            if (id == null || username == null)
            {
                var usersToContact = await _userManager.Users.ToListAsync();
                if (usersToContact == null)
                {
                    return NotFound();
                }
                var emailsToContact = usersToContact.Select(u => u.Email).ToList();
                var modelWithoutDVD = new ContactViewModel { dvd = null, username = null, userAContacter = new List<string>(), userContactable = emailsToContact };
                return View("Contact", modelWithoutDVD);
            }
            var dvd = await _context.DVDs.FindAsync(id);
            if (dvd == null)
            {
                return NotFound();
            }
            Console.WriteLine(dvd.TitreFrancais);
            var model = new ContactViewModel { dvd = dvd, username = username, userAContacter = new List<string>(), userContactable = null };
            if (statusMessage != null)
            {
                model.StatusMessage = statusMessage;
            }
            return View("Contact",model);
        }
        // POST: DVDs/Contact/
        public async Task<IActionResult> SendEmail(ContactViewModel model)
        {
            if (!ModelState.IsValid || model.sujet == null || model.message == null || (model.username == null && model.userAContacter == null))
            {
                if(model.dvdId == null)
                    return RedirectToAction("Contact", new { statusMessage = "Error : Veuillez renseigner tous les champs." });
                return RedirectToAction("Contact", new { id = model.dvdId, model.username, statusMessage = "Error : Veuillez renseigner tous les champs." });
            }
            var dvd = _context.DVDs.Find(model.dvdId);

            if (dvd == null)
            {
                if (model.userAContacter.Contains("all"))
                {
                    model.userAContacter = model.userContactable;
                }
                await _emailSender.SendEmailAsync(User.Identity.Name, model.userAContacter, model.sujet, model.message);
                model.userAContacter = new List<string>();
            }
            else
            {
                model.dvd = dvd;
                await _emailSender.SendEmailAsync(User.Identity.Name, model.username, model.sujet, model.message, dvd);
            }
            model.StatusMessage = "Votre message a été envoyé avec succès.";
            return View("Contact", model);
        }
    }
}
