using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Globalization;

namespace OPDB.Controllers
{
    public class ActividadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Actividades/

        public ActionResult Index()
        {
            List<Activity> activities = (from activity in db.Activities where activity.DeletionDate == null select activity).ToList();

            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            

            foreach (var activity in activities)
            {
                var CreateUser = db.Users.Find(activity.CreateUser);
                var UpdateUser = db.Users.Find(activity.UpdateUser);

                if (CreateUser.UserTypeID == 3 && UpdateUser.UserTypeID == 3)
                {
                    
                   activityViewModel.Information.Add(new UserInfoViewModel
                    {
                        Activity = activity,
                        CreateEntity = db.OutreachEntityDetails.First(u => u.UserID == activity.CreateUser),
                        UpdateEntity = db.OutreachEntityDetails.First(u => u.UserID == activity.UpdateUser),
                        CreateUser = new UserDetail
                        {
                            FirstName = "",
                            MiddleInitial = "",
                            LastName = "",
                        },
                        UpdateUser = new UserDetail
                        {
                            FirstName = "",
                            MiddleInitial = "",
                            LastName = ""
                        }

                    });
                }
                else if (CreateUser.UserTypeID == 3 && UpdateUser.UserTypeID != 3)
                {
                    activityViewModel.Information.Add(new UserInfoViewModel
                    {
                        Activity = activity,
                        CreateEntity = db.OutreachEntityDetails.First(u => u.UserID == activity.CreateUser),
                        UpdateUser = db.UserDetails.First(u => u.UserID == activity.UpdateUser),
                        CreateUser = new UserDetail
                        {
                            FirstName = "",
                            MiddleInitial = "",
                            LastName = "",

                        },
                        UpdateEntity = new OutreachEntityDetail
                        {
                            OutreachEntityName = ""
                        }

                    });
                }
                else if (CreateUser.UserTypeID != 3 && UpdateUser.UserTypeID == 3)
                {
                    activityViewModel.Information.Add(new UserInfoViewModel
                    {
                        Activity = activity,
                        CreateUser = db.UserDetails.First(u => u.UserID == activity.CreateUser),
                        UpdateEntity = db.OutreachEntityDetails.First(u => u.UserID == activity.UpdateUser),
                        UpdateUser = new UserDetail
                        {
                            FirstName = "",
                            MiddleInitial = "",
                            LastName = ""
                        },
                        CreateEntity = new OutreachEntityDetail
                        {
                            OutreachEntityName = ""
                        }

                    });
                }
                else
                {
                    activityViewModel.Information.Add(new UserInfoViewModel
                    {
                        Activity = activity,
                        CreateUser = db.UserDetails.First(u => u.UserID == activity.CreateUser),
                        UpdateUser = db.UserDetails.First(u => u.UserID == activity.UpdateUser),
                        CreateEntity = new OutreachEntityDetail
                        {
                            OutreachEntityName = ""
                        },
                        UpdateEntity = new OutreachEntityDetail
                        {
                            OutreachEntityName = ""
                        }

                    });
                }

            }

            return PartialView("Index", activityViewModel);
        }

        public ActionResult Lista()
        {
            var activities = (from activity in db.Activities where activity.DeletionDate == null orderby activity.UpdateDate descending select activity).ToList();

            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            foreach (var activity in activities)
            {
                activityViewModel.Information.Add(new UserInfoViewModel
                {
                    Activity = activity,
                    OutreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == activity.UserID)
                });
            }

            return View(activityViewModel);
        }

        //
        // GET: /Actividades/Details/5

        public ActionResult Detalles(int id = 0)
        {
            Activity foundActivity = db.Activities.Find(id);
            var result = (from feedback in db.Feedbacks join details in db.UserDetails on feedback.UserID equals details.UserID where feedback.ActivityID == id && feedback.DeletionDate == null select feedback).ToList();
            var list = new List<UserInfoViewModel>();

            foreach(var feedback in result){
                list.Add(new UserInfoViewModel{
                    Feedback = feedback,
                    User = db.UserDetails.Find(feedback.UserID)
                });
            }
            

            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Activity = foundActivity,
                ActivityDate = foundActivity.ActivityDate.Value.ToString("dd/MM/yyyy"),
                Feedbacks = list,
                Notes = from note in db.ActivityNotes.Include(note => note.NoteType) where note.ActivityID == id && note.DeletionDate == null select note
            };
            
            if (activityViewModel.Activity == null)
            {
                return HttpNotFound();
            }
            return View(activityViewModel);
        }

        // GET: /Actividades/Create

        public ActionResult Crear()
        {
            ActivityViewModel activityViewModel = new ActivityViewModel {
                ActivityTypes = getActivityTypes(), 
                SchoolList = getSchools()
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

                activityViewModel.Activity.UserID = 9;

                activityViewModel.Activity.UpdateDate = DateTime.Now;
                activityViewModel.Activity.CreateDate = DateTime.Now;

                activityViewModel.Activity.CreateUser = 3;
                activityViewModel.Activity.UpdateUser = 3;

                db.Activities.Add(activityViewModel.Activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activityViewModel.Activity);
        }

        // GET: /Actividades/Edit/5

        public ActionResult Editar(int id = 0)
        {
            ActivityViewModel activityViewModel = new ActivityViewModel {
                Activity = db.Activities.Find(id)
            };

            if (activityViewModel.Activity == null)
            {
                return HttpNotFound();
            }

            activityViewModel.ActivityDate = activityViewModel.Activity.ActivityDate.Value.ToString("dd/MM/yyyy");
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
                activityViewModel.Activity.ActivityDate = DateTime.ParseExact(activityViewModel.ActivityDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                activityViewModel.Activity.ActivityTime = activityViewModel.Activity.ActivityTime.Replace(" ", "");
                activityViewModel.Activity.UpdateUser = 3;
                activityViewModel.Activity.UpdateDate = DateTime.Now;
                db.Entry(activityViewModel.Activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");               

            }

            activityViewModel.ActivityTypes = getActivityTypes();
            activityViewModel.SchoolList = getSchools();

            return View(activityViewModel.Activity);
        }

        [HttpPost]
        public ActionResult EditarNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            ActivityViewModel activityViewModel = new ActivityViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                Note = db.ActivityNotes.Find(id)

            };

            return PartialView("EditarNota", activityViewModel);
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

        public ActionResult RemoverComentario(int id = 0)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            else
            {
                feedback.DeletionDate = DateTime.Now;
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Detalles", "Actividades", new { id = feedback.ActivityID });
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
            activityViewModel.Note.UpdateUser = 2;
            activityViewModel.Note.UpdateDate = DateTime.Now;
            
            if (activityViewModel.Note.ActivityNoteID == 0)
            {
                if (ModelState.IsValid)
                {
                    activityViewModel.Note.CreateDate = DateTime.Now;

                    activityViewModel.Note.UserID = 2;
                    activityViewModel.Note.CreateUser = 2;

                    db.ActivityNotes.Add(activityViewModel.Note);
                    db.SaveChanges();

                    return View("_Hack");
                }

                return Content(GetErrorsFromModelState(activityViewModel));
            }
            else if (ModelState.IsValid)
            {

                db.Entry(activityViewModel.Note).State = EntityState.Modified;
                db.SaveChanges();
                return View("_Hack");

            }

            return Content(GetErrorsFromModelState(activityViewModel));            
        }

        [HttpPost]
        public ActionResult CrearNota(int id)
        {
            EscuelasController controller = new EscuelasController();

            ActivityViewModel activityViewModel = new ActivityViewModel
            {

                NoteTypes = controller.getNoteTypes(),
                Note = new ActivityNote
                {

                    ActivityID = id
                }

            };

            return PartialView("CrearNota", activityViewModel);
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

       public String GetErrorsFromModelState(ActivityViewModel activityViewModel)
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

       [HttpPost]
       public ActionResult VerNota(int id = 0)
       {
           ActivityNote activityNote = db.ActivityNotes.Find(id);
           activityNote.NoteType = db.NoteTypes.Find(activityNote.NoteTypeID);
           activityNote.Activity = db.Activities.Find(activityNote.ActivityID);

           ActivityViewModel activityViewModel = new ActivityViewModel
           {
              Note = activityNote
           };

           if (activityViewModel.Note == null)
           {
               return HttpNotFound();
           }

           return PartialView("VerNota", activityViewModel);
       }

       [HttpPost]
       public ActionResult NuevoComentario(int id)
       {
           ActivityViewModel activityViewModel = new ActivityViewModel
           {
               Feedback = new Feedback
               {
                   ActivityID = id
               }

           };

           return PartialView("NuevoComentario", activityViewModel);
       }

       [HttpPost]
       public ActionResult EditarComentario(int id)
       {
           ActivityViewModel activityViewModel = new ActivityViewModel
           {
               Feedback = db.Feedbacks.Find(id)

           };

           return PartialView("EditarComentario", activityViewModel);
       }

       [HttpPost]
       public ActionResult GuardarComentario(ActivityViewModel activityViewModel)
       {
           activityViewModel.Feedback.UpdateUser = 2;
           activityViewModel.Feedback.UpdateDate = DateTime.Now;

           if (activityViewModel.Feedback.FeedbackID == 0)
           {
               if (ModelState.IsValid)
               {
                   activityViewModel.Feedback.CreateDate = DateTime.Now;

                   activityViewModel.Feedback.UserID = 2;
                   activityViewModel.Feedback.CreateUser = 2;

                   db.Feedbacks.Add(activityViewModel.Feedback);
                   db.SaveChanges();

                   return View("_Hack");
               }

               return Content(GetErrorsFromModelState(activityViewModel));
           }
           else if (ModelState.IsValid)
           {

               db.Entry(activityViewModel.Feedback).State = EntityState.Modified;
               db.SaveChanges();
               return View("_Hack");

           }

           return Content(GetErrorsFromModelState(activityViewModel));  
       }
    }
}