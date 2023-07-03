using System.Collections.Generic;
using UnityEngine;

namespace BedRockRuntime.UI
{
    [CreateAssetMenu(fileName = "ViewMetaAsset", menuName = "BedRock/UI ViewMeta Asset", order = 9999)]
    public class ViewMetaAsset : ScriptableObject
    {
        [SerializeField] private List<ViewMetaData> viewMetaDatas;
    }
}