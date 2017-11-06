/*
The MIT License (MIT)

Copyright (c) 2014 Play-Em

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

	public class TextPicRenameEditor : EditorWindow {
		[MenuItem("Window/UI/Extensions/TextPic Rename Icons and Text")]
		protected static void ShowTextPicRenameEditor() {
			var wnd = GetWindow<TextPicRenameEditor>();
			wnd.titleContent.text = "Rename Icon List";
			wnd.Show();
		}

		private GameObject o;

		private static int columnWidth = 300;

		private string prefix;
		private string suffix;
		private string originalText;
		private string replacementText;

		public void Rename(GameObject o) {
			#if UNITY_EDITOR
			Debug.Log("Changing icons and text for " + o.name);


			TextPic[] children = o.GetComponentsInChildren<TextPic>(true);
			for(int i = 0; i < children.Length; i++) {
				if (children[i] != null) {
					for (int j = 0; j < children[i].inspectorIconList.Length; j++) {
						if (!string.IsNullOrEmpty(originalText) 
						&& children[i].inspectorIconList[j].name.Contains(originalText)) { 
							children[i].text.Replace(originalText, replacementText);
							children[i].inspectorIconList[j].name = children[i].inspectorIconList[j].name.Replace(originalText, replacementText);
							Debug.Log("Renamed icon for " + children[i].inspectorIconList[j].name);
						}

						if (!string.IsNullOrEmpty(prefix) 
						&& !string.IsNullOrEmpty(suffix) 
						&& !children[i].inspectorIconList[j].name.StartsWith(prefix) 
						&& !children[i].inspectorIconList[j].name.EndsWith(suffix)) {
							children[i].text.Replace(children[i].inspectorIconList[j].name, prefix + children[i].inspectorIconList[j].name + suffix);
							children[i].inspectorIconList[j].name = prefix + children[i].inspectorIconList[j].name + suffix;
							Debug.Log("Renamed icon for " + children[i].inspectorIconList[j].name);
						}
					}
					children[i].ResetIconList();

					Debug.Log("Renamed icons for " + children[i].name);
				}
			}
			#endif
		}

		public void OnGUI() {
			GUILayout.Label("Select a GameObject to rename TextPic icons and text", EditorStyles.boldLabel);
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
				
				GUILayout.Label("Prefix:", GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				
				prefix = EditorGUILayout.TextField(prefix, GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();
				
				GUILayout.Label("Original Text:", GUILayout.Width(columnWidth));
				
				GUILayout.Label("Replacement Text:", GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginHorizontal();

				originalText = EditorGUILayout.TextField(originalText, GUILayout.Width(columnWidth));

				replacementText = EditorGUILayout.TextField(replacementText, GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Suffix:", GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();
				suffix = EditorGUILayout.TextField(suffix, GUILayout.Width(columnWidth));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Rename Icons and Text")) {
					#if UNITY_EDITOR
					Rename(o);
					#endif
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();
			}
		}
	}

}