using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetFinal_GuyllaumePaulChristiane.Data;
using ProjetFinal_GuyllaumePaulChristiane.Models;
using CsvHelper;
using System.Text;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using ProjetFinal_GuyllaumePaulChristiane.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace ProjetFinal_GuyllaumePaulChristiane.Controllers
{
    [Authorize] // Cette attribute assure que seuls les utilisateurs authentifiés peuvent accéder à ces actions
    public class DVDsController : Controller
    {
        private readonly ProjetFinal_GPC_DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DVDsController(ProjetFinal_GPC_DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> Index(string[] sortedBy, int? pageNoParam)
        {
            //Initial values
            int pageNo = pageNoParam ?? 1;
            int moviesPerPages = 12; //Needs to be selected with Personalisation

            //Collection en Query for dynamic sorting
            var query = _context.DVDs.AsQueryable();

            query = query.Where(d => d.VisibleATous || d.UtilisateurEmprunteur == User.Identity.Name);

            if (sortedBy != null && sortedBy.Any())
            {
                // Apply dynamic sorting
                bool isFirstSort = true;
                foreach (string sortOption in sortedBy) //for each sorting options selected, add it the OrderBy Clause
                {
                    switch (sortOption)
                    {
                        case "Titre":
                            query = isFirstSort ? query.OrderBy(d => d.TitreFrancais) : ((IOrderedQueryable<DVD>)query).ThenBy(d => d.TitreFrancais); //either normal OrderBy or add it to already existing order by
                            break;
                        case "User":
                            query = isFirstSort ? query.OrderBy(d => d.UtilisateurEmprunteur) : ((IOrderedQueryable<DVD>)query).ThenBy(d => d.UtilisateurEmprunteur);
                            break;
                    }
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

            var viewModel = new DVDViewModel { DVDs = pagedResults, TotalPages = totalPages, currentPage = pageNo };


            return View(viewModel);
        }
        // GET: DVDs/IndexConnectedUser
        public async Task<IActionResult> IndexConnectedUser(int? pageNoParam)
        {
            //Initial values
            int pageNo = pageNoParam ?? 1;
            int moviesPerPages = 12; //Needs to be selected with Personalisation

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
            int moviesPerPages = 12; //Needs to be selected with Personalisation

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
            ViewBag.Categories = GetCategoriesList();
            return View();
        }

        // POST: DVDs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TitreFrancais,TitreOriginal,AnneeSortie,Categorie,DerniereMiseAJour,DerniereMiseAJourPar,DescriptionSupplements,Duree,EstOriginal,Format,ImagePochette,Langue,NombreDisques,NomProducteur,NomRealisateur,NomsActeursPrincipaux,ResumeFilm,SousTitres,UtilisateurProprietaire,UtilisateurEmprunteur,VersionEtendue,VisibleATous")] DVD dVD)
        {
            if (ModelState.IsValid)
            {
                dVD.DerniereMiseAJour = DateTime.Now;
                dVD.DerniereMiseAJourPar = User.Identity.Name;
                _context.Add(dVD);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = GetCategoriesList();
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
            ViewBag.Categories = GetCategoriesList();
            return View(dVD);
        }

        // POST: DVDs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TitreFrancais,TitreOriginal,AnneeSortie,Categorie,DerniereMiseAJour,DerniereMiseAJourPar,DescriptionSupplements,Duree,EstOriginal,Format,ImagePochette,Langue,NombreDisques,NomProducteur,NomRealisateur,NomsActeursPrincipaux,ResumeFilm,SousTitres,UtilisateurProprietaire,UtilisateurEmprunteur,VersionEtendue,VisibleATous")] DVD dVD)
        {
            if (id != dVD.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dVD.DerniereMiseAJour = DateTime.Now;
                    dVD.DerniereMiseAJourPar = User.Identity.Name;
                    _context.Update(dVD);
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDExists(int id)
        {
            return _context.DVDs.Any(e => e.Id == id);
        }

        // méthode pour l'insertion de données DVD détaillées
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportPost()
        {
            try
            {
                Console.WriteLine("Début de la méthode ImportPost");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SampleDVDs.csv");
                Console.WriteLine($"Chemin du fichier CSV : {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine("Le fichier CSV n'a pas été trouvé.");
                    ModelState.AddModelError("", "Le fichier CSV n'a pas été trouvé.");
                    return View("Import");
                }

                int importedCount = 0;
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("fr-CA"))
                {
                    Delimiter = ",",
                    HeaderValidated = null,
                    MissingFieldFound = null
                }))
                {
                    var records = csv.GetRecords<DVD>().ToList();
                    Console.WriteLine($"Nombre d'enregistrements lus : {records.Count}");
                    foreach (var record in records)
                    {
                        Console.WriteLine(record.DescriptionSupplements);
                        record.DerniereMiseAJour = DateTime.Now;
                        record.DerniereMiseAJourPar = User.Identity.Name;
                        _context.DVDs.Add(record);
                        importedCount++;
                    }
                    Console.WriteLine($"Nombre d'enregistrements ajoutés : {importedCount}");
                    await _context.SaveChangesAsync();
                }

                Console.WriteLine($"Importation terminée. {importedCount} DVDs importés.");
                TempData["Message"] = $"{importedCount} DVDs ont été importés avec succès.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'importation : {ex.Message}");
                Console.WriteLine($"StackTrace : {ex.StackTrace}");
                ModelState.AddModelError("", "Une erreur s'est produite lors de l'importation.");
                return View("Import");
            }
        }

        // méthode pour l'appropriation d'un DVD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Appropriation(int id)
        {
            var dvd = await _context.DVDs.FindAsync(id);
            if (dvd == null)
            {
                return NotFound();
            }

            dvd.UtilisateurEmprunteur = User.Identity.Name;
            dvd.DerniereMiseAJour = DateTime.Now;
            dvd.DerniereMiseAJourPar = User.Identity.Name;

            _context.Update(dvd);
            await _context.SaveChangesAsync();

            // logique pour envoyer un email

            return RedirectToAction(nameof(Index));
        }

    }
}
