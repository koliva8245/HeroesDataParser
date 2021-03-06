﻿using Heroes.Models;

namespace HeroesData.FileWriter.Tests.EmoticonData
{
    public class EmoticonDataOutputBase : FileOutputTestBase<Emoticon>
    {
        public EmoticonDataOutputBase()
            : base(nameof(EmoticonData))
        {
        }

        protected override void SetTestData()
        {
            Emoticon emoticon = new Emoticon()
            {
                Name = "Lunara Angry",
                Id = "lunara_angry",
                Description = new TooltipDescription("Use emoticons for lunara"),
                DescriptionLocked = new TooltipDescription("Some locked message"),
                HyperlinkId = string.Empty,
                HeroId = "Lunara",
                HeroSkinId = "LunaraWitch",
            };
            emoticon.Image.FileName = "emoticon_image.png";
            emoticon.Image.Count = 2;
            emoticon.Image.DurationPerFrame = 1000;
            emoticon.Image.Width = 34;
            emoticon.LocalizedAliases.Add(":lunaraangry:");
            emoticon.LocalizedAliases.Add(":lunaangry:");
            emoticon.LocalizedAliases.Add(":lunaangry:");
            emoticon.UniversalAliases.Add(":(");
            emoticon.UniversalAliases.Add("(:");
            emoticon.UniversalAliases.Add("(:");
            emoticon.TextureSheet.Image = "emoticon_image_texture.png";
            emoticon.TextureSheet.Rows = 1;
            emoticon.TextureSheet.Columns = 2;

            TestData.Add(emoticon);

            Emoticon emoticon2 = new Emoticon()
            {
                Name = "Lunara Sad",
                Id = "lunara_sad",
                IsHidden = true,
            };

            TestData.Add(emoticon2);
        }
    }
}
