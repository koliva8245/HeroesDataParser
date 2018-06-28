﻿using Heroes.Models;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class WeaponOverride : PropertyOverrideBase<UnitWeapon>
    {
        public WeaponOverride(GameData gameData)
            : base(gameData)
        {
        }

        public WeaponOverride(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<UnitWeapon>> propertyOverrides)
        {
            if (string.IsNullOrEmpty(propertyValue))
                return;

            if (propertyName == nameof(UnitWeapon.Range))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    weapon.Range = double.Parse(propertyValue);
                });
            }
            else if (propertyName == nameof(UnitWeapon.Damage))
            {
                propertyOverrides.Add(propertyName, (weapon) =>
                {
                    weapon.Damage = GetValue(propertyValue);
                });
            }
        }
    }
}
