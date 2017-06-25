/// Credit tanoshimi
/// Sourced from - https://forum.unity3d.com/threads/read-only-fields.68976/
/// 
using UnityEditor;

namespace UnityEngine.UI.Extensions
{ 
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
