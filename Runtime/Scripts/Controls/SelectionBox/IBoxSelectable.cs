///Original Credit Korindian
///Sourced from - http://forum.unity3d.com/threads/rts-style-drag-selection-box.265739/
///Updated Credit BenZed
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/


namespace UnityEngine.UI.Extensions
{

    /*
	 * Implement this interface on any MonoBehaviour that you'd like to be considered selectable.
	 */
    public interface IBoxSelectable {
		bool selected {
			get; 
			set;
		}
		
		bool preSelected {
			get; 
			set;
		}
		
		//This property doesn't actually need to be implemented, as this interface should already be placed on a MonoBehaviour, which will
		//already have it. Defining it here only allows us access to the transform property by casting through the selectable interface.
		Transform transform {
			get;
		}
	}

}