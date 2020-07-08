/// Credit FireOApache 
/// sourced from: http://answers.unity3d.com/questions/1149417/ui-button-onclick-sensitivity-for-high-dpi-devices.html#answer-1197307

/*USAGE:
Simply place the script on A Text control in the scene to display the current PPI / DPI of the screen*/

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Extensions/PPIViewer")]
    public class PPIViewer : MonoBehaviour
    {
        private Text label;

        void Awake()
        {
            label = GetComponent<Text>();
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