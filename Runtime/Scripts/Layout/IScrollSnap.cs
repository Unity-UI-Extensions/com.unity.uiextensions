/// Credit SimonDarksideJ
/// Required for scrollbar support to work across ALL scroll snaps


namespace UnityEngine.UI.Extensions
{
    internal interface IScrollSnap
    {
        void ChangePage(int page);
        void SetLerp(bool value);
        int CurrentPage();
        void StartScreenChange();
    }
}
