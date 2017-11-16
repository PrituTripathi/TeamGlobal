using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamGlobal.Infrastructure.Model
{
    public class ScheduleResponseEnvelope
    {
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public int EnvelopeID { get; set; }
    }

    public class Latitude
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }

    public class Longitude
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }

    public class ScheduleResponseDetail
    {
        public string LloydsvesselID { get; set; }
        public string VesselVoyageID { get; set; }
        public string VesselName { get; set; }
        public string Voyage { get; set; }
        public string IMONumber { get; set; }
        public string CarrierSCAC { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CutOffDateTime { get; set; }
        public string ETD { get; set; }
        public string ETA { get; set; }
        public string RoutingVia { get; set; }
        public int TransitTimeCFSToCFS { get; set; }
        public int TransitTimePortToPort { get; set; }
        public string RoutingviaPortname { get; set; }
        public string Name { get; set; }
        public string Imo { get; set; }
        public string Country { get; set; }
        public string Yearbuilt { get; set; }
        public string Vesseltype { get; set; }
        public Latitude Latitude { get; set; }
        public Longitude Longitude { get; set; }
    }

    public class DestinationWareHouse
    {
        public string Warehousename { get; set; }
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public string State { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Contactperson { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }

    public class ScheduleResponse
    {
        public ScheduleResponseEnvelope ScheduleResponseEnvelope { get; set; }
        public IList<ScheduleResponseDetail> ScheduleResponseDetails { get; set; }
        [JsonIgnore]
        public IList<object> originWareHouse { get; set; }
        [JsonIgnore]
        public DestinationWareHouse destinationWareHouse { get; set; }
       
    }

    public class ResponceObject
    {
        public ScheduleResponse ScheduleResponse { get; set; }
    }
}
