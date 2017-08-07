using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.Examples
{
    public class Example01ScrollView : FancyScrollView<Example01CellDto>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController;

        new void Awake()
        {
            base.Awake();
            scrollPositionController.OnUpdatePosition.AddListener(UpdatePosition);
        }

        public void UpdateData(List<Example01CellDto> data)
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
        }
    }
}
