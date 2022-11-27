/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [CustomEditor(typeof(CUIImage))]
    public class CUIImageEditor : CUIGraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIImage script = (CUIImage)this.target;

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(!(script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Tiled));
            Vector2 newCornerRatio = EditorGUILayout.Vector2Field("Corner Ratio", script.cornerPosRatio);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "Change Corner Ratio");
                EditorUtility.SetDirty(script);
                script.cornerPosRatio = newCornerRatio;
            }

            if (GUILayout.Button("Use native corner ratio"))
            {
                Undo.RecordObject(script, "Change Corner Ratio");
                EditorUtility.SetDirty(script);
                script.cornerPosRatio = script.OriCornerPosRatio;
            }

            if (script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Filled)
            {
                EditorGUILayout.HelpBox("With CUIImage, you can also adjust the size of the corners for filled or sliced Image. The grey sphere in the editor scene could also be moved to change the corner's size.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("With CUIImage, you can also adjust the size of the corners for filled or sliced Image. You need to set Image to filled or sliced to use this feature.", MessageType.Info);
            }

            EditorGUI.EndDisabledGroup();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            CUIImage script = (CUIImage)this.target;

            if (script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Tiled)
            {
                Vector3 cornerPos = Vector3.zero;//

                if (script.IsCurved)
                {
                    cornerPos = script.GetBCurveSandwichSpacePoint(script.cornerPosRatio.x, script.cornerPosRatio.y);
                }
                else
                {
                    cornerPos.x = script.cornerPosRatio.x * script.RectTrans.rect.width - script.RectTrans.pivot.x * script.RectTrans.rect.width;
                    cornerPos.y = script.cornerPosRatio.y * script.RectTrans.rect.height - script.RectTrans.pivot.y * script.RectTrans.rect.height;
                }

                Handles.color = Color.gray;
                EditorGUI.BeginChangeCheck();
#if UNITY_2022_1_OR_NEWER
                Vector3 newCornerPos = Handles.FreeMoveHandle(script.transform.TransformPoint(cornerPos), HandleUtility.GetHandleSize(script.transform.TransformPoint(cornerPos)) / 7, Vector3.one, Handles.SphereHandleCap);
#else
                Vector3 newCornerPos = Handles.FreeMoveHandle(script.transform.TransformPoint(cornerPos), script.transform.rotation, HandleUtility.GetHandleSize(script.transform.TransformPoint(cornerPos)) / 7, Vector3.one, Handles.SphereHandleCap);
#endif

                Handles.Label(newCornerPos, string.Format("Corner Mover"));

                newCornerPos = script.transform.InverseTransformPoint(newCornerPos);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(script, "Move Corner");
                    EditorUtility.SetDirty(script);

                    script.cornerPosRatio = new Vector2(newCornerPos.x, newCornerPos.y);
                    script.cornerPosRatio.x = (script.cornerPosRatio.x + script.RectTrans.pivot.x * script.RectTrans.rect.width) / script.RectTrans.rect.width;
                    script.cornerPosRatio.y = (script.cornerPosRatio.y + script.RectTrans.pivot.y * script.RectTrans.rect.height) / script.RectTrans.rect.height;
                }
            }
        }
    }
}