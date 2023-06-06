using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(BlurredImage), true)]
    [CanEditMultipleObjects]
    public class BlurredImageEditor : RawImageEditor
    {
        private SerializedProperty specifiedCameraTypeSP;
        private SerializedProperty blurRadiusSP;
        private SerializedProperty iterationSP;
        private SerializedProperty downScalingSP;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            specifiedCameraTypeSP = serializedObject.FindProperty("m_SpecifiedCameraType");
            blurRadiusSP = serializedObject.FindProperty("m_BlurRadius");
            iterationSP = serializedObject.FindProperty("m_Iteration");
            downScalingSP = serializedObject.FindProperty("m_DownScaling");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(specifiedCameraTypeSP);
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("Blur Setting", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(blurRadiusSP);
                EditorGUILayout.PropertyField(iterationSP);
                EditorGUILayout.PropertyField(downScalingSP);
            }
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}