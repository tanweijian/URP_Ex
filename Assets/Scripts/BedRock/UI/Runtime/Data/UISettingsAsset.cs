using System.Collections.Generic;
using UnityEngine;

namespace BedRockRuntime.UI
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "BedRock/UI Settings Asset", order = 9999)]
    public class UISettingsAsset : ScriptableObject
    {
        public List<ViewMetaData> ViewMetaDatas;
    }
}
