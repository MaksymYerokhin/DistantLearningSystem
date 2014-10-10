using System.Linq;
using System.Web;
using DistantLearningSystem.Models.LogicModels.Managers;
using System.Web.Mvc;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Filters;

namespace DistantLearningSystem.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NoPermission()
        {
            return View();
        }

        #region Concept

        [ConceptLimitFilter]
        public ActionResult AddConcept(int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        public ActionResult EditConcept(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(DataManager.Concept.GetConcept(id));
        }

        public ActionResult DeleteConcept(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            ViewBag.conceptId = id;
            return View(DataManager.Concept.GetConcept(id));
        }

        [ConceptLimitFilter]
        [HttpPost]
        public ActionResult ManageConceptAdding(string name, string abbreviation, string definition, HttpPostedFileBase imageUpload)
        {
            var user = DataManager.DefineUser(HttpContext);
            ProcessResult result = DataManager.Concept.AddConcept(user.Id, name,
                abbreviation,
                definition,
                imageUpload,
                Server);
            if (result.Succeeded)
                return RedirectToAction("GetConcepts", "Search", new { result = result.Id });
            else return RedirectToAction("AddConcept", new { result = result.Id });
        }

        [HttpPost]
        public ActionResult ManageConceptEditing(int id, string name, string abbreviation,
            HttpPostedFileBase imageUpload, bool deleteImage = false)
        {
            var processResult = DataManager.Concept.EdtiConcept(id, name, abbreviation, imageUpload,
                Server, deleteImage);
            if (processResult.Succeeded)
                return RedirectToAction("Concept", "Search", new { result = processResult.Id, id = id });
            else
                return RedirectToAction("EditConcept", new { id = id, result = processResult.Id });
        }

        [HttpPost]
        public ActionResult ManageConceptDeleting(int id)
        {
            var processResult = DataManager.Concept.RemoveConcept(id);
            if (processResult.Succeeded)
                return RedirectToAction("GetConcepts", "Search", new { result = processResult.Id });
            else
                return RedirectToAction("DeleteConcept", new { id = id, result = processResult.Id });
        }

        #endregion

        #region Definition

        [DefinitionLimitFilter]
        public ActionResult AddDefinition(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            ViewBag.ConceptId = id;
            return View();
        }

        public ActionResult EditDefinition(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(DataManager.Concept.GetDefinition(id));
        }

        public ActionResult DeleteDefinition(int id, int? result)
        {
            ViewBag.definitionId = id;
            return View(DataManager.Concept.GetDefinition(id));
        }

        [DefinitionLimitFilter]
        [HttpPost]
        public ActionResult ManageDefinitionAdding(string text, int conceptId)
        {
            var user = DataManager.DefineUser(HttpContext);
            ProcessResult result = DataManager.Concept.AddDefinition(conceptId, text, user.Id);
            if (result.Succeeded)
                return RedirectToAction("Concept", "Search", new { result = result.Id, id = conceptId });
            else return RedirectToAction("AddDefinition", new { id = conceptId, result = result.Id });
        }

        [HttpPost]
        public ActionResult ManageDefinitionEditing(string text, int id)
        {
            var user = DataManager.DefineUser(HttpContext);
            if (user == null)
                return RedirectToAction("GetConcepts", "Search");
            ProcessResult result = DataManager.Concept.EditDefinition(id, text);
            if (result.Succeeded)
                return RedirectToAction("Concept", "Search", new { result = result.Id, id = id });
            else return RedirectToAction("EditDefinition", new { id = id, result = result.Id });
        }

        [HttpPost]
        public ActionResult ManageDefinitionDeleting(int id, int conceptId)
        {
            var result = DataManager.Concept.DeleteDefinition(id);
            return RedirectToAction("Concept", "Search", new { id = conceptId, result = result.Id });
        }

        #endregion

        #region Source

        public ActionResult AddSource(int? result)
        {
            var user = DataManager.DefineUser(HttpContext);
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        public ActionResult EditSource(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View(DataManager.Source.GetSource(id));
        }

        [HttpPost]
        public ActionResult ManageSourceAdding(int typeId, string fulltitle, string author, int year)
        {
            var user = DataManager.DefineUser(HttpContext);
            ProcessResult result = DataManager.Source.AddSource(year, fulltitle, typeId, author);
            if (result.Succeeded)
                return RedirectToAction("Sources", "Search", new { result = result.Id });
            else return RedirectToAction("AddSource", new { result = result.Id });
        }

        [HttpPost]
        public ActionResult ManageSourceEditing(int id, int typeId, string fulltitle,
            string author, int year)
        {
            var result = DataManager.Source.EditSsource(id, typeId, fulltitle, author, year);

            if (result.Succeeded)
                return RedirectToAction("Sources", "Search", new { result = result.Id });
            else return RedirectToAction("AddSource", new { result = result.Id });
        }

        public ActionResult ManageSourceDeleting(int id)
        {
            var result = DataManager.Source.DeleteSource(id);
            return RedirectToAction("Sources", "Search", new { result = result.Id });
        }

        #endregion

        #region Formulations

        [FormulationLimitFilter]
        public ActionResult AddFormulation(int id, int? result)
        {
            if (result.HasValue)
                ViewBag.Result = result.Value;
            ViewBag.definitionId = id;
            return View();
        }

        [FormulationLimitFilter]
        [HttpPost]
        public ActionResult ManageFormulationAdding(string specificDifference,
            int definitionId)
        {
            var user = DataManager.DefineUser(HttpContext);
            var result = DataManager.Concept.AddFormulation(definitionId, user.Id, specificDifference);
            if (result.Succeeded)
                return RedirectToAction("Definition", "Search", new { id = definitionId });
            else return RedirectToAction("AddFormulation", "Dashboard", new { id = definitionId });
        }

        [HttpPost]
        public ActionResult ManageFormulationEditing(string specificDifference,
        int id, int definitionId)
        {
            var user = DataManager.DefineUser(HttpContext);
            var result = DataManager.Concept.EditFormulation(id, specificDifference);
            if (result.Succeeded)
                return RedirectToAction("Definition", "Search", new { id = definitionId, result = result.Id });
            else return RedirectToAction("EditFormulation", "Dashboard", new { id = id, result = result.Id });
        }


        public ActionResult EditFormulation(int id)
        {
            return View(DataManager.Concept.GetFormulationOfTheDefinition(id));
        }

        public ActionResult DeleteFormulation(int id, int definitionId)
        {
            var result = DataManager.Concept.RemoveFormulationOfDefinition(id);
            return RedirectToAction("Definition", "Search", new { id = definitionId, result = result.Id });
        }

        #endregion

        #region Classifications

        [ClassificationLimitFilter]
        public ActionResult AddClassification(int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }

        [ClassificationLimitFilter]
        public ActionResult ManageClassificationAdding(string Base, int type)
        {
            var user = DataManager.DefineUser(HttpContext);
            var result = DataManager.Classification.Add(Base, type, user.Id);
            if (result.Succeeded)
                return RedirectToAction("Classifications", "Search", new { result = result.Id });
            else
                return RedirectToAction("AddClassification", new { result = result.Id });
        }

        public ActionResult EditClassification(int id)
        {
            return View(DataManager.Classification.GetClassification(id));
        }

        [HttpPost]
        public ActionResult ManageClassificationEditing(int id, string Base, int type)
        {
            ProcessResult result = DataManager.Classification.EditClassification(id, Base, type);
            return RedirectToAction("Classifications", "Search", new { result = result.Id });
        }

        public ActionResult DeleteClassification(int id)
        {
            var result = DataManager.Classification.Remove(id);
            return RedirectToAction("Classifications", "Search", new { result = result.Id });
        }

        #endregion

        public ActionResult GetAnnouncements(int id) 
        {
            ViewBag.Cocnepts = DataManager.Lecturer.GetConcepts(id).ToList();
            ViewBag.Definitions = DataManager.Lecturer.GetDefinitions(id).ToList();
            ViewBag.Classifications = DataManager.Lecturer.GetClassifications(id).ToList();

            return View();
        }

        public ActionResult EditStatus(int table, int id)
        {
            int status = 0;

            switch (table)
            {
                case 0:
                    status = DataManager.Concept.GetConcept(id).Status;
                    break;
                case 1:
                    status = DataManager.Concept.GetDefinition(id).Status;
                    break;
                case 2:
                    status = DataManager.Concept.GetFormulationOfTheDefinition(id).Status;
                    break;
            }

            ViewBag.table = table;
            ViewBag.id = id;
            ViewBag.status = status;

            return View();
        }

        public ActionResult ManageStatusEditing(int table, int id, int status)
        {
            switch (table)
            {
                case 0:
                    DataManager.Concept.SetConceptStatus(id, status);
                    return RedirectToAction("Concept", "Search", new { id = id });
                case 1:
                    DataManager.Concept.SetDefinitionStatus(id, status);
                    return RedirectToAction("Definition", "Search", new { id = id });
                case 2:
                    var formulation = DataManager.Concept.GetFormulationOfTheDefinition(id);
                    return RedirectToAction("Definition", "Search", new { id = formulation.DefinitionId });
            }

            return RedirectToAction("GetConcepts", "Search");
        }
    }
}