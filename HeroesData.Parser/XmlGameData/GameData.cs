﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlGameData
{
    public class GameData
    {
        private readonly string ModsFolderPath;
        private readonly Dictionary<(string Catalog, string Entry, string Field), double> ScaleValueByLookupId = new Dictionary<(string Catalog, string Entry, string Field), double>();

        private GameData(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;

            Initialize();
        }

        /// <summary>
        /// Gets the number of xml files that were to added <see cref="XmlGameData"/>.
        /// </summary>
        public int XmlFileCount { get; private set; } = 0;

        /// <summary>
        /// Gets a XDocument of all the combined xml game data.
        /// </summary>
        public XDocument XmlGameData { get; private set; }

        /// <summary>
        /// Loads all the required game files.
        /// </summary>
        /// <param name="modsFolderPath">The file path of the mods folder.</param>
        /// <returns></returns>
        public static GameData Load(string modsFolderPath)
        {
            return new GameData(modsFolderPath);
        }

        /// <summary>
        /// Gets the scale value of the given lookup id.
        /// </summary>
        /// <param name="lookupId">lookupId.</param>
        /// <returns></returns>
        public double? ScaleValue((string Catalog, string Entry, string Field) lookupId)
        {
            if (ScaleValueByLookupId.TryGetValue(lookupId, out double value))
                return value;
            else
                return null;
        }

        private void Initialize()
        {
            LoadXmlFiles();
            GetLevelScalingData();
        }

        // loads xml files, order is important
        private void LoadXmlFiles()
        {
            string coreStormModFolderPath = Path.Combine(ModsFolderPath, @"core.stormmod\base.stormdata\GameData\");
            string heroesdataStormModFolderPath = Path.Combine(ModsFolderPath, @"heroesdata.stormmod\base.stormdata\GameData\");
            string oldHeroesFolderPath = Path.Combine(heroesdataStormModFolderPath, "Heroes");
            string newHeroesFolderPath = Path.Combine(ModsFolderPath, "heromods");

            XDocument xDoc = new XDocument();

            // loads all core.stormmod xml files
            foreach (string gameDataFile in Directory.GetFiles(coreStormModFolderPath))
            {
                if (Path.GetExtension(gameDataFile) != ".xml")
                    continue;

                if (xDoc.LastNode == null)
                {
                    xDoc = XDocument.Load(gameDataFile);
                    XmlFileCount++;
                }
                else
                {
                    xDoc.Root.Add(XDocument.Load(gameDataFile).Root.Elements());
                    XmlFileCount++;
                }
            }

            // load all heroesdata.stormmod xml files
            foreach (string gameDataFile in Directory.GetFiles(heroesdataStormModFolderPath))
            {
                if (Path.GetExtension(gameDataFile) != ".xml")
                    continue;

                xDoc.Root.Add(XDocument.Load(gameDataFile).Root.Elements());
                XmlFileCount++;
            }

            // old heroes
            foreach (string heroFolderPath in Directory.GetDirectories(oldHeroesFolderPath))
            {
                string heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("Data"))
                    continue;

                string xmlHeroPath = Path.Combine(oldHeroesFolderPath, heroFolder, heroFolder + ".xml");
                xDoc.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                XmlFileCount++;
            }

            // new heroes
            foreach (string heroFolderPath in Directory.GetDirectories(newHeroesFolderPath))
            {
                string heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("stormmod") || heroFolder == "herointeractions.stormmod")
                    continue;

                string heroName = heroFolder.Split('.')[0];
                string xmlHeroPath = Path.Combine(newHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}Data.xml");
                string xmlHeroNamePath = Path.Combine(newHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}.xml");
                string xmlHeroDataPath = Path.Combine(newHeroesFolderPath, heroFolder, @"base.stormdata\GameData\HeroData.xml");

                if (File.Exists(xmlHeroPath))
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                    XmlFileCount++;
                }
                else
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroNamePath).Root.Elements());
                    XmlFileCount++;
                }

                if (File.Exists(xmlHeroDataPath))
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroDataPath).Root.Elements());
                    XmlFileCount++;
                }
            }

            XmlGameData = xDoc;
        }

        private void GetLevelScalingData()
        {
            IEnumerable<XElement> levelScalingArrays = XmlGameData.Root.Descendants("LevelScalingArray");

            foreach (XElement scalingArray in levelScalingArrays)
            {
                foreach (XElement modification in scalingArray.Elements("Modifications"))
                {
                    string catalog = modification.Element("Catalog")?.Attribute("value")?.Value;
                    string entry = modification.Element("Entry")?.Attribute("value")?.Value;
                    string field = modification.Element("Field")?.Attribute("value")?.Value;
                    string value = modification.Element("Value")?.Attribute("value")?.Value;

                    if (string.IsNullOrEmpty(value))
                        continue;

                    if (ScaleValueByLookupId.ContainsKey((catalog, entry, field)))
                        ScaleValueByLookupId[(catalog, entry, field)] = double.Parse(value); // replace
                    else
                        ScaleValueByLookupId.Add((catalog, entry, field), double.Parse(value));
                }
            }
        }
    }
}