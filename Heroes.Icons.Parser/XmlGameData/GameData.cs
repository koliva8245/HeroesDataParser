﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.XmlGameData
{
    public class GameData
    {
        private readonly string ModsFolderPath;

        public GameData(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
        }

        /// <summary>
        /// Gets or sets the number of xml files that were to added <see cref="XmlGameData"/>
        /// </summary>
        public int XmlFileCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the an XDoucment of game data
        /// </summary>
        public XDocument XmlGameData { get; set; }

        /// <summary>
        /// Gets or sets the scaling value by the lookup id: catalog#entry#field
        /// </summary>
        public Dictionary<string, double> ScaleValueByLookupId { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Loads all the needed xml data files
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            LoadXmlFiles();
            GetLevelScalingData();
        }

        private void LoadXmlFiles()
        {
            string gameDataFolderPath = Path.Combine(ModsFolderPath, @"heroesdata.stormmod\base.stormdata\GameData\");
            string oldHeroesFolderPath = Path.Combine(gameDataFolderPath, "Heroes");
            string newHeroesFolderPath = Path.Combine(ModsFolderPath, "heromods");

            XDocument xDoc = new XDocument();

            // load all GameData xml files
            foreach (var gameDataFile in Directory.GetFiles(gameDataFolderPath))
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

            // old heroes
            foreach (var heroFolderPath in Directory.GetDirectories(oldHeroesFolderPath))
            {
                var heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("Data"))
                    continue;

                string xmlHeroPath = Path.Combine(oldHeroesFolderPath, heroFolder, heroFolder + ".xml");
                xDoc.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                XmlFileCount++;
            }

            // new heroes
            foreach (var heroFolderPath in Directory.GetDirectories(newHeroesFolderPath))
            {
                var heroFolder = Path.GetFileName(heroFolderPath);

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

                    string id = $"{catalog}#{entry}#{field}";

                    if (ScaleValueByLookupId.ContainsKey(id))
                        ScaleValueByLookupId[id] = double.Parse(value); // replace
                    else
                        ScaleValueByLookupId.Add(id, double.Parse(value));
                }
            }
        }
    }
}