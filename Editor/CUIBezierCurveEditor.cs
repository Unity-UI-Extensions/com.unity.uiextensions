/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [CustomEditor(typeof(CUIBezierCurve))]
    [CanEditMultipleObjects]
    public class CUIBezierCurveEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        protected void OnSceneGUI()
        {
            CUIBezierCurve script = (CUIBezierCurve)this.target;

            if (script.ControlPoints != null)
            {
                Vector3[] controlPoints = script.ControlPoints;

                Transform handleTransform = script.transform;
                Quaternion handleRotation = script.transform.rotation;

                for (int p = 0; p < CUIBezierCurve.CubicBezierCurvePtNum; p++)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 newPt = Handles.DoPositionHandle(handleTransform.TransformPoint(controlPoints[p]), handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {

                        Undo.RecordObject(script, "Move Point");
                        EditorUtility.SetDirty(script);
                        controlPoints[p] = handleTransform.InverseTransformPoint(newPt);
                        script.Refresh();
                    }
                }

                Handles.color = Color.gray;
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[0]), handleTransform.TransformPoint(controlPoints[1]));
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[1]), handleTransform.TransformPoint(controlPoints[2]));
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[2]), handleTransform.TransformPoint(controlPoints[3]));

                int sampleSize = 10;

                Handles.color = Color.white;
                for (int s = 0; s < sampleSize; s++)
                {
                    Handles.DrawLine(handleTransform.TransformPoint(script.GetPoint((float)s / sampleSize)), handleTransform.TransformPoint(script.GetPoint((float)(s + 1) / sampleSize)));
                }

                script.EDITOR_ControlPoints = controlPoints;
            }
        }
    }
}