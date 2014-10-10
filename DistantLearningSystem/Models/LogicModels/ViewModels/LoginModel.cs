using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.ViewModels
{
    public class LoginModel
    {
        public string LoginOrEmail { get; set; }

        public string Password { get; set; }

        public UserType UserType { get; set; }
    }
}