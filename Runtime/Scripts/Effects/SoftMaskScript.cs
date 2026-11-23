
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

        Graphic cachedGraphic = null;
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
                cachedGraphic = text;
                cachedCanvas = text.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
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
                mat = new Material(ShaderLibrary.GetShaderInstance("UI Extensions/SoftMaskShader"));
                graphic.material = mat;
                cachedGraphic = graphic;
                cachedCanvas = graphic.canvas;
                cachedCanvasTransform = cachedCanvas.transform;
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
            maskScale.Set(1.0f / size.x, 1.0f / size.y);
            maskOffset = -worldRect.min;
            maskOffset.Scale(maskScale);

            ApplyMaskProperties(mat);

            // When the graphic is maskable and sits under a Unity Mask, the material actually
            // rendered is a stencil copy of "mat" created by the StencilMaterial cache.
            // That copy does not pick up later changes made to "mat", so the mask would be
            // stuck with stale values (or not shown at all). Update the rendered material too.
            if (cachedGraphic != null)
            {
                var canvasRenderer = cachedGraphic.canvasRenderer;
                if (canvasRenderer != null && canvasRenderer.materialCount > 0)
                {
                    var renderMaterial = canvasRenderer.GetMaterial();
                    if (renderMaterial != null && renderMaterial != mat && renderMaterial.shader == mat.shader)
                    {
                        ApplyMaskProperties(renderMaterial);
                    }
                }
            }
        }

        void ApplyMaskProperties(Material material)
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