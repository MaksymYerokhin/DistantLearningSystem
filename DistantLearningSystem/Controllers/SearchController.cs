using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DistantLearningSystem.Controllers
{
    public class SearchController : Controller
    {
        // GET: /Search/
        public ActionResult Concept(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            var concept = DataManager.Concept.GetConcept(id);
            return View(concept);
        }

        public ActionResult Definition(int id, int? result)
        {
            var definition = DataManager.Concept.GetDefinition(id);
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(definition);
        }

        public ActionResult Classifications(int? result) 
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            var f = DataManager.Classification.GetClassifications();

            return View(DataManager.Classification.GetClassifications());
        }

        public ActionResult GetGroups()
        {
            return View(DataManager.University.GetGroups());
        }

        public ActionResult GetConcepts(int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(DataManager.Concept.GetConcepts(confirmed: false));
        }

        public ActionResult GetLectures()
        {
            return View(DataManager.Lecturer.GetLectures());
        }

        public ActionResult GetStudents()
        {
            var students = DataManager.Student.GetStudents();
            return View(students);
        }

        public ActionResult Sources(int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(DataManager.Source.GetSources());
        }

        public ActionResult GetGroup(int Id)
        {
            var group = DataManager.University.GetGroup(Id);
            return View(group);
        }
    }
}