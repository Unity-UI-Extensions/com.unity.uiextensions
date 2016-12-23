/// Credit NemoKrad (aka Charles Humphrey)
/// Sourced from - http://www.randomchaos.co.uk/SoftAlphaUIMask.aspx

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        Material mat;
        Canvas cachedCanvas= null;

        [Tooltip("The area that is to be used as the container.")]
        public RectTransform MaskArea;
        RectTransform myRect;

        [Tooltip("Texture to be used to do the soft alpha")]
        public Texture AlphaMask;

        [Tooltip("At what point to apply the alpha min range 0-1")]
        [Range(0, 1)]
        public float CutOff = 0;

        [Tooltip("Implement a hard blend based on the Cutoff")]
        public bool HardBlend = false;

        [Tooltip("Flip the masks alpha value")]
        public bool FlipAlphaMask = false;

        [Tooltip("If Mask Scals Rect is given, and this value is true, the area around the mask will not be clipped")]
        public bool DontClipMaskScalingRect = false;

        Vector3[] worldCorners = new Vector3[4];
        Vector2 maskOffset = Vector2.zero;
        Vector2 maskScale = Vector2.one;

        bool isText = false;

        // Use this for initialization
        void Start()
        {
            myRect = GetComponent<RectTransform>();

            if (MaskArea == null)
            {
                MaskArea = myRect;
            }

            var text = GetComponent<Text>();
            if (text != null)
            {
                isText = true;
                mat = new Material(Shader.Find("UI Extensions/SoftMaskShaderText"));
                text.material = mat;
                cachedCanvas = text.canvas;

                // For some reason, having the mask control on the parent and disabled stops the mouse interacting
                // with the texture layer that is not visible.. Not needed for the Image.
                if (transform.parent.GetComponent<Mask>() == null)
                    transform.parent.gameObject.AddComponent<Mask>();

                transform.parent.GetComponent<Mask>().enabled = false;
                return;
            }

            var graphic = GetComponent<Graphic>();
            if (graphic != null)
            {
                mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
                graphic.material = mat;
                cachedCanvas = graphic.canvas;
            }

        }

        Transform GetParentTranform(Transform t)
        {
            return t.parent;
        }

        void Update()
        {
            if (cachedCanvas != null)
            {
                SetMask();
            }
        }

        void SetMask()
        {
            var maskRectXform = MaskArea;
            var worldRect = RectTransformUtility.PixelAdjustRect(MaskArea, cachedCanvas);

            var size = worldRect.size;
            maskScale.Set(1.0f / size.x, 1.0f / size.y);
            maskOffset = -worldRect.min;
            maskOffset.Scale(maskScale);

            mat.SetTextureOffset("_AlphaMask", maskOffset);
            mat.SetTextureScale("_AlphaMask", maskScale);
            mat.SetTexture("_AlphaMask", AlphaMask);

            mat.SetFloat("_HardBlend", HardBlend ? 1 : 0);
            mat.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
            mat.SetInt("_NoOuterClip", DontClipMaskScalingRect ? 1 : 0);
            mat.SetFloat("_CutOff", CutOff);
        }
    }
}