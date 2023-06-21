using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEngine.Pool;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(BinderComponents))]
    public class BinderComponentsEditor : Editor
    {
        private SerializedProperty sp_ReferenceObjects;
        private SerializedProperty sp_ReferenceObjectKeys;
        private ReorderableList referenceObjectsList;

        private BinderComponents binderComponents;
        
        private void OnEnable()
        {
            binderComponents = target as BinderComponents;
            
            sp_ReferenceObjects = serializedObject.FindProperty("m_ReferenceObjects");
            sp_ReferenceObjectKeys = sp_ReferenceObjects.FindPropertyRelative("m_Keys");
            
            referenceObjectsList = new ReorderableList(serializedObject, sp_ReferenceObjectKeys, false, true, false, true);
            referenceObjectsList.drawHeaderCallback += OnDrawHeaderCallback;
            referenceObjectsList.drawElementCallback += OnDrawElementCallback;
            referenceObjectsList.onRemoveCallback += OnRemoveCallback;
            referenceObjectsList.drawNoneElementCallback += OnDrawNoneElementCallback;
        }

        public override void OnInspectorGUI()
        {
            DrawDragRect();
            EditorGUILayout.Space(10f);
            referenceObjectsList.displayRemove = referenceObjectsList.count > 0;
            referenceObjectsList.DoLayoutList();
        }

        private void DrawDragRect()
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
                        EditorUtility.DisplayDialog("BinderComponents", "Only accpet child of target object", "Understand");
                        return;
                    }
                    var components = ListPool<Component>.Get();
                    gameObject.GetComponents(typeof(Component), components);
                    foreach (Component component in components)
                    {
                        Debug.Log(component.GetType().Name);
                    }
                    ListPool<Component>.Release(components);
                }
            }
        }

        private void DrawComponentSelection()
        {
            
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Reference Objects", EditorStyles.boldLabel);
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
        }

        private void OnRemoveCallback(ReorderableList list)
        {
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