namespace DistantLearningSystem.Models.DataModels
{
    public partial class Lecturer
    {
        public static explicit operator UserModel(Lecturer lecturer)
        {
            if (lecturer == null)
                return null;
            return new UserModel()
            {
                Id = lecturer.Id,
                Login = lecturer.Login,
                Name = lecturer.Name,
                Password = lecturer.Password,
                UserType = UserType.Lecturer,
                LastName = lecturer.LastName,
                Email = lecturer.Email,
                ImgSrc = lecturer.ImgSrc
            };
        }

    }
}