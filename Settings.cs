using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityModManagerNet;

namespace ShowVFXs
{
	public class Settings : UnityModManager.ModSettings
	{
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			var filepath = Path.Combine(modEntry.Path, "Settings.xml");
			using (var writer = new StreamWriter(filepath))
				new XmlSerializer(GetType()).Serialize(writer, this);
		}

		public bool filterEnable = true;
		public float filterX = 0;
		public float filterY = 0;
		public int filterTextSize = 35;
		public bool filterBoldText = false;
		public bool filterShadowText = true;
		public TextAnchor filterTextAlign = TextAnchor.LowerLeft;
		public FilterTextOrder filterTextOrder;
		public string filterTextFormat = "{name}: {value}%";
		public int filterIntensityDecimal = 5;
		public string filterOnOffTextFormat = "{name}: On";
		public string filterTextSeparator = "{newLine}";
		public bool filterShowTextEmpty = true;
		public string filterTextEmpty = "No filters applied!";

		public bool flashEnable = true;
		public float flashX = 0;
		public float flashY = 0;
		public int flashTextSize = 35;
		public bool flashBoldText = false;
		public bool flashShadowText = true;
		public TextAnchor flashTextAlign = TextAnchor.LowerLeft;
		public FlashPlane flashTextOrder;
		public string flashForegroundTextFormat = "Foreground Flash: <color=#{rgb}>#{rgba}</color>";
		public string flashBackgroundTextFormat = "Background Flash: <color=#{rgb}>#{rgba}</color>";
		public string flashTextSeparator = "{newLine}";
		public bool flashShowForegroundTextEmpty = true;
		public string flashForegroundTextEmpty = "Foreground Flash: Off";
		public bool flashShowBackgroundTextEmpty = true;
		public string flashBackgroundTextEmpty = "Background Flash: Off";

		public bool bloomEnable = true;
		public float bloomX = 0;
		public float bloomY = 0;
		public int bloomTextSize = 35;
		public bool bloomBoldText = false;
		public bool bloomShadowText = true;
		public TextAnchor bloomTextAlign = TextAnchor.LowerLeft;
		public string bloomTextFormat = "Bloom: Threshold {threshold}, Intensity {intensity}, Color <color=#{rgb}>#{rgba}</color>";
		public int bloomThresholdDecimal = 6;
		public int bloomIntensityDecimal = 4;
		public bool bloomShowTextEmpty = true;
		public string bloomTextEmpty = "Bloom: Off";

        public bool advFilterEnable = true;
        public float advFilterX = 0;
        public float advFilterY = 0;
        public int advFilterTextSize = 35;
        public bool advFilterBoldText = false;
        public bool advFilterShadowText = true;
        public TextAnchor advFilterTextAlign = TextAnchor.LowerLeft;
        public FilterTextOrder advFilterTextOrder;
        public string advFilterTextFormat = "{name}: On";
        public string advFilterOn = "On";
        public string advFilterOff = "Off";
        public string advFilterTextSeparator = "{newLine}";
        public bool advFilterShowTextEmpty = true;
        public string advFilterTextEmpty = "No Advanced Filters applied!";

        public bool entireEnable = false;
		public float entireX = 0;
		public float entireY = 0;
		public int entireTextSize = 35;
		public bool entireBoldText = false;
		public bool entireShadowText = true;
		public TextAnchor entireTextAlign = TextAnchor.LowerLeft;
		public string entireTextFormat = "{filterText}{newLine}{flashText}{newLine}{bloomText}";
	}
}
