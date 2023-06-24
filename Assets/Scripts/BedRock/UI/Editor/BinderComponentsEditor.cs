using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(BinderComponents))]
    public class BinderComponentsEditor : Editor
    {
        private SerializedProperty sp_ReferenceObjects;
        private SerializedProperty sp_ReferenceObjectKeys;
        private SerializedProperty sp_ReferenceObjectValues;
        private ReorderableList referenceObjectsList;
        private int focusedIndex;
        
        private BinderComponents binderComponents;

        private Dictionary<string, Component> dragedTargetComponents = new Dictionary<string, Component>();

        private void OnEnable()
        {
            binderComponents = target as BinderComponents;

            sp_ReferenceObjects = serializedObject.FindProperty("m_ReferenceObjects");
            sp_ReferenceObjectKeys = sp_ReferenceObjects.FindPropertyRelative("m_Keys");
            sp_ReferenceObjectValues = sp_ReferenceObjects.FindPropertyRelative("m_Values");

            referenceObjectsList = new ReorderableList(serializedObject, sp_ReferenceObjectKeys, false, true, false, true);
            referenceObjectsList.drawHeaderCallback += OnDrawHeaderCallback;
            referenceObjectsList.drawElementCallback += OnDrawElementCallback;
            referenceObjectsList.onRemoveCallback += OnRemoveCallback;
            referenceObjectsList.drawNoneElementCallback += OnDrawNoneElementCallback;
        }

        private void OnDisable()
        {
            dragedTargetComponents.Clear();
        }

        public override void OnInspectorGUI()
        {
            DrawDragRect();
            EditorGUILayout.Space(10f);
            referenceObjectsList.displayRemove = referenceObjectsList.count > 0;
            referenceObjectsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDragRect()
        {
            if (dragedTargetComponents.Count > 0)
            {
                bool selected = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (var component in dragedTargetComponents)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(component.Key, GUILayout.Width(250)))
                    {
                        AddReferenceObject(component.Value);
                        selected = true;
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5f);
                if (GUILayout.Button("Clear", GUILayout.Width(80f)))
                {
                    selected = true;
                }

                if (selected)
                {
                    dragedTargetComponents.Clear();
                }
            }
            else
            {
                Rect dragRect = EditorGUILayout.GetControlRect(GUILayout.Height(100));
                EditorGUI.LabelField(dragRect, "Drag Reference Object Here", EditorStyles.centeredGreyMiniLabel);
                EditorGUI.HelpBox(dragRect, "", MessageType.None);
                Event current = Event.current;
                if (dragRect.Contains(current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (current.type == EventType.DragPerform)
                    {
                        current.Use();
                        DragAndDrop.AcceptDrag();
                        Object @object = DragAndDrop.objectReferences[0];
                        if (EditorUtility.IsPersistent(@object))
                        {
                            EditorUtility.DisplayDialog("BinderComponents", "Only accpet scene object", "Understand");
                            return;
                        }

                        GameObject gameObject = @object as GameObject;
                        if (gameObject == null)
                        {
                            return;
                        }

                        if (!IsChildOfTarget(gameObject.transform))
                        {
                            EditorUtility.DisplayDialog("BinderComponents", "Only accpet child of target object",
                                "Understand");
                            return;
                        }

                        var components = ListPool<Component>.Get();
                        gameObject.GetComponents(typeof(Component), components);
                        dragedTargetComponents.Clear();
                        foreach (Component component in components)
                        {
                            string fullName = component.GetType().FullName;
                            if (fullName != null)
                            {
                                dragedTargetComponents.Add(fullName, component);
                            }
                        }

                        ListPool<Component>.Release(components);
                    }
                }
            }
        }

        private void AddReferenceObject(Component component)
        {
            string key = component.gameObject.name;
            int arraySize = sp_ReferenceObjectKeys.arraySize;
            for (int i = 0; i < sp_ReferenceObjectKeys.arraySize; i++)
            {
                if (sp_ReferenceObjectKeys.GetArrayElementAtIndex(i).stringValue == key)
                {
                    key += component.GetInstanceID();
                    break;
                }
            }
            sp_ReferenceObjectKeys.InsertArrayElementAtIndex(arraySize);
            SerializedProperty newKey = sp_ReferenceObjectKeys.GetArrayElementAtIndex(arraySize);
            newKey.stringValue = key;
            sp_ReferenceObjectValues.InsertArrayElementAtIndex(arraySize);
            SerializedProperty newValue = sp_ReferenceObjectValues.GetArrayElementAtIndex(arraySize);
            newValue.objectReferenceValue = component;
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Reference Objects", EditorStyles.boldLabel);
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                focusedIndex = index;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            rect.width = 30f;
            EditorGUI.LabelField(rect, "key:", EditorStyles.boldLabel);
            rect.x += 30f;
            rect.width = 200f;
            string key = EditorGUI.TextField(rect, sp_ReferenceObjectKeys.GetArrayElementAtIndex(index).stringValue);
            rect.x += 250f;
            rect.width = 50f;
            EditorGUI.LabelField(rect, "value:", EditorStyles.boldLabel);
            rect.x += 50f;
            rect.width = 200f;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, sp_ReferenceObjectValues.GetArrayElementAtIndex(index), GUIContent.none);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            sp_ReferenceObjectKeys.DeleteArrayElementAtIndex(focusedIndex);
            sp_ReferenceObjectValues.DeleteArrayElementAtIndex(focusedIndex);
        }

        private void OnDrawNoneElementCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "No Reference Object", EditorStyles.boldLabel);
        }

        private bool IsChildOfTarget(Transform transform)
        {
            while (transform != null)
            {
                if (transform == binderComponents.transform)
                {
                    return true;
                }

                transform = transform.parent;
            }

            return false;
        }
    }
}