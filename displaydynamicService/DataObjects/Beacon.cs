using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace displaydynamicService.DataObjects
{
    public class Beacon : EntityData
    {
        public int BeaconId { get; set; }
        public DateTime LastPing { get; set; }
        public int CurrentSiteId { get; set; }
        public string CurrentSiteName { get; set; }

    }

}
