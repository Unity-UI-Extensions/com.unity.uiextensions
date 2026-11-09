
using static System.Net.Mime.MediaTypeNames;
/// Credit NemoKrad (aka Charles Humphrey) / valtain
/// Sourced from - http://www.randomchaos.co.uk/SoftAlphaUIMask.aspx
/// Updated by valtain - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/pull-requests/33

namespace UnityEngine.UI.Extensions
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        Material mat;
        Graphic cachedGraphic;

        Canvas cachedCanvas = null;
        Transform cachedCanvasTransform = null;
        readonly Vector3[] m_WorldCorners = new Vector3[4];
        readonly Vector3[] m_CanvasCorners = new Vector3[4];

        [Tooltip("The area that is to be used as the container.")]
        public RectTransform MaskArea;

        [Tooltip("Texture to be used to do the soft alpha")]
        public Texture AlphaMask;

        [Tooltip("At what point to apply the alpha min range 0-1")]
        [Range(0, 1)]
        public float CutOff = 0;

        [Tooltip("Implement a hard blend based on the Cutoff")]
        public bool HardBlend = false;

        [Tooltip("Flip the masks alpha value")]
        public bool FlipAlphaMask = false;

        [Tooltip("If a different Mask Scaling Rect is given, and this value is true, the area around the mask will not be clipped")]
        public bool DontClipMaskScalingRect = false;

        Vector2 maskOffset = Vector2.zero;
        Vector2 maskScale = Vector2.one;

        // Use this for initialization
        void Start()
        {
            if (MaskArea == null)
            {
                MaskArea = GetComponent<RectTransform>();
            }

            var text = GetComponent<TMPro.TMP_Text>();
            if (text != null)
            {
                mat = new Material(ShaderLibrary.GetShaderInstance("UI Extensions/SoftMaskShader"));
                text.material = mat;
                cachedCanvas = text.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
                // For some reason, having the mask control on the parent and disabled stops the mouse interacting
                // with the texture layer that is not visible.. Not needed for the Image.
                if (transform.parent.GetComponent<Mask>() == null)
                    transform.parent.gameObject.AddComponent<Mask>();

                transform.parent.GetComponent<Mask>().enabled = false;
                return;
            }

            cachedGraphic = GetComponent<Graphic>();
            if (cachedGraphic != null)
            {
                mat = new Material(ShaderLibrary.GetShaderInstance("UI Extensions/SoftMaskShader"));
                cachedCanvas = cachedGraphic.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
                SetMask();
                cachedGraphic.material = mat;
            }
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
            var worldRect = GetCanvasRect();
            var size = worldRect.size;
            if (size.x == 0f || size.y == 0f)
                return;

            maskScale.Set(1.0f / size.x, 1.0f / size.y);
            maskOffset = -worldRect.min;
            maskOffset.Scale(maskScale);

            ApplyMask(mat);

            if (cachedGraphic == null || !cachedGraphic.isActiveAndEnabled || cachedGraphic.material != mat)
                return;

            // A parent Mask can give the renderer a cached stencil copy of our material.
            var renderer = cachedGraphic.canvasRenderer;
            if (renderer.materialCount == 0)
                return;

            var renderingMaterial = renderer.GetMaterial(0);
            if (renderingMaterial != null && renderingMaterial != mat && renderingMaterial.shader == mat.shader)
                ApplyMask(renderingMaterial);
        }

        void ApplyMask(Material material)
        {
            material.SetTextureOffset("_AlphaMask", maskOffset);
            material.SetTextureScale("_AlphaMask", maskScale);
            material.SetTexture("_AlphaMask", AlphaMask);

            material.SetFloat("_HardBlend", HardBlend ? 1 : 0);
            material.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
            material.SetInt("_NoOuterClip", DontClipMaskScalingRect ? 1 : 0);
            material.SetFloat("_CutOff", CutOff);
        }

        public Rect GetCanvasRect()
        {
            if (cachedCanvas == null)
                return new Rect();

            MaskArea.GetWorldCorners(m_WorldCorners);
            for (int i = 0; i < 4; ++i)
                m_CanvasCorners[i] = cachedCanvasTransform.InverseTransformPoint(m_WorldCorners[i]);

            return new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
        }
    }
}