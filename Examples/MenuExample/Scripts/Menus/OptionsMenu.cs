namespace UnityEngine.UI.Extensions.Examples
{
    public class OptionsMenu : SimpleMenu<OptionsMenu>
    {
        public Slider Slider;

        public void OnMagicButtonPressed()
        {
            AwesomeMenu.Show(Slider.value);
        }
    }
}