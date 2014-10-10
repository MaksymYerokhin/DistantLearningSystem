
namespace DistantLearningSystem.Models.LogicModels.Services
{
    using System;

    //Этот класс будет хранить настройки для подгрузки фотографий на сервер
    public class StaticSettings
    {
        public static string UploadFolderPath
        {
            get { return "Images"; }
        }

        public static string CountriesUploadFolderPath
        {
            get { return UploadFolderPath + "/Countries/"; }
        }

        public static string ConceptIconsUploadPath
        {
            get { return UploadFolderPath + "/Concepts"; }
        }

        public static string AvatarsUploadFolderPath
        {
            get { return UploadFolderPath + "/Avatars/"; }
        }

        public static int MinUserExperince
        {
            get { return 10; }
        }

        public static string ConfirmationMessage
        {
            get
            {
                return "Для подтверждения регистрации перейдите по ссылке : ";
            }
        }

        public static String LecturerRegistration
        {
            get
            {
                return "Регистрация преподавателя";
            }
        }

        public static string ConfirmationTitle
        {
            get { return "Подтверждение регистрации"; }
        }
    }
}