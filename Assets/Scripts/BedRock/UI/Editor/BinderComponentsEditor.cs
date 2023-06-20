using UnityEngine.UI;
using UnityEditorInternal;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(BinderComponents))]
    public class BinderComponentsEditor : Editor
    {
        private SerializedProperty sp_ReferenceObjects;
        private SerializedProperty sp_ReferenceObjectKeys;
        private ReorderableList referenceObjectsList;
        
        private void OnEnable()
        {
            sp_ReferenceObjects = serializedObject.FindProperty("m_ReferenceObjects");
            sp_ReferenceObjectKeys = sp_ReferenceObjects.FindPropertyRelative("m_Keys");
            referenceObjectsList = new ReorderableList(serializedObject, sp_ReferenceObjectKeys, false, false, false, true);
        }

        public override void OnInspectorGUI()
        {
            referenceObjectsList.DoLayoutList();
        }
    }
}