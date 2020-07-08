/// Credit FireOApache 
/// sourced from: http://answers.unity3d.com/questions/1149417/ui-button-onclick-sensitivity-for-high-dpi-devices.html#answer-1197307

/*USAGE:
Simply place the script on the EventSystem in the scene to correct the drag thresholds*/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("UI/Extensions/DragCorrector")]
    public class DragCorrector : MonoBehaviour
    {
        public int baseTH = 6;
        public int basePPI = 210;
        public int dragTH = 0;

        void Start()
        {
            dragTH = baseTH * (int)Screen.dpi / basePPI;

            EventSystem es = GetComponent<EventSystem>();

            if (es)
            {
                es.pixelDragThreshold = dragTH;
            }
        }
    }
}