namespace UnityEngine.UI.Extensions.Examples
{
    public class UpdateScrollSnap : MonoBehaviour
    {

        public UnityEngine.UI.Extensions.HorizontalScrollSnap HSS;
        public UnityEngine.UI.Extensions.VerticalScrollSnap VSS;
        public GameObject HorizontalPagePrefab;
        public GameObject VerticalPagePrefab;
        public UnityEngine.UI.InputField JumpPage;


        public void AddButton()
        {
            if (HSS)
            {
                var newHSSPage = GameObject.Instantiate(HorizontalPagePrefab);
                HSS.AddChild(newHSSPage);
            }
            if (VSS)
            {
                var newVSSPage = GameObject.Instantiate(VerticalPagePrefab);
                VSS.AddChild(newVSSPage);
            }
        }

        public void RemoveButton()
        {
            GameObject removed, removed2;
            if (HSS)
            {
                HSS.RemoveChild(HSS.CurrentPage, out removed);
                removed.SetActive(false);
            }
            if (VSS)
            {
                VSS.RemoveChild(VSS.CurrentPage, out removed2);
                removed2.SetActive(false);
            }
        }

        public void JumpToPage()
        {
            int jumpPage = int.Parse(JumpPage.text);
            if (HSS)
            {
                HSS.GoToScreen(jumpPage);
            }
            if (VSS)
            {
                VSS.GoToScreen(jumpPage);
            }
        }

        public void SelectionStartChange()
        {
            Debug.Log("Scroll Snap change started");
        }
        public void SelectionEndChange()
        {
            Debug.Log("Scroll Snap change finished");
        }
        public void PageChange(int page)
        {
            Debug.Log(string.Format("Scroll Snap page changed to {0}", page));
        }

        public void RemoveAll()
        {
            GameObject[] children;
            HSS.RemoveAllChildren(out children);
            VSS.RemoveAllChildren(out children);
        }

        public void JumpToSelectedToggle(int page)
        {
            HSS.GoToScreen(page);
        }
    }
}