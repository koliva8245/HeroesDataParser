﻿using Heroes.Models.AbilityTalents;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides.PropertyOverrides
{
    internal abstract class PropertyOverrideBase<T>
        where T : class
    {
        public void SetOverride(AbilityTalentId abilityTalentId, XElement element, Dictionary<AbilityTalentId, Dictionary<string, Action<T>>> propertyOverrideMethodByElementId)
        {
            Dictionary<string, Action<T>> propertyOverrides = new Dictionary<string, Action<T>>();

            // loop through each override child element
            foreach (XElement property in element.Elements())
            {
                string propertyName = property.Name.LocalName;
                string propertyValue = property.Attribute("value")?.Value;

                // text will override attribute value
                if (!string.IsNullOrEmpty(property.Value))
                    propertyValue = property.Value;

                // remove existing property override - duplicates will override previous
                if (propertyOverrides.ContainsKey(propertyName))
                    propertyOverrides.Remove(propertyName);

                SetPropertyValues(propertyName, propertyValue, propertyOverrides);
            }

            if (!propertyOverrideMethodByElementId.ContainsKey(abilityTalentId) && propertyOverrides.Count > 0)
                propertyOverrideMethodByElementId.Add(abilityTalentId, propertyOverrides);
        }

        protected abstract void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<T>> propertyOverrides);

        protected double GetDoubleValue(string textValue)
        {
            if (string.IsNullOrEmpty(textValue))
                return 0;

            if (double.TryParse(textValue, out double doubleValue))
                return doubleValue;
            else
                throw new ArgumentException($"{nameof(textValue)} must be a valid number");
        }
    }
}
