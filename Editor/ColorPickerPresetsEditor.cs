using UnityEngine;
using UnityEditor;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[CustomEditor(typeof(ColorPickerPresets))]
	public class ColorPickerPresetsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var colorPickerPresets = (ColorPickerPresets)target;
			if (colorPickerPresets.saveMode != ColorPickerPresets.SaveType.JsonFile)
				return;

			string fileLocation = colorPickerPresets.JsonFilePath;

			if (!System.IO.File.Exists(fileLocation))
				return;

			if (GUILayout.Button("Open JSON file."))
			{
				Application.OpenURL(fileLocation);
			}
		}
	}
}