using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DistantLearningSystem;
using DistantLearningSystem.LogicModels.ViewModels;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.Managers;
using DistantLearningSystem.Filters;

namespace DistantLearningSystem.Controllers
{
    public class ArborController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Classifications()
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();
            List<string> bases = new List<string>();
            List<string> id = new List<string>();

            foreach (var i in db.Classifications)
            {
                bases.Add(i.Base);
                id.Add((i.Id).ToString());
            }
            option.Data = bases.ToArray();
            option.Length = bases.Count;
            option.Value = id.ToArray();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult EmptyConcepts()
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();

            List<string> enames = new List<string>();
            List<string> eid = new List<string>();

            List<string> names = new List<string>();
            List<string> id = new List<string>();

            foreach (var i in db.Connections)
            {
                if (i.Concept != null && !names.Contains(i.Concept.Name))
                {
                    names.Add(i.Concept.Name);
                    id.Add((i.Id).ToString());
                }
                if (i.Concept1 != null && !names.Contains(i.Concept1.Name))
                {
                    names.Add(i.Concept1.Name);
                    id.Add((i.Id).ToString());
                }
            }

            foreach (var i in db.Concepts)
            {
                if(names.Count() == 0)
                {
                    enames.Add(i.Name);
                    eid.Add((i.Id).ToString());
                }
                for (int j = 0; j < names.Count(); j++)
                {
                    if (!names.Contains(i.Name))
                    {
                        enames.Add(i.Name);
                        eid.Add((i.Id).ToString());
                    }
                }
            }


            option.Data = enames.ToArray();
            option.Length = enames.Count;
            option.Value = eid.ToArray();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult Concepts(int data)
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();
            List<string> names = new List<string>();
            List<string> id = new List<string>();

            foreach (var i in db.Connections)
            {
                if (i.ClassificationId == data && i.ParentConceptId == null)
                {
                    names.Add(i.Concept.Name);
                    id.Add((i.Concept.Id).ToString());
                }
            }
            option.Data = names.ToArray();
            option.Length = names.Count;
            option.Value = id.ToArray();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult ByConcepts(int idclass, int idconc)
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();
            List<string> names = new List<string>();
            List<string> id = new List<string>();

            foreach (var i in db.Connections)
            {
                if (i.ClassificationId == idclass && i.ParentConceptId == idconc)
                {
                    names.Add(i.Concept.Name);
                    id.Add((i.ChildConceptId).ToString());
                }
            }
            option.Data = names.ToArray();
            option.Length = names.Count;
            option.Value = id.ToArray();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        /// <summary>
        /// Checks if student can add connections to graph model.
        /// </summary>
        /// <returns>
        /// Returns "can" property set to True if student can add False when student can`t add connection. 
        /// </returns>
        [ConnectionLimitFilter]
        public JsonResult CanAddConnection()
        {
            var result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { can = true }
            };
            return result;
        }

        [ConnectionLimitFilter]
        public JsonResult AddConnection(int idclass, int id1, int id2)
        {            
            AjaxResponseModel option = new AjaxResponseModel();
            List<string> names = new List<string>();
            List<string> id = new List<string>();

            var db = new Models.DataModels.CourseDataBaseEntities();

            Connection connection;

            if (id1 == -1)
            {
                connection = db.Connections.Add(new Connection()
                {
                    ChildConceptId = id2,
                    ClassificationId = idclass
                });
            }
            else
            {
                connection = db.Connections.Add(new Connection()
                {
                    ChildConceptId = id2,
                    ParentConceptId = id1,
                    ClassificationId = idclass
                });
            }

            var studentId = DataManager.DefineUser(HttpContext).Id;

            db.StudentConnections.Add(new StudentConnection()
            {
                Status = 0,
                ConnectionId = connection.Id,
                StudentId = studentId,//DistantLearningSystem.Models.Authorization.AuthorizedObject.Id,
                AddedDate = DateTime.Now,
                Rating = 0,
            });

            db.SaveChanges();

            option.Data = names.ToArray();
            option.Length = names.Count;
            option.Value = id.ToArray();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult CheckConnection(int idclass, int id1, int id2)
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();
            int isvalid = 1;

            foreach (var i in db.StudentConnections)
            {
                if (i.Connection.ClassificationId == idclass &&
                    ((i.Connection.ParentConceptId == id1 && i.Connection.ChildConceptId == id2) ||
                    (i.Connection.ParentConceptId == null && i.Connection.ChildConceptId == id2 && id1 == -1))
                    )
                {
                    if (i.Status == 0)
                        isvalid = 0;
                }
            }
            option.Length = isvalid;
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult DeleteConfirmConnection(int idclass, int id1, int id2, int act)
        {
            AjaxResponseModel option = new AjaxResponseModel();

            var db = new Models.DataModels.CourseDataBaseEntities();

            foreach (var i in db.StudentConnections)
            {
                if (i.Connection.ClassificationId == idclass &&
                    ((i.Connection.ParentConceptId == id1 && i.Connection.ChildConceptId == id2) ||
                    (i.Connection.ParentConceptId == null && i.Connection.ChildConceptId == id2 && id1 == -1))
                    )
                {
                    if (act == 0)
                    {
                        db.Connections.Remove(i.Connection);
                        db.StudentConnections.Remove(i);
                        break;
                    }
                    i.Status = 1;
                    i.Rating = 1;
                    break;
                }
            }
            db.SaveChanges();
            JsonResult res = Json(option);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
    }
}