namespace UnityEngine.UI.Extensions.Examples
{
    public class MainMenu : SimpleMenu<MainMenu>
    {
        public void OnPlayPressed()
        {
            GameMenu.Show();
        }

        public void OnOptionsPressed()
        {
            OptionsMenu.Show();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}