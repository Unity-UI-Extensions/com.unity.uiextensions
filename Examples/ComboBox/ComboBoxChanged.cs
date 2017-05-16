using UnityEngine;

public class ComboBoxChanged : MonoBehaviour {


	public void ComboBoxChangedEvent (string text) {

        Debug.Log("ComboBox changed [" + text + "]");
	}

    public void AutoCompleteComboBoxChangedEvent(string text)
    {

        Debug.Log("AutoCompleteComboBox changed [" + text + "]");
    }

    public void DropDownChangedEvent(int newValue)
    {

        Debug.Log("DropDown changed [" + newValue + "]");
    }
}
