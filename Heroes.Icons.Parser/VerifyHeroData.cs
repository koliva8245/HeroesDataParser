﻿using Heroes.Icons.Parser.Models;
using System.Collections.Generic;

namespace Heroes.Icons.Parser
{
    public static class VerifyHeroData
    {
        private static string HeroName;
        private static List<string> Warnings = new List<string>();

        /// <summary>
        /// Verifies the all the hero data for missing data. Returns a list of warnings.
        /// </summary>
        /// <param name="heroData">A list of all hero data</param>
        /// <returns></returns>
        public static List<string> Verify(List<Hero> heroData)
        {
            foreach (var hero in heroData)
            {
                HeroName = hero.Name;

                if (string.IsNullOrEmpty(hero.AttributeId))
                    AddWarning($"{nameof(hero.AttributeId)} is null or empty");

                if (string.IsNullOrEmpty(hero.Description))
                    AddWarning($"{nameof(hero.Description)} is null or empty");

                if (hero.Difficulty == HeroDifficulty.Unknown)
                    AddWarning($"{nameof(hero.Difficulty)} is Unknown");

                if (hero.Franchise == HeroFranchise.Unknown)
                    AddWarning($"{nameof(hero.Franchise)} is Unknown");

                if (hero.Roles[0] == HeroRole.Unknown)
                    AddWarning($"{nameof(hero.Roles)} is Unknown");

                if (hero.Abilities.Count < 1)
                    AddWarning("Hero has no abilities");

                if (hero.Talents.Count < 1)
                    AddWarning("Hero has no talents");

                if (hero.Life <= 0)
                    AddWarning($"{nameof(hero.Life)} is 0");

                if (hero.Energy > 0 && hero.EnergyType == EnergyType.None)
                    AddWarning($"{nameof(hero.Energy)} > 0 and {nameof(hero.EnergyType)} is NONE");

                if (hero.Sight <= 0)
                    AddWarning($"{nameof(hero.Sight)} is 0");

                if (hero.Speed <= 0)
                    AddWarning($"{nameof(hero.Speed)} is 0");

                if (hero.HeroWeapon.Damage <= 0)
                    AddWarning($"{nameof(hero.HeroWeapon)} {nameof(hero.HeroWeapon.Damage)} is 0");

                if (hero.HeroWeapon.Period <= 0)
                    AddWarning($"{nameof(hero.HeroWeapon)} {nameof(hero.HeroWeapon.Period)} is 0");

                if (hero.HeroWeapon.Range <= 0)
                    AddWarning($"{nameof(hero.HeroWeapon)} {nameof(hero.HeroWeapon.Range)} is 0");

                foreach (var ability in hero.Abilities)
                {
                    if (string.IsNullOrEmpty(ability.Value.IconFileName))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.IconFileName)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Name))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Name)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.ReferenceNameId))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.ReferenceNameId)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.FullDescriptionNameId))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.FullDescriptionNameId)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.ShortDescriptionNameId))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.ShortDescriptionNameId)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.ShortTooltip))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.ShortTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(ability.Value.Tooltip.FullTooltip))
                        AddWarning($"[{ability.Key}] {nameof(ability.Value.Tooltip.FullTooltip)} is null or empty");
                }

                foreach (var talent in hero.Talents)
                {
                    if (string.IsNullOrEmpty(talent.Value.IconFileName))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.IconFileName)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Name))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Name)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ReferenceNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ReferenceNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.FullDescriptionNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.FullDescriptionNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.ShortDescriptionNameId))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.ShortDescriptionNameId)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.ShortTooltip))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.ShortTooltip)} is null or empty");

                    if (string.IsNullOrEmpty(talent.Value.Tooltip.FullTooltip))
                        AddWarning($"[{talent.Key}] {nameof(talent.Value.Tooltip.FullTooltip)} is null or empty");
                }
            }

            return Warnings;
        }

        private static void AddWarning(string message)
        {
            Warnings.Add($"[{HeroName}] {message}");
        }
    }
}