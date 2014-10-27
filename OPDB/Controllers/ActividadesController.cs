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
                if (activity.ActivityDate == null)
                    activity.ActivityDate = new DateTime();

                if (activity.ActivityTime == null)
                    activity.ActivityTime = "";

                var interest = (from i in db.Interests where i.UserID == 1 && i.ActivityID == activity.ActivityID select i).ToList();
                bool interested = false;

                if (interest.Count == 1)
                    interested = true;

                activityViewModel.Information.Add(new UserInfoViewModel
                {
                    Activity = activity,
                    OutreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == activity.UserID),
                    Interested = interested
                });
            }

            return View(activityViewModel);
        }

        public ActionResult DeInteres()
        {
            var interested = (from interest in db.Interests join activity in db.Activities on interest.ActivityID equals activity.ActivityID where interest.DeletionDate == null orderby activity.UpdateDate descending select interest).ToList();


            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Information = new List<UserInfoViewModel>()
            };

            foreach (var interest in interested)
            {
                var activity = db.Activities.Find(interest.ActivityID);

                activityViewModel.Information.Add(new UserInfoViewModel
                {
                    Activity = activity,
                    OutreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == activity.UserID)
                });
            }

            return View("Interes", activityViewModel);
        }

        //
        // GET: /Actividades/Details/5

        public ActionResult Detalles(int id = 0)
        {
            Activity foundActivity = db.Activities.Find(id);

            string date = "";

            if (foundActivity.ActivityDate != null) 
               date = foundActivity.ActivityDate.Value.ToString("dd/MM/yyyy");

            var allFeedback = (from feedback in db.Feedbacks where feedback.ActivityID == id && feedback.DeletionDate == null select feedback).ToList();
            var feedbackList = new List<UserInfoViewModel>();
            var allContacts = (from contact in db.Contacts where contact.ActivityID == id && contact.DeletionDate == null select contact).ToList();
            var contactList = new List<UserInfoViewModel>();
            var interest = (from i in db.Interests where i.UserID == 1 && i.ActivityID == id select i).ToList();
            bool interested = false;

            if (interest.Count == 1)
                interested = true;


            foreach (var feedback in allFeedback)
            {
                feedbackList.Add(new UserInfoViewModel
                {
                    Feedback = feedback,
                    UserDetail = db.UserDetails.First(user => user.UserID == feedback.UserID)
                });
            }

            foreach (var contact in allContacts)
            {
                contactList.Add(new UserInfoViewModel
                {
                    Contact = contact,
                    User = db.Users.Find(contact.UserID),
                    UserDetail = db.UserDetails.First(user => user.UserID == contact.UserID)
                });
            }
            
            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Activity = foundActivity,
                ActivityDate = date,
                Feedbacks = feedbackList,
                ActivityContacts = contactList,
                Notes = from note in db.ActivityNotes.Include(note => note.NoteType) where note.ActivityID == id && note.DeletionDate == null select note,
                Interested = interested,
                Videos = (from video in db.Media where video.ActivityID == id && video.MediaType == "Video" && video.DeletionDate == null select video).ToList(),
                Photos = (from photo in db.Media where photo.ActivityID == id && photo.MediaType == "Foto" && photo.DeletionDate == null select photo).ToList()
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
                SchoolList = getSchools(),
                Activity = new Activity {
                    UserID = 9
                },
                ContactIDs = new List<int>(),
                Contacts = getContacts(),
                Resources = getResources(),
                ResourceIDs = new List<int>()
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

                if (activityViewModel.ContactIDs.Count > 0)
                {
                    activityViewModel.Activity.Contacts = new List<Contact>();

                    foreach (var contact in activityViewModel.ContactIDs)
                    {
                        Contact ActivityContact = new Contact
                        {
                            UserID = contact,
                            CreateUser = 9,
                            CreateDate = DateTime.Now,
                            UpdateUser = 9,
                            UpdateDate = DateTime.Now
                        };

                        activityViewModel.Activity.Contacts.Add(ActivityContact);
                    }

                }

                if (activityViewModel.ResourceIDs.Count > 0)
                {
                    activityViewModel.Activity.ActivityResources = new List<ActivityResource>();

                    foreach (var resource in activityViewModel.ResourceIDs)
                    {
                        ActivityResource Resource = new ActivityResource
                        {
                            ResourceID = resource,
                            ResourceStatus = false,
                            CreateUser = 9,
                            CreateDate = DateTime.Now,
                            UpdateUser = 9,
                            UpdateDate = DateTime.Now
                        };

                        activityViewModel.Activity.ActivityResources.Add(Resource);
                    }

                }

                activityViewModel.Activity.CreateUser = 3;
                activityViewModel.Activity.UpdateUser = 3;

                db.Activities.Add(activityViewModel.Activity);
                db.SaveChanges();

                var outreachEntity = db.OutreachEntityDetails.First(u => u.UserID == activityViewModel.Activity.UserID);

                return RedirectToAction("Detalles", "Alcance", new { id = outreachEntity.OutreachEntityDetailID });
            }

            return View(activityViewModel);
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

            string date = "";

            if(activityViewModel.Activity.ActivityDate != null)
               activityViewModel.Activity.ActivityDate.Value.ToString("dd/MM/yyyy");

            activityViewModel.ActivityDate = date;
            activityViewModel.ActivityTypes = getActivityTypes();
            activityViewModel.SchoolList = getSchools();
            
            activityViewModel.ContactIDs = (from contact in db.Contacts where contact.ActivityID == id && contact.DeletionDate == null select contact.UserID).ToList();
            activityViewModel.Contacts = getContacts();

            activityViewModel.ResourceIDs = (from resource in db.ActivityResources where resource.ActivityID == id && resource.DeletionDate == null select resource.ResourceID).ToList();
            activityViewModel.Resources = getResources();

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
                if (activityViewModel.ActivityDate != "" && activityViewModel.ActivityDate != null)
                    activityViewModel.Activity.ActivityDate = DateTime.ParseExact(activityViewModel.ActivityDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                
                if(activityViewModel.Activity.ActivityTime != "" && activityViewModel.Activity.ActivityTime != null)
                    activityViewModel.Activity.ActivityTime = activityViewModel.Activity.ActivityTime.Replace(" ", "");
                
                activityViewModel.Activity.UpdateUser = 9;
                activityViewModel.Activity.UpdateDate = DateTime.Now;

                if (activityViewModel.ContactIDs != null) 
                { 
                    if (activityViewModel.ContactIDs.Count > 0)
                    {
                        var contacts = (from contact in db.Contacts where contact.ActivityID == activityViewModel.Activity.ActivityID && contact.DeletionDate == null select contact).ToList();

                        if (contacts.Count > 0) 
                        { 
                            foreach (var contact in contacts)
                            {
                                if (!activityViewModel.ContactIDs.Contains(contact.UserID)) 
                                { 
                                    contact.DeletionDate = DateTime.Now;
                                    db.Entry(contact).State = EntityState.Modified;
                                }
                            }
                        }

                        var removedContacts = (from contact in db.Contacts where contact.ActivityID == activityViewModel.Activity.ActivityID && contact.DeletionDate != null select contact).ToList();

                        if (removedContacts.Count > 0)
                        {
                            foreach (var contact in removedContacts)
                            {
                                if (activityViewModel.ContactIDs.Contains(contact.UserID))
                                {
                                    contact.DeletionDate = null;
                                    db.Entry(contact).State = EntityState.Modified;
                                }
                            }
                        }

                        foreach (var id in activityViewModel.ContactIDs)
                        {
                            var contact = db.Contacts.First(user => user.UserID == id);

                            if (contact == null)
                            {
                                Contact ActivityContact = new Contact
                                {
                                    UserID = id,
                                    CreateUser = 9,
                                    CreateDate = DateTime.Now,
                                    UpdateUser = 9,
                                    UpdateDate = DateTime.Now
                                };

                                activityViewModel.Activity.Contacts.Add(ActivityContact);
                            }                    
                        }
                    }
                }
                else
                {
                    var contacts = (from contact in db.Contacts where contact.ActivityID == activityViewModel.Activity.ActivityID && contact.DeletionDate == null select contact).ToList();

                    if (contacts.Count > 0)
                    {
                        foreach (var contact in contacts)
                        {                            
                            contact.DeletionDate = DateTime.Now;
                            db.Entry(contact).State = EntityState.Modified;
                        }
                    }
                }


                db.Entry(activityViewModel.Activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");               

            }

            activityViewModel.ActivityTypes = getActivityTypes();
            activityViewModel.SchoolList = getSchools();

            return View(activityViewModel);
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

       public List<SelectListItem> getOutreachEntities()
       {
           var outreachEntities = from outreach in db.OutreachEntityDetails where outreach.DeletionDate == null select outreach;
           List<SelectListItem> types = new List<SelectListItem>();

           foreach (var outreachEntity in outreachEntities)
           {
               types.Add(new SelectListItem()
               {
                   Text = outreachEntity.OutreachEntityName,
                   Value = outreachEntity.UserID + ""

               });

           }

           return types;
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

       [HttpPost]
       public ActionResult MediaUpload(int id = 0)
       {
           ActivityViewModel activityViewModel = new ActivityViewModel
           {
               MediaTypes = getMediaTypes(),
               Media = new Medium
               {
                   ActivityID = id
               }
           };

           return PartialView("Upload", activityViewModel);

       }

       public List<SelectListItem> getMediaTypes()
       {
           String[] mediaTypes = new String[] { "Video", "Foto"};

           List<SelectListItem> types = new List<SelectListItem>();

           for(int i = 0; i < mediaTypes.Length; i ++)
           {
               types.Add(new SelectListItem()
               {
                   Text = mediaTypes[i],
                   Value = mediaTypes[i]
               });
           }

           return types;


       }

       [HttpPost]
       public ActionResult Upload(ActivityViewModel activityViewModel)
       {

           activityViewModel.Media.UpdateDate = DateTime.Now;

           //To be changed with login implementation.
           activityViewModel.Media.UpdateUser = 1;

           if (activityViewModel.Media.MediaID == 0)
           {
               if (ModelState.IsValid) 
               { 
                   //To be changed with login implementation.
                   activityViewModel.Media.CreateUser = 1;
                                             
                   activityViewModel.Media.CreateDate = DateTime.Now;

                   db.Media.Add(activityViewModel.Media);
                   db.SaveChanges();

                   return View("_Hack");
               }
               else
               {
                   return Content(GetErrorsFromModelState(activityViewModel));
               }
           }
           else
           {
                if (ModelState.IsValid) 
                   { 
                       db.Entry(activityViewModel.Media).State = EntityState.Modified;
                       db.SaveChanges();
                       return View("_Hack");
                   }
                   else
                   {
                       return Content(GetErrorsFromModelState(activityViewModel));
                   }
               }
           }

           [HttpPost]
           public ActionResult Interes(int id)
           {
               Interest Interest = new Interest
               {
                   ActivityID = id,
                   UserID = 1,
                   CreateUser = 1,
                   CreateDate = DateTime.Now,
                   UpdateUser = 1,
                   UpdateDate = DateTime.Now
                   
               };

               db.Interests.Add(Interest);
               db.SaveChanges();

               return Content("");               
           }

           public List<SelectListItem> getContacts()
           {
               var users = (from user in db.Users where (user.UserTypeID == 4 || user.UserTypeID == 5) && user.DeletionDate == null select user).ToList();

               List<SelectListItem> types = new List<SelectListItem>();

               if (users.Count > 0) { 
                   foreach (var user in users)
                   {
                       var userDetail = db.UserDetails.FirstOrDefault(u => u.UserID == user.UserID);

                       types.Add(new SelectListItem()
                       {
                           Text = userDetail.FirstName + " " + userDetail.MiddleInitial + " " + userDetail.LastName,
                           Value = user.UserID + "",
                       });
                   }
               }

               return types;


           }

           public List<SelectListItem> getResources()
           {
               var resources = (from resource in db.Resources where resource.DeletionDate == null select resource).ToList();

               List<SelectListItem> types = new List<SelectListItem>();

               if (resources.Count > 0)
               {
                   foreach (var resource in resources)
                   {
                      types.Add(new SelectListItem()
                       {
                           Text = resource.Resource1,
                           Value = resource.ResourceID + "",
                       });
                   }
               }

               return types;


           }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Admin activity creation methods.
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

       public ActionResult CrearActividad()
       {
           ActivityViewModel activityViewModel = new ActivityViewModel
           {
               ActivityTypes = getActivityTypes(),
               SchoolList = getSchools(),
               OutreachEntities = getOutreachEntities(),
               Contacts = getContacts(),
               ContactIDs = new List<int>()
           };

         
           

           return View(activityViewModel);
       }

       [HttpPost]
       public ActionResult CrearActividad(ActivityViewModel activityViewModel)
       {
           if (ModelState.IsValid)
           {
               //TODO needs to acquire current user 
               activityViewModel.Activity.UpdateDate = DateTime.Now;
               activityViewModel.Activity.CreateDate = DateTime.Now;

               if (activityViewModel.ContactIDs.Count > 0)
               {
                   activityViewModel.Activity.Contacts = new List<Contact>();

                   foreach (var contact in activityViewModel.ContactIDs)
                   {
                       Contact ActivityContact = new Contact
                       {
                           UserID = contact,
                           CreateUser = 9,
                           CreateDate = DateTime.Now,
                           UpdateUser = 9,
                           UpdateDate = DateTime.Now
                       };

                       activityViewModel.Activity.Contacts.Add(ActivityContact);
                   }

               }

               if (activityViewModel.ResourceIDs.Count > 0)
               {
                   activityViewModel.Activity.ActivityResources = new List<ActivityResource>();

                   foreach (var resource in activityViewModel.ResourceIDs)
                   {
                       ActivityResource Resource = new ActivityResource
                       {
                           ResourceID = resource,
                           ResourceStatus = false,
                           CreateUser = 9,
                           CreateDate = DateTime.Now,
                           UpdateUser = 9,
                           UpdateDate = DateTime.Now
                       };

                       activityViewModel.Activity.ActivityResources.Add(Resource);
                   }

               }

               activityViewModel.Activity.CreateUser = 3;
               activityViewModel.Activity.UpdateUser = 3;

               db.Activities.Add(activityViewModel.Activity);
               db.SaveChanges();
               return RedirectToAction("Index");
           }

           activityViewModel.ActivityTypes = getActivityTypes();
           activityViewModel.SchoolList = getSchools();
           activityViewModel.OutreachEntities = getOutreachEntities();

           return View(activityViewModel);
       }

       public ActionResult EditarActividad(int id = 0)
       {
           ActivityViewModel activityViewModel = new ActivityViewModel
           {
               Activity = db.Activities.Find(id)
           };

           if (activityViewModel.Activity == null)
           {
               return HttpNotFound();
           }

           activityViewModel.ActivityDate = activityViewModel.Activity.ActivityDate.Value.ToString("dd/MM/yyyy");
           activityViewModel.ActivityTypes = getActivityTypes();
           activityViewModel.SchoolList = getSchools();
           activityViewModel.OutreachEntities = getOutreachEntities();

           return View(activityViewModel);
       }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult EditarActividad(ActivityViewModel activityViewModel)
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
           activityViewModel.OutreachEntities = getOutreachEntities();

           return View(activityViewModel);
       }

       
    }
}