/// Credit dakka
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1752415
/// Notes - Mod from Yilmaz Kiymaz's editor scripts presentation at Unite 2013
/// Updated ddreaper - removed Linq use, not required.

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    public class CanvasGroupActivator : EditorWindow
    {
        [MenuItem("Window/UI/Extensions/Canvas Groups Activator")]
        public static void InitWindow()
        {
            EditorWindow.GetWindow<CanvasGroupActivator>();
        }

        CanvasGroup[] canvasGroups;

        void OnEnable()
        {
            ObtainCanvasGroups();
        }

        void OnFocus()
        {
            ObtainCanvasGroups();
        }

        void ObtainCanvasGroups()
        {
            canvasGroups = GameObject.FindObjectsOfType<CanvasGroup>();
        }

        void OnGUI()
        {
            if (canvasGroups == null)
            {
                return;
            }

            GUILayout.Space(10f);
            GUILayout.Label("Canvas Groups");

            for (int i = 0; i < canvasGroups.Length; i++)
            {
                if (canvasGroups[i] == null) { continue; }

                bool initialActive = false;
                if (canvasGroups[i].alpha == 1.0f)
                    initialActive = true;

                bool active = EditorGUILayout.Toggle(canvasGroups[i].name, initialActive);
                if (active != initialActive)
                {
                    //If deactivated and initially active
                    if (!active && initialActive)
                    {
                        //Deactivate this
                        canvasGroups[i].alpha = 0f;
                        canvasGroups[i].interactable = false;
                        canvasGroups[i].blocksRaycasts = false;
                    }
                    //If activated and initially deactive
                    else if (active && !initialActive)
                    {
                        //Deactivate all others and activate this
                        HideAllGroups();

                        canvasGroups[i].alpha = 1.0f;
                        canvasGroups[i].interactable = true;
                        canvasGroups[i].blocksRaycasts = true;
                    }
                }
            }

            GUILayout.Space(5f);

            if (GUILayout.Button("Show All"))
            {
                ShowAllGroups();
            }

            if (GUILayout.Button("Hide All"))
            {
                HideAllGroups();
            }
        }

        void ShowAllGroups()
        {
            foreach (var group in canvasGroups)
            {
                if (group != null)
                {
                    group.alpha = 1.0f;
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }
            }
        }

        void HideAllGroups()
        {
            foreach (var group in canvasGroups)
            {
                if (group != null)
                {
                    group.alpha = 0;
                    group.interactable = false;
                    group.blocksRaycasts = false;
                }
            }
        }
    }
}
