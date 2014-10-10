using System;
using System.Collections.Generic;

namespace DistantLearningSystem.Models.DataModels
{

    public class RevisionStatus
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public RevisionStatus(int Id, string value) 
        {
            this.Id = Id;
            Value = value;
        }
    }

    public class RevisionStatuses
    {
        private static RevisionStatus[] types = new RevisionStatus[6] 
        {
            new RevisionStatus(0, "Не проверено"),
            new RevisionStatus(1, "Не правильно"),
            new RevisionStatus(2, "Имеется грубая ошибка"),
            new RevisionStatus(3, "Средне"),
            new RevisionStatus(4, "Хорошо"),
            new RevisionStatus(5, "Отлично")
        };

        public static IEnumerable<RevisionStatus> GetSourceTypes()
        {
            return types;
        }

        public static RevisionStatus GetById(int id)
        {
            if (id < 0 || id >= types.Length)
                throw new IndexOutOfRangeException();
            return types[id];
        }
    }

    //public class RevisionStatuses
    //{
    //    public static RevisionStatus GetRevisionStatus(int i)
    //    {
    //        return (RevisionStatus)i;
    //    }

    //    public static int GetRevisionStatus(RevisionStatus i)
    //    {
    //        return (int)i;
    //    }

    //    public static String GetRevisionStatusString(int i)
    //    {
    //        RevisionStatus rez = GetRevisionStatus(i);
    //        switch (rez)
    //        {
    //            case RevisionStatus.NotChecked:
    //                return "Не проверено.";
    //            case RevisionStatus.Uncorrect:
    //                return "Не корректно.";
    //            case RevisionStatus.Middle:
    //                return "Нормально.";
    //            case RevisionStatus.Correct:
    //                return "Корректно.";
    //            case RevisionStatus.Good:
    //                return "Хорошо.";
    //            default:
    //                throw new ArgumentException();
    //        }
    //    }
    //}
}