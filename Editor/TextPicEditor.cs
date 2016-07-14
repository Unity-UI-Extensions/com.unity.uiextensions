/// Credit drobina, w34edrtfg, playemgames 
/// Sourced from - http://forum.unity3d.com/threads/sprite-icons-with-text-e-g-emoticons.265927/

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextPic))]
    public class TextPicEditor : UnityEditor.UI.TextEditor
    {

        private SerializedProperty ImageScalingFactorProp;
        private SerializedProperty hyperlinkColorProp;
        private SerializedProperty imageOffsetProp;
        private SerializedProperty iconList;

        protected override void OnEnable()
        {
            base.OnEnable();
            ImageScalingFactorProp = serializedObject.FindProperty("ImageScalingFactor");
            hyperlinkColorProp = serializedObject.FindProperty("hyperlinkColor");
            imageOffsetProp = serializedObject.FindProperty("imageOffset");
            iconList = serializedObject.FindProperty("inspectorIconList");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(imageOffsetProp, new GUIContent("Image Offset"));
            EditorGUILayout.PropertyField(ImageScalingFactorProp, new GUIContent("Image Scaling Factor"));
            EditorGUILayout.PropertyField(hyperlinkColorProp, new GUIContent("Hyperlink Color"));
            EditorGUILayout.PropertyField(iconList, new GUIContent("Icon List"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
