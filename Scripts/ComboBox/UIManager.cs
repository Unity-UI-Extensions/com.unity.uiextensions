///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    private static GameObject canvas;

    public static GameObject GetCanvas()
    {

        canvas = FindObjectOfType<Canvas>().gameObject;
        return canvas;
    }

}
