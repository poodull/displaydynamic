using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using displaydynamicService.DataObjects;
using displaydynamicService.Models;

namespace displaydynamicService.Controllers
{
    public class BeaconPacketController : TableController<BeaconPacket>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            displaydynamicContext context = new displaydynamicContext();
            DomainManager = new EntityDomainManager<BeaconPacket>(context, Request, Services);
        }

        // GET tables/BeaconPacket
        public IQueryable<BeaconPacket> GetAllBeaconPacket()
        {
            return Query(); 
        }

        // GET tables/BeaconPacket/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<BeaconPacket> GetBeaconPacket(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/BeaconPacket/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<BeaconPacket> PatchBeaconPacket(string id, Delta<BeaconPacket> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/BeaconPacket
        public async Task<IHttpActionResult> PostBeaconPacket(BeaconPacket item)
        {
            BeaconPacket current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/BeaconPacket/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteBeaconPacket(string id)
        {
             return DeleteAsync(id);
        }
        //internal string addBeaconPacket(BeaconPacket pkt)
        //{
        //    try
        //    {
                
        //        var ret = Database.SqlQuery<string>("exec updateEndpoint @beaconId, @minuteIndex, @mac",
        //            new SqlParameter("@beaconId", pkt.BeaconId),
        //            new SqlParameter("@minuteIndex", pkt.MinuteIndex),
        //            new SqlParameter("@mac", pkt.MAC));

        //        return ret.ToString();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return ex.ToString();
        //    }

        //}

    }
}