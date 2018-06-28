﻿using Heroes.Models;
using HeroesData.Parser.UnitData.Overrides;
using Xunit;

namespace HeroesData.Parser.Tests.Overrides.HeroOverrideTest
{
    public class AlexstraszaHeroTests : OverrideBase, IHeroOverride
    {
        private readonly string Hero = "Alexstrasza";

        public AlexstraszaHeroTests()
            : base()
        {
        }

        protected override string CHeroId => Hero;

        [Fact]
        public void CUnitOverrideTest()
        {
            Assert.False(HeroOverride.CUnitOverride.Enabled);
        }

        [Fact]
        public void EnergyOverrideTest()
        {
            Assert.True(HeroOverride.EnergyOverride.Enabled);
            Assert.Equal(0, HeroOverride.EnergyOverride.Energy);
        }

        [Fact]
        public void EnergyTypeOverrideTest()
        {
            Assert.True(HeroOverride.EnergyTypeOverride.Enabled);
            Assert.Equal(UnitEnergyType.None, HeroOverride.EnergyTypeOverride.EnergyType);
        }

        [Fact]
        public void NameOverrideTest()
        {
            Assert.False(HeroOverride.NameOverride.Enabled);
        }

        [Fact]
        public void ShortNameOverrideTest()
        {
            Assert.False(HeroOverride.ShortNameOverride.Enabled);
        }
    }
}
