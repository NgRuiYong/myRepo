using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DriverManagementWebApp.Models
{
    public class UserInfoModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool IsDispatchManager { get; set; }
    }
}