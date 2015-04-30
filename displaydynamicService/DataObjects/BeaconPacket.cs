using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace displaydynamicService.DataObjects
{
    public class BeaconPacket: EntityData
    {
        public int BeaconId { get; set; }
        public int MinuteIndex { get; set; }
        public string MAC { get; set; }
    }
}