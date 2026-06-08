/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

namespace UnityEngine.UI.Extensions
{
    public class ReorderableListDebug : MonoBehaviour
    {
        public UnityEngine.UI.Text DebugLabelLegacy;
        public TMPro.TMP_Text DebugLabel;

        void Awake()
        {
            foreach (var list in FindObjectsByType<ReorderableList>(FindObjectsSortMode.None))
            {
                list.OnElementDropped.AddListener(ElementDropped);
            }
        }

        private void ElementDropped(ReorderableList.ReorderableListEventStruct droppedStruct)
        {
            var text = "";
            text += "Dropped Object: " + (droppedStruct.DroppedObject != null ? droppedStruct.DroppedObject.name : "<none>") + "\n";
            text += "Is Clone ?: " + droppedStruct.IsAClone + "\n";
            if (droppedStruct.IsAClone)
                text += "Source Object: " + (droppedStruct.SourceObject != null ? droppedStruct.SourceObject.name : "<none>") + "\n";
            text += string.Format("From {0} at Index {1} \n", droppedStruct.FromList != null ? droppedStruct.FromList.name : "<none>", droppedStruct.FromIndex);
            text += string.Format("To {0} at Index {1} \n", droppedStruct.ToList == null ? "Empty space" : droppedStruct.ToList.name, droppedStruct.ToIndex);

            if (DebugLabel != null)
            {
                DebugLabel.text = text;
            }
            else if (DebugLabelLegacy != null)
            {
                DebugLabelLegacy.text = text;
            }
            else
            {
                Debug.LogWarning("[ReorderableListDebug] No DebugLabel assigned (neither Text nor TMP_Text); cannot display drop info.", this);
            }
        }
    }
}