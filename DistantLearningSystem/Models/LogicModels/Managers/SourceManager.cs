using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class SourceManager : Manager
    {
        public ProcessResult AddSource(int year, string fulltitle, int type, string author)
        {
            var _source = GetSource(fulltitle);
            if (_source != null)
                return ProcessResults.SourceAlreadyExisting;

            var source = new Source
            {
                Type = type,
                Author = author,
                PublicationYear = year
            };

            if (type == 0)
            {
                if (fulltitle.Contains("http://"))
                    source.FullTitle = fulltitle;
                else source.FullTitle = "http://" + fulltitle;
            }
            else 
                source.FullTitle = fulltitle;
            entities.Sources.Add(source);

            SaveChanges();
            return ProcessResults.SourceSuccessfullyAdded;
        }

        public Source GetSource(string title)
        {
            return entities.Sources.FirstOrDefault(x => x.FullTitle.ToLower() == title.ToLower());
        }

        public Source GetSource(int sourceId)
        {
            return entities.Sources.FirstOrDefault(x => x.Id == sourceId);
        }

        public bool EditSsource(Source editedSource)
        {
            var src = GetSource(editedSource.Id);
            if (src == null)
                return false;
            src.Type = editedSource.Type;
            src.Author = editedSource.Author;
            src.FullTitle = editedSource.FullTitle;
            src.PublicationYear = editedSource.PublicationYear;
            SaveChanges();
            return true;
        }

        public ProcessResult DeleteSource(int sourceId)
        {
            var source = GetSource(sourceId);
            if (source == null)
                return ProcessResults.SourceNotFound;

            entities.Sources.Remove(source);
            SaveChanges();
            return ProcessResults.SourceWasDeleted;
        }

        internal IEnumerable<Source> GetSources()
        {
            return entities.Sources.ToList();
        }

        public ProcessResult EditSsource(int id, int typeId, string fulltitle, string author, int year)
        {
            var source = GetSource(id);
            if (source == null)
                return ProcessResults.SourceNotFound;

            source.Type = typeId;
            source.FullTitle = fulltitle;
            source.Author = author;
            source.PublicationYear = year;

            SaveChanges();
            return ProcessResults.SourceSuccessfullyEdited;
        }
    }
}