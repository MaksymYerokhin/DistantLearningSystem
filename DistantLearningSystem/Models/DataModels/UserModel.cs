namespace DistantLearningSystem.Models
{
    public enum UserType
    {
        Student,
        Lecturer,
        Moderator
    };

    
    public class UserModel
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string ImgSrc { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public UserType UserType { get; set; }

        public bool HasModeratorAccess()
        {
            return UserType == Models.UserType.Lecturer || UserType == Models.UserType.Moderator;
        }
    }
}