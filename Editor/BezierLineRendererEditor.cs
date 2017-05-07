///Credit Jason Horsburgh
///Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/127/uilinerenderer-mesh-not-updating-in-editor

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [CustomEditor(typeof(UILineRenderer))]
    public class BezierLineRendererEditor : Editor
    {
        void OnSceneGUI()
        {
            UILineRenderer curveRenderer = target as UILineRenderer;

            if (!curveRenderer || curveRenderer.Points == null || curveRenderer.Points.Length < 2)
            {
                return;
            }

            var oldMatrix = Handles.matrix;
            var transform = curveRenderer.GetComponent<RectTransform>();
            //Pivot must be 0,0 to edit
            //transform.pivot = Vector2.zero;
            Handles.matrix = transform.localToWorldMatrix;

            var points = curveRenderer.Points;
            //Need to transform points to worldspace! when set to Relative

            for (int i = 0; i < points.Length - 1; i += 2)
            {
                Handles.DrawLine(points[i], points[i + 1]);
            }

            for (int i = 0; i < points.Length; ++i)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var p = Handles.PositionHandle(points[i], Quaternion.identity);

                    if (check.changed)
                    {
                        Undo.RecordObject(curveRenderer, "Changed Curve Position");
                        curveRenderer.Points[i] = p;
                        curveRenderer.transform.gameObject.SetActive(false);
                        curveRenderer.transform.gameObject.SetActive(true);
                    }
                }
            }

            Handles.matrix = oldMatrix;
        }
    }
}