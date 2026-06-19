/// Credit Alastair Aitchison
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/123/uilinerenderer-issues-with-specifying

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI Line Connector")]
    [RequireComponent(typeof(UILineRenderer))]
    [ExecuteInEditMode]
    public class UILineConnector : MonoBehaviour
    {

        // The elements between which line segments should be drawn
        public RectTransform[] transforms;
        private Vector3[] previousPositions;
        private Vector3 previousLrPos;
        private Vector3 previousGlobalScale;
        private RectTransform rt;
        private UILineRenderer lr;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            lr = GetComponent<UILineRenderer>();
        }

        private void OnEnable()
        {
            if (transforms == null || transforms.Length < 1)
            {
                return;
            }

            CalculateLinePoints();
        }

        private void Update()
        {
            if (lr.RelativeSize)
            {
                Debug.LogWarning("While using UILineConnector, UILineRenderer should not use relative size, so that even if this RectTransform has a zero-size Rect, the positions of the points can still be calculated");
                lr.RelativeSize = false;
            }

            if (transforms == null || transforms.Length < 1)
            {
                return;
            }

            // Get world position of UILineRenderer
            Vector3 lrWorldPos = rt.position;

            /*Performance check to only redraw when the child transforms move,
            or the world position of UILineRenderer moves */
            bool updateLine = lrWorldPos != previousLrPos;
            updateLine = rt.lossyScale != previousGlobalScale;

            if (!updateLine)
            {
                updateLine = previousPositions.Length != transforms.Length;
            }

            if (!updateLine && previousPositions != null)
            {
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (transforms[i] == null)
                    {
                        continue;
                    }
                    if (!updateLine && previousPositions[i] != transforms[i].position)
                    {
                        updateLine = true;
                        break;
                    }
                }
            }  
            if (!updateLine) return;


            // Calculate delta from the local position
            CalculateLinePoints();


            //save previous states
            previousLrPos = lrWorldPos;
            previousGlobalScale = rt.lossyScale;
            previousPositions = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == null)
                {
                    continue;
                }
                previousPositions[i] = transforms[i].position;
            }
        }

        private void CalculateLinePoints()
        {
            Vector2[] points = new Vector2[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == null)
                {
                    continue;
                }
                var offsetPos = rt.InverseTransformPoint(transforms[i].position);
                points[i] = new Vector2(offsetPos.x, offsetPos.y);
            }

            // And assign the converted points to the line renderer
            lr.Points = points;
            lr.RelativeSize = false;
            lr.drivenExternally = true;           
        }
    }
}