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
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{

	public class TextPicIconListCopier : EditorWindow {
		[MenuItem("Window/UI/Extensions/TextPic Copy Icon Lists")]
		protected static void ShowTextPicIconListCopier() {
			var wnd = GetWindow<TextPicIconListCopier>();
			wnd.titleContent.text = "Copy Icons in TextPic";
			wnd.Show();
		}

		private List<TextPic> textPicList = new List<TextPic>();

		#if UNITY_EDITOR
		void OnSelectionChange() {
			if (Selection.objects.Length > 1 )
			{
				Debug.Log ("Length? " + Selection.objects.Length);
				textPicList.Clear();

				foreach ( Object o in Selection.objects ) {
					if ( o is GameObject ) {
						TextPic tp = ((GameObject)o).GetComponent<TextPic>();
						if (tp != null) {
							textPicList.Add(tp);
						}
					}
				}
			}
			else if (Selection.activeObject is GameObject) {
				textPicList.Clear();
				TextPic tp = ((GameObject)Selection.activeObject).GetComponent<TextPic>();
				if (tp != null) {
					textPicList.Add(tp);
				}
			} 
			else {
				textPicList.Clear();
			}
			
			this.Repaint();
		}
		#endif

		private static int columnWidth = 300;

		private TextPic textPic;

		public void Copy() {
			#if UNITY_EDITOR
			foreach(TextPic tp in textPicList) {
				if (tp != null) {
					tp.inspectorIconList = new TextPic.IconName[textPic.inspectorIconList.Length];
					textPic.inspectorIconList.CopyTo(tp.inspectorIconList, 0);

					tp.ResetIconList();

					Debug.Log("Copied icons to " + tp.name);
				}
			}
			#endif
		}

		public void OnGUI() {
			GUILayout.Label("TextPic to copy icons", EditorStyles.boldLabel);
			EditorGUILayout.Separator();
			GUILayout.Label("TextPic", EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();

			textPic = EditorGUILayout.ObjectField(textPic, typeof(TextPic), true) as TextPic;
			EditorGUI.EndChangeCheck();

			if (textPicList.Count > 0) {
				if ( textPicList.Count == 1 )
				{
					textPicList[0] = ((TextPic)EditorGUILayout.ObjectField(
						textPicList[0],
						typeof(TextPic),
						true,
						GUILayout.Width(columnWidth))
						);
				} 
				else
				{
					GUILayout.Label("Multiple TextPic: " + textPicList.Count, GUILayout.Width(columnWidth));
				}

				if (textPic != null) {

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Copy Icons")) {
						#if UNITY_EDITOR
						Copy();
						#endif
					}

					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Separator();
				}
			}
			else {
				GUILayout.Label("Please select objects that have a TextPic component", EditorStyles.boldLabel);
			}
		}
	}

}