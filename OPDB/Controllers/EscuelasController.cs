using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;

namespace OPDB.Controllers
{
    public class EscuelasController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Escuelas/

        public ActionResult Index(string requested)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(requested))
                {
                    var schools = (from school in db.Schools where school.DeletionDate == null select school).ToList();

                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {
                        Information = new List<UserInfoViewModel>()
                    };

                    foreach (var school in schools)
                    {
                        schoolViewModel.Information.Add(new UserInfoViewModel
                        {
                            School = school,
                            CreateUser = db.UserDetails.First(sch => sch.UserID == school.CreateUser),
                            UpdateUser = db.UserDetails.First(sch => sch.UserID == school.UpdateUser)

                        });
                    }

                    return PartialView("Index", schoolViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult MenuEscuelas()
        {
            return PartialView("Escuelas");
        }

        //
        // GET: /Escuelas/Details/5

        /// <summary>
        /// This method is responsible for searching for the particular Notes associated with a particular school,
        /// by means of using its specific id (the parameter recevied by the method).
        /// </summary>
        /// 
        /// <param name="id">
        /// A unique integer that identifies each individual school.
        /// </param>
        /// 
        /// <returns> 
        /// Returns a schoolViewModel 
        /// </returns>

        public ActionResult Detalles(int id = 0)
        {
            SchoolViewModel schoolViewModel = new SchoolViewModel
            {
                School = db.Schools.Find(id),
                Notes = from note in db.SchoolNotes.Include(note => note.NoteType) where note.SchoolID == id && note.DeletionDate == null select note
            };

            if (schoolViewModel.School == null)
            {
                return HttpNotFound();
            }
            return View(schoolViewModel);
        }

        //
        // GET: /Escuelas/Create

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// 
        /// </returns>

        [HttpPost]
        public ActionResult PopUpCrear()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {
                        Towns = getTowns()
                    };

                    return PartialView("Crear", schoolViewModel);
                }
            }
            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // POST: /Escuelas/Create

        [HttpPost]
        public ActionResult Crear(SchoolViewModel schoolViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var existingSchool = from school in db.Schools where school.SchoolSequenceNumber == schoolViewModel.School.SchoolSequenceNumber && school.DeletionDate == null select school;

                    if (existingSchool.Count() != 0)
                        ModelState.AddModelError("", Resources.WebResources.School_SchoolSequenceNumber_Unique);

                    if (ModelState.IsValid)
                    {

                        schoolViewModel.School.UpdateDate = DateTime.Now;
                        schoolViewModel.School.CreateDate = DateTime.Now;

                        schoolViewModel.School.CreateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);
                        schoolViewModel.School.UpdateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);

                        db.Schools.Add(schoolViewModel.School);
                        db.SaveChanges();

                        return View("_Hack");
                    }
                    return Content(GetErrorsFromModelState(schoolViewModel));
                }
            }
            return RedirectToAction("AccesoDenegado", "Home");
        }

       
        [HttpPost]
        public ActionResult PopUpEditar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {
                        School = db.Schools.Find(id),
                        Towns = getTowns()
                    };

                    if (schoolViewModel.School == null)
                    {
                        return HttpNotFound();
                    }

                    return PartialView("Editar", schoolViewModel);
                }
            }
            return RedirectToAction("AccesoDenegado", "Home");
        }      

        [HttpPost]
        public ActionResult Editar(SchoolViewModel schoolViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if(Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    List<School> schools = (from school in db.Schools where school.SchoolSequenceNumber == schoolViewModel.School.SchoolSequenceNumber && school.DeletionDate == null select school).ToList();

                    School currentSchool = db.Schools.Find(schoolViewModel.School.SchoolID);

                    if (schools.Count == 1 && schools.First().SchoolID != currentSchool.SchoolID)
                        ModelState.AddModelError("", Resources.WebResources.School_SchoolSequenceNumber_Unique);

                    if (ModelState.IsValid)
                    {
                        schoolViewModel.School.UpdateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);
                        schoolViewModel.School.UpdateDate = DateTime.Now;

                        db.Entry(currentSchool).CurrentValues.SetValues(schoolViewModel.School);
                        db.SaveChanges();

                        return View("_Hack");
                    }

                    return Content(GetErrorsFromModelState(schoolViewModel));             
                }
             }
            return RedirectToAction("AccesoDenegado", "Home");      
        }
        
        [HttpPost]
        public ActionResult Remover(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 1 && Boolean.Parse(User.Identity.Name)))
                {
                    School school = db.Schools.Find(id);

                    if (school == null)
                    {
                        return HttpNotFound();
                    }

                    else
                    {
                        school.DeletionDate = DateTime.Now;
                        db.Entry(school).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Administracion", "Home", null);
                }
            }
            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult Restaurar(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    School school = db.Schools.Find(id);

                    if (school == null)
                    {
                        return HttpNotFound();
                    }

                    else
                    {
                        school.DeletionDate = null;
                        db.Entry(school).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Administracion", "Home");
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult RemoverNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var note = db.SchoolNotes.Find(id);
                    note.DeletionDate = DateTime.Now;
                    db.Entry(note).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Detalles", "Escuelas", new { id = note.SchoolID });
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult GuardarNota(SchoolViewModel schoolViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    schoolViewModel.Note.UpdateUser = Int32.Parse(User.Identity.Name.Split(',')[0]);
                    schoolViewModel.Note.UpdateDate = DateTime.Now;

                    if (schoolViewModel.Note.SchoolNoteID == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            schoolViewModel.Note.CreateDate = DateTime.Now;

                            schoolViewModel.Note.UserID = 2;
                            schoolViewModel.Note.CreateUser = 2;

                            db.SchoolNotes.Add(schoolViewModel.Note);
                            db.SaveChanges();

                            return View("_Hack");
                        }

                        return Content(GetErrorsFromModelState(schoolViewModel));
                    }
                    else if (ModelState.IsValid)
                    {

                        db.Entry(schoolViewModel.Note).State = EntityState.Modified;
                        db.SaveChanges();
                        return View("_Hack");

                    }

                    return Content(GetErrorsFromModelState(schoolViewModel)); 
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");   
        }

        [HttpPost]
        public ActionResult CrearNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {

                        NoteTypes = getNoteTypes(),
                        Note = new SchoolNote
                        {

                            SchoolID = id
                        }
                    };

                    return PartialView(schoolViewModel);
                }
            }

            return PartialView("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult VerNota(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[2]) == 2 || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {

                    SchoolNote schoolNote = db.SchoolNotes.Find(id);
                    schoolNote.NoteType = db.NoteTypes.Find(schoolNote.NoteTypeID);
                    schoolNote.School = db.Schools.Find(schoolNote.SchoolID);

                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {
                        Note = schoolNote
                    };

                    if (schoolViewModel.Note == null)
                    {
                        return HttpNotFound();
                    }

                    return PartialView("VerNota", schoolViewModel);

                }
            }       
            return PartialView("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult EditarNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {

                        NoteTypes = getNoteTypes(),
                        Note = db.SchoolNotes.Find(id)
                    };

                    return PartialView(schoolViewModel);
                }
            }

            return PartialView("AccesoDenegado", "Home");
        }

        public List<SelectListItem> getNoteTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            foreach (var type in db.NoteTypes)
            {
                types.Add(new SelectListItem()
                {

                    Text = type.NoteType1, 
                    Value = type.NoteTypeID + ""

                });
            }

            return types;
        }

        public ActionResult Lista()
        {
            var schools = db.Schools.Include(s => s.User).Include(s => s.User1);
            return View(schools.ToList());
        }

        public String GetErrorsFromModelState(SchoolViewModel schoolViewModel)
        {

            //retrieves the validation messages from the ModelState as strings    
            var str = "";
            var errorSates = from state in ModelState.Values
                             from error in state.Errors
                             select error.ErrorMessage;

            var errorList = errorSates.ToList();
            foreach (var m in errorList)
            {
                str = str + "<li>* " + m + "</li>";
            }

            return str;
        }

        public ActionResult Removidos()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    var schools = (from school in db.Schools where school.DeletionDate != null select school).ToList();

                    SchoolViewModel schoolViewModel = new SchoolViewModel
                    {
                        Information = new List<UserInfoViewModel>()
                    };

                    foreach (var school in schools)
                    {
                        schoolViewModel.Information.Add(new UserInfoViewModel
                        {
                            School = school,
                            CreateUser = db.UserDetails.First(sch => sch.UserID == school.CreateUser),
                            UpdateUser = db.UserDetails.First(sch => sch.UserID == school.UpdateUser)

                        });
                    }

                    return PartialView("Removidos", schoolViewModel);
                }
            }

           return PartialView("AccesoDenegado", "Home");
        }


        public List<SelectListItem> getTowns()
        {
            List<SelectListItem> towns = new List<SelectListItem>();

            String[] town = new String[] { "Adjuntas", "Aguada", "Aguadilla", "Aguas Buenas", "Aibonito", "Añasco",
            "Arecibo", "Arroyo", "Barceloneta", "Barranquitas", "Bayamón", "Cabo Rojo", "Caguas", "Camuy", "Canóvanas",
            "Carolina", "Cataño", "Cayey", "Ceiba", "Ciales", "Cidra", "Coamo", "Comerío", "Corozal", "Culebra", "Dorado",
            "Fajardo", "Florida", "Guánica", "Guayama", "Guayanilla", "Guaynabo", "Gurabo", "Hatillo", "Hormigueros", "Humacao",
            "Isabela", "Jayuya", "Juana Díaz", "Juncos", "Lajas", "Lares", "Las Marías", "Las Piedras", "Loíza", "Luquillo", "Manatí",
            "Maricao", "Maunabo", "Mayagüez", "Moca", "Morovis", "Naguabo", "Naranjito", "Orocovis", "Patillas", "Peñuelas", "Ponce",
            "Quebradillas", "Rincón", "Río Grande", "Sabana Grande", "Salinas", "San Germán", "San Juan", "San Lorenzo", "San Sebastián",
            "Santa Isabel", "Toa Alta", "Toa Baja", "Trujillo Alto", "Utuado", "Vega Alta", "Vega Baja", "Vieques", "Villalba", "Yabucoa",
            "Yauco"};

            for (int i = 0; i < town.Length; i++)
            {
                towns.Add(new SelectListItem()
                {
                    Text = town[i],
                    Value = town[i]
                });
            }
        
            return towns;

        }
    }
}