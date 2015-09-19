/// Usage: Add this component to the input and add the
/// function to execute to the EnterSubmit event of this script

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(InputField))]
public class InputFieldEnterSubmit : MonoBehaviour
{

    [System.Serializable]
    public class EnterSubmitEvent : UnityEvent<string> { }

    public EnterSubmitEvent EnterSubmit;
    private InputField _input;

    void Awake()
    {
        _input = GetComponent<InputField>();
        _input.onEndEdit.AddListener(OnEndEdit);
    }

    public void OnEndEdit(string txt)
    {
        if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
        EnterSubmit.Invoke(txt);
    }

}
