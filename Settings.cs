using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityModManagerNet;

namespace ShowCurrentFilters
{
    internal class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
			var filepath = Path.Combine(modEntry.Path, "Settings.xml");
			using (var writer = new StreamWriter(filepath))
				new XmlSerializer(GetType()).Serialize(writer, this);
		}

		internal float x = 0;
		internal float y = 0;
		internal int textSize = 35;
		internal bool boldText = false;
		internal bool shadowText = true;
		[XmlIgnore]
		internal TextAnchor textAlign
        {
			get {
				return (TextAnchor)mTextAlign;
            }

			set
            {
				mTextAlign = (int)value;
            }
        }
		private int mTextAlign = (int)TextAnchor.LowerLeft;
		internal string textFormat = "{name}: {value}";
		internal string textSeparator = "{newLine}";
		internal string textEmpty = "No filters applied!";
	}
}
