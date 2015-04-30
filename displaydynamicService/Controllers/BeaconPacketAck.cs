using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace displaydynamicService.Controllers
{
    public class BeaconPacketAck
    {
        public bool isError { get; set; }
        public int beaconId { get; set; }

        public string message { get; set; }
        public string payload { get; set; }
    }
}