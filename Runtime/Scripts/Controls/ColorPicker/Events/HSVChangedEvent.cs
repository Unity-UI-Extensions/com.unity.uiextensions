using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.ColorPicker
{
    [Serializable]
    public class HSVChangedEvent : UnityEvent<float, float, float>
    {

    }
}