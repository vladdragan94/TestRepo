using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDictionary.Models
{
    public class LoginRegisterModel
    {
        public LoginViewModel LoginModel { get; set; }

        public RegisterViewModel RegisterModel { get; set; }
    }
}