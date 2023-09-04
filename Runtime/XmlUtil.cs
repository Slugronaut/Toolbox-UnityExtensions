using System;
using System.Xml;
using System.IO;
using System.Text;


namespace Peg
{
	public static class XmlUtil
	{
		/// <summary>
		/// Formats the incoming text to human-readable xml standards.
		/// </summary>
		/// <returns>The xml.</returns>
		/// <param name="text">Text.</param>
		public static string BeautyXml(string text)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);
			
			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.IndentChar = ' ';
			writer.Indentation = 4;
			
			doc.WriteContentTo(writer);
			writer.Flush();
			stream.Flush();
			
			stream.Position = 0;
			
			StreamReader reader = new StreamReader(stream);
			string xml = reader.ReadToEnd();
			return xml;
		}
	}
}

