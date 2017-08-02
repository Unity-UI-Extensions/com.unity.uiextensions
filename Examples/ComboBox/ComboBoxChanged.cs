namespace UnityEngine.UI.Extensions.Examples
{
    public class ComboBoxChanged : MonoBehaviour
    {
        public void ComboBoxChangedEvent(string text)
        {

            Debug.Log("ComboBox changed [" + text + "]");
        }

        public void AutoCompleteComboBoxChangedEvent(string text)
        {

            Debug.Log("AutoCompleteComboBox changed [" + text + "]");
        }

        public void AutoCompleteComboBoxSelectionChangedEvent(string text, bool valid)
        {

            Debug.Log("AutoCompleteComboBox selection changed [" + text + "] and its validity was [" + valid + "]");
        }

        public void DropDownChangedEvent(int newValue)
        {

            Debug.Log("DropDown changed [" + newValue + "]");
        }
    }
}