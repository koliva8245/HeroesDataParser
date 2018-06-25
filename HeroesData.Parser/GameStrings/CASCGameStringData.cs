﻿using CASCLib;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.Parser.GameStrings
{
    public class CASCGameStringData : GameStringData
    {
        private readonly CASCHandler CASCHandlerData;
        private readonly CASCFolder CASCFolderData;

        public CASCGameStringData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath)
            : base(modsFolderPath)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;

            Initialize();
        }

        public CASCGameStringData(CASCHandler cascHandler, CASCFolder cascFolder, string modsFolderPath, int? hotsBuild)
            : base(modsFolderPath, hotsBuild)
        {
            CASCHandlerData = cascHandler;
            CASCFolderData = cascFolder;

            Initialize();
        }

        protected override void ParseGameStringFiles()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, OldDescriptionsPath);

            ParseFiles(CASCHandlerData.OpenFile(((CASCFile)currentFolder.GetEntry(GameStringFile)).FullName));
            ParseNewHeroes();
        }

        protected override void ParseNewHeroes()
        {
            CASCFolder currentFolder = CASCExtensions.GetDirectory(CASCFolderData, HeroModsPath);

            foreach (KeyValuePair<string, ICASCEntry> heroFolder in currentFolder.Entries)
            {
                ICASCEntry enUsStormdata = ((CASCFolder)heroFolder.Value).GetEntry("enus.stormdata");
                ICASCEntry localizedData = ((CASCFolder)enUsStormdata).GetEntry("LocalizedData");

                ICASCEntry gameStringFile = ((CASCFolder)localizedData).GetEntry(GameStringFile);
                Stream data = CASCHandlerData.OpenFile(((CASCFile)gameStringFile).FullName);

                ParseFiles(data);
            }
        }

        private void ParseFiles(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                ReadFile(reader);
            }
        }
    }
}