using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DriverManagementWebApp.Models
{
    public class DeliveryViewModel
    {
        public delivery Delivery { get; set; }
        public bool NotifyMe { get; set; }
        public int Delivery_Driver_ID { get; set; }
    }
}