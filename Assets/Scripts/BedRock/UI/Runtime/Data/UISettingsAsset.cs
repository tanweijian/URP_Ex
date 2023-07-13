using System;
using UnityEngine;
using System.Collections.Generic;

namespace BedRockRuntime.UI
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "BedRock/UI Settings Asset", order = 9999)]
    public class UISettingsAsset : ScriptableObject
    {
        [SerializeField] private string m_AtlasOutputPath;
        [SerializeField] private List<ViewMetaData> m_ViewMetaDatas;

        public string AtlasOutputPath => m_AtlasOutputPath;
        public IReadOnlyList<ViewMetaData> ViewMetaDatas => m_ViewMetaDatas;
    }
}
