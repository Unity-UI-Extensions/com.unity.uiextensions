using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReorderableListDebug : MonoBehaviour
{

    public Text DebugLabel;

    void Awake()
    {
        foreach (var list in FindObjectsOfType<ReorderableList>())
        {
            list.OnElementDropped.AddListener(ElementDropped);
        }
    }

    private void ElementDropped(ReorderableList.ReorderableListEventStruct droppedStruct)
    {
        DebugLabel.text = "";
        DebugLabel.text += "Dropped Object: " + droppedStruct.DropedObject.name + "\n";
        DebugLabel.text += "Is Clone ?: " + droppedStruct.IsAClone + "\n";
        if (droppedStruct.IsAClone)
        DebugLabel.text += "Source Object: " + droppedStruct.SourceObject.name + "\n";
        DebugLabel.text += string.Format("From {0} at Index {1} \n", droppedStruct.FromList.name,droppedStruct.FromIndex);
        DebugLabel.text += string.Format("To {0} at Index {1} \n", droppedStruct.ToList.name,droppedStruct.ToIndex);
    }
}
