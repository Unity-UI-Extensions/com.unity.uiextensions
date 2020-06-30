/// Credit Martin Sharkbomb 
/// Sourced from - http://www.sharkbombs.com/2015/08/26/unity-ui-scrollrect-tools/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectLinker")]
    public class ScrollRectLinker : MonoBehaviour
    {

        public bool clamp = true;

        [SerializeField]
        ScrollRect controllingScrollRect = null;
        ScrollRect scrollRect = null;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            if (controllingScrollRect != null)
                controllingScrollRect.onValueChanged.AddListener(MirrorPos);
        }

        void MirrorPos(Vector2 scrollPos)
        {

            if (clamp)
                scrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(scrollPos.x), Mathf.Clamp01(scrollPos.y));
            else
                scrollRect.normalizedPosition = scrollPos;
        }

    }
}