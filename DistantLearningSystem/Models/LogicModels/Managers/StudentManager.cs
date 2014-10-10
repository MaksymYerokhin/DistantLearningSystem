using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using DistantLearningSystem.Models.LogicModels.Services;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    using System.Configuration;

    public class StudentManager : Manager
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentManager"/> class.
        /// </summary>
        public StudentManager()
        {
            this.StudentActionLimits = new Dictionary<string, int>(5)
                                           {
                                               {
                                                   "Concept",
                                                   int.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ConceptLimit"])
                                               },
                                               {
                                                   "Connection",
                                                   int.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ConnectionLimit"])
                                               },
                                               {
                                                   "Definition",
                                                   int.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "DefinitionLimit"])
                                               },
                                               {
                                                   "Formulation",
                                                   int.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "FormulationLimit"])
                                               },
                                               {
                                                   "Classification",
                                                   int.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ClassificationLimit"])
                                               }
                                           };
            this.StudentActionRates = new Dictionary<string, double>(5)
                                           {
                                               {
                                                   "Concept",
                                                   double.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ConceptRate"])
                                               },
                                               {
                                                   "Connection",
                                                   double.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ConnectionRate"])
                                               },
                                               {
                                                   "Definition",
                                                   double.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "DefinitionRate"])
                                               },
                                               {
                                                   "Formulation",
                                                   double.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "FormulationRate"])
                                               },
                                               {
                                                   "Classification",
                                                   double.Parse(
                                                       ConfigurationManager.AppSettings[
                                                           "ClassificationRate"])
                                               }
                                           };
        }

        /// <summary>
        /// Gets the limits of student actions defined in web.config.
        /// </summary>
        public IDictionary<string, int> StudentActionLimits { get; private set; }

        /// <summary>
        /// Gets the rates of student actions defined in web.config.
        /// </summary>
        public IDictionary<string, double> StudentActionRates { get; private set; }

        public ProcessResult RegistrateStudent(
            HttpContextBase context,
            Student student,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            Func<Student, bool> func = x => x.Email == student.Email && x.Login == student.Login;
            var exists = GetStudent(func);
            student.Password = Security.GetHashString(student.Password);
            student.Activation = (int)UserStatus.Unconfirmed;
            if (exists != null)
                return ProcessResults.UserAlreadyExists;

            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                student.ImgSrc = SaveUserImage(student.Id,
                    StaticSettings.AvatarsUploadFolderPath,
                    student.Email,
                    imageUpload,
                    server);
            }

            if (!SendConfirmationMail(context, student.Email, student.Password, UserType.Student.ToString()))
                return ProcessResults.ErrorOccured;
            student.LastVisitDate = DateTime.Now;
            student.RegDate = DateTime.Now;
            var st = entities.Students.Add(student);
            SaveChanges();
            return ProcessResults.RegistrationCompleted;
        }

        /// <summary>
        /// Calculates user mark, considering all student activities, 
        /// activities` rates and limits
        /// and scales it to maxMark. 
        /// </summary>
        /// <param name="studentId">
        /// The student id.
        /// </param>
        /// <param name="maxMark">
        /// Max mark student can be given.
        /// The result is scaled using this value.
        /// </param>
        /// <returns>
        /// Scaled student mark.
        /// </returns>
        public double CalculateUserMark(int studentId, int maxMark)
        {
            var student = this.GetStudent(studentId);
            return this.CalculateUserMark(student, maxMark);
        }

        /// <summary>
        /// Calculates user mark, considering all student activities, 
        /// activities` rates and limits
        /// and scales it to maxMark. 
        /// </summary>
        /// <param name="student">
        /// Student object to calculate mark.
        /// </param>
        /// <param name="maxMark">
        /// Max mark student can be given.
        /// The result is scaled using this value.
        /// </param>
        /// <returns>
        /// Scaled student mark.
        /// </returns>
        public double CalculateUserMark(Student student, int maxMark)
        {
            double result = 0;
            double studentTotalInternalMark = 0;

            var maxUserInternatMark = this.GetMaxStudentActions() * 5;

            var rate = (double)maxMark / (double)maxUserInternatMark;

            studentTotalInternalMark = this.CalculateInternalStudentMark(student);

            return studentTotalInternalMark * rate;
        }

        public void SetLastVisitDate(int studentId, DateTime date)
        {
            var student = GetStudent(x => x.Id == studentId);
            student.LastVisitDate = date;
            SaveChanges();
        }

        public Student LogInStudent(LoginModel model)
        {
            var find = entities.Students.ToList().FirstOrDefault(x =>
                (x.Login == model.LoginOrEmail ||
                x.Email == model.LoginOrEmail) &&
                model.Password == x.Password);

            if (find == null)
                return null;

            UpdateLastVisitDate(find);
            SaveChanges();
            return find;
        }

        public void UpdateLastVisitDate(int id)
        {
            var std = GetStudent(id);
            if (std == null)
                return;
            std.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public void UpdateLastVisitDate(Student student)
        {
            student.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public Student GetStudent(int id)
        {
            return entities.Students.FirstOrDefault(x => x.Id == id);
        }

        public Student GetStudent(Func<Student, bool> predicate, bool confirmedOnly = false)
        {
            foreach (var student in entities.Students.ToList())
            {
                if (confirmedOnly)
                {
                    if (predicate(student) && (UserStatus)student.Activation == UserStatus.Confirmed)
                        return student;
                }
                else
                {
                    if (predicate(student))
                        return student;
                }
            }

            return null;
        }

        public bool RemoveStudent(int studentId)
        {
            var studentToRemove = entities.Students.FirstOrDefault(x => x.Id == studentId);
            if (studentToRemove == null)
                return false;

            entities.Students.Remove(studentToRemove);
            SaveChanges();
            return true;
        }

        public IEnumerable<Student> GetStudents()
        {
            return entities.Students.ToList();
        }

        public ProcessResult EditStudent(Student newStudent,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            var studentToEdit = entities.Students.FirstOrDefault(x => x.Id == newStudent.Id);
            if (studentToEdit == null)
                return ProcessResults.ErrorOccured;

            studentToEdit.Login = newStudent.Login;
            studentToEdit.Name = newStudent.Name;
            if (!String.IsNullOrEmpty(newStudent.Password))
                studentToEdit.Password = Security.GetHashString(newStudent.Password);
            studentToEdit.Email = newStudent.Email;
            studentToEdit.LastName = newStudent.LastName;
            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                if (studentToEdit.ImgSrc != null)
                    DeleteImage(studentToEdit.ImgSrc, server);
                studentToEdit.ImgSrc = SaveUserImage(studentToEdit.Id,
                    StaticSettings.AvatarsUploadFolderPath,
                    studentToEdit.Email,
                    imageUpload,
                    server);
            }
            else if (!String.IsNullOrEmpty(studentToEdit.ImgSrc))
            {
                DeleteImage(studentToEdit.ImgSrc, server);
                studentToEdit.ImgSrc = null;
            }

            SaveChanges();
            return ProcessResults.ProfileEditedSuccessfully;
        }
        public StudentConnection AddStudentConnection(int studentId, int connectionId)
        {
            var studConnection = entities.StudentConnections.Add(new StudentConnection()
            {
                AddedDate = DateTime.Now,
                ConnectionId = connectionId,
                StudentId = studentId
            });

            SaveChanges();
            return studConnection;
        }

        public bool RemoveStudentConnection(int studentConnectionId)
        {
            var connectionToRemove = entities.StudentConnections.FirstOrDefault(x => x.Id == studentConnectionId);
            if (connectionToRemove == null)
                return false;

            entities.StudentConnections.Remove(connectionToRemove);
            SaveChanges();
            return true;
        }

        /// <summary>
        /// Calculates total student internal mark 
        /// considering web.config rates
        /// </summary>
        /// <param name="student">
        /// The student.
        /// </param>
        /// <returns>
        /// total student internal mark
        /// </returns>
        private double CalculateInternalStudentMark(Student student)
        {
            var studentTotalInternalMark =
                this.StudentActionRates["Concept"] *
                student.Classifications.Aggregate(
                0,
                (total, classification) => classification.Rating != null ? (total += classification.Rating.Value) : 0);

            studentTotalInternalMark +=
                this.StudentActionRates["Concept"] *
                student.Concepts.Aggregate(
                0,
                (total, concepts) => concepts.Rating != null ? (total += concepts.Rating.Value) : 0);

            studentTotalInternalMark +=
                this.StudentActionRates["Definition"] *
                student.Definitions.Aggregate(
                0,
                (total, definitions) => definitions.Rating != null ? (total += definitions.Rating.Value) : 0);

            studentTotalInternalMark +=
                this.StudentActionRates["Formulation"] *
                student.Formulations.Aggregate(
                0,
                (total, formulations) => formulations.Rating != null ? (total += formulations.Rating.Value) : 0);

            studentTotalInternalMark +=
                this.StudentActionRates["Classification"] *
                student.StudentConnections.Aggregate(
                0,
                (total, studentConnections) => studentConnections.Rating != null ? (total += studentConnections.Rating.Value) : 0);

            return studentTotalInternalMark;
        }

        /// <summary>
        /// Calculates maximum student internal mark 
        /// considering web.config limitations and rates
        /// </summary>
        /// <returns>
        /// maximum possible student internal mark
        /// </returns>
        private double GetMaxStudentActions()
        {
            double maxStudentActions =
                this.StudentActionLimits["Concept"] *
                this.StudentActionRates["Concept"];

            maxStudentActions +=
                this.StudentActionLimits["Connection"] *
                this.StudentActionRates["Connection"];

            maxStudentActions +=
                this.StudentActionLimits["Definition"] *
                this.StudentActionRates["Definition"];

            maxStudentActions +=
                this.StudentActionLimits["Formulation"] *
                this.StudentActionRates["Formulation"];

            maxStudentActions +=
                this.StudentActionLimits["Classification"] *
                this.StudentActionRates["Classification"];
            return maxStudentActions;
        }
    }
}