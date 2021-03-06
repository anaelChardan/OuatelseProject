﻿using Ouatelse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ouatelse.Managers
{
    /// <summary>
    /// Classe singleton qui permet de gérér l'authentification
    /// </summary>
    public class AuthManager : BaseManager<Employee>
    {
        private static AuthManager _instance = null;

        public Employee User { get; set; }

        public static AuthManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthManager();
                    AuthManager.Instance.User = null;
                }
                return _instance;
            }
        }

        public bool Login(string username, string password)
        {
            User = EmployeeManager.Instance.First("WHERE identifiant='" + username + "' AND mot_de_passe='" + password + "'");
            if (User == null)
                return false;

            if (User.Role.Name != "Administrateur" && User.Store.Id != Properties.Settings.Default.CurrentStore.Id)
                return false;

            return true;
        }

        public void Logout()
        {
            User = null;
        }
    }
}
