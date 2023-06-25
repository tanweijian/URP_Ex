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
        private int bindIndex;
        private int focusedIndex;
        
        private BinderComponents binderComponents;

        private Dictionary<string, Object> dragedTargetComponents = new Dictionary<string, Object>();

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
            referenceObjectsList.headerHeight = EditorGUIUtility.singleLineHeight * 1.4f;
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

                        bindIndex = -1;
                        GetTargetComponents(gameObject, dragedTargetComponents);
                    }
                }
            }
        }

        private void AddReferenceObject(Object component)
        {
            if (bindIndex == -1)
            {
                string key = component.name;
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
            else if (focusedIndex == bindIndex)
            {
                SerializedProperty value = sp_ReferenceObjectValues.GetArrayElementAtIndex(bindIndex);
                value.objectReferenceValue = component;
            }
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

            float width = rect.width;
            const float keyLabelWidth = 30f;
            const float valueLabelWidth = 40f;
            const float spacingWidth = 20f;
            const float buttonWidth = 60f;
            float filedWidth = Mathf.Clamp((width - keyLabelWidth - spacingWidth * 2 - valueLabelWidth - buttonWidth) / 2, 5, 180);
            
            rect.height = EditorGUIUtility.singleLineHeight * 1.2f;
            EditorGUI.HelpBox(rect, "", MessageType.None);
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight * 0.1f;
            rect.x += 5f;
            rect.width = keyLabelWidth;
            EditorGUI.LabelField(rect, "key:", EditorStyles.boldLabel);
            rect.x += keyLabelWidth;
            rect.width = filedWidth;
            string key = EditorGUI.TextField(rect, sp_ReferenceObjectKeys.GetArrayElementAtIndex(index).stringValue);
            for (int i = 0; i < sp_ReferenceObjectKeys.arraySize; i++)
            {
                if (i == index || sp_ReferenceObjectKeys.GetArrayElementAtIndex(i).stringValue != key)
                {
                    continue;
                }
                key = sp_ReferenceObjectKeys.GetArrayElementAtIndex(index).stringValue;
                break;
            }
            sp_ReferenceObjectKeys.GetArrayElementAtIndex(index).stringValue = key;
            rect.x += filedWidth + spacingWidth;
            rect.width = valueLabelWidth;
            EditorGUI.LabelField(rect, "value:", EditorStyles.boldLabel);
            rect.x += valueLabelWidth;
            rect.width = filedWidth;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, sp_ReferenceObjectValues.GetArrayElementAtIndex(index), GUIContent.none);
            EditorGUI.EndDisabledGroup();
            if (isFocused)
            {
                rect.x = Mathf.Max(filedWidth + 10f + rect.x, width - buttonWidth + 20f);
                rect.width = buttonWidth;
                if (GUI.Button(rect,"ReBind"))
                {
                    bindIndex = index;
                    SerializedProperty objectReferenceValue = sp_ReferenceObjectValues.GetArrayElementAtIndex(bindIndex);
                    GameObject referenced = objectReferenceValue.objectReferenceValue as GameObject;
                    if (referenced == null)
                    {
                        Component component = objectReferenceValue.objectReferenceValue as Component;
                        if (component != null)
                        {
                            referenced = component.gameObject;
                        }
                    }
                    GetTargetComponents(referenced, dragedTargetComponents);
                }
            }
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

        private void GetTargetComponents(GameObject gameObject, IDictionary<string, Object> saved)
        {
            saved.Clear();
            if (gameObject == null)
            {
                return;
            }
            var components = ListPool<Component>.Get();
            gameObject.GetComponents(typeof(Component), components);
            string key = gameObject.GetType().FullName;
            if (key != null)
            {
                saved.Add(key, gameObject);
                foreach (Component component in components)
                {
                    string fullName = component.GetType().FullName;
                    if (fullName != null)
                    {
                        saved.Add(fullName, component);
                    }
                }
            }
            ListPool<Component>.Release(components);
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