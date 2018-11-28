﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.UnitData.Overrides;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Data
{
    public class TalentData : AbilityTalentData
    {
        private Dictionary<string, List<string>> AbilityTalentIdsByTalentIdUpgrade = new Dictionary<string, List<string>>();

        public TalentData(GameData gameData, HeroOverride heroOverride, ParsedGameStrings parsedGameStrings, TextValueData textValueData, Localization localization)
            : base(gameData, heroOverride, parsedGameStrings, textValueData, localization)
        {
        }

        public void SetTalentData(Hero hero, XElement talentElement)
        {
            hero.Talents = hero.Talents ?? new Dictionary<string, Talent>();

            string referenceName = talentElement.Attribute("Talent").Value;
            string tier = talentElement.Attribute("Tier").Value;
            string column = talentElement.Attribute("Column").Value;

            Talent talent = new Talent
            {
                ReferenceNameId = referenceName,
                Column = int.Parse(column),
            };

            if (tier == "1")
                talent.Tier = TalentTier.Level1;
            else if (tier == "2")
                talent.Tier = TalentTier.Level4;
            else if (tier == "3")
                talent.Tier = TalentTier.Level7;
            else if (tier == "4")
                talent.Tier = TalentTier.Level10;
            else if (tier == "5")
                talent.Tier = TalentTier.Level13;
            else if (tier == "6")
                talent.Tier = TalentTier.Level16;
            else if (tier == "7")
                talent.Tier = TalentTier.Level20;
            else
                talent.Tier = TalentTier.Old;

            XElement cTalentElement = GameData.XmlGameData.Root.Elements("CTalent").FirstOrDefault(x => x.Attribute("id")?.Value == referenceName);

            // desc name
            XElement talentFaceElement = cTalentElement.Element("Face");
            if (talentFaceElement != null)
            {
                talent.FullTooltipNameId = talentFaceElement.Attribute("value").Value;

                XElement cButtonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == talent.FullTooltipNameId);
                if (cButtonElement != null)
                {
                    SetAbilityTalentName(cButtonElement, talent);
                    SetAbilityTalentIcon(cButtonElement, talent);

                    XElement talentAbilElement = cTalentElement.Elements("Abil").FirstOrDefault();
                    XElement talentActiveElement = cTalentElement.Elements("Active").FirstOrDefault();
                    if (talentAbilElement != null && talentActiveElement != null)
                    {
                        string effectId = talentAbilElement.Attribute("value").Value;

                        if (talentActiveElement.Attribute("value").Value == "1")
                            SetTooltipSubInfo(hero, effectId, talent);
                    }

                    SetTooltipDescriptions(cButtonElement, hero, talent);

                    // if not active, it shouldn't have a cooldown
                    if (talentActiveElement == null)
                        talent.Tooltip.Cooldown.CooldownTooltip = null;
                }

                SetAbilityType(hero, talent, cTalentElement);
                SetAbilityTalentLinkIds(hero, talent);
                hero.Talents.Add(referenceName, talent);
            }
        }

        /// <summary>
        /// Acquire TooltipAppender data for abilityTalentLinkIds.
        /// </summary>
        public void SetButtonTooltipAppenderData(Hero hero)
        {
            List<XElement> buttonElements = new List<XElement>();

            foreach (Ability ability in hero.Abilities.Values)
            {
                XElement buttonElement = GameData.XmlGameData.Root.Elements("CButton").FirstOrDefault(x => x.Attribute("id")?.Value == ability.ButtonName);
                if (buttonElement != null)
                    buttonElements.Add(buttonElement);
            }

            foreach (XElement buttonElement in buttonElements)
            {
                string abilityTalentId = buttonElement.Attribute("id").Value;

                IEnumerable<XElement> tooltipAppenderElements = buttonElement.Elements("TooltipAppender").Where(x => !string.IsNullOrEmpty(x.Attribute("Validator")?.Value));
                foreach (XElement tooltipAppenderElement in tooltipAppenderElements)
                {
                    string validatorId = tooltipAppenderElement.Attribute("Validator").Value;

                    XElement validatorPlayerTalentElement = GameData.XmlGameData.Root.Elements("CValidatorPlayerTalent").FirstOrDefault(x => x.Attribute("id")?.Value == validatorId);
                    if (validatorPlayerTalentElement != null)
                    {
                        string referenceNameId = validatorPlayerTalentElement.Element("Value").Attribute("value")?.Value;

                        if (AbilityTalentIdsByTalentIdUpgrade.ContainsKey(referenceNameId))
                            AbilityTalentIdsByTalentIdUpgrade[referenceNameId].Add(abilityTalentId);
                        else
                            AbilityTalentIdsByTalentIdUpgrade.Add(referenceNameId, new List<string>() { abilityTalentId });
                    }
                }
            }
        }

        private void SetAbilityType(Hero hero, Talent talent, XElement talentElement)
        {
            XElement talentTraitElement = talentElement.Element("Trait");
            XElement talentAbilElement = talentElement.Element("Abil");
            XElement talentActiveElement = talentElement.Element("Active");
            XElement talentQuestElement = talentElement.Element("QuestData");

            if (talentTraitElement != null && talentTraitElement.Attribute("value")?.Value == "1")
            {
                talent.AbilityType = AbilityType.Trait;
            }
            else if (talentAbilElement != null)
            {
                string abilValue = talentAbilElement.Attribute("value").Value;
                if (hero.Abilities.TryGetValue(abilValue, out Ability ability))
                    talent.AbilityType = ability.AbilityType;
                else
                    talent.AbilityType = AbilityType.Active;
            }
            else
            {
                talent.AbilityType = AbilityType.Passive;
            }

            if (talentActiveElement != null && talentActiveElement.Attribute("value")?.Value == "1")
                talent.IsActive = true;

            if (talentQuestElement != null && !string.IsNullOrEmpty(talentQuestElement.Attribute("StackBehavior")?.Value))
                talent.IsQuest = true;
        }

        private void SetAbilityTalentLinkIds(Hero hero, Talent talent)
        {
            if (AbilityTalentIdsByTalentIdUpgrade.TryGetValue(talent.ReferenceNameId, out List<string> abilityTalentIds))
            {
                talent.AbilityTalentLinkId = abilityTalentIds;
            }

            if (talent.AbilityType == AbilityType.Heroic)
            {
            }
        }
    }
}
