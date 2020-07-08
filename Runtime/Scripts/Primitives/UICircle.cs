/// Credit zge, jeremie sellam
/// Sourced from - http://forum.unity3d.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2293224
/// Updated from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/65/a-better-uicircle

/// Update 10.9.2017 (tswalker, https://bitbucket.org/tswalker/)
/// 
/// * Modified component to utilize vertex stream instead of quads
/// * Improved accuracy of geometry fill to prevent edge "sliding" and redundant tris
/// * Added progress capability to allow component to be used as an indicator
/// * Added methods for use during runtime and event system(s) with other components
/// * Change some terminology of members to reflect other component property changes
/// * Added padding capability
/// * Only utilizes UV0 set for sprite/texture mapping (maps UV to geometry 0,1 boundary)
/// * Sample usage in scene "UICircleProgress"
/// Note: moving the pivot around from center to an edge can cause strange things
///       as well as having the RectTransform be smaller than the Thickness and/or Padding.
///       When making an initial layout for the component, it would be best to test multiple
///       aspect ratios and resolutions to ensure consistent behaviour.

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
    public class UICircle : UIPrimitiveBase
    {
        [Tooltip("The Arc Invert property will invert the construction of the Arc.")]
        public bool ArcInvert = true;

        [Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
        [Range(0, 1)]
        public float Arc = 1;

        [Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
        [Range(0, 1000)]
        public int ArcSteps = 100;

        [Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
        [Range(0, 360)]
        public int ArcRotation = 0;

        [Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
        [Range(0, 1)]
        public float Progress = 0;
        private float _progress = 0;

        public Color ProgressColor = new Color(255, 255, 255, 255);
        public bool Fill = true; //solid circle
        public float Thickness = 5;
        public int Padding = 0;

        private List<int> indices = new List<int>();  //ordered list of vertices per tri
        private List<UIVertex> vertices = new List<UIVertex>();
        private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            int _inversion = ArcInvert ? -1 : 1;
            float Diameter = (rectTransform.rect.width < rectTransform.rect.height ? rectTransform.rect.width : rectTransform.rect.height) - Padding; //correct for padding and always fit RectTransform
            float outerDiameter = -rectTransform.pivot.x * Diameter;
            float innerDiameter = -rectTransform.pivot.x * Diameter + Thickness;

            vh.Clear();
            indices.Clear();
            vertices.Clear();

            int i = 0;
            int j = 1;
            int k = 0;

            float stepDegree = (Arc * 360f) / ArcSteps;
            _progress = ArcSteps * Progress;
            float rad = _inversion * Mathf.Deg2Rad * ArcRotation;
            float X = Mathf.Cos(rad);
            float Y = Mathf.Sin(rad);

            var vertex = UIVertex.simpleVert;
            vertex.color = _progress > 0 ? ProgressColor : color;

            //initial vertex
            vertex.position = new Vector2(outerDiameter * X, outerDiameter * Y);
            vertex.uv0 = new Vector2(vertex.position.x / Diameter + 0.5f, vertex.position.y / Diameter + 0.5f);
            vertices.Add(vertex);

            var iV = new Vector2(innerDiameter * X, innerDiameter * Y);
            if (Fill) iV = Vector2.zero; //center vertex to pivot
            vertex.position = iV;
            vertex.uv0 = Fill ? uvCenter : new Vector2(vertex.position.x / Diameter + 0.5f, vertex.position.y / Diameter + 0.5f);
            vertices.Add(vertex);

            for (int counter = 1; counter <= ArcSteps; counter++)
            {
                rad = _inversion * Mathf.Deg2Rad * (counter * stepDegree + ArcRotation);
                X = Mathf.Cos(rad);
                Y = Mathf.Sin(rad);

                vertex.color = counter > _progress ? color : ProgressColor;
                vertex.position = new Vector2(outerDiameter * X, outerDiameter * Y);
                vertex.uv0 = new Vector2(vertex.position.x / Diameter + 0.5f, vertex.position.y / Diameter + 0.5f);
                vertices.Add(vertex);

                //add additional vertex if required and generate indices for tris in clockwise order
                if (!Fill)
                {
                    vertex.position = new Vector2(innerDiameter * X, innerDiameter * Y);
                    vertex.uv0 = new Vector2(vertex.position.x / Diameter + 0.5f, vertex.position.y / Diameter + 0.5f);
                    vertices.Add(vertex);
                    k = j;
                    indices.Add(i);
                    indices.Add(j + 1);
                    indices.Add(j);
                    j++;
                    i = j;
                    j++;
                    indices.Add(i);
                    indices.Add(j);
                    indices.Add(k);
                }
                else
                {
                    indices.Add(i);
                    indices.Add(j + 1);
                    //Fills (solid circle) with progress require an additional vertex to 
                    // prevent the base circle from becoming a gradient from center to edge
                    if (counter > _progress)
                    {
                        indices.Add(ArcSteps + 2);
                    }
                    else
                    {
                        indices.Add(1);
                    }

                    j++;
                    i = j;
                }
            }

            //this vertex is added to the end of the list to simplify index ordering on geometry fill
            if (Fill)
            {
                vertex.position = iV;
                vertex.color = color;
                vertex.uv0 = uvCenter;
                vertices.Add(vertex);
            }
            vh.AddUIVertexStream(vertices, indices);
        }

        //the following methods may be used during run-time
        //to update the properties of the component
        public void SetProgress(float progress)
        {
            Progress = progress;
            SetVerticesDirty();
        }

        public void SetArcSteps(int steps)
        {
            ArcSteps = steps;
            SetVerticesDirty();
        }

        public void SetInvertArc(bool invert)
        {
            ArcInvert = invert;
            SetVerticesDirty();
        }

        public void SetArcRotation(int rotation)
        {
            ArcRotation = rotation;
            SetVerticesDirty();
        }

        public void SetFill(bool fill)
        {
            Fill = fill;
            SetVerticesDirty();
        }

        public void SetBaseColor(Color color)
        {
            this.color = color;
            SetVerticesDirty();
        }

        public void UpdateBaseAlpha(float value)
        {
            var _color = this.color;
            _color.a = value;
            this.color = _color;
            SetVerticesDirty();
        }

        public void SetProgressColor(Color color)
        {
            ProgressColor = color;
            SetVerticesDirty();
        }

        public void UpdateProgressAlpha(float value)
        {
            ProgressColor.a = value;
            SetVerticesDirty();
        }

        public void SetPadding(int padding)
        {
            Padding = padding;
            SetVerticesDirty();
        }

        public void SetThickness(int thickness)
        {
            Thickness = thickness;
            SetVerticesDirty();
        }
    }
}