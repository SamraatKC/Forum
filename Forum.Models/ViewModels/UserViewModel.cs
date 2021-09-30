using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Models.ViewModels
{
   public class UserViewModel
    {
        [Required(ErrorMessage ="Please enter your First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter your Last Name")]
        public string LastName { get; set; }

        //[Remote("IsUserNameTaken","User",ErrorMessage ="UserName already taken")]
        //public string UserName { get; set; }
        [Required]    
      
        public string RoleName { get; set; }
        [Required(ErrorMessage = "Please enter your Email")]
        [Remote("IsEmailTaken", "User", ErrorMessage = "Email already taken")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your Password")]
        public string Password { get; set; }
    }
}
