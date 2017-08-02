using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.Examples
{
    public class Example03ScrollView : FancyScrollView<Example03CellDto, Example03ScrollViewContext>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController;

        new void Awake()
        {
            scrollPositionController.OnUpdatePosition.AddListener(UpdatePosition);
            SetContext(new Example03ScrollViewContext { OnPressedCell = OnPressedCell });
            base.Awake();
        }

        public void UpdateData(List<Example03CellDto> data)
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
        }

        void OnPressedCell(Example03ScrollViewCell cell)
        {
            scrollPositionController.ScrollTo(cell.DataIndex, 0.4f);
            context.SelectedIndex = cell.DataIndex;
            UpdateContents();
        }

    }
}
