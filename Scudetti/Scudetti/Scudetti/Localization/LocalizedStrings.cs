﻿namespace Scudetti.Localization
{
    public class LocalizedStrings
    {
        private static readonly AppResources _localizedResources = new AppResources();
        public AppResources LocalizedResources { get { return _localizedResources; } }
        public LocalizedStrings() { }
    }
}