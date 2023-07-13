using System;
using UnityEditor;
using UnityEngine;
using BedRockRuntime.UI;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace BedRockEditor.UI
{
    public class UISettingsEditor : EditorWindow
    {
        internal static void Open()
        {
            UISettingsEditor window = GetWindow<UISettingsEditor>("UI Settings");
            window.minSize = new Vector2(500, 500);
            window.ShowPopup();
        }

        private ViewMataDataView treeView;
        private SerializedObject settings;
        private SerializedProperty viewMetaDatas;
        private SerializedProperty atlasOutputPath;

        private void OnEnable()
        {
            string assetPath = EditorPrefs.GetString(PrefsKey.UISettingsAseetPath);
            UISettingsAsset asset = AssetDatabase.LoadAssetAtPath<UISettingsAsset>(assetPath);
            if (asset == null)
            {
                return;
            }

            settings = new SerializedObject(asset);
            viewMetaDatas = settings.FindProperty("m_ViewMetaDatas");
            atlasOutputPath = settings.FindProperty("m_AtlasOutputPath");
            treeView = new ViewMataDataView(viewMetaDatas);
        }

        private void OnDisable()
        {
            settings?.Dispose();
        }

        private void OnGUI()
        {
            if (treeView == null)
            {
                EditorGUILayout.LabelField("can't find ui settings asset");
                return;
            }

            EditorGUILayout.Space(5f);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"Atlas Outpu Path: {atlasOutputPath.stringValue}");
                if (GUILayout.Button("Select"))
                {
                    atlasOutputPath.stringValue = EditorUtility.OpenFolderPanel("Atlas Output Path","","");
                }
            }

            EditorGUILayout.Space(20f);
            using (new EditorGUILayout.ScrollViewScope(Vector2.zero, false, false, GUILayout.Width(position.width),
                       GUILayout.Height(position.height)))
            {
                treeView.OnGUI(new Rect(0, 0, position.width, position.height));
            }

            settings.ApplyModifiedProperties();
        }
    }

    internal class ViewMataDataView : TreeView
    {
        private readonly SerializedProperty viewMataData;

        public ViewMataDataView(SerializedProperty data) : this(data, new TreeViewState())
        {
        }

        private ViewMataDataView(SerializedProperty data, TreeViewState state) : this(data, state,
            new MultiColumnHeader(new MultiColumnHeaderState(
                new MultiColumnHeaderState.Column[]
                {
                    new()
                    {
                        headerContent = new GUIContent("ModuleName"), canSort = false,
                        headerTextAlignment = TextAlignment.Center, minWidth = 200, maxWidth = 300, width = 250
                    },
                    new()
                    {
                        headerContent = new GUIContent("ModuleDesc"), canSort = false,
                        headerTextAlignment = TextAlignment.Center, minWidth = 300, maxWidth = 400, width = 350
                    },
                    new()
                    {
                        headerContent = new GUIContent("Prefab"), canSort = false,
                        headerTextAlignment = TextAlignment.Center, minWidth = 200, maxWidth = 300, width = 250
                    },
                    new()
                    {
                        headerContent = new GUIContent("ModuleType"), canSort = true,
                        headerTextAlignment = TextAlignment.Center, minWidth = 150, maxWidth = 250, width = 200
                    },
                    new()
                    {
                        headerContent = new GUIContent("CacheType"), canSort = true,
                        headerTextAlignment = TextAlignment.Center, minWidth = 150, maxWidth = 250, width = 200
                    },
                    new()
                    {
                        headerContent = new GUIContent("FullScreen"), canSort = true,
                        headerTextAlignment = TextAlignment.Center, minWidth = 100, maxWidth = 100, width = 100
                    },
                    new()
                    {
                        headerContent = new GUIContent(""), canSort = false,
                        headerTextAlignment = TextAlignment.Center, minWidth = 100, maxWidth = 100, width = 100
                    }
                })))
        {
        }

        private ViewMataDataView(SerializedProperty data, TreeViewState state, MultiColumnHeader multiColumnHeader) :
            base(state,
                multiColumnHeader)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            rowHeight = EditorGUIUtility.singleLineHeight * 1.2f;
            viewMataData = data;
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            Debug.Log("BuildRoot");
            TreeViewItem root = new() { depth = -1 };
            var children = new List<TreeViewItem>();
            int size = viewMataData.arraySize;
            for (int i = 0; i < size; i++)
            {
                TreeViewItem item = new(i);
                children.Add(item);
            }

            root.children = children;
            return root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            SerializedProperty data = viewMataData.GetArrayElementAtIndex(args.item.id);
            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                Rect rect = args.GetCellRect(visibleColumnIndex);
                rect.x += 1f;
                rect.y += EditorGUIUtility.singleLineHeight * 0.1f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width -= 2f;
                int column = args.GetColumn(visibleColumnIndex);
                switch (column)
                {
                    case 0:
                        SerializedProperty moduleName = data.FindPropertyRelative("moduleName");
                        rect.width -= 2f;
                        moduleName.stringValue = EditorGUI.TextArea(rect, moduleName.stringValue);
                        break;
                    case 1:
                        SerializedProperty moduleDesc = data.FindPropertyRelative("moduleDesc");
                        moduleDesc.stringValue = EditorGUI.TextArea(rect, moduleDesc.stringValue);
                        break;
                    case 2:
                        GameObject gameObject = null;
                        SerializedProperty prefabGUID = data.FindPropertyRelative("prefabGUID");
                        if (!string.IsNullOrEmpty(prefabGUID.stringValue))
                        {
                            string path = AssetDatabase.GUIDToAssetPath(prefabGUID.stringValue);
                            if (!string.IsNullOrEmpty(path))
                            {
                                gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                            }
                        }

                        gameObject = EditorGUI.ObjectField(rect, gameObject, typeof(GameObject), false) as GameObject;
                        if (gameObject != null)
                        {
                            prefabGUID.stringValue = AssetDatabase
                                .GUIDFromAssetPath(AssetDatabase.GetAssetPath(gameObject)).ToString();
                        }

                        break;
                    case 3:
                        SerializedProperty moduleType = data.FindPropertyRelative("moduleType");
                        moduleType.enumValueIndex =
                            (int)(ModuleType)EditorGUI.EnumPopup(rect, (ModuleType)moduleType.enumValueIndex);
                        break;
                    case 4:
                        SerializedProperty cacheType = data.FindPropertyRelative("cacheType");
                        cacheType.enumValueIndex =
                            (int)(CacheType)EditorGUI.EnumPopup(rect, (CacheType)cacheType.enumValueIndex);
                        break;
                    case 5:
                        SerializedProperty fullScreen = data.FindPropertyRelative("fullScreen");
                        rect.x += rect.width / 2f - 10f;
                        fullScreen.boolValue = EditorGUI.Toggle(rect, fullScreen.boolValue);
                        break;
                    case 6:
                        if (args.selected)
                        {
                            if (GUI.Button(rect, "Delete"))
                            {
                                viewMataData.DeleteArrayElementAtIndex(args.item.id);
                                Reload();
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(column), column, null);
                }
            }
        }
    }
}