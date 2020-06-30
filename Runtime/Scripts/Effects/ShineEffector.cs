/// Credit ömer faruk sayılır
/// Sourced from - https://bitbucket.org/snippets/Lordinarius/nrn4L

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode, RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Effects/Extensions/Shining Effect")]
    public class ShineEffector : MonoBehaviour
    {

        public ShineEffect effector;
        [SerializeField, HideInInspector]
        GameObject effectRoot;
        [Range(-1, 1)]
        public float yOffset = -1;

        public float YOffset
        {
            get
            {
                return yOffset;
            }
            set
            {
                ChangeVal(value);
                yOffset = value;
            }
        }

        [Range(0.1f, 1)]
        public float width = 0.5f;
        RectTransform effectorRect;
        void OnEnable()
        {
            if (effector == null)
            {
                GameObject effectorobj = new GameObject("effector");

                effectRoot = new GameObject("ShineEffect");
                effectRoot.transform.SetParent(this.transform);
                effectRoot.AddComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
                effectRoot.GetComponent<Image>().type = gameObject.GetComponent<Image>().type;
                effectRoot.AddComponent<Mask>().showMaskGraphic = false;
                effectRoot.transform.localScale = Vector3.one;
                effectRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                effectRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
                effectRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                effectRoot.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                effectRoot.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                effectRoot.transform.SetAsFirstSibling();

                effectorobj.AddComponent<RectTransform>();
                effectorobj.transform.SetParent(effectRoot.transform);
                effectorRect = effectorobj.GetComponent<RectTransform>();
                effectorRect.localScale = Vector3.one;
                effectorRect.anchoredPosition3D = Vector3.zero;

                effectorRect.gameObject.AddComponent<ShineEffect>();
                effectorRect.anchorMax = Vector2.one;
                effectorRect.anchorMin = Vector2.zero;

                effectorRect.Rotate(0, 0, -8);
                effector = effectorobj.GetComponent<ShineEffect>();
                effectorRect.offsetMax = Vector2.zero;
                effectorRect.offsetMin = Vector2.zero;
                OnValidate();
            }
        }

        void OnValidate()
        {
            effector.Yoffset = yOffset;
            effector.Width = width;

            if (yOffset <= -1 || yOffset >= 1)
            {
                effectRoot.SetActive(false);
            }
            else if (!effectRoot.activeSelf)
            {
                effectRoot.SetActive(true);
            }
            {

            }
        }

        void ChangeVal(float value)
        {
            effector.Yoffset = value;
            if (value <= -1 || value >= 1)
            {
                effectRoot.SetActive(false);
            }
            else if (!effectRoot.activeSelf)
            {
                effectRoot.SetActive(true);
            }
            {

            }
        }
        void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(effectRoot);
            }
            else
            {
                Destroy(effectRoot);
            }
        }
    }
}