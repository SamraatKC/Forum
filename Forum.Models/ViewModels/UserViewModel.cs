using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Models.ViewModels
{
   public class UserViewModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        //[Remote("IsUserNameTaken","User",ErrorMessage ="UserName already taken")]
        //public string UserName { get; set; }
        [Remote("IsEmailTaken", "User", ErrorMessage = "Email already taken")]
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
