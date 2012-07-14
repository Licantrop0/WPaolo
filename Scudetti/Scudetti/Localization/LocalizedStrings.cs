namespace Scudetti.Localization
{
    public class LocalizedStrings
    {
        private static readonly AppResources _localizedResources = new AppResources();
        public AppResources LocalizedResources { get { return _localizedResources; } }

        private static readonly ShieldResources _shieldResources = new ShieldResources();
        public ShieldResources ShieldResources { get { return _shieldResources; } }

        public LocalizedStrings() { }
    }
}