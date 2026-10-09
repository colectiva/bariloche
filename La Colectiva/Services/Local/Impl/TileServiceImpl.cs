using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace La_Colectiva.Services.Local.Impl
{
    public class TileServiceImpl : TileService
    {

        public void UpdatePrimaryTile(NetworkStation station)
        {
            if (station == null) return;
            if (station.LastTide == null) return;
            string c = $"{station.LastTide.Value}";
            string xml = $@"
        <tile>
            <visual version=""2"">
                <!-- Tile Mediano (2x2) -->
                <binding template=""TileMedium"">
                    <text hint-style=""caption"" hint-wrap=""true"">{station.Name}</text>
                    <text hint-style=""header"">{c}</text>
                    <text hint-style=""captionSubtle"">{station.LastTideTime}</text>
                </binding>

                <!-- Tile Ancho (4x2) -->
                <binding template=""TileWide"">
                    <text hint-style=""header"">{c}</text>
                    <text hint-style=""caption"">{station.Name}</text>
                    <text hint-style=""captionSubtle"">Última medición: {station.LastTideTime}</text>
                </binding>
            </visual>
        </tile>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var notification = new TileNotification(doc);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

    }
}
