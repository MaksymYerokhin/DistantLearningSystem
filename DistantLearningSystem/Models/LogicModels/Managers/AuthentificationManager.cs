using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.Services;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using System.Linq;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class AuthentificationManager : Manager
    {
        public ProcessResult LogInUser(LoginModel model, out UserModel userModel)
        {
            if (model.UserType == UserType.Lecturer)
                userModel = (UserModel)DataManager.Lecturer.LogInLecturer(model);
            else
                userModel = (UserModel)DataManager.Student.LogInStudent(model);


            if (userModel == null && model.UserType == UserType.Lecturer)
                return ProcessResults.RegistrationNotConfirmed;
            else if (userModel == null) return ProcessResults.InvalidEmailOrPassword;

            return ProcessResults.LoggedInSuccessfull;
        }

        public ProcessResult ConfirmRegistration(string hash)
        {
            var students = entities.Students.ToList();
            var lectures = entities.Lecturers.ToList();

            foreach (var lecturer in lectures)
            {
                string curHash = Security.GetHashString(lecturer.Email + lecturer.Password + UserType.Lecturer.ToString());
                if (curHash == hash)
                {
                    lecturer.Activation = (int)UserStatus.Confirmed;
                    SaveChanges();
                    return ProcessResults.RegistrationConfirmed;
                }
            }

            foreach (var student in students)
            {
                string curHash = Security.GetHashString(student.Email + student.Password + UserType.Student.ToString());
                if (curHash == hash)
                {
                    student.Activation = (int)UserStatus.Confirmed;
                    SaveChanges();
                    return ProcessResults.RegistrationConfirmed;
                }
            }

            return ProcessResults.ErrorOccured;
        }
    }
}