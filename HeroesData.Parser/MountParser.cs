﻿using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class MountParser : ParserBase<Mount, MountDataOverride>, IParser<Mount, MountParser>
    {
        public MountParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cMountElements = GameData.Elements("CMount").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement mountElement in cMountElements)
                {
                    string id = mountElement.Attribute("id").Value;
                    if (mountElement.Element("AttributeId") != null && id != "Random")
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        public MountParser GetInstance()
        {
            return new MountParser(GameData, DefaultData);
        }

        public Mount Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement mountElement = GameData.MergeXmlElements(GameData.Elements("CMount").Where(x => x.Attribute("id")?.Value == id));
            if (mountElement == null)
                return null;

            Mount mount = new Mount()
            {
                Id = id,
            };

            SetDefaultValues(mount);
            SetMountData(mountElement, mount);

            if (mount.ReleaseDate == DefaultData.HeroData.HeroReleaseDate)
                mount.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(mount.HyperlinkId))
                mount.HyperlinkId = id;

            return mount;
        }

        private void SetMountData(XElement mountElement, Mount mount)
        {
            // parent lookup
            string parentValue = mountElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CMount").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetMountData(parentElement, mount);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.MountInfoText.Replace(DefaultData.IdPlaceHolder, mountElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    mount.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in mountElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "INFOTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        mount.Description = new TooltipDescription(text);
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        mount.SortName = text;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.MountReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.MountReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.MountReleaseDate.Year;

                    mount.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    mount.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    mount.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        mount.Rarity = heroRarity;
                    else
                        mount.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        mount.Name = text;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        mount.SearchText = text;
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    mount.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    mount.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "MOUNTCATEGORY")
                {
                    mount.MountCategory = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Mount mount)
        {
            mount.Name = GameData.GetGameString(DefaultData.MountName.Replace(DefaultData.IdPlaceHolder, mount.Id));
            mount.SortName = GameData.GetGameString(DefaultData.MountSortName.Replace(DefaultData.IdPlaceHolder, mount.Id));
            mount.Description = new TooltipDescription(GameData.GetGameString(DefaultData.MountInfoText.Replace(DefaultData.IdPlaceHolder, mount.Id)));
            mount.HyperlinkId = DefaultData.MountHyperlinkId.Replace(DefaultData.IdPlaceHolder, mount.Id);
            mount.ReleaseDate = DefaultData.MountReleaseDate;
            mount.Rarity = Rarity.None;

            mount.SearchText = GameData.GetGameString(DefaultData.MountAdditionalSearchText.Replace(DefaultData.IdPlaceHolder, mount.Id));
            if (!string.IsNullOrEmpty(mount.SearchText))
                mount.SearchText = mount.SearchText.Trim();
        }
    }
}
