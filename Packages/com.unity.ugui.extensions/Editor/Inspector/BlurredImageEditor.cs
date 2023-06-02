using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(BlurredImage), true)]
    [CanEditMultipleObjects]
    public class BlurredImageEditor : RawImageEditor
    {
        private SerializedProperty _specifiedCameraType;

        protected override void OnEnable()
        {
            base.OnEnable();
            _specifiedCameraType = serializedObject.FindProperty("m_SpecifiedCameraType");
        }

        public override void OnInspectorGUI()
        {
            BlurredImage.SpecifiedCameraType type = (BlurredImage.SpecifiedCameraType)EditorGUILayout.EnumPopup("Specified Camera", (BlurredImage.SpecifiedCameraType)_specifiedCameraType.enumValueIndex);
            _specifiedCameraType.enumValueIndex = (int)type;
            serializedObject.ApplyModifiedProperties();
        }
    }
}