///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.grid-raw-image

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	[CustomPropertyDrawer(typeof(GridShape))]
	public class GridShapeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUILayout.PropertyField(property);

			//property.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(property.isExpanded, label);
			if (property.isExpanded)
			{
				EditorGUI.indentLevel += 2;
				SerializedProperty size = property.FindPropertyRelative("_size");
				SerializedProperty prop = property.FindPropertyRelative("_bitArray");
				// EditorGUILayout.PropertyField(property.FindPropertyRelative("_readable"));
				// EditorGUILayout.PropertyField(size);

				EditorGUI.BeginChangeCheck();

				BitArray256 bitArray = (BitArray256)prop.boxedValue;
				float last = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 0;
				for (int j = size.vector2IntValue.y - 1; j >= 0; j--)
				{
					GUILayout.BeginHorizontal();

					for (int i = 0; i < size.vector2IntValue.x; i++)
					{
						int index = j * size.vector2IntValue.x + i;
						bool value = bitArray[(uint)index];
						bitArray[(uint)index] = EditorGUILayout.Toggle(GUIContent.none, value, GUILayout.Width(20));
					}
					GUILayout.EndHorizontal();
				}

				EditorGUIUtility.labelWidth = last;
				if (EditorGUI.EndChangeCheck())
				{
					foreach (Object targetObject in property.serializedObject.targetObjects)
					{
						Undo.RecordObject(targetObject, "GridShape flags");
						prop.boxedValue = bitArray;
					}
				}
				EditorGUI.indentLevel -= 2;
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}
