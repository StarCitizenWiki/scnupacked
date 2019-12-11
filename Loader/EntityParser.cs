using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using scdb.Xml.Entities;

namespace Loader
{
	public class EntityParser
	{
		public EntityClassDefinition Parse(string fullXmlPath, Func<string, string> onXmlLoadout)
		{
			if (!File.Exists(fullXmlPath))
			{
				Console.WriteLine("Ship entity definition file does not exist");
				return null;
			}

			return ParseShipDefinition(fullXmlPath, onXmlLoadout);
		}

		EntityClassDefinition ParseShipDefinition(string shipEntityPath, Func<string, string> onXmlLoadout)
		{
			string rootNodeName;
			using (var reader = XmlReader.Create(new StreamReader(shipEntityPath)))
			{
				reader.MoveToContent();
				rootNodeName = reader.Name;
			}

			var split = rootNodeName.Split('.');
			string className = split[split.Length - 1];

			var xml = File.ReadAllText(shipEntityPath);
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			var serialiser = new XmlSerializer(typeof(EntityClassDefinition), new XmlRootAttribute { ElementName = rootNodeName });
			using (var stream = new XmlNodeReader(doc))
			{
				var entity = (EntityClassDefinition)serialiser.Deserialize(stream);
				entity.ClassName = className;

				if (entity.Components?.SEntityComponentDefaultLoadoutParams?.loadout?.SItemPortLoadoutXMLParams != null)
				{
					entity.Components.SEntityComponentDefaultLoadoutParams.loadout.SItemPortLoadoutXMLParams.loadoutPath = onXmlLoadout(entity.Components.SEntityComponentDefaultLoadoutParams.loadout.SItemPortLoadoutXMLParams.loadoutPath);
				}

				return entity;
			}
		}
	}
}
