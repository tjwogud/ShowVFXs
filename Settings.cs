using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityModManagerNet;

namespace ShowCurrentFilters
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
			var filepath = Path.Combine(modEntry.Path, "Settings.xml");
			using (var writer = new StreamWriter(filepath))
				new XmlSerializer(GetType()).Serialize(writer, this);
		}

		public float x = 0;
		public float y = 0;
		public int textSize = 35;
		public bool boldText = false;
		public bool shadowText = true;
		[XmlIgnore]
		public TextAnchor textAlign
        {
			get {
				return (TextAnchor)mTextAlign;
            }

			set
            {
				mTextAlign = (int)value;
            }
		}
		public int mTextAlign = (int)TextAnchor.LowerLeft;
		public string textFormat1 = "{name}: {value}%";
		public string textFormat2 = "{name}: {value}";
		public string textSeparator = "{newLine}";
		public string textEmpty = "No filters applied!";
	}
}
