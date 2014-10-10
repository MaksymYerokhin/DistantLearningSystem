using DistantLearningSystem.Models.LogicModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    //Используется для инкапсуляции доступа к бд
    public class DataManager
    {
        public static ClassificationManager Classification { get; private set; }

        public static ConceptManager Concept { get; private set; }

        public static LecturerManager Lecturer { get; private set; }

        public static ReferenceManager Reference { get; private set; }

        public static SourceManager Source { get; private set; }

        public static StudentManager Student { get; private set; }

        public static UniversityManager University { get; private set; }

        public static AuthentificationManager Authentification { get; private set; }

        static DataManager()
        {
            Classification = new ClassificationManager();
            Concept = new ConceptManager();
            Lecturer = new LecturerManager();
            Reference = new ReferenceManager();
            Source = new SourceManager();
            Student = new StudentManager();
            University = new UniversityManager();
            Authentification = new AuthentificationManager();
        }



        public static UserModel DefineUser(HttpContextBase context)
        {
            var cookieUser = context.Request.Cookies["UserId"];
            var cookieKey = context.Request.Cookies["Key"];
            var cookieUserType = context.Request.Cookies["UserType"];
            UserModel user = null;
            if (cookieUser != null && cookieKey != null && cookieUserType != null)
            {
                UserType userType = (UserType)Convert.ToInt32(cookieUserType.Value);
                int id = Convert.ToInt32(cookieUser.Value);

                if (userType == UserType.Lecturer)
                    user = (UserModel)DataManager.Lecturer.GetLecturer(x => x.Id == id);
                else if (userType == UserType.Student)
                    user = (UserModel)DataManager.Student.GetStudent(x => x.Id == id);
            }

            if (user != null && !KeyMatch(user, cookieKey.Value))
                user = null;

            return user;
        }

        private static bool KeyMatch(UserModel user, string key)
        {
            if (user == null)
                return false;
            return user.Password == key;
        }
    }
}