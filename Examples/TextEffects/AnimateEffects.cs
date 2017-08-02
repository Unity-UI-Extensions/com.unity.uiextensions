namespace UnityEngine.UI.Extensions.Examples
{
    public class AnimateEffects : MonoBehaviour
    {
        public LetterSpacing letterSpacing;
        float letterSpacingMax = 10, letterSpacingMin = -10, letterSpacingModifier = 0.1f;
        public CurvedText curvedText;
        float curvedTextMax = 0.05f, curvedTextMin = -0.05f, curvedTextModifier = 0.001f;
        public Gradient2 gradient2;
        float gradient2Max = 1, gradient2Min = -1, gradient2Modifier = 0.01f;
        public CylinderText cylinderText;
        private Transform cylinderTextRT;
        Vector3 cylinderRotation = new Vector3(0, 1, 0);
        public SoftMaskScript SAUIM;
        float SAUIMMax = 1, SAUIMMin = 0, SAUIMModifier = 0.01f;
        // Use this for initialization
        void Start()
        {
            cylinderTextRT = cylinderText.GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            letterSpacing.spacing += letterSpacingModifier;
            if (letterSpacing.spacing > letterSpacingMax || letterSpacing.spacing < letterSpacingMin)
            {
                letterSpacingModifier = -letterSpacingModifier;
            }
            curvedText.CurveMultiplier += curvedTextModifier;
            if (curvedText.CurveMultiplier > curvedTextMax || curvedText.CurveMultiplier < curvedTextMin)
            {
                curvedTextModifier = -curvedTextModifier;
            }
            gradient2.Offset += gradient2Modifier;
            if (gradient2.Offset > gradient2Max || gradient2.Offset < gradient2Min)
            {
                gradient2Modifier = -gradient2Modifier;
            }

            cylinderTextRT.Rotate(cylinderRotation);

            SAUIM.CutOff += SAUIMModifier;
            if (SAUIM.CutOff > SAUIMMax || SAUIM.CutOff < SAUIMMin)
            {
                SAUIMModifier = -SAUIMModifier;
            }

        }
    }
}