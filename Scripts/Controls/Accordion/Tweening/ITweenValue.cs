///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/


namespace UnityEngine.UI.Extensions.Tweens
{
    internal interface ITweenValue
	{
		void TweenValue(float floatPercentage);
		bool ignoreTimeScale { get; }
		float duration { get; }
		bool ValidTarget();
		void Finished();
	}
}