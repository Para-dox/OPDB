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
    public class ActividadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Actividades/

        public ActionResult Index()
        {
            var activities = from a in db.Activities.Include(a => a.ActivityType).Include(a => a.School).Include(a => a.User).Include(a => a.User1).Include(a => a.User2) where a.DeletionDate == null select a;
            return View(activities.ToList());
        }

        //
        // GET: /Actividades/Details/5

        public ActionResult Detalles(int id = 0)
        {
            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                activity = db.Activities.Find(id),
                Feedbacks = from feedback in db.Feedbacks where feedback.ActivityID == id && feedback.DeletionDate == null select feedback,
                Notes = from note in db.ActivityNotes.Include(note => note.NoteType) where note.ActivityID == id && note.DeletionDate == null select note
            };
            
            if (activityViewModel.activity == null)
            {
                return HttpNotFound();
            }
            return View(activityViewModel);
        }

        // GET: /Actividades/Create

        public ActionResult Crear()
        {
            ActivityViewModel activityViewModel = new ActivityViewModel {
                ActivityTypes = getActivityTypes(), SchoolList = getSchools()
            };

            return View(activityViewModel);
        }

        // POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ActivityViewModel activityViewModel)
        {
            if (ModelState.IsValid)
            {
                //TODO needs to acquire current user 

                activityViewModel.activity.UserID = 3;

                activityViewModel.activity.UpdateDate = DateTime.Now;
                activityViewModel.activity.CreateDate = DateTime.Now;

                activityViewModel.activity.CreateUser = 3;
                activityViewModel.activity.UpdateUser = 3;

                db.Activities.Add(activityViewModel.activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activityViewModel.activity);
        }

        // GET: /Actividades/Edit/5

        public ActionResult Editar(int id = 0)
        {
            ActivityViewModel activityViewModel = new ActivityViewModel {
                activity = db.Activities.Find(id)
            };
            if (activityViewModel.activity == null)
            {
                return HttpNotFound();
            }
            
            activityViewModel.ActivityTypes = getActivityTypes();
            activityViewModel.SchoolList = getSchools();

            return View(activityViewModel);
        }

        //
        // POST: /Actividades/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ActivityViewModel activityViewModel)
        {
            if (ModelState.IsValid)
            {
                //TODO acquire current user
                activityViewModel.activity.UpdateUser = 3;
                activityViewModel.activity.UpdateDate = DateTime.Now;
                db.Entry(activityViewModel.activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            activityViewModel.ActivityTypes = getActivityTypes();
            activityViewModel.SchoolList = getSchools();

            return View(activityViewModel.activity);
        }

        //
        // GET: /Actividades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            else
            {
                activity.DeletionDate = DateTime.Now;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(activity);
        }

        public ActionResult RemoverNota(int id)
        {
            var note = db.Activities.Find(id);
            note.DeletionDate = DateTime.Now;
            db.Entry(note).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Detalles", "Actividades", new { id = note.ActivityID });
        }

        //
        // POST: /Actividades/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GuardarNota(ActivityViewModel activityViewModel)
        {
            var id = activityViewModel.activity.ActivityID;
            activityViewModel.note.CreateDate = DateTime.Now;
            activityViewModel.note.UpdateDate = DateTime.Now;
            activityViewModel.note.ActivityID = activityViewModel.activity.ActivityID;

            activityViewModel.note.CreateUser = 2;
            activityViewModel.note.UpdateUser = 2;
            activityViewModel.note.UserID = 2;

            db.ActivityNotes.Add(activityViewModel.note);
            db.SaveChanges();

            return RedirectToAction("Detalles", "Actividades", new {id = activityViewModel.activity.ActivityID});
        }

        public ActionResult CrearNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            ActivityViewModel activityViewModel = new ActivityViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                activity = new Activity
                {

                    ActivityID = id
                }
            };

            return View(activityViewModel);
        }

       protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

       public List<SelectListItem> getActivityTypes()
       {
           List<SelectListItem> types = new List<SelectListItem>();
           foreach (var activityType in db.ActivityTypes)
           {
               types.Add(new SelectListItem()
                {
                    Text = activityType.ActivityType1,
                    Value = activityType.ActivityTypeID + ""

                });

           }

           return types;
       }

       public List<SelectListItem> getSchools()
       {
           List<SelectListItem> schoolList = new List<SelectListItem>();

           schoolList.Add(new SelectListItem()
           {
               Text = null,
               Value = null
           });

           foreach (var school in db.Schools)
           {
               schoolList.Add(new SelectListItem()
               {
                   Text = school.SchoolName,
                   Value = school.SchoolID + ""
               });
           }

           return schoolList;
       }
    }
}