using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.DataModels
{
    public class ClassType
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class ClassTypes
    {
        private static ClassType[] types = new ClassType[2] 
        {
            new ClassType{Id = 0, Value = "Родо-видовая"},
            new ClassType{Id = 1, Value = "Партитивная"}
        };

        public static IEnumerable<ClassType> GetSourceTypes()
        {
            return types;
        }

        public static ClassType GetById(int id)
        {
            if (id < 0 || id >= types.Length)
                throw new IndexOutOfRangeException();
            return types[id];
        }

    }
}