using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class ReferenceManager : Manager
    {
        public Reference AddReference(int conceptId, int definitionId, int sourceId, int studentId, string pages)
        {
            var reference = entities.References.Add(new Reference
            {
                AddedDate = DateTime.Now,
                DefinitionId = definitionId,
                SourceId = sourceId,
                StudentId = studentId,
                Pages = pages
            });

            SaveChanges();
            return reference;
        }

        public Reference GetReference(int referenceId)
        {
            return entities.References.FirstOrDefault(x => x.Id == referenceId);
        }

        public bool RemoveReference(int referenceId)
        {
            var reference = GetReference(referenceId);
            if (reference == null)
                return false;

            entities.References.Remove(reference);
            SaveChanges();
            return true;
        }

        public bool EditReference(Reference newReference)
        {
            var reference = GetReference(newReference.Id);

            if (reference == null)
                return false;
            reference.StudentId = newReference.StudentId;
            reference.SourceId = newReference.SourceId;
            reference.Rating = newReference.Rating;
            reference.Status = newReference.Status;
            reference.Pages = newReference.Pages;

            SaveChanges();
            return true;
        }
    }
}