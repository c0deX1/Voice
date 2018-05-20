using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Voice.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name ="Логин")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string Role { get; set; }
    }

    public class InformationModel
    {

        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Пол")]
        public string Sex { get; set; }
    }
}