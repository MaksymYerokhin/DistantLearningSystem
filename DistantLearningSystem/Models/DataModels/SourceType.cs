using System;
using System.Collections.Generic;

namespace DistantLearningSystem.Models.DataModels
{
    public class SourceType
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public SourceType(int id, string val)
        {
            Id = id;
            Value = val;
        }
    }

    public class SourceTypes
    {
        private static SourceType[] types = new SourceType[5] 
        {
            new SourceType(0, "Веб-ресурс"),
            new SourceType(1, "Книга"),
            new SourceType(2, "Журнал"),
            new SourceType(3, "Методические указания"),
            new SourceType(4, "Научная работа"),
        };

        public static IEnumerable<SourceType> GetSourceTypes() 
        {
            return types;
        }

        public static SourceType GetById(int id) 
        {
            if (id < 0 || id >= types.Length)
                throw new IndexOutOfRangeException();
            return types[id];
        }
    }
}