/// Credit FireOApache 
/// sourced from: http://answers.unity3d.com/questions/1149417/ui-button-onclick-sensitivity-for-high-dpi-devices.html#answer-1197307

/*USAGE:
Simply place the script on A Text control in the scene to display the current PPI / DPI of the screen*/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    [AddComponentMenu("UI/Extensions/PPIViewer")]
    public class PPIViewer : MonoBehaviour
    {
        private TMPro.TMP_Text label;

        void Awake()
        {
            label = GetComponentInChildren<TMPro.TMP_Text>();
        }

        void Start()
        {
            if (label != null)
            {
                label.text = "PPI: " + Screen.dpi.ToString();
            }
        }
    }
}