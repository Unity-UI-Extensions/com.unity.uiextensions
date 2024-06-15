/// Credit Soprachev Andrei and Shikhar Srivastava

using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Squircle")]
    public class UISquircle : UIPrimitiveBase
    {
        const float c = 1.0f;
        public enum Type
        {
            Classic,
            Scaled
        }

        [Space]
        public Type squircleType = Type.Scaled;
        [Range(0, 64)]
        public float n = 4;
        [Min(0.1f)]
        public float delta = 1f;
        [Range(0.1f, 40f)]
        public float quality = 0.1f;
        [Min(0)]
        public float radius = 32;

        [HideInInspector] public bool fillCenter = true;
        [HideInInspector] public float borderWidth = 9f;
        [HideInInspector] public Corners corners;

        private float a, b;

#if UNITY_EDITOR
        [HideInInspector] public int vertsCount = 0;
        [HideInInspector] public int trisCount = 0;
#endif

        private float SquircleFunc(float t, bool xByY)
        {
            if (xByY)
                return (float)System.Math.Pow(c - System.Math.Pow(t / a, n), 1f / n) * b;

            return (float)System.Math.Pow(c - System.Math.Pow(t / b, n), 1f / n) * a;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float dx = 0;
            float dy = 0;

            float quadrantWidth = rectTransform.rect.width / 2;
            float quadrantHeight = rectTransform.rect.height / 2;

            Vector2 pivotOffset = new Vector2((rectTransform.pivot.x - 0.5f) * rectTransform.rect.width, (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height);
            Vector2 pivotOffsetTimesTwo = pivotOffset * 2;
            Vector2 centerPoint = new Vector2(-pivotOffset.x, -pivotOffset.y);

            if (squircleType == Type.Classic)
            {
                a = quadrantWidth;
                b = quadrantHeight;
            }
            else
            {
                a = Mathf.Min(quadrantWidth, quadrantHeight, radius);
                b = a;

                dx = quadrantWidth - a;
                dy = quadrantHeight - a;
            }

            float x = 0;
            float y = 1;

            //create curved vert 1st quadrant
            List<Vector2> _topRightCurvedVert = new List<Vector2>
            {
                new Vector2(-pivotOffset.x, quadrantHeight - pivotOffset.y)
            };

            while (x < y)
            {
                y = SquircleFunc(x, true);
                _topRightCurvedVert.Add(new Vector2(dx + x - pivotOffset.x, dy + y - pivotOffset.y));
                x += delta;
            }

            if (float.IsNaN(_topRightCurvedVert[_topRightCurvedVert.Count - 1].y))
            {
                _topRightCurvedVert.RemoveAt(_topRightCurvedVert.Count - 1);
            }

            while (y > 0)
            {
                x = SquircleFunc(y, false);
                _topRightCurvedVert.Add(new Vector2(dx + x - pivotOffset.x, dy + y - pivotOffset.y));
                y -= delta;
            }

            for (int i = 1; i < _topRightCurvedVert.Count - 1; i++)
            {
                if (_topRightCurvedVert[i].x + pivotOffset.x < _topRightCurvedVert[i].y + pivotOffset.y)
                {
                    if (_topRightCurvedVert[i - 1].y - _topRightCurvedVert[i].y < quality)
                    {
                        _topRightCurvedVert.RemoveAt(i);
                        i -= 1;
                    }
                }
                else
                {
                    if (_topRightCurvedVert[i].x - _topRightCurvedVert[i - 1].x < quality)
                    {
                        _topRightCurvedVert.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            //create flat vert 1st quadrant
            List<Vector2> _topRightFlatVert = null;
            //Atleast one corner flat
            if (corners.topRight == false || corners.bottomRight == false || corners.bottomLeft == false || corners.topLeft == false)
            {
                _topRightFlatVert = new List<Vector2>
            {
                _topRightCurvedVert[0],
                new Vector2(quadrantWidth - pivotOffset.x, quadrantHeight - pivotOffset.y),
                new Vector2(quadrantWidth - pivotOffset.x,  - pivotOffset.y),
            };
            }

            List<Vector2> _vert = new List<Vector2>();
            if (corners.topRight)
            {
                _vert.AddRange(_topRightCurvedVert);
            }
            else
            {
                _vert.AddRange(_topRightFlatVert);
            }

            if (corners.bottomRight)
            {
                for (int i = _topRightCurvedVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightCurvedVert[i];
                    reversedVector.y = -reversedVector.y - pivotOffsetTimesTwo.y;
                    _vert.Add(reversedVector);
                }
            }
            else
            {
                for (int i = _topRightFlatVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.y = -reversedVector.y - pivotOffsetTimesTwo.y;
                    _vert.Add(reversedVector);
                }
            }

            if (corners.bottomLeft)
            {
                for (int i = 0; i < _topRightCurvedVert.Count; i++)
                {
                    Vector2 reversedVector = _topRightCurvedVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetTimesTwo.x;
                    reversedVector.y = -reversedVector.y - pivotOffsetTimesTwo.y;
                    _vert.Add(reversedVector);
                }
            }
            else
            {
                for (int i = 0; i < _topRightFlatVert.Count; i++)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.y = -reversedVector.y - pivotOffsetTimesTwo.y;
                    reversedVector.x = -reversedVector.x - pivotOffsetTimesTwo.x;
                    _vert.Add(reversedVector);
                }
            }

            if (corners.topLeft)
            {
                for (int i = _topRightCurvedVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector2 = _topRightCurvedVert[i];
                    reversedVector2.x = -reversedVector2.x - pivotOffsetTimesTwo.x;
                    _vert.Add(reversedVector2);
                }
            }
            else
            {
                for (int i = _topRightFlatVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetTimesTwo.x;
                    _vert.Add(reversedVector);
                }
            }

            int predefinedLength = _vert.Count * 2;
            Vector2[] vertFinal = new Vector2[predefinedLength];
            for (int i = _vert.Count - 1; i >= 0; i--)
            {
                int timesTwo = i * 2;
                vertFinal[timesTwo] = _vert[i];
                vertFinal[timesTwo + 1] = getInsidePointForAGivenOuterPoint(_vert[i], pivotOffset, centerPoint);
            }

            vh.Clear();
#if UNITY_EDITOR
            vertsCount = 0;
            trisCount = 0;
#endif
            for (int i = predefinedLength - 3; i >= 0; i--)
            {
                vh.AddVert(vertFinal[i + 2], color, Vector2.zero);
                vh.AddVert(vertFinal[i + 1], color, Vector2.zero);
                vh.AddVert(vertFinal[i], color, Vector2.zero);
                int timesThree = i * 3;
                vh.AddTriangle(timesThree, timesThree + 1, timesThree + 2);
#if UNITY_EDITOR
                vertsCount += 3;
                trisCount += 1;
#endif
            }
        }

        Vector2 getInsidePointForAGivenOuterPoint(Vector2 outerPoint, Vector2 pivotOffset, Vector2 centerPoint)
        {
            if (fillCenter == false)
            {
                Vector2 insidePoint;
                float insidePointX;
                float insidePointY;
                if (outerPoint.x > -pivotOffset.x)
                {
                    if (outerPoint.y > -pivotOffset.y)
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x - borderWidth, -pivotOffset.x, outerPoint.x);
                        insidePointY = Mathf.Clamp(outerPoint.y - borderWidth, -pivotOffset.y, outerPoint.y);
                    }
                    else if (outerPoint.y < -pivotOffset.y)
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x - borderWidth, -pivotOffset.x, outerPoint.x);
                        insidePointY = Mathf.Clamp(outerPoint.y + borderWidth, outerPoint.y, -pivotOffset.y);
                    }
                    else
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x - borderWidth, -pivotOffset.x, outerPoint.x);
                        insidePointY = outerPoint.y;
                    }

                    insidePoint = new Vector2(insidePointX, insidePointY);
                }
                else if (outerPoint.x < -pivotOffset.x)
                {
                    if (outerPoint.y > -pivotOffset.y)
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x + borderWidth, outerPoint.x, -pivotOffset.x);
                        insidePointY = Mathf.Clamp(outerPoint.y - borderWidth, -pivotOffset.y, outerPoint.y);
                    }
                    else if (outerPoint.y < -pivotOffset.y)
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x + borderWidth, outerPoint.x, -pivotOffset.x);
                        insidePointY = Mathf.Clamp(outerPoint.y + borderWidth, outerPoint.y, -pivotOffset.y);
                    }
                    else
                    {
                        insidePointX = Mathf.Clamp(outerPoint.x + borderWidth, outerPoint.x, -pivotOffset.x);
                        insidePointY = outerPoint.y;
                    }

                    insidePoint = new Vector2(insidePointX, insidePointY);
                }
                else
                {
                    if (outerPoint.y > -pivotOffset.y)
                    {
                        insidePointX = outerPoint.x;
                        insidePointY = Mathf.Clamp(outerPoint.y - borderWidth, -pivotOffset.y, outerPoint.y);
                    }
                    else if (outerPoint.y < -pivotOffset.y)
                    {
                        insidePointX = outerPoint.x;
                        insidePointY = Mathf.Clamp(outerPoint.y + borderWidth, outerPoint.y, -pivotOffset.y);
                    }
                    else
                    {
                        insidePointX = outerPoint.x;
                        insidePointY = outerPoint.y;
                    }
                    insidePoint = new Vector2(insidePointX, insidePointY);
                }
                return insidePoint;
            }
            else
            {
                return centerPoint;
            }
        }

        [System.Serializable]
        public class Corners
        {
            public bool topLeft = true;
            public bool topRight = true;
            public bool bottomLeft = true;
            public bool bottomRight = true;
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(UISquircle))]
        [CanEditMultipleObjects]
        public class UISquircleEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                DrawDefaultInspector();

                SerializedProperty fillCenterProp = serializedObject.FindProperty("fillCenter");
                SerializedProperty borderWidthProp = serializedObject.FindProperty("borderWidth");
                SerializedProperty cornersProp = serializedObject.FindProperty("corners");
                SerializedProperty vertsCountProp = serializedObject.FindProperty("vertsCount");
                SerializedProperty trisCountProp = serializedObject.FindProperty("trisCount");


                EditorGUILayout.PropertyField(fillCenterProp);
                if (!fillCenterProp.boolValue)
                {
                    // Get the RectTransform component
                    RectTransform rectTransform = ((UISquircle)target).GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // Get the width of the RectTransform
                        float maxWidth = rectTransform.rect.width;
                        //EditorGUILayout.PropertyField(borderWidthProp);
                        EditorGUILayout.Slider(borderWidthProp, 0.1f, maxWidth / 2f, new GUIContent("Border Width"));
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No RectTransform found on the target object.", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  ", GUILayout.Width(60)); // Placeholder for top-left corner
                EditorGUILayout.LabelField("Left", GUILayout.Width(60));
                EditorGUILayout.LabelField("Right", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Top", GUILayout.Width(67.5f));
                EditorGUILayout.PropertyField(cornersProp.FindPropertyRelative("topLeft"), GUIContent.none, GUILayout.Width(60));
                EditorGUILayout.PropertyField(cornersProp.FindPropertyRelative("topRight"), GUIContent.none, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Bottom", GUILayout.Width(67.5f));
                EditorGUILayout.PropertyField(cornersProp.FindPropertyRelative("bottomLeft"), GUIContent.none, GUILayout.Width(60));
                EditorGUILayout.PropertyField(cornersProp.FindPropertyRelative("bottomRight"), GUIContent.none, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Vertex count: " + vertsCountProp.intValue.ToString());
                EditorGUILayout.LabelField("Tris count: " + trisCountProp.intValue.ToString());

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}