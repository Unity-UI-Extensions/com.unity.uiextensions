/// <summary>
/// Created by Freezy - ElicitIce.nl
/// Posted on Unity Forums http://forum.unity3d.com/threads/cut-corners-primative.359494/
///
/// Free for any use and alteration, source code may not be sold without my permission.
/// If you make improvements on this script please share them with the community.
///
///
/// Here is a script that will take a rectangular TransformRect and cut off some corners based on the corner size.
/// This is great for when you need a quick and easy non-square panel/image.
/// Enjoy!
/// It adds an additional square if the relevant side has a corner cut, it then offsets the ends to simulate a cut corner.
/// UVs are being set, but might be skewed when a texture is applied.
/// You could hide the additional colors by using the following:
/// http://rumorgames.com/hide-in-inspector/
///
/// </summary>

namespace UnityEngine.UI.Extensions {
    [AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
    public class UICornerCut : UIPrimitiveBase
    {
         public Vector2 cornerSize = new Vector2(16, 16);

        [Header("Corners to cut")]
        public bool cutUL = true;
        public bool cutUR;
        public bool cutLL;
        public bool cutLR;
 
        [Tooltip("Up-Down colors become Left-Right colors")]
        public bool makeColumns = false;
 
        [Header("Color the cut bars differently")]
        public bool useColorUp;
//        [HideUnless("useColorUp")]
        public Color32 colorUp = Color.blue;
 
        public bool useColorDown;
//        [HideUnless("useColorDown")]
        public Color32 colorDown = Color.green;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var rect = rectTransform.rect;
            var rectNew = rect;

            Color32 color32 = color;
            bool up = cutUL | cutUR;
            bool down = cutLL | cutLR;
            bool left = cutLL | cutUL;
            bool right = cutLR | cutUR;
            bool any = up | down;

            if (any && cornerSize.sqrMagnitude > 0)
            {

                //nibble off the sides
                vh.Clear();
                if (left)
                    rectNew.xMin += cornerSize.x;
                if (down)
                    rectNew.yMin += cornerSize.y;
                if (up)
                    rectNew.yMax -= cornerSize.y;
                if (right)
                    rectNew.xMax -= cornerSize.x;

                //add two squares to the main square
                Vector2 ul, ur, ll, lr;

                if (makeColumns)
                {
                    ul = new Vector2(rect.xMin, cutUL ? rectNew.yMax : rect.yMax);
                    ur = new Vector2(rect.xMax, cutUR ? rectNew.yMax : rect.yMax);
                    ll = new Vector2(rect.xMin, cutLL ? rectNew.yMin : rect.yMin);
                    lr = new Vector2(rect.xMax, cutLR ? rectNew.yMin : rect.yMin);

                    if (left)
                        AddSquare(
                            ll, ul,
                            new Vector2(rectNew.xMin, rect.yMax),
                            new Vector2(rectNew.xMin, rect.yMin),
                            rect, useColorUp ? colorUp : color32, vh);
                    if (right)
                        AddSquare(
                            ur, lr,
                            new Vector2(rectNew.xMax, rect.yMin),
                            new Vector2(rectNew.xMax, rect.yMax),
                            rect, useColorDown ? colorDown : color32, vh);
                }
                else
                {
                    ul = new Vector2(cutUL ? rectNew.xMin : rect.xMin, rect.yMax);
                    ur = new Vector2(cutUR ? rectNew.xMax : rect.xMax, rect.yMax);
                    ll = new Vector2(cutLL ? rectNew.xMin : rect.xMin, rect.yMin);
                    lr = new Vector2(cutLR ? rectNew.xMax : rect.xMax, rect.yMin);
                    if (down)
                        AddSquare(
                            lr, ll,
                            new Vector2(rect.xMin, rectNew.yMin),
                            new Vector2(rect.xMax, rectNew.yMin),
                            rect, useColorDown ? colorDown : color32, vh);
                    if (up)
                        AddSquare(
                            ul, ur,
                            new Vector2(rect.xMax, rectNew.yMax),
                            new Vector2(rect.xMin, rectNew.yMax),
                            rect, useColorUp ? colorUp : color32, vh);
                }

                //center
                if (makeColumns)
                    AddSquare(new Rect(rectNew.xMin, rect.yMin, rectNew.width, rect.height), rect, color32, vh);
                else
                    AddSquare(new Rect(rect.xMin, rectNew.yMin, rect.width, rectNew.height), rect, color32, vh);

            }
        }
 
        private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh) {
            int v0 = AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
            int v1 = AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
            int v2 = AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
            int v3 = AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
 
            vh.AddTriangle(v0, v1, v2);
            vh.AddTriangle(v2, v3, v0);
        }
 
        private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh) {
            int v0 = AddVert(a.x, a.y, rectUV, color32, vh);
            int v1 = AddVert(b.x, b.y, rectUV, color32, vh);
            int v2 = AddVert(c.x, c.y, rectUV, color32, vh);
            int v3 = AddVert(d.x, d.y, rectUV, color32, vh);
 
            vh.AddTriangle(v0, v1, v2);
            vh.AddTriangle(v2, v3, v0);
        }
 
        /// <summary>
        /// Auto UV handler within the assigned area
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="area"></param>
        /// <param name="color32"></param>
        /// <param name="vh"></param>
        private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh) {
            var uv = new Vector2(
                Mathf.InverseLerp(area.xMin, area.xMax, x),
                Mathf.InverseLerp(area.yMin, area.yMax, y)
            );
            vh.AddVert(new Vector3(x, y), color32, uv);
            return vh.currentVertCount - 1;
        }
    }
}