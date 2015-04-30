using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace displaydynamicService.DataObjects
{
    public class BeaconReport : EntityData
    {
        public int BeaconId { get; set; }
        public int Date { get; set; }
    }

    public class BeaconReportMacMinute : BeaconReport
    {
        public string Mac { get; set; }
        public bool[] MinuteMask { get; set; }
    }

    public class BeaconReportHourSummary : BeaconReport
    {
        public short[] HourMask { get; set; }
    }

    public class BeaconReportHourDetail : BeaconReportHourSummary
    {
        public string Mac { get; set; }
        public int EndpointId { get; set; }
        public bool IsNew { get; set; }
    }
}