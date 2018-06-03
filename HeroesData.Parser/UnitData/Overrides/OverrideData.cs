﻿using HeroesData.Parser.Models;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class OverrideData
    {
        private readonly GameData GameData;

        private Dictionary<string, HeroOverride> HeroOverridesByCHeroId = new Dictionary<string, HeroOverride>();

        private OverrideData(GameData gameData)
        {
            GameData = gameData;
            Initialize();
        }

        public static string HeroDataOverrideXmlFile => @"HeroOverrides.xml";

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData)
        {
            return new OverrideData(gameData);
        }

        /// <summary>
        /// Gets the HeroOverride for the given cHeroId. Returns null if none found.
        /// </summary>
        /// <param name="cHeroId">CHero id of hero name.</param>
        /// <returns></returns>
        public HeroOverride HeroOverride(string cHeroId)
        {
            if (HeroOverridesByCHeroId.TryGetValue(cHeroId, out HeroOverride overrideData))
                return overrideData;
            else
                return null;
        }

        private void Initialize()
        {
            XDocument cHeroDocument = XDocument.Load(HeroDataOverrideXmlFile);
            IEnumerable<XElement> cHeroes = cHeroDocument.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            foreach (XElement heroElement in cHeroes)
            {
                SetHeroOverrides(heroElement);
            }
        }

        private void SetHeroOverrides(XElement heroElement)
        {
            HeroOverride heroOverride = new HeroOverride();
            AbilityOverride abilityOverride = new AbilityOverride(GameData);
            WeaponOverride weaponOverride = new WeaponOverride(GameData);
            string cHeroId = heroElement.Attribute("id").Value;

            foreach (var dataElement in heroElement.Elements())
            {
                string elementName = dataElement.Name.LocalName;

                if (elementName == "Name")
                {
                    heroOverride.NameOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "ShortName")
                {
                    heroOverride.ShortNameOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "CUnit")
                {
                    heroOverride.CUnitOverride = (true, dataElement.Attribute("value").Value);
                }
                else if (elementName == "EnergyType")
                {
                    string energyType = dataElement.Attribute("value").Value;
                    if (Enum.TryParse(energyType, out UnitEnergyType heroEnergyType))
                        heroOverride.EnergyTypeOverride = (true, heroEnergyType);
                    else
                        heroOverride.EnergyTypeOverride = (true, UnitEnergyType.None);
                }
                else if (elementName == "Energy")
                {
                    string energyValue = dataElement.Attribute("value").Value;
                    if (int.TryParse(energyValue, out int value))
                        heroOverride.EnergyOverride = (true, value);
                    else
                        heroOverride.EnergyOverride = (true, 0);
                }
                else if (elementName == "Ability")
                {
                    string abilityId = dataElement.Attribute("id")?.Value;
                    string valid = dataElement.Attribute("valid")?.Value;
                    string add = dataElement.Attribute("add")?.Value;
                    string button = dataElement.Attribute("button")?.Value;

                    if (string.IsNullOrEmpty(abilityId))
                        continue;

                    // valid
                    if (bool.TryParse(valid, out bool result))
                    {
                        heroOverride.IsValidAbilityByAbilityId.Add(abilityId, result);

                        if (!result)
                            continue;
                    }

                    // add
                    if (bool.TryParse(add, out result))
                    {
                        heroOverride.AddedAbilitiesByAbilityId.Add(abilityId, (button, result));

                        if (!result)
                            continue;
                    }

                    // override
                    var overrideElement = dataElement.Elements("Override").FirstOrDefault();
                    if (overrideElement != null)
                        abilityOverride.SetOverride(abilityId, overrideElement, heroOverride.PropertyOverrideMethodByAbilityId);
                }
                else if (elementName == "LinkedAbilities")
                {
                    SetLinkAbilities(dataElement, heroOverride);
                }
                else if (elementName == "Weapon")
                {
                    string weaponId = dataElement.Attribute("id")?.Value;
                    string valid = dataElement.Attribute("valid")?.Value;

                    if (string.IsNullOrEmpty(weaponId))
                        continue;

                    if (bool.TryParse(valid, out bool result))
                    {
                        heroOverride.IsValidWeaponByWeaponId.Add(weaponId, result);

                        if (!result)
                            continue;
                    }

                    var overrideElement = dataElement.Elements("Override").FirstOrDefault();
                    if (overrideElement != null)
                        weaponOverride.SetOverride(weaponId, overrideElement, heroOverride.PropertyOverrideMethodByWeaponId);
                }
                else if (elementName == "HeroUnit")
                {
                    string heroUnitId = dataElement.Attribute("id")?.Value;

                    if (string.IsNullOrEmpty(heroUnitId))
                        continue;

                    AddHeroUnits(heroUnitId, dataElement, heroOverride);
                }
                else if (elementName == "ParentLink")
                {
                    heroOverride.ParentLinkOverride = (true, dataElement.Attribute("value").Value);
                }
            }

            if (!HeroOverridesByCHeroId.ContainsKey(cHeroId))
                HeroOverridesByCHeroId.Add(cHeroId, heroOverride);
        }

        private void SetLinkAbilities(XElement element, HeroOverride heroOverride)
        {
            foreach (var linkAbility in element.Elements())
            {
                string id = linkAbility.Attribute("id")?.Value;
                string elementName = linkAbility.Attribute("element")?.Value;
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(elementName))
                    continue;

                // add or update
                heroOverride.LinkedElementNamesByAbilityId[id] = elementName;
            }
        }

        private void AddHeroUnits(string elementId, XElement element, HeroOverride heroOverride)
        {
            heroOverride.HeroUnits.Add(elementId);
            SetHeroOverrides(element);
        }
    }
}
