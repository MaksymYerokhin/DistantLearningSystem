using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class ClassificationManager : Manager
    {
        public ClassificationType AddClassificationType(ClassificationType newType)
        {
            var exists = entities.ClassificationTypes.FirstOrDefault(x => x.Name.ToLower() == newType.Name.ToLower());
            if (exists != null)
                return null;

            var cstypes = entities.ClassificationTypes.Add(newType);
            SaveChanges();
            return cstypes;
        }

        public IEnumerable<Classification> GetClassifications()
        {
            return entities.Classifications.ToList();
        }

        public Classification Add(Classification classification)
        {
            var exists = entities.Classifications.FirstOrDefault(x =>
                x.ClassificationTypeId == classification.ClassificationTypeId &&
                x.Base.ToLower() == classification.Base.ToLower());

            if (exists != null)
                return null;

            var cs = entities.Classifications.Add(classification);
            SaveChanges();

            return cs;
        }

        public IEnumerable<Classification> GetAll()
        {
            return entities.Classifications;
        }

        public Classification GetClassification(int classificationId)
        {
            return entities.Classifications.FirstOrDefault(x => x.Id == classificationId);
        }

        public void EditClassification(Classification newClassification)
        {
            var oldClassification = GetClassification(newClassification.Id);
            oldClassification.Rating = newClassification.Rating;
            oldClassification.Status = newClassification.Status;
            oldClassification.StudentId = newClassification.StudentId;
            oldClassification.Base = newClassification.Base;
            oldClassification.ClassificationTypeId = newClassification.ClassificationTypeId;
            SaveChanges();
        }

        public void SetRating(int classificationId, int rating)
        {
            var classsification = GetClassification(classificationId);
            classsification.Rating = rating;
            SaveChanges();
        }

        public void SetCheckStatus(int classificationId, MarkStatus check)
        {
            var classification = GetClassification(classificationId);
            classification.Status = (int)check;
            SaveChanges();
        }

        public ProcessResult Remove(int classificationId)
        {
            var classification = GetClassification(classificationId);
            entities.Classifications.Remove(classification);
            SaveChanges();
            return ProcessResults.ClassificationDeleted;
        }

        public Classification GetClassification(string _base)
        {
            return entities.Classifications.FirstOrDefault(x => x.Base.ToLower() == _base.ToLower());
        }

        public ProcessResult Add(string Base, int classificationType, int studentId)
        {
            var classification = GetClassification(Base);

            if (classification != null)
                return ProcessResults.ClassificationExisting;

            var Classification = new Classification()
            {
                Base = Base,
                ClassificationTypeId = classificationType,
                StudentId = studentId,
                AddedDate = DateTime.Now
            };

            entities.Classifications.Add(Classification);
            entities.SaveChanges();

            return ProcessResults.ClassificationAdded;
        }

        public ProcessResult EditClassification(int id, string Base, int classificationType)
        {
            var classification = GetClassification(id);
            classification.Base = Base;
            classification.ClassificationTypeId = classificationType;
            SaveChanges();
            return ProcessResults.ClassificationEdited;
        }
    }
}