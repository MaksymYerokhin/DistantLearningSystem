using System;
using System.Web;
using System.Web.Mvc;
using DistantLearningSystem.Models;
using System.Linq;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.Managers;
using DistantLearningSystem.Models.LogicModels.Services;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using System.Collections.Generic;

namespace DistantLearningSystem.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            var user = DataManager.DefineUser(HttpContext);
            if (user == null)
                return RedirectToAction("LogIn");
            return View(user);
        }

        public ActionResult ManageGroupAdding(string groupId, string lecturerId)
        {
            ProcessResult processResult = null;
            var user = DataManager.DefineUser(HttpContext);
            if (user.Id.ToString() == lecturerId)
                processResult = DataManager.University.SetGroupLecturer(user.Id, Convert.ToInt32(groupId));

            return RedirectToAction("Index", "Home", new { result = processResult.Id });
        }

        public ActionResult AddGroup()
        {
            var user = DataManager.DefineUser(HttpContext);
            if (user.UserType != UserType.Lecturer)
                RedirectToAction("Index", "Home", new
                {
                    result = ProcessResults.ErrorOccured.Id
                });
            ViewBag.Groups = DataManager.University.GetGroups();
            ViewBag.UserId = user.Id;
            return View();
        }

        public ActionResult EditProfile()
        {
            var user = DataManager.DefineUser(HttpContext);
            if (user.UserType == UserType.Student)
                return RedirectToAction("EditStudent");
            else return RedirectToAction("EditLecturer");
        }

        public ActionResult EditLecturer()
        {
            var user = DataManager.DefineUser(HttpContext);
            var lecturer = DataManager.Lecturer.GetLecturer(user.Id);
            return View(lecturer);
        }

        public ActionResult ManageLecturerEditing(string Id, string Name,
            string LastName, string Login,
            string Email, string Password,
            HttpPostedFileBase imageUpload)
        {
            var lecturer = new Lecturer()
            {
                Id = Convert.ToInt32(Id),
                Name = Name,
                Login = Login,
                Email = Email,
                Password = Password,
                LastName = LastName
            };

            ProcessResult p = DataManager.Lecturer.EditLecturer(lecturer, Server, imageUpload);
            return RedirectToAction("Index", "Home", new { result = p.Id });
        }

        public ActionResult ManageStudentEditing(string Id, string Name,
        string LastName, string Login,
        string Email, string Password,
        HttpPostedFileBase imageUpload)
        {
            var lecturer = new Student()
            {
                Id = Convert.ToInt32(Id),
                Name = Name,
                Login = Login,
                Email = Email,
                Password = Password,
                LastName = LastName
            };

            ProcessResult p = DataManager.Student.EditStudent(lecturer, Server, imageUpload);
            return RedirectToAction("Index", "Home", new { result = p.Id });
        }


        public ActionResult Activity(int id) 
        {
            var student = DataManager.Student.GetStudent(id);
            return View(student);
        }

        public ActionResult EditStudent()
        {
            var user = DataManager.DefineUser(HttpContext);
            var student = DataManager.Student.GetStudent(user.Id);
            return View(student);
        }

        public ActionResult Profile()
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user == null)
                return RedirectToAction("LogIn");
            if (user.UserType == UserType.Student)
                return RedirectToAction("GetStudentInfo");
            else return RedirectToAction("GetLecturerInfo");
        }

        public ActionResult GetLecturerInfo(int userId = -1)
        {
            UserModel user = null;
            Lecturer lecturer = null;
            if (userId == -1)
            {
                user = DataManager.DefineUser(HttpContext);
                lecturer = DataManager.Lecturer.GetLecturer(user.Id);
            }
            else
                lecturer = DataManager.Lecturer.GetLecturer(userId);
            ViewBag.Concepts = DataManager.Lecturer.GetConcepts(userId).ToList();
            ViewBag.Definitions = DataManager.Lecturer.GetDefinitions(userId).ToList();
            ViewBag.Classifications = DataManager.Lecturer.GetClassifications(userId).ToList();
            return View(lecturer);
        }


        public ActionResult GetStudentInfo(int userId = -1)
        {
            UserModel user = null;
            Student student = null;
            if (userId == -1)
            {
                user = DataManager.DefineUser(HttpContext);
                student = DataManager.Student.GetStudent(user.Id);
            }
            else
                student = DataManager.Student.GetStudent(userId);
            return View(student);
        }

        public ActionResult Logout()
        {
            var userCookie = new HttpCookie("UserId")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            var keyCookie = new HttpCookie("Key")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            var typeCookie = new HttpCookie("UserType")
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            HttpContext.Response.Cookies.Set(userCookie);
            HttpContext.Response.Cookies.Set(keyCookie);
            HttpContext.Response.Cookies.Set(typeCookie);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogIn(int? result)
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user != null)
                return RedirectToAction("Index");
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        [HttpPost]
        public ActionResult ManageLogIn(string LoginOrEmail, string Password, string WhoAreU)
        {
            var loginModel = new LoginModel()
            {
                LoginOrEmail = LoginOrEmail,
                Password = Security.GetHashString(Password),
                UserType = WhoAreU.ToLower() == "студент" ? UserType.Student : UserType.Lecturer
            };

            UserModel user;
            ProcessResult result = DataManager.Authentification.LogInUser(loginModel, out user);
            if (result.Succeeded && user != null)
            {
                SetUser(user, user.Password);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("LogIn",
                "User",
                new
                {
                    result = result.Id
                });
        }

        public ActionResult Registration(int? result)
        {
            UserModel user = DataManager.DefineUser(HttpContext);
            if (user != null)
                return RedirectToAction("Index");
            ViewBag.Groups = DataManager.University.GetGroups();
            if (result.HasValue)
            {
                ViewBag.Result = ProcessResults.GetById(result.Value);
            }
            return View();
        }

        public ActionResult AjaxRegistrationFormLoad(string data)
        {
            if (data.ToLower() == "студент")
                return RedirectToAction("RegistrateStudent");
            else return RedirectToAction("RegistrateLecturer");
        }

        public ActionResult RegistrateLecturer()
        {
            return PartialView();
        }

        public ActionResult RegistrateStudent()
        {
            List<StudentGroup> groups = DataManager.University.GetGroups().ToList();
            ViewBag.Groups = groups;
            return PartialView();
        }

        [HttpPost]
        public ActionResult ManageLecturerRegistration(
            string Name, string LastName, string Login,
            string Email, string Password, string Subject,
            string Position, HttpPostedFileBase imageUpload)
        {
            var registrationModel = new Lecturer()
            {
                Name = Name,
                LastName = LastName,
                Login = Login,
                Password = Password,
                DepartmentId = 1,
                Faculty = "КН",
                Email = Email,
                Subject = Subject,
                Position = Position
            };

            ProcessResult result = DataManager.
                Lecturer.
                RegistrateLecturer(HttpContext,
                registrationModel,
                Server,
                imageUpload);

            return RedirectToAction("Registration", "User", new { result = result.Id });
        }

        [HttpPost]
        public ActionResult ManageStudentRegistration(
            string Name, string LastName, string Login,
            string Email, string Password, string groupId, HttpPostedFileBase imageUpload)
        {
            ProcessResult result = DataManager.
                Student.
                RegistrateStudent(HttpContext,
                new Student
                {
                    Name = Name,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    Login = Login,
                    GroupId = Convert.ToInt32(groupId)
                },
                Server,
                imageUpload);

            return RedirectToAction("Registration", "User", new { result = result.Id });
        }

        public ActionResult Confirm(string hash)
        {
            var result = DataManager.Authentification.ConfirmRegistration(hash);
            return RedirectToAction("Registration", "User", new { result = result.Id });
        }

        private void SetUser(UserModel user, string hashedKey)
        {
            if (user == null) return;
            var cookieUser = new HttpCookie("UserId")
            {
                Value = Convert.ToString(user.Id),
                Expires = DateTime.MaxValue
            };

            var cookieKey = new HttpCookie("Key")
            {
                Value = hashedKey,
                Expires = DateTime.MaxValue
            };

            var cookieuserType = new HttpCookie("UserType")
            {
                Value = ((int)user.UserType).ToString(),
                Expires = DateTime.MaxValue
            };

            HttpContext.Response.Cookies.Remove("UserId");
            HttpContext.Response.Cookies.Remove("Key");
            HttpContext.Response.Cookies.Remove("UserType");

            HttpContext.Response.SetCookie(cookieUser);
            HttpContext.Response.SetCookie(cookieKey);
            HttpContext.Response.SetCookie(cookieuserType);
        }

        //public ActionResult Bookmarks()
        //{
        //    User user = DefineUser(HttpContext);
        //    if (user == null)
        //        return RedirectToAction("LogIn");
        //    Culture culture = DefineLanguage(HttpContext);
        //    ViewBag.Culture = culture;
        //    return View(DataManager.Routes.GetUserBookmarks(user.Id, culture: culture));
        //}
    }
}