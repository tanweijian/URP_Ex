using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BedRockEditor.UI
{
    public class UISettingsEditor : EditorWindow
    {
        internal static void Open()
        {
            UISettingsEditor window = GetWindow<UISettingsEditor>("UI Settings");
            window.ShowPopup();
        }

        private Vector2 scrollPosition;
        private SettingTreeView treeView;

        private void OnEnable()
        {
            treeView = new SettingTreeView();
        }

        private void OnGUI()
        {
            using EditorGUILayout.ScrollViewScope scrollView = new(scrollPosition, false, false, GUILayout.Width(200),
                GUILayout.Height(50));
            scrollPosition = scrollView.scrollPosition;
            treeView.OnGUI(new Rect());
        }
    }

    internal class SettingTreeViewItem : TreeViewItem
    {
    }

    internal class SettingTreeView : TreeView
    {
        public SettingTreeView() : this(new TreeViewState())
        {
        }

        private SettingTreeView(TreeViewState state) : this(state,
            new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[]
            {
                new() { headerContent = new GUIContent("ModuleName"), width = 50 },
                new() { headerContent = new GUIContent("Prefab"), width = 50 },
                new() { headerContent = new GUIContent("ModuleType"), width = 20 },
                new() { headerContent = new GUIContent("CacheType"), width = 20 },
                new() { headerContent = new GUIContent("FullScreen"), width = 20 },
            })))
        {
        }

        private SettingTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state,
            multiColumnHeader)
        {
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem() { depth = -1 };
            var children = new List<TreeViewItem>();
            
            return root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
        }
    }
}