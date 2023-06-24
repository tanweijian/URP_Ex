using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    internal static class MenuOptionsEx
    {
        private static MethodInfo placeUIElementRoot;
        private static MethodInfo getStandardResources;

        [InitializeOnLoadMethod]
        internal static void ReflectionMethod()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name == "UnityEditor.UI")
                {
                    Type menuOptions = assembly.GetType("UnityEditor.UI.MenuOptions");
                    if (menuOptions == null)
                    {
                        Debug.LogError("fuck!");
                    }

                    placeUIElementRoot = menuOptions?.GetMethod("PlaceUIElementRoot", BindingFlags.NonPublic | BindingFlags.Static);
                    getStandardResources = menuOptions?.GetMethod("GetStandardResources", BindingFlags.NonPublic | BindingFlags.Static);
                    break;
                }
            }
        }

        [MenuItem("GameObject/UI/Loop Horizontal Scroll Rect", false, 2151)]
        internal static void AddLoopHorizontalScrollRect(MenuCommand menuCommand)
        {
            if (placeUIElementRoot != null && getStandardResources != null)
            {
                DefaultControls.Resources resources = (DefaultControls.Resources)getStandardResources.Invoke(null, null);
                GameObject go = DefaultControlsEx.CreateLoopHorizontalScrollRect(resources);
                placeUIElementRoot.Invoke(null, new object[] { go, menuCommand });
            }
        }

        [MenuItem("GameObject/UI/Loop Vertical Scroll Rect", false, 2152)]
        internal static void AddLoopVerticalScrollRect(MenuCommand menuCommand)
        {
            if (placeUIElementRoot != null && getStandardResources != null)
            {
                DefaultControls.Resources resources = (DefaultControls.Resources)getStandardResources.Invoke(null, null);
                GameObject go = DefaultControlsEx.CreateLoopVerticalScrollRect(resources);
                placeUIElementRoot.Invoke(null, new object[] { go, menuCommand });
            }
        }
    }
}
