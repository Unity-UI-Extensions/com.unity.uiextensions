/// Credit Soprachev Andrei

using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Squircle")]
    public class UISquircle : UIPrimitiveBase
    {
        const float C = 1.0f;
        public enum Type
        {
            Classic,
            Scaled
        }

        [Space]
        public Type squircleType = Type.Scaled;
        [Range(1, 40)]
        public float n = 4;
        [Min(0.1f)]
        public float delta = 0.5f;
        public float quality = 0.1f;
        [Min(0)]
        public float radius = 32;
        public Corners corners;

        private float a, b;
        private List<Vector2> vert = new List<Vector2>();

        private float SquircleFunc(float t, bool xByY)
        {
            if (xByY)
                return (float)System.Math.Pow(C - System.Math.Pow(t / a, n), 1f / n) * b;

            return (float)System.Math.Pow(C - System.Math.Pow(t / b, n), 1f / n) * a;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float dx = 0;
            float dy = 0;

            float width = rectTransform.rect.width / 2;
            float height = rectTransform.rect.height / 2;

            // Adjust vertices based on pivot offset
            Vector2 pivotOffset = new Vector2(rectTransform.pivot.x - 0.5f, rectTransform.pivot.y - 0.5f);
            float pivotOffsetX = pivotOffset.x * rectTransform.rect.width;
            float pivotOffsetXTimesTwo = pivotOffsetX * 2;
            float pivotOffsetY = pivotOffset.y * rectTransform.rect.height;
            float pivotOffsetYTimesTwo = pivotOffsetY * 2;

            if (squircleType == Type.Classic)
            {
                a = width;
                b = height;
            }
            else
            {
                a = Mathf.Min(width, height, radius);
                b = a;

                dx = width - a;
                dy = height - a;
            }

            float x = 0;
            float y = 1;

            //create curved vert 1st quadrant
            List<Vector2> _topRightCurvedVert = new List<Vector2>
            {
                new Vector2(-pivotOffsetX, height - pivotOffsetY)
            };

            while (x < y)
            {
                y = SquircleFunc(x, true);
                _topRightCurvedVert.Add(new Vector2(dx + x - pivotOffsetX, dy + y - pivotOffsetY));
                x += delta;
            }

            if (float.IsNaN(_topRightCurvedVert.Last().y))
            {
                _topRightCurvedVert.RemoveAt(_topRightCurvedVert.Count - 1);
            }

            while (y > 0)
            {
                x = SquircleFunc(y, false);
                _topRightCurvedVert.Add(new Vector2(dx + x - pivotOffsetX, dy + y - pivotOffsetY));
                y -= delta;
            }

            _topRightCurvedVert.Add(new Vector2(width - pivotOffsetX, -pivotOffsetY));

            for (int i = 1; i < _topRightCurvedVert.Count - 1; i++)
            {
                if (_topRightCurvedVert[i].x < _topRightCurvedVert[i].y)
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
                new Vector2(width - pivotOffsetX, height - pivotOffsetY),
                new Vector2(width - pivotOffsetX, 0 - pivotOffsetY),
                new Vector2(0 - pivotOffsetX, 0 - pivotOffsetY)
            };
            }

            vert.Clear();
            if (corners.topRight)
            {
                vert.AddRange(_topRightCurvedVert);
            }
            else
            {
                vert.AddRange(_topRightFlatVert);
            }


            //The .Reverse().Select() operation can be optimized. The given line of code creates a new list of Vector2 by reversing _topRightCurvedVert and applying a transformation to each element. However, the operation involves iterating through each element of _topRightCurvedVert twice (once for reversing and once for selecting), which is inefficient.
            if (corners.bottomRight)
            {
                //vert.AddRange(_topRightCurvedVert.AsEnumerable().Reverse().Select(t => new Vector2(t.x, -t.y - pivotOffsetYTimesTwo)));
                for (int i = _topRightCurvedVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightCurvedVert[i];
                    reversedVector.y = -reversedVector.y - pivotOffsetYTimesTwo;
                    //reversedVector.x = reversedVector.x + pivotOffsetXTimesTwo;
                    vert.Add(reversedVector);
                }
            }
            else
            {
                //vert.AddRange(_topRightFlatVert.AsEnumerable().Reverse().Select(t => new Vector2(t.x, -t.y - pivotOffsetYTimesTwo)));
                for (int i = _topRightFlatVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.y = -reversedVector.y - pivotOffsetYTimesTwo;
                    vert.Add(reversedVector);
                }
            }

            //Reset the vertex pointer to center
            vert.Add(new Vector2(-pivotOffsetX, -pivotOffsetY));

            if (corners.bottomLeft)
            {
                //vert.AddRange(_topRightCurvedVert.AsEnumerable().Reverse().Select(t => new Vector2(-t.x - pivotOffsetXTimesTwo, -t.y - pivotOffsetYTimesTwo)));


                for (int i = _topRightCurvedVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightCurvedVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetXTimesTwo;
                    reversedVector.y = -reversedVector.y - pivotOffsetYTimesTwo;
                    vert.Add(reversedVector);
                }
            }
            else
            {
                //vert.AddRange(_topRightFlatVert.AsEnumerable().Reverse().Select(t => new Vector2(-t.x - pivotOffsetXTimesTwo, -t.y - pivotOffsetYTimesTwo)));
                for (int i = _topRightFlatVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetXTimesTwo;
                    reversedVector.y = -reversedVector.y - pivotOffsetYTimesTwo;
                    vert.Add(reversedVector);
                }
            }

            //Reset the vertex pointer to center
            vert.Add(new Vector2(-pivotOffsetX, -pivotOffsetY));

            if (corners.topLeft)
            {
                //vert.AddRange(_topRightCurvedVert.AsEnumerable().Reverse().Select(t => new Vector2(-t.x - pivotOffsetXTimesTwo, t.y)));
                for (int i = _topRightCurvedVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightCurvedVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetXTimesTwo;
                    vert.Add(reversedVector);
                }
            }
            else
            {

                //vert.AddRange(_topRightFlatVert.AsEnumerable().Reverse().Select(t => new Vector2(-t.x - pivotOffsetXTimesTwo, t.y)));
                for (int i = _topRightFlatVert.Count - 1; i >= 0; i--)
                {
                    Vector2 reversedVector = _topRightFlatVert[i];
                    reversedVector.x = -reversedVector.x - pivotOffsetXTimesTwo;
                    vert.Add(reversedVector);
                }
            }

            //vert.AddRange(vert.AsEnumerable().Reverse().Select(t => new Vector2(-t.x - pivotOffsetXTimesTwo, t.y)));


            vh.Clear();
            for (int i = 0; i < vert.Count - 1; i++)
            {
                vh.AddVert(vert[i], color, Vector2.zero);
                vh.AddVert(vert[i + 1], color, Vector2.zero);
                vh.AddVert(new Vector2(-pivotOffsetX, -pivotOffsetY), color, Vector2.zero);
                int timesThree = i * 3;
                vh.AddTriangle(timesThree, timesThree + 1, timesThree + 2);
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
        public class UISquircleEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                UISquircle script = (UISquircle)target;
                GUILayout.Space(10f);
                GUILayout.Label("Vertex count: " + script.vert.Count().ToString());
            }
        }
#endif
    }
}