using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using MoreMapsTileFetcher;
using System.Net;
using System.IO;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebSvc.Controllers
{
    [Route("api/[controller]")]
    public class MapController : Controller
    {
        internal string buildKartverketURLString(string mapKey)
        {
            string _wmsCacheKartverketUrlHead = @"http://opencache3.statkart.no/gatekeeper/gk/gk.open_gmaps?layers=";
            string _wmsCacheKartverketUrlTail = @"&zoom={0}&x={1}&y={2}";
            string theWMSUrl = _wmsCacheKartverketUrlHead + mapKey + _wmsCacheKartverketUrlTail;
            return theWMSUrl;
        }

        public static string GetElektronTicket()
        {
            string theTicket = "empty";
            string url = "http://beta.norgeskart.no/ws/esk.py?wms.ecc_enc";

            var task = MakeAsyncRequest(url, "text/html");
            theTicket = task.Result;

            var trimmedTicket = theTicket.Trim('\"', '\n');

            return trimmedTicket;
        }

        public static Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            //request.Method = WebRequestMethods.Http.Get;
            //request.Timeout = 20000;
            request.Proxy = null;

            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }

        public static string BuildElektronUrl(int x, int y, int zoom, string theTicket)
        {
            string url = "";

            var theTileBounds = GlobalMercator.TileBounds(new GlobalMercator.Tile(x, y), zoom);
            string urlBase = "http://wms.geonorge.no/skwms1/wms.ecc_enc?LAYERS=cells&STYLES=style-id-263&FORMAT=image%2Fpng&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&SRS=EPSG%3A900913";
            string urlBBox = "&BBOX=" + theTileBounds.West.ToString(CultureInfo.InvariantCulture) + ","
                                        + theTileBounds.South.ToString(CultureInfo.InvariantCulture) + ","
                                        + theTileBounds.East.ToString(CultureInfo.InvariantCulture) + ","
                                        + theTileBounds.North.ToString(CultureInfo.InvariantCulture);
            string urlTileSize = "&WIDTH=256&HEIGHT=256";
            string urlTicket = "&ticket=" + theTicket;
            url = urlBase + urlBBox + urlTileSize + urlTicket;
            return url;
        }

        private const string defName = "empty";
        private const int defX = -1;
        private const int defY = -1;
        private const int defZoom = -1;
        // GET api/map?X=xvalue....
        [HttpGet]
        public string Get(string name = defName, int x = defX, int y = defY, int zoom = defZoom)
        {
            // Look here for mental model http://www.maptiler.org/google-maps-coordinates-tile-bounds-projection/
            //int x = 69;
            //int y = 29;
            //int zoom = 7;

            // Andenes havn
            // x =4462
            // y =1877
            // zoom=13

            if (name == defName || x == defX || y == defY || zoom == defZoom)
            {
                var helpful = string.Format("Try this: http://localhost:33005/api/map?name={0}&x={1}&y={2}&zoom={3}", "Electron", 4462, 1877, 13);
                return helpful;
            }
            else
            {
                var dill = string.Format("Hello! Will now fetch {0} map with coordinates {1}/{2}/{3}", name, x, y, zoom);
                string electronUrl = BuildElektronUrl(x, y, zoom, GetElektronTicket());
                string conventionalKartverket = string.Format(buildKartverketURLString("topo2"), zoom, x, y);
                dill = dill + "\r\n\r\n" + electronUrl + "\r\n\r\n" + conventionalKartverket;

                return dill;
            }
        }
    }
}
