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

            if (!curveRenderer || curveRenderer.drivenExternally || curveRenderer.Points == null || curveRenderer.Points.Length < 2)
            {
                return;
            }

            var oldMatrix = Handles.matrix;
            var transform = curveRenderer.GetComponent<RectTransform>();
            //Pivot must be 0,0 to edit
            //transform.pivot = Vector2.zero;
            Handles.matrix = transform.localToWorldMatrix;

            var sizeX = curveRenderer.rectTransform.rect.width;
            var sizeY = curveRenderer.rectTransform.rect.height;
            var offsetX = -curveRenderer.rectTransform.pivot.x * sizeX;
            var offsetY = -curveRenderer.rectTransform.pivot.y * sizeY;

            Vector2[] points = new Vector2[curveRenderer.Points.Length];
            for (int i = 0; i < curveRenderer.Points.Length; i++)
            {
                points[i] = curveRenderer.Points[i];
            }

            //Need to transform points to worldspace! when set to Relative
            if (curveRenderer.RelativeSize)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Vector2(points[i].x * sizeX + offsetX, points[i].y * sizeY + offsetY);
                }
            }

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
                        if (curveRenderer.RelativeSize)
                        {
                            curveRenderer.Points[i] = new Vector2((p.x - offsetX) / sizeX, (p.y - offsetY) / sizeY);
                        }
                        else
                        {
                            curveRenderer.Points[i] = p;
                        }
                        curveRenderer.transform.gameObject.SetActive(false);
                        curveRenderer.transform.gameObject.SetActive(true);
                    }
                }
            }

            Handles.matrix = oldMatrix;
        }
    }
}