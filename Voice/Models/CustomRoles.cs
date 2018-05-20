using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Voice.Models
{
    public class AuthRolesAttribute: AuthorizeAttribute
    {
        public AuthRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
    public static class Role
    {
        public const string Patient = "Пользователь";
        public const string Foniator = "Фониатор";
    }
}