using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPDB.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OPDB.Controllers
{
    public class ActividadesController : Controller
    {
        private OPDBEntities db = new OPDBEntities();

        //
        // GET: /Actividades/

        public ActionResult Index(string requested)
        {
            if (requested != null) 
            { 
                if (User.Identity.IsAuthenticated && Boolean.Parse(requested)) {

                    if (Int32.Parse(User.Identity.Name.ToString().Split(',')[1]) == 1) 
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
                    else
                    {
                        return RedirectToAction("AccesoDenegado", "Home");
                    }
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult Lista()
        {
            var activities = (from activity in db.Activities where activity.DeletionDate == null orderby activity.UpdateDate descending select activity).ToList();

            User currentUser = null;

            if (User.Identity.IsAuthenticated)
            {
                int currentUserID = Int32.Parse(User.Identity.Name.Split(',')[1]);
                currentUser = db.Users.Find(currentUserID);
            }

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

                List<OPDB.Models.Interest> interest = new List<OPDB.Models.Interest>();

                if (currentUser != null)
                {
                    interest = (from i in db.Interests where i.UserID == currentUser.UserID && i.ActivityID == activity.ActivityID select i).ToList();
                }

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

            User currentUser = null;

            if (User.Identity.IsAuthenticated)
                currentUser = db.Users.Find(Int32.Parse(User.Identity.Name.Split(',')[1]));

            string date = "";

            if (foundActivity.ActivityDate != null) 
               date = foundActivity.ActivityDate.Value.ToString("dd/MM/yyyy");

            var allFeedback = (from feedback in db.Feedbacks where feedback.ActivityID == id && feedback.DeletionDate == null select feedback).ToList();
            var feedbackList = new List<UserInfoViewModel>();
            
            var interest = new List<Interest>();

            if(currentUser != null)
                interest = (from i in db.Interests where i.UserID == currentUser.UserID && i.ActivityID == id select i).ToList();

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

            
            
            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                User = db.Users.Find(foundActivity.UserID),
                OutreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == foundActivity.UserID),
                Activity = foundActivity,
                ActivityDate = date,
                Feedbacks = feedbackList,
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

        public ActionResult Crear(int id)
        {
            ActivityViewModel activityViewModel = new ActivityViewModel {
                ActivityTypes = getActivityTypes(), 
                SchoolList = getSchools(),
                Activity = new Activity {
                    UserID = id
                },
                ContactIDs = new List<int>(),
                Contacts = getContacts(),
                Resources = getResources(),
                ResourceIDs = new List<int>(),
                Information = new List<UserInfoViewModel>()
            };

            return View(activityViewModel);
        }

        // POST

        [HttpPost]
        public ActionResult Crear(ActivityViewModel activityViewModel)
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) 
                {
                    int id = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    if (activityViewModel.Activity.ActivityDate != null)
                    {
                        if(activityViewModel.Activity.ActivityDate.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                            ModelState.AddModelError("Activity_ActivityDate_EarlierThanCurrentDate", Resources.WebResources.Activity_ActivityDate_EarlierThanCurrentDate);
                    }

                    if (activityViewModel.Activity.Details != null && activityViewModel.Activity.Details != "")
                    {
                        string pattern = @"^<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(activityViewModel.Activity.Details);
                
                        if (matches.Count > 0)
                            ModelState.AddModelError("Activity_Details_Invalid", Resources.WebResources.Activity_Details_Invalid);
                
                    }

                    if (ModelState.IsValid)
                    {
                        //TODO needs to acquire current user 

                        activityViewModel.Activity.UserID = id;

                        activityViewModel.Activity.UpdateDate = DateTime.Now;
                        activityViewModel.Activity.CreateDate = DateTime.Now;

                        if (activityViewModel.ContactIDs != null)
                        {
                            if (activityViewModel.ContactIDs.Count > 0 && activityViewModel.ContactIDs.First() != 0)
                            {
                                activityViewModel.Activity.Contacts = new List<Contact>();

                                foreach (var contact in activityViewModel.ContactIDs)
                                {
                                    Contact ActivityContact = new Contact
                                    {
                                        UserID = contact,
                                        CreateUser = id,
                                        CreateDate = DateTime.Now,
                                        UpdateUser = id,
                                        UpdateDate = DateTime.Now
                                    };

                                    activityViewModel.Activity.Contacts.Add(ActivityContact);
                                }

                            }
                        }
                        if (activityViewModel.ResourceIDs != null) 
                        { 
                            if (activityViewModel.ResourceIDs.Count > 0 && activityViewModel.ResourceIDs.First() != 0)
                            {
                                activityViewModel.Activity.ActivityResources = new List<ActivityResource>();

                                foreach (var resource in activityViewModel.ResourceIDs)
                                {
                                    ActivityResource Resource = new ActivityResource
                                    {
                                        ResourceID = resource,
                                        ResourceStatus = false,
                                        CreateUser = id,
                                        CreateDate = DateTime.Now,
                                        UpdateUser = id,
                                        UpdateDate = DateTime.Now
                                    };

                                    activityViewModel.Activity.ActivityResources.Add(Resource);
                                }

                            }
                        }

                        activityViewModel.Information = CheckForConflicts(activityViewModel.Activity);
                        if (activityViewModel.Information.Count == 0 || activityViewModel.ForceCreate == true) 
                        {
                            activityViewModel.Activity.CreateUser = id;
                            activityViewModel.Activity.UpdateUser = id;

                            db.Activities.Add(activityViewModel.Activity);
                            db.SaveChanges();

                            return RedirectToAction("Detalles", "Alcance", new { id = activityViewModel.Activity.UserID });
                        }
                        else
                        {
                            activityViewModel.Action = "Crear";
                            return View("Conflictos", activityViewModel);
                        }
                    }

                    activityViewModel.ActivityTypes = getActivityTypes();
                    activityViewModel.SchoolList = getSchools();
                    activityViewModel.Contacts = getContacts();
                    activityViewModel.Resources = getResources();
                    return View(activityViewModel);
                }
            }
                
            return RedirectToAction("AccesoDenegado", "Home");
          
        }

        public ActionResult ContactosyRecursos(int id)
        {
            var allContacts = (from contact in db.Contacts where contact.ActivityID == id && contact.DeletionDate == null select contact).ToList();
            var contactList = new List<UserInfoViewModel>();

            var allResources = (from resource in db.ActivityResources where resource.ActivityID == id && resource.DeletionDate == null select resource).ToList();
            var resourceList = new List<UserInfoViewModel>();

            foreach (var contact in allContacts)
            {
                contactList.Add(new UserInfoViewModel
                {
                    Contact = contact,
                    User = db.Users.Find(contact.UserID),
                    UserDetail = db.UserDetails.First(user => user.UserID == contact.UserID)
                });
            }

            foreach (var resource in allResources)
            {
                var tempResource = db.Resources.Find(resource.ResourceID);

                var unit = db.Units.Find(tempResource.UnitID);
                    
                resourceList.Add(new UserInfoViewModel
                {
                    ActivityResource = resource,
                    Resource = tempResource,
                    Unit = unit,
                    
                });
            }

            ActivityViewModel activityViewModel = new ActivityViewModel{
                Activity = db.Activities.Find(id),
                ActivityContacts = contactList,
                ActivityResources = resourceList
            
            };


            return PartialView("ContactosyRecursos", activityViewModel);
        }


        public ActionResult Notas(int id)
        {
            ActivityViewModel activityViewModel = new ActivityViewModel
            {
                Activity = db.Activities.Find(id),
                Notes = from note in db.ActivityNotes.Include(note => note.NoteType) where note.ActivityID == id && note.DeletionDate == null select note
            };

            return PartialView("Notas", activityViewModel);
        }

        // GET: /Actividades/Edit/5

        public ActionResult Editar(int id = 0)
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) 
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
            }
                
            return RedirectToAction("AccesoDenegado", "Home");
            

        }

        //
        // POST: /Actividades/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ActivityViewModel activityViewModel)
        {
            if (User.Identity.IsAuthenticated) 
            {

                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name)) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1) 
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    if (activityViewModel.ActivityDate != "" && activityViewModel.ActivityDate != null)
                        activityViewModel.Activity.ActivityDate = DateTime.ParseExact(activityViewModel.ActivityDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (activityViewModel.Activity.ActivityDate != null)
                    {
                        if (activityViewModel.Activity.ActivityDate.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                            ModelState.AddModelError("Activity_ActivityDate_EarlierThanCurrentDate", Resources.WebResources.Activity_ActivityDate_EarlierThanCurrentDate);
                    }

                    if (activityViewModel.Activity.Details != null && activityViewModel.Activity.Details != "")
                    {
                        string pattern = @"^<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>$";
                        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                        MatchCollection matches = rgx.Matches(activityViewModel.Activity.Details);

                        if (matches.Count > 0)
                            ModelState.AddModelError("Activity_Details_Invalid", Resources.WebResources.Activity_Details_Invalid);

                    }

                    if (ModelState.IsValid)
                    {
                        if(activityViewModel.Activity.ActivityTime != "" && activityViewModel.Activity.ActivityTime != null)
                            activityViewModel.Activity.ActivityTime = activityViewModel.Activity.ActivityTime.Replace(" ", "");

                        activityViewModel.Activity.UpdateUser = userID;
                        activityViewModel.Activity.UpdateDate = DateTime.Now;

                        ///Contacts///
                
                        ///Check if ID list is null, because if it is BOOM!
                        if (activityViewModel.ContactIDs != null) 
                        { 
                            //Now check if it contains anything.
                            if (activityViewModel.ContactIDs.Count > 0 && activityViewModel.ContactIDs.First() != 0)
                            {
                                //Retrieve any pre-existing contacts.
                                var contacts = (from contact in db.Contacts where contact.ActivityID == activityViewModel.Activity.ActivityID select contact).ToList();

                                //Verify if any pre-existing contacts exist.
                                if (contacts.Count > 0) 
                                { 
                                    foreach (var contact in contacts)
                                    {
                                        //If there are existing contacts, verify if these contacts are contained in the new id list.
                                        if (contact.DeletionDate == null && !activityViewModel.ContactIDs.Contains(contact.UserID)) 
                                        { 
                                            //If not delete them.
                                            contact.DeletionDate = DateTime.Now;
                                            db.Entry(contact).State = EntityState.Modified;
                                        }
                                        //Else if they have been removed but are included in the ID list restore them.
                                        else if (contact.DeletionDate != null && activityViewModel.ContactIDs.Contains(contact.UserID))
                                        {
                                            contact.DeletionDate = null;
                                            db.Entry(contact).State = EntityState.Modified;

                                        }
                                    }
                                }
                                                
                                //Now to take care of any new contacts.
                                foreach (var id in activityViewModel.ContactIDs)
                                {
                                    //Retrieve all existing contacts, those with deletion date and without that match the id in the list.
                                    var contactList = (from cont in db.Contacts where cont.ActivityID == activityViewModel.Activity.ActivityID && cont.UserID == id select cont).ToList();

                                    Contact contact = null;

                                    //Is the list empty?
                                    if (contactList.Count > 0)
                                        contact = contactList.First();

                                    //If so that means it's new so we must add it.
                                    if (contact == null)
                                    {
                                        Contact ActivityContact = new Contact
                                        {
                                            UserID = id,
                                            ActivityID = activityViewModel.Activity.ActivityID,
                                            CreateUser = userID,
                                            CreateDate = DateTime.Now,
                                            UpdateUser = userID,
                                            UpdateDate = DateTime.Now
                                        };

                                        //Since the activity already exists, we just add the new contact to the Contacts DB table.
                                        db.Contacts.Add(ActivityContact);
                                    }                    
                                }
                            }
                        }
                        else
                        {
                            //If none of the above things happen, verify if there are any existing contacts and remove them, 
                            //because all contacts may have been removed.
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

                        ////Resources////
                
                        //Check if the ID list is null, because if so BOOM!
                        if (activityViewModel.ResourceIDs != null)
                        {
                            //Now, is it empty?
                            if (activityViewModel.ResourceIDs.Count > 0 && activityViewModel.ResourceIDs.First() != 0)
                            {
                                //Retrieve all existing resources that match this activity ID.
                                var resources = (from resource in db.ActivityResources where resource.ActivityID == activityViewModel.Activity.ActivityID select resource).ToList();

                                //Are there any matches?
                                if (resources.Count > 0)
                                {
                                    foreach (var resource in resources)
                                    {
                                        //If they are not contained in the current list, remove them.
                                        if (resource.DeletionDate == null && !activityViewModel.ResourceIDs.Contains(resource.ResourceID))
                                        {
                                            resource.DeletionDate = DateTime.Now;
                                            db.Entry(resource).State = EntityState.Modified;
                                        }
                                        //If they have been removed but are now restored.
                                        else if(resource.DeletionDate != null && activityViewModel.ResourceIDs.Contains(resource.ResourceID))
                                        {
                                            //Then restore them in the DB as well.
                                            resource.DeletionDate = null;
                                            db.Entry(resource).State = EntityState.Modified;
                                        }
                                    }
                                }

                        
                                //Now for all the remaining resource IDs
                                foreach (var id in activityViewModel.ResourceIDs)
                                {
                                    //Verify if a resource already exists with this ID.
                                    var resourceList = (from activityResource in db.ActivityResources where activityResource.ActivityID == activityViewModel.Activity.ActivityID && activityResource.ResourceID == id select activityResource).ToList();

                                    ActivityResource resource = null;

                                    if (resourceList.Count > 0)
                                        resource = resourceList.First();

                                    //If it's new, then add it.
                                    if (resource == null)
                                    {
                                        ActivityResource ActivityResource = new ActivityResource
                                        {
                                            ResourceID = id,
                                            ActivityID = activityViewModel.Activity.ActivityID,
                                            ResourceStatus = false,
                                            CreateUser = userID,
                                            CreateDate = DateTime.Now,
                                            UpdateUser = userID,
                                            UpdateDate = DateTime.Now
                                        };

                                        //This activity already exists so we just add this resource to the ActivityResource DB table.
                                        db.ActivityResources.Add(ActivityResource);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Check if any resources exist, and if they do, they may have been removed.
                            var resources = (from resource in db.ActivityResources where resource.ActivityID == activityViewModel.Activity.ActivityID && resource.DeletionDate == null select resource).ToList();

                            if (resources.Count > 0)
                            {
                                foreach (var resource in resources)
                                {
                                    //So remove them all.
                                    resource.DeletionDate = DateTime.Now;
                                    db.Entry(resource).State = EntityState.Modified;
                                }
                            }
                        }

                        activityViewModel.Information = CheckForConflicts(activityViewModel.Activity);
                        if (activityViewModel.Information.Count == 0 || activityViewModel.ForceCreate == true)
                        {
                            db.Entry(activityViewModel.Activity).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Detalles", "Alcance", new { id = activityViewModel.Activity.UserID });
                        }
                        else
                        {
                            activityViewModel.Action = "Editar";
                            return View("Conflictos", activityViewModel);
                        }
            

                    }

                    activityViewModel.ActivityTypes = getActivityTypes();
                    activityViewModel.SchoolList = getSchools();
                    activityViewModel.Contacts = getContacts();
                    activityViewModel.Resources = getResources();
                    return View(activityViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult EditarNota(int id)
        {
            if (User.Identity.IsAuthenticated) 
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1) 
                { 
                    EscuelasController controller = new EscuelasController();

                    ActivityViewModel activityViewModel = new ActivityViewModel
                    {

                        NoteTypes = controller.getNoteTypes(),
                        Note = db.ActivityNotes.Find(id)

                    };

                    return PartialView("EditarNota", activityViewModel);
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        //
        // GET: /Actividades/Delete/5

        public ActionResult Remover(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                Activity activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }
                else if (Int32.Parse(User.Identity.Name.Split(',')[0]) == activity.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    activity.DeletionDate = DateTime.Now;
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (userID == activity.UserID)
                        return RedirectToAction("Detalles", "Alcance", new { id = activity.UserID });
                    else
                        return RedirectToAction("Administracion", "Home");
                }
                    
               
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult RemoverNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var note = db.ActivityNotes.Find(id);
                
                if(note == null)
                    return HttpNotFound();

                else if (Int32.Parse(User.Identity.Name.Split(',')[0]) == note.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    note.DeletionDate = DateTime.Now;
                    db.Entry(note).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Detalles", "Actividades", new { id = note.ActivityID });
                }
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        public ActionResult RemoverComentario(int id = 0)
        {
            if (User.Identity.IsAuthenticated)
            {                
                Feedback feedback = db.Feedbacks.Find(id);                
                if (feedback == null)
                {
                    return HttpNotFound();
                }

                else if(Int32.Parse(User.Identity.Name.Split(',')[0]) == feedback.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    feedback.DeletionDate = DateTime.Now;
                    db.Entry(feedback).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Detalles", "Actividades", new { id = feedback.ActivityID });
                }
                
              }

                return RedirectToAction("AccesoDenegado", "Home");
            }
        

        [HttpPost]
        public ActionResult GuardarNota(ActivityViewModel activityViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                {
                    int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                    activityViewModel.Note.UpdateUser = userID;
                    activityViewModel.Note.UpdateDate = DateTime.Now;

                    if (activityViewModel.Note.ActivityNoteID == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            activityViewModel.Note.CreateDate = DateTime.Now;

                            activityViewModel.Note.UserID = userID;
                            activityViewModel.Note.CreateUser = userID;

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
            }

            return RedirectToAction("AccesoDenegado", "Home");
        }

        [HttpPost]
        public ActionResult CrearNota(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
            }

            return RedirectToAction("AccesoDenegado", "Home");
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
           if (User.Identity.IsAuthenticated)
           {
               if ((Int32.Parse(User.Identity.Name.Split(',')[1]) == 3 && Boolean.Parse(User.Identity.Name.Split(',')[2])) || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
           }

           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       public ActionResult NuevoComentario(int id)
       {
           if (User.Identity.IsAuthenticated)
           {
               var activity = db.Activities.Find(id);

               if (activity == null)
                   return HttpNotFound();

               else
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
           }

           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       public ActionResult EditarComentario(int id)
       {
           if (User.Identity.IsAuthenticated)
           {
               ActivityViewModel activityViewModel = new ActivityViewModel
               {
                   Feedback = db.Feedbacks.Find(id)

               };

               if (activityViewModel.Feedback == null)
                   return HttpNotFound();

               else if (Int32.Parse(User.Identity.Name.Split(',')[0]) == activityViewModel.Feedback.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
               {
                   return PartialView("EditarComentario", activityViewModel);
               }
           }

           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       public ActionResult GuardarComentario(ActivityViewModel activityViewModel)
       {
           if (User.Identity.IsAuthenticated)
           {
               
                   int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                   activityViewModel.Feedback.UpdateUser = userID;
                   activityViewModel.Feedback.UpdateDate = DateTime.Now;

                   if (activityViewModel.Feedback.FeedbackID == 0)
                   {
                       if (ModelState.IsValid)
                       {
                           activityViewModel.Feedback.CreateDate = DateTime.Now;

                           activityViewModel.Feedback.UserID = userID;
                           activityViewModel.Feedback.CreateUser = userID;

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

           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       public ActionResult MediaUpload(int id = 0)
       {
           if (User.Identity.IsAuthenticated)
           {
               var foundActivity = (from activity in db.Activities where activity.DeletionDate == null && activity.ActivityID == id select activity).ToList();

               if (foundActivity.Count() == 0)
                   return HttpNotFound();

               if (Int32.Parse(User.Identity.Name.Split(',')[0]) == foundActivity.First().UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
           }

           return RedirectToAction("AccesoDenegado", "Home");

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
               if (User.Identity.IsAuthenticated)
               {

                   if (Int32.Parse(User.Identity.Name.Split(',')[0]) == activityViewModel.Activity.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                   {
                       int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                       activityViewModel.Media.UpdateDate = DateTime.Now;

                       //To be changed with login implementation.
                       activityViewModel.Media.UpdateUser = userID;

                       if (activityViewModel.Media.MediaID == 0)
                       {
                           if (ModelState.IsValid)
                           {
                               //To be changed with login implementation.
                               activityViewModel.Media.CreateUser = userID;

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
               }
               return RedirectToAction("AccesoDenegado", "Home");
           }

           [HttpPost]
           public ActionResult Interes(int id)
           {
               if (User.Identity.IsAuthenticated) 
               {
                   int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                   Interest Interest = new Interest
                   {
                       ActivityID = id,
                       UserID = userID,
                       CreateUser = userID,
                       CreateDate = DateTime.Now,
                       UpdateUser = userID,
                       UpdateDate = DateTime.Now
                   
                   };

                   db.Interests.Add(Interest);
                   db.SaveChanges();
                   return Content("");
               }
               else
               {
                   return RedirectToAction("AccesoDenegado", "Home");
               }
                            
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

        [HttpPost]
           public ActionResult Aprobar(int id)
           {
               if (User.Identity.IsAuthenticated)
               {
                   var resource = db.ActivityResources.Find(id);

                   if (resource == null)
                       return HttpNotFound();

                   var activity = db.Activities.First(a => a.ActivityID == resource.ActivityID);

                   if (Int32.Parse(User.Identity.Name.Split(',')[0]) == activity.UserID || Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
                   {
                       int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                       resource.ResourceStatus = true;
                       resource.UpdateDate = DateTime.Now;

                       resource.UpdateUser = userID;

                       db.Entry(resource).State = EntityState.Modified;
                       db.SaveChanges();

                       return RedirectToAction("Detalles", "Actividades", new { id = resource.ActivityID });
                   }

                  
               }

               return RedirectToAction("AccesoDenegado", "Home");

           }

            private List<UserInfoViewModel> CheckForConflicts(Activity createdActivity)
            {
                List<UserInfoViewModel> conflictingActivities = new List<UserInfoViewModel>();

                var activities = (from activity in db.Activities where activity.DeletionDate == null select activity).ToList();

                if (createdActivity.ActivityDate != null && createdActivity.ActivityTime != null && createdActivity.SchoolID != null) 
                { 
                    foreach (var activity in activities)
                    {
                        if (activity.ActivityDate != null && activity.ActivityTime != null && activity.SchoolID != null) 
                        {
                            if (activity.ActivityDate.Value.Date.Equals(createdActivity.ActivityDate.Value.Date) && activity.ActivityTime == createdActivity.ActivityTime && activity.SchoolID == createdActivity.SchoolID) 
                            { 
                                conflictingActivities.Add(new UserInfoViewModel { 
                                    Activity = activity,
                                    OutreachEntity = db.OutreachEntityDetails.First(outreach => outreach.UserID == activity.UserID)
                                });
                            }
                        }
                    }
                }
                return conflictingActivities;
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Admin activity creation methods.
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

       public ActionResult CrearActividad()
       {
           if (User.Identity.IsAuthenticated)
           {
               if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
               {
                   ActivityViewModel activityViewModel = new ActivityViewModel
                   {
                       ActivityTypes = getActivityTypes(),
                       SchoolList = getSchools(),
                       OutreachEntities = getOutreachEntities(),
                       Contacts = getContacts(),
                       ContactIDs = new List<int>(),
                       Resources = getResources(),
                       ResourceIDs = new List<int>()
                   };

                   return View(activityViewModel);
               }
           }

           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       public ActionResult CrearActividad(ActivityViewModel activityViewModel)
       {
           if (User.Identity.IsAuthenticated)
           {
               if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
               {
                   int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                   if (activityViewModel.Activity.ActivityDate != null)
                   {
                       if (activityViewModel.Activity.ActivityDate.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                           ModelState.AddModelError("Activity_ActivityDate_EarlierThanCurrentDate", Resources.WebResources.Activity_ActivityDate_EarlierThanCurrentDate);
                   }

                   if (activityViewModel.Activity.Details != null && activityViewModel.Activity.Details != "")
                   {
                       string pattern = @"^<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>$";
                       Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                       MatchCollection matches = rgx.Matches(activityViewModel.Activity.Details);

                       if (matches.Count > 0)
                           ModelState.AddModelError("Activity_Details_Invalid", Resources.WebResources.Activity_Details_Invalid);

                   }

                   if (ModelState.IsValid)
                   {
                       //TODO needs to acquire current user 
                       activityViewModel.Activity.UpdateDate = DateTime.Now;
                       activityViewModel.Activity.CreateDate = DateTime.Now;

                       if (activityViewModel.ContactIDs != null)
                       {
                           if (activityViewModel.ContactIDs.Count > 0 && activityViewModel.ContactIDs.First() != 0)
                           {
                               activityViewModel.Activity.Contacts = new List<Contact>();

                               foreach (var contact in activityViewModel.ContactIDs)
                               {
                                   Contact ActivityContact = new Contact
                                   {
                                       UserID = contact,
                                       CreateUser = userID,
                                       CreateDate = DateTime.Now,
                                       UpdateUser = userID,
                                       UpdateDate = DateTime.Now
                                   };

                                   activityViewModel.Activity.Contacts.Add(ActivityContact);
                               }

                           }
                       }

                       if (activityViewModel.ResourceIDs != null)
                       {
                           if (activityViewModel.ResourceIDs.Count > 0 && activityViewModel.ResourceIDs.First() != 0)
                           {
                               activityViewModel.Activity.ActivityResources = new List<ActivityResource>();

                               foreach (var resource in activityViewModel.ResourceIDs)
                               {
                                   ActivityResource Resource = new ActivityResource
                                   {
                                       ResourceID = resource,
                                       ResourceStatus = false,
                                       CreateUser = userID,
                                       CreateDate = DateTime.Now,
                                       UpdateUser = userID,
                                       UpdateDate = DateTime.Now
                                   };

                                   activityViewModel.Activity.ActivityResources.Add(Resource);
                               }

                           }
                       }

                       activityViewModel.Information = CheckForConflicts(activityViewModel.Activity);
                       if (activityViewModel.Information.Count == 0 || activityViewModel.ForceCreate == true)
                       {
                           activityViewModel.Activity.CreateUser = userID;
                           activityViewModel.Activity.UpdateUser = userID;

                           db.Activities.Add(activityViewModel.Activity);
                           db.SaveChanges();

                           return RedirectToAction("Administracion", "Home", null);
                       }
                       else
                       {
                           activityViewModel.Action = "CrearActividad";
                           return View("Conflictos", activityViewModel);
                       }
                   }

                   activityViewModel.ActivityTypes = getActivityTypes();
                   activityViewModel.SchoolList = getSchools();
                   activityViewModel.OutreachEntities = getOutreachEntities();
                   activityViewModel.Contacts = getContacts();
                   activityViewModel.Resources = getResources();
                   return View(activityViewModel);
               }
           }

           return RedirectToAction("AccesoDenegado", "Home");

       }

       public ActionResult EditarActividad(int id = 0)
       {
           if (User.Identity.IsAuthenticated)
           {
               if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
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
                   activityViewModel.Contacts = getContacts();
                   activityViewModel.Resources = getResources();
                   return View(activityViewModel);
               }
           }
           return RedirectToAction("AccesoDenegado", "Home");
       }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult EditarActividad(ActivityViewModel activityViewModel)
       {
           if (User.Identity.IsAuthenticated)
           {
               if (Int32.Parse(User.Identity.Name.Split(',')[1]) == 1)
               {
                   int userID = Int32.Parse(User.Identity.Name.Split(',')[0]);

                   if (activityViewModel.ActivityDate != "" && activityViewModel.ActivityDate != null)
                       activityViewModel.Activity.ActivityDate = DateTime.ParseExact(activityViewModel.ActivityDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                   if (activityViewModel.Activity.ActivityDate != null)
                   {
                       if (activityViewModel.Activity.ActivityDate.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                           ModelState.AddModelError("Activity_ActivityDate_EarlierThanCurrentDate", Resources.WebResources.Activity_ActivityDate_EarlierThanCurrentDate);
                   }

                   if (activityViewModel.Activity.Details != null && activityViewModel.Activity.Details != "")
                   {
                       string pattern = @"^<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>$";
                       Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                       MatchCollection matches = rgx.Matches(activityViewModel.Activity.Details);

                       if (matches.Count > 0)
                           ModelState.AddModelError("Activity_Details_Invalid", Resources.WebResources.Activity_Details_Invalid);

                   }

                   if (ModelState.IsValid)
                   {
                       if (activityViewModel.Activity.ActivityTime != "" && activityViewModel.Activity.ActivityTime != null)
                           activityViewModel.Activity.ActivityTime = activityViewModel.Activity.ActivityTime.Replace(" ", "");

                       activityViewModel.Activity.UpdateUser = userID;
                       activityViewModel.Activity.UpdateDate = DateTime.Now;

                       ///Contacts///

                       ///Check if ID list is null, because if it is BOOM!
                       if (activityViewModel.ContactIDs != null)
                       {
                           //Now check if it contains anything.
                           if (activityViewModel.ContactIDs.Count > 0 && activityViewModel.ContactIDs.First() != 0)
                           {
                               //Retrieve any pre-existing contacts.
                               var contacts = (from contact in db.Contacts where contact.ActivityID == activityViewModel.Activity.ActivityID select contact).ToList();

                               //Verify if any pre-existing contacts exist.
                               if (contacts.Count > 0)
                               {
                                   foreach (var contact in contacts)
                                   {
                                       //If there are existing contacts, verify if these contacts are contained in the new id list.
                                       if (contact.DeletionDate == null && !activityViewModel.ContactIDs.Contains(contact.UserID))
                                       {
                                           //If not delete them.
                                           contact.DeletionDate = DateTime.Now;
                                           db.Entry(contact).State = EntityState.Modified;
                                       }
                                       //Else if they have been removed but are included in the ID list restore them.
                                       else if (contact.DeletionDate != null && activityViewModel.ContactIDs.Contains(contact.UserID))
                                       {
                                           contact.DeletionDate = null;
                                           db.Entry(contact).State = EntityState.Modified;

                                       }
                                   }
                               }

                               //Now to take care of any new contacts.
                               foreach (var id in activityViewModel.ContactIDs)
                               {
                                   //Retrieve all existing contacts, those with deletion date and without that match the id in the list.
                                   var contactList = (from cont in db.Contacts where cont.ActivityID == activityViewModel.Activity.ActivityID && cont.UserID == id select cont).ToList();

                                   Contact contact = null;

                                   //Is the list empty?
                                   if (contactList.Count > 0)
                                       contact = contactList.First();

                                   //If so that means it's new so we must add it.
                                   if (contact == null)
                                   {
                                       Contact ActivityContact = new Contact
                                       {
                                           UserID = id,
                                           ActivityID = activityViewModel.Activity.ActivityID,
                                           CreateUser = userID,
                                           CreateDate = DateTime.Now,
                                           UpdateUser = userID,
                                           UpdateDate = DateTime.Now
                                       };

                                       //Since the activity already exists, we just add the new contact to the Contacts DB table.
                                       db.Contacts.Add(ActivityContact);
                                   }
                               }
                           }
                       }
                       else
                       {
                           //If none of the above things happen, verify if there are any existing contacts and remove them, 
                           //because all contacts may have been removed.
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

                       ////Resources////

                       //Check if the ID list is null, because if so BOOM!
                       if (activityViewModel.ResourceIDs != null)
                       {
                           //Now, is it empty?
                           if (activityViewModel.ResourceIDs.Count > 0 && activityViewModel.ResourceIDs.First() != 0)
                           {
                               //Retrieve all existing resources that match this activity ID.
                               var resources = (from resource in db.ActivityResources where resource.ActivityID == activityViewModel.Activity.ActivityID select resource).ToList();

                               //Are there any matches?
                               if (resources.Count > 0)
                               {
                                   foreach (var resource in resources)
                                   {
                                       //If they are not contained in the current list, remove them.
                                       if (resource.DeletionDate == null && !activityViewModel.ResourceIDs.Contains(resource.ResourceID))
                                       {
                                           resource.DeletionDate = DateTime.Now;
                                           db.Entry(resource).State = EntityState.Modified;
                                       }
                                       //If they have been removed but are now restored.
                                       else if (resource.DeletionDate != null && activityViewModel.ResourceIDs.Contains(resource.ResourceID))
                                       {
                                           //Then restore them in the DB as well.
                                           resource.DeletionDate = null;
                                           db.Entry(resource).State = EntityState.Modified;
                                       }
                                   }
                               }


                               //Now for all the remaining resource IDs
                               foreach (var id in activityViewModel.ResourceIDs)
                               {
                                   //Verify if a resource already exists with this ID.
                                   var resourceList = (from activityResource in db.ActivityResources where activityResource.ActivityID == activityViewModel.Activity.ActivityID && activityResource.ResourceID == id select activityResource).ToList();

                                   ActivityResource resource = null;

                                   if (resourceList.Count > 0)
                                       resource = resourceList.First();

                                   //If it's new, then add it.
                                   if (resource == null)
                                   {
                                       ActivityResource ActivityResource = new ActivityResource
                                       {
                                           ResourceID = id,
                                           ActivityID = activityViewModel.Activity.ActivityID,
                                           CreateUser = userID,
                                           CreateDate = DateTime.Now,
                                           UpdateUser = userID,
                                           UpdateDate = DateTime.Now
                                       };

                                       //This activity already exists so we just add this resource to the ActivityResource DB table.
                                       db.ActivityResources.Add(ActivityResource);
                                   }
                               }
                           }
                       }
                       else
                       {
                           //Check if any resources exist, and if they do, they may have been removed.
                           var resources = (from resource in db.ActivityResources where resource.ActivityID == activityViewModel.Activity.ActivityID && resource.DeletionDate == null select resource).ToList();

                           if (resources.Count > 0)
                           {
                               foreach (var resource in resources)
                               {
                                   //So remove them all.
                                   resource.DeletionDate = DateTime.Now;
                                   db.Entry(resource).State = EntityState.Modified;
                               }
                           }
                       }

                       activityViewModel.Information = CheckForConflicts(activityViewModel.Activity);
                       if (activityViewModel.Information.Count == 0 || activityViewModel.ForceCreate == true)
                       {
                           db.Entry(activityViewModel.Activity).State = EntityState.Modified;
                           db.SaveChanges();
                           return RedirectToAction("Administracion", "Home", null);
                       }
                       else
                       {
                           activityViewModel.Action = "EditarActividad";
                           return View("Conflictos", activityViewModel);
                       }

                   }

                   activityViewModel.ActivityTypes = getActivityTypes();
                   activityViewModel.SchoolList = getSchools();
                   activityViewModel.OutreachEntities = getOutreachEntities();
                   activityViewModel.Contacts = getContacts();
                   activityViewModel.Resources = getResources();
                   return View(activityViewModel);
               }
           }

           return RedirectToAction("AccesoDenegado", "Home");
       }

       
    }
}