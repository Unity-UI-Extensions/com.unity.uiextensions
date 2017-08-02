using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.Examples
{
    public class Example02ScrollView : FancyScrollView<Example02CellDto, Example02ScrollViewContext>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController;

        new void Awake()
        {
            scrollPositionController.OnUpdatePosition.AddListener(UpdatePosition);
            SetContext(new Example02ScrollViewContext { OnPressedCell = OnPressedCell });
            base.Awake();
        }

        public void UpdateData(List<Example02CellDto> data)
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
        }

        void OnPressedCell(Example02ScrollViewCell cell)
        {
            scrollPositionController.ScrollTo(cell.DataIndex, 0.4f);
            context.SelectedIndex = cell.DataIndex;
            UpdateContents();
        }
    }
}
