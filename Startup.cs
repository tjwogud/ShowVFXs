using HarmonyLib;
using System;
using System.IO;
using UnityModManagerNet;

namespace ShowCurrentFilters
{
    internal static class Startup
    {
        internal static void Load(UnityModManager.ModEntry modEntry)
        {
            LoadAssembly("Mods/ShowCurrentFilters/Localizations.dll");
            AccessTools.Method("ShowCurrentFilters.Main:Setup").Invoke(null, new object[] { modEntry });
        }

        internal static void LoadAssembly(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                AppDomain.CurrentDomain.Load(data);
            }
        }
    }
}
