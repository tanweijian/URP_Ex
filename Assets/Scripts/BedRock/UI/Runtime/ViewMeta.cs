namespace BedRockRuntime.UI
{
    public class ViewMeta
    {
        public string ModuleName { get; }
        public ModuleType ModuleType { get; }
        public CacheType CacheType { get; }
        public bool FullScreen { get; }
        
        public ViewMeta(string moduleName, ModuleType moduleType, CacheType cacheType, bool fullScreen)
        {
            ModuleName = moduleName;
            ModuleType = moduleType;
            CacheType = cacheType;
            FullScreen = fullScreen;
        }
    }

    public enum ModuleType
    {
        None,
        HUD,
        Main,
        Secondary,
        Popup,
        Overlay
    }

    public enum CacheType
    {
        Auto,
        Cache,
        DontCache
    }
}
