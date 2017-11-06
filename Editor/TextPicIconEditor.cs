/*
The MIT License (MIT)

Copyright (c) 2017 Play-Em

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;

namespace UnityEngine.UI.Extensions
{

	public class TextPicIconEditor : EditorWindow {
		[MenuItem("Window/UI/Extensions/TextPic Edit Icons")]
		protected static void ShowTextPicIconEditor() {
			var wnd = GetWindow<TextPicIconEditor>();
			wnd.titleContent.text = "Edit Icons in TextPic";
			wnd.Show();
		}

		private GameObject o;

		private static int columnWidth = 300;

		private string iconName;
		private Sprite icon;

		public void Swap(GameObject o) {
			#if UNITY_EDITOR
			Debug.Log("Editing icons for " + o.name);


			TextPic[] children = o.GetComponentsInChildren<TextPic>(true);
			for(int i = 0; i < children.Length; i++) {
				if (children[i] != null) {
					for (int j = 0; j < children[i].inspectorIconList.Length; j++) {
						if (!string.IsNullOrEmpty(iconName) 
						&& children[i].inspectorIconList[j].name == iconName) { 
							children[i].inspectorIconList[j].sprite = icon;
							Debug.Log("Swapped icon for " + children[i].inspectorIconList[j].name);
						}
					}
					children[i].ResetIconList();

					Debug.Log("Swapped icons for " + children[i].name);
				}
			}
			#endif
		}

		public void OnGUI() {
			GUILayout.Label("Select a GameObject to edit TextPic icons", EditorStyles.boldLabel);
			EditorGUILayout.Separator();
			GUILayout.Label("GameObject", EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();
			
			if (Selection.activeGameObject != null) {
				o = Selection.activeGameObject;
			}
			EditorGUILayout.ObjectField(o, typeof(GameObject), true);
			EditorGUI.EndChangeCheck();

			if (o != null) {
				EditorGUILayout.BeginHorizontal();

				GUILayout.Label("Icon Name:", GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				iconName = EditorGUILayout.TextField(iconName, GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();

				GUILayout.Label("New Sprite:", GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();

				icon = (Sprite)EditorGUILayout.ObjectField(icon, typeof(Sprite), false, GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Edit Icons")) {
					#if UNITY_EDITOR
					Swap(o);
					#endif
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
			}
		}
	}

}