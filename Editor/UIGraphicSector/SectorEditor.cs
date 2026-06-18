///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-graphic-sector

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI.Extensions
{
	[CustomEditor(typeof(Sector)), CanEditMultipleObjects]
	public class SectorEditor : GraphicEditor
	{
		HashSet<string> _excluded = new() { "m_OnCullStateChanged", "m_RaycastPadding" };

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			DrawPropertiesExcludingCustom(serializedObject);
			if (EditorGUI.EndChangeCheck())
			{
				foreach (Sector sector in targets)
				{
					sector.SetAllDirty();
				}
			}
			serializedObject.ApplyModifiedProperties();
		}

		void DrawPropertiesExcludingCustom(SerializedObject obj)
		{
			SerializedProperty iterator = obj.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				if (DrawParentOffsetTransform(iterator)) continue;
				DrawClockwise(iterator.name);
				DrawPivotSector(iterator);
				enterChildren = false;
				if (iterator.name == "<Settings>k__BackingField")
				{
					enterChildren = true;
				}
				else
				{
					if (iterator.name == "m_Script") GUI.enabled = false;
					if (!_excluded.Contains(iterator.name))
					{
						EditorGUILayout.PropertyField(iterator, iterator.name != "<Settings>k__BackingField");
					}
					if (iterator.name == "m_Script") GUI.enabled = true;
				}
			}
		}

		bool DrawParentOffsetTransform(SerializedProperty property)
		{
			if (property.name != nameof(Settings.ParentOffsetTransform)) return false;

			var prop = serializedObject.FindProperty("<Settings>k__BackingField").FindPropertyRelative(nameof(Settings.CloneParentSectorSettings));
			var prop2 = serializedObject.FindProperty("<Settings>k__BackingField").FindPropertyRelative(nameof(Settings.ShapeSource));
			if (prop.boolValue || prop2.enumValueIndex == (int)ShapeSource.ParentOffsetTransform)
			{
				EditorGUILayout.PropertyField(property, false);
			}
			return true;
		}

		void DrawClockwise(string propertyName)
		{
			if (propertyName == nameof(Settings.Clockwise))
			{
				GUI.enabled = !serializedObject.FindProperty("<Settings>k__BackingField").FindPropertyRelative(nameof(Settings.CloneParentSectorSettings)).boolValue;
			}
			if (propertyName == nameof(Settings.GeometryResolution))
			{
				GUI.enabled = true;
			}
		}

		void DrawPivotSector(SerializedProperty property)
		{
			if (property.name != nameof(Settings.PivotToSector)) return;
			var prop = serializedObject.FindProperty("<Settings>k__BackingField").FindPropertyRelative(nameof(Settings.LocalRescaleDelta));
			if (!property.boolValue && prop.floatValue != 0)
			{
				EditorGUILayout.HelpBox("LocalRescaleDelta not 0. PivotToSector should be 'true', for valid rescale", MessageType.Warning);
			}
		}
	}
}