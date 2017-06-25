///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI.Extensions
{
    [CustomEditor(typeof(AccordionElement), true)]
	public class AccordionElementEditor : ToggleEditor {
	
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_MinHeight"));
			this.serializedObject.ApplyModifiedProperties();
			
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_IsOn"));
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_Interactable"));
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}