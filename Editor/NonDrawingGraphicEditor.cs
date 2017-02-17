/// Credit Slipp Douglas Thompson 
/// Sourced from - https://gist.github.com/capnslipp/349c18283f2fea316369
/// 
using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI.Extensions
{
    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[0]);
            // skipping AppearanceControlsGUI
            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}