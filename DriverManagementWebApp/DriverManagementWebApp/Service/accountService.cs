using System.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using DriverManagementWebApp.Models;

namespace DriverManagementWebApp.Service
{
    public class AccountService
    {
        private Entities db;
        public AccountService()
        {
            db = new Entities();
        }
        #region Login
        public bool DoLogin(string username, string password)
        {
            var user = db.accounts.Where(x => x.UserName == username && x.Password == password).FirstOrDefault();
            return user != null;
        }

        public account GetUserById(int userid)
        {
            return db.accounts.Where(x => x.UserID == userid).FirstOrDefault();
        }
        public account GetUserByUserName(string username)
        {
            return db.accounts.Where(x => x.UserName == username).FirstOrDefault();
        }

        internal bool UpdateUser(account user)
        {
            try
            {
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        #endregion

    }
}
