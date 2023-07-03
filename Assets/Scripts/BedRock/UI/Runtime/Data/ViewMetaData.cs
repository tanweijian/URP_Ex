using System;
using UnityEditor;
using UnityEngine;

namespace BedRockRuntime.UI
{
    [Serializable]
    public class ViewMetaData : ISerializationCallbackReceiver
    {
        [SerializeField] private string moduleName;
        [SerializeField] private string prefabPath;
        [SerializeField] private ModuleType moduleType;
        [SerializeField] private CacheType cacheType;
        [SerializeField] private bool fullScreen;
        
#if UNITY_EDITOR
        [SerializeField] private string moduleDesc;
        [SerializeField] private string prefabGUID;
#endif

        public string ModuleName => moduleName;
        public string PrefabPath => prefabPath;
        public ModuleType ModuleType => moduleType;
        public CacheType CacheType => cacheType;
        public bool FullScreen => fullScreen;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(prefabGUID))
            {
                prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
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