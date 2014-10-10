namespace DistantLearningSystem.Models.DataModels
{
    using System;

    public partial class StudentGroup
    {
        public int GetGroupCourse()
        {
            var strs = Name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var dt = new DateTime(2000 + Convert.ToInt32(strs[1]), 9, 1);
            var now = DateTime.Now;
            var rez = now.Subtract(dt);
            return ((int)(rez.TotalDays + 365 - 1)) / 365;
        }

    }
}