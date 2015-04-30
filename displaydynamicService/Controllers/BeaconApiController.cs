using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using displaydynamicService.DataObjects;
using System.Data.SqlClient;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using System.Globalization;

namespace displaydynamicService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class BeaconApiController : ApiController
    {
        private static string connStr = "Server=tcp:xrwl65jzoy.database.windows.net,1433;Database=displaydynamic;User ID=displaydynamic@xrwl65jzoy;Password=Cablebox1;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        public ApiServices Services { get; set; }

        public string Get()
        {
            return "this is a response!";
        }

        // GET api/BeaconApi
        [Route("api/GetBeacons")]
        public HttpResponseMessage GetBeacons()
        {
            var ret = new List<Beacon>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT beacon.id 'BeaconId', site_beacon.lastping 'LastPing', " +
                            " site.id 'CurrentSiteId', site.name 'CurrentSiteName' " +
                            " FROM site_beacon INNER JOIN site ON site_beacon.site_id = site.id " +
                            " INNER JOIN beacon ON beacon.id = site_beacon.beacon_id;";

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            ret.Add(new Beacon()
                            {
                                BeaconId = int.Parse(reader["BeaconId"].ToString()),
                                CurrentSiteId = int.Parse(reader["CurrentSiteId"].ToString()),
                                CurrentSiteName = reader["CurrentSiteName"].ToString(),
                                LastPing = DateTime.Parse(reader["LastPing"].ToString()) //CHECK FOR NULL!!!
                            });
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse<Exception>(HttpStatusCode.BadRequest, ex, formatJson());
            }
            return Request.CreateResponse<List<Beacon>>(HttpStatusCode.Accepted, ret, formatJson());
        }
        // GET api/BeaconApi
        [Route("api/GetBeaconReportHourDetailed")]
        public HttpResponseMessage GetBeaconReportHourDetail(int beaconId, int dateInt)
        {
            var ret = new List<BeaconReportHourDetail>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT imprint_hour.hour_0 '0', " +
                           " imprint_hour.hour_1 '1', imprint_hour.hour_2 '2', " +
                            " imprint_hour.hour_3 '3', imprint_hour.hour_4 '4', " +
                            " imprint_hour.hour_5 '5', imprint_hour.hour_6 '6', " +
                            " imprint_hour.hour_7 '7', imprint_hour.hour_8 '8', " +
                            " imprint_hour.hour_9 '9', imprint_hour.hour_10 '10', " +
                            " imprint_hour.hour_11 '11', imprint_hour.hour_12 '12', " +
                            " imprint_hour.hour_13 '13', imprint_hour.hour_14 '14', " +
                            " imprint_hour.hour_15 '15', imprint_hour.hour_16 '16', " +
                            " imprint_hour.hour_17 '17', imprint_hour.hour_18 '18', " +
                            " imprint_hour.hour_19 '19', imprint_hour.hour_20 '20', " +
                            " imprint_hour.hour_21 '21', imprint_hour.hour_22 '22', " +
                            " imprint_hour.hour_23 '23', endpoint.wlan_mac 'mac', " +
                            " endpoint.creation_dt , imprint_hour.endpoint_id" +
                            " FROM imprint_hour INNER JOIN endpoint ON endpoint.id = imprint_hour.endpoint_id " +
                            " WHERE imprint_hour.Beacon_Id = " + beaconId +
                            "   AND imprint_hour.date = " + dateInt.ToString();

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            BeaconReportHourDetail val = new BeaconReportHourDetail()
                            {
                                BeaconId = beaconId,
                                Date = dateInt,
                                HourMask = new short[24],
                                Mac = reader["mac"].ToString(),
                                EndpointId = int.Parse(reader["endpoint_id"].ToString())
                            };

                            int cdateInt = int.Parse(DateTime.Parse(reader["creation_dt"].ToString()).ToString("yyyyMMdd"));
                            val.IsNew = cdateInt == dateInt;

                            for (int i = 0; i < 24; i++)
                            {
                                val.HourMask[i] = short.Parse(reader[i].ToString()); //could be DBNull!
                            }
                            ret.Add(val);
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                //probably got DBNull, ie no data
                return Request.CreateResponse<Exception>(HttpStatusCode.BadRequest, ex, formatJson());
            }
            return Request.CreateResponse<List<BeaconReportHourDetail>>(HttpStatusCode.Accepted, ret, formatJson());
        }

        // GET api/BeaconApi
        [Route("api/GetBeaconReportHourSummary")]
        public HttpResponseMessage GetBeaconReportHourSummary(int beaconId, int dateInt)
        {
            var ret = new BeaconReportHourSummary();
            var err = doHourDetailRollup(dateInt);
            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * " +
                            // "SELECT SUM(imprint_hour.hour_0) '0', " +
                            // " SUM(imprint_hour.hour_1) '1', SUM(imprint_hour.hour_2) '2', " +
                            // " SUM(imprint_hour.hour_3) '3', SUM(imprint_hour.hour_4) '4', " +
                            // " SUM(imprint_hour.hour_5) '5', SUM(imprint_hour.hour_6) '6', " +
                            // " SUM(imprint_hour.hour_7) '7', SUM(imprint_hour.hour_8) '8', " +
                            // " SUM(imprint_hour.hour_9) '9', SUM(imprint_hour.hour_10) '10', " +
                            // " SUM(imprint_hour.hour_11) '11', SUM(imprint_hour.hour_12) '12', " +
                            // " SUM(imprint_hour.hour_13) '13', SUM(imprint_hour.hour_14) '14', " +
                            // " SUM(imprint_hour.hour_15) '15', SUM(imprint_hour.hour_16) '16', " +
                            // " SUM(imprint_hour.hour_17) '17', SUM(imprint_hour.hour_18) '18', " +
                            // " SUM(imprint_hour.hour_19) '19', SUM(imprint_hour.hour_20) '20', " +
                            // " SUM(imprint_hour.hour_21) '21', SUM(imprint_hour.hour_22) '22', " +
                            // " SUM(imprint_hour.hour_23) '23' " +
                            " FROM imprint_hour " +
                            " WHERE imprint_hour.Beacon_Id = " + beaconId +
                            "   AND imprint_hour.date = " + dateInt.ToString();

                        var reader = cmd.ExecuteReader();

                        ret = new BeaconReportHourSummary()
                        {
                            BeaconId = beaconId,
                            Date = dateInt,
                            HourMask = new short[24]
                        };

                        while (reader.Read())
                        {
                            for (int i = 0; i < 24; i++)
                            {
                                int val = int.Parse(reader["hour_" + i].ToString());

                                ret.HourMask[i] += (short)(val > 0 ? 1 : 0); //could be DBNull!
                            }
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                //probably got DBNull, ie no data
                return Request.CreateResponse<Exception>(HttpStatusCode.BadRequest, ex, formatJson());
            }
            return Request.CreateResponse<BeaconReportHourSummary>(HttpStatusCode.Accepted, ret, formatJson());
        }


        public HttpResponseMessage Post(List<BeaconPacket> packets)
        {
            if (packets == null || packets.Count == 0)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable);

            var ret = addBeaconPacket(packets);

            var response = Request.CreateResponse<BeaconPacketAck>(HttpStatusCode.Accepted, ret);
            return response;
        }

        //[Route("api/BeaconApi")]
        public HttpResponseMessage PostNew(List<BeaconPacket> packets, DateTime date)
        {
            if (packets == null || packets.Count == 0)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable);

            var ret = addBeaconPacket(packets, date);

            var response = Request.CreateResponse<BeaconPacketAck>(HttpStatusCode.Accepted, ret);
            return response;
        }

        public string Put(int beaconId2)
        {
            return "PUTIT!";
        }

        private BeaconPacketAck addBeaconPacket(List<BeaconPacket> packets)
        {
            DateTime now = DateTime.Now; //this is whereever the host is serving this page up tho...
            return addBeaconPacket(packets, now);
        }

        private BeaconPacketAck addBeaconPacket(List<BeaconPacket> packets, DateTime date)
        {
            var ret = new BeaconPacketAck()
            {
                beaconId = packets.Count
            };
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    foreach (var pkt in packets)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "exec updateEndpoint @beaconId, @minuteIndex, @mac";
                            cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@beaconId", pkt.BeaconId),
                            new SqlParameter("@minuteIndex", pkt.MinuteIndex),
                            new SqlParameter("@mac", pkt.MAC),
                            new SqlParameter("@date", date)
                        });

                            var response = cmd.ExecuteNonQuery();
                            ret.message += response.ToString();
                        }
                        //var response = Database.SqlQuery<string>("exec updateEndpoint @beaconId, @minuteIndex, @mac",
                        //    new SqlParameter("@beaconId", pkt.BeaconId),
                        //    new SqlParameter("@minuteIndex", pkt.MinuteIndex),
                        //    new SqlParameter("@mac", pkt.MAC));
                    }
                }
            }
            catch (System.Exception ex)
            {
                ret.message = ex.ToString();
                ret.isError = true;
            }
            return ret;

        }
        private JsonMediaTypeFormatter formatJson()
        {
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;

            //json.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            json.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.Formatting = Newtonsoft.Json.Formatting.Indented;
            json.ContractResolver = new CamelCasePropertyNamesContractResolver(); //Forces camelCase
            json.Culture = new CultureInfo("it-IT");

            return formatter;
        }

        private string doHourDetailRollup(int dateInt)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "exec imprintRollupHour @date";
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@date", dateInt)
                        });

                        var response = cmd.ExecuteNonQuery();
                        return response.ToString();
                    }
                }
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
