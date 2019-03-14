﻿namespace HeroesData.Parser.Overrides.DataOverrides
{
    public class MatchAwardDataOverride : IDataOverride
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public (bool Enabled, string Value) IdOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the real name.
        /// </summary>
        public (bool Enabled, string Value) NameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the hyperlink id.
        /// </summary>
        public (bool Enabled, string Value) HyperlinkIdOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the mvp screen image original file name.
        /// </summary>
        public (bool Enabled, string Value) MVPScreenImageFileNameOriginalOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the mvp screen image file name.
        /// </summary>
        public (bool Enabled, string Value) MVPScreenImageFileNameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the score screen image original file name.
        /// </summary>
        public (bool Enabled, string Value) ScoreScreenImageFileNameOriginalOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the score screen image file name.
        /// </summary>
        public (bool Enabled, string Value) ScoreScreenImageFileNameOverride { get; set; } = (false, string.Empty);

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public (bool Enabled, string Value) DescriptionOverride { get; set; } = (false, string.Empty);
    }
}
