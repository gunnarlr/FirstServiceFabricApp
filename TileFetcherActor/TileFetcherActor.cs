using TileFetcherActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using MoreMapsTileFetcher;
using System.Globalization;

namespace TileFetcherActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The ITileFetcherActor interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by TileFetcherActor objects.
    /// </remarks>
    internal class TileFetcherActor : StatelessActor, ITileFetcherActor
    {
        
        public Task<MemoryStream> FetchTile(string name, int x, int y, int zoom)
        {
            // TODO: Replace the following with your own logic.
            ActorEventSource.Current.ActorMessage(this, "Doing Work");

            string theURL = "";
            if (name.ToLowerInvariant() == "elektron")
            {
                var theTicket = GetElektronTicket();
                theURL = BuildElektronUrl(x, y, zoom, theTicket);
            }
            else
            {
                theURL = string.Format(buildKartverketURLString("topo2"), zoom, x, y);
            }

            MemoryStream memStream = FetchHttpStreamSync(theURL); // Actor is one thread. Keep stuff synchronous.
            return Task.FromResult(memStream);
        }

        #region Helpers. REFACTOR ME.

        internal string buildKartverketURLString(string mapKey)
        {
            string _wmsCacheKartverketUrlHead = @"http://opencache3.statkart.no/gatekeeper/gk/gk.open_gmaps?layers=";
            string _wmsCacheKartverketUrlTail = @"&zoom={0}&x={1}&y={2}";
            string theWMSUrl = _wmsCacheKartverketUrlHead + mapKey + _wmsCacheKartverketUrlTail;
            return theWMSUrl;
        }
        public static MemoryStream FetchHttpStreamSync(string url)
        {
            var ms = new MemoryStream();

            var request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            var theResponse = ((HttpWebResponse)response).StatusDescription;
            Stream dataStream = response.GetResponseStream();
            dataStream.CopyTo(ms);
            ms.Position = 0;
            return ms;
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
        #endregion
    }
}
