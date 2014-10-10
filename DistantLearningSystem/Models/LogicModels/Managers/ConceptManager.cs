using System;
using System.Collections.Generic;
using System.Linq;
using DistantLearningSystem.Models.DataModels;
using System.Web;
using DistantLearningSystem.Models.LogicModels.Services;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class ConceptManager : Manager
    {
        public ProcessResult AddConcept(int userId,
            string name,
            string abbreviation,
            string definition,
            HttpPostedFileBase imageUpload,
            HttpServerUtilityBase server)
        {
            var exists = entities.Concepts.FirstOrDefault(x => x.Name.ToLower()
                == name.ToLower());

            if (exists != null)
                return ProcessResults.ConceptAlreadyExisting;

            var concept = new Concept
            {
                Name = name,
                AddedDate = DateTime.Now,
                Abbreviation = abbreviation,
                StudentId = userId
            };

            Concept cn = entities.Concepts.Add(concept);
            SaveChanges();

            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                cn.ImgSrc = SaveImage(cn.Id, StaticSettings.ConceptIconsUploadPath, imageUpload, server);
            }

            AddDefinition(cn.Id, definition, userId);
            //SaveChanges();
            return ProcessResults.ConceptAdded;
        }

        public void SetConceptStatus(int conceptId, int status) 
        {
            var concept = GetConcept(conceptId);
            concept.Status = status;
            SaveChanges();
        }

        public void SetDefinitionStatus(int definitionId, int status)
        {
            var concept = GetDefinition(definitionId);
            concept.Status = status;
            SaveChanges();
        }

        public void SetFormulationStatus(int definitionId, int status)
        {
            var concept = GetFormulationOfTheDefinition(definitionId);
            concept.Status = status;
            SaveChanges();
        }


        public ProcessResult EdtiConcept(int id, string name,
        string abbreviation,
        HttpPostedFileBase imageUpload, HttpServerUtilityBase Server, bool deleteImage)
        {
            var concept = GetConcept(id);
            if (concept == null)
                return ProcessResults.ConceptNotExisting;
            if (deleteImage)
                concept.ImgSrc = "";
            else if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                concept.ImgSrc = SaveImage(id, StaticSettings.ConceptIconsUploadPath, imageUpload, Server);
            }
            concept.Name = name;
            concept.Abbreviation = abbreviation;

            return ProcessResults.ConceptEditedSuccesfully;
        }

        public ProcessResult RemoveConcept(int conceptId)
        {
            var conceptTodelete = GetConcept(conceptId);
            if (conceptTodelete == null)
                return ProcessResults.ConceptNotExisting;

            entities.Concepts.Remove(conceptTodelete);
            SaveChanges();
            return ProcessResults.ConceptDeletedSuccesfully;
        }

        public Definition GetDefinition(int definitionId)
        {
            return entities.Definitions.FirstOrDefault(x => x.Id == definitionId);
        }

        public Formulation GetFormulationOfTheDefinition(int formulationId)
        {
            return entities.Formulations.FirstOrDefault(x => x.Id == formulationId); ;
        }

        public ProcessResult RemoveFormulationOfDefinition(int formulationId)
        {
            var formulation = GetFormulationOfTheDefinition(formulationId);
            if (formulation == null)
                return ProcessResults.FormulationNotExisting;
            entities.Formulations.Remove(formulation);

            SaveChanges();
            return ProcessResults.FormulationRemvoed;
        }

        public Formulation AddFormulationForDefinition(int definitionId, Formulation formulation)
        {
            formulation.DefinitionId = definitionId;
            Formulation fm = entities.Formulations.Add(formulation);
            SaveChanges();
            return fm;
        }

        public IEnumerable<Concept> GetConcepts(bool confirmed = true)
        {
            return entities.Concepts.ToList();
        }

        public Concept GetConcept(int conceptId)
        {
            return entities.Concepts.FirstOrDefault(x => x.Id == conceptId);
        }

        public ProcessResult AddDefinition(int conceptId, string text, int studentId)
        {
            var concept = GetConcept(conceptId);
            if (concept == null)
                return ProcessResults.ConceptNotExisting;
            var def = new Definition
            {
                AddedDate = DateTime.Now,
                Text = text,
                StudentId = studentId,
                ConceptId = conceptId
            };

            entities.Definitions.Add(def);
            SaveChanges();
            return ProcessResults.DefinitionAddedSuccessfully;
        }

        public bool RemoveDefinition(int conceptId, int definitionId)
        {
            var defToRemove = entities.Definitions.FirstOrDefault(x => x.Id == definitionId && x.ConceptId == conceptId);
            if (defToRemove == null)
                return false;
            entities.Definitions.Remove(defToRemove);
            SaveChanges();
            return true;
        }

        public IEnumerable<Definition> GetDefinitions(int conceptId, bool confirmed = true)
        {
            var concept = entities.Concepts.FirstOrDefault(x => x.Id == conceptId);
            if (concept != null)
                return concept.Definitions.ToList();
            return null;
        }

        public ProcessResult AddFormulation(int definitionId,
        int studentId,
        string specificdifference)
        {
            var definition = GetDefinition(definitionId);
            if (definition == null)
                return ProcessResults.DefinitionNotExisting;

            var formulation = new Formulation 
            {
                AddedDate = DateTime.Now,
                DefinitionId = definitionId,
                StudentId = studentId,
                SpecificDifference = specificdifference,
            };

            entities.Formulations.Add(formulation);
            SaveChanges();
            return ProcessResults.FormulationSuccessfullyAdded;
        }

        public ProcessResult EditDefinition(int id, string text)
        {
            var definition = GetDefinition(id);
            if (definition == null)
                return ProcessResults.DefinitionNotExisting;
            definition.Text = text;
            SaveChanges();

            return ProcessResults.DefinitionSuccessfullyEdited;
        }

        public ProcessResult DeleteDefinition(int id)
        {
            var defintion = GetDefinition(id);
            if (defintion == null)
                return ProcessResults.DefinitionNotExisting;

            entities.Definitions.Remove(defintion);
            SaveChanges();

            return ProcessResults.DefinitionWasDeleted;
        }

        public ProcessResult EditFormulation(int formulationId, string specificDifference)
        {
            var formulation = GetFormulationOfTheDefinition(formulationId);
            if (formulation == null)
                return ProcessResults.FormulationNotExisting;

            formulation.SpecificDifference = specificDifference;
            SaveChanges();
            return ProcessResults.FormulationEdited;
        }
    }
}