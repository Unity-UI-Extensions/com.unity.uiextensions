/// Credit Brad Nelson (playemgames - bitbucket)
/// Modified Gradient effect script from http://answers.unity3d.com/questions/1086415/gradient-text-in-unity-522-basevertexeffect-is-obs.html
/// <summary>
/// -Uses Unity's Gradient class to define the color
/// -Offset is now limited to -1,1
/// -Multiple color blend modes
/// 
/// Remember that the colors are applied per-vertex so if you have multiple points on your gradient where the color changes and there aren't enough vertices, you won't see all of the colors.
/// </summary>
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient2")]
	public class Gradient2 : BaseMeshEffect {
	 [SerializeField]
	 Type _gradientType;

	 [SerializeField]
	 Blend _blendMode = Blend.Multiply;

	 [SerializeField]
	 [Range(-1, 1)]
	 float _offset = 0f;

	 [SerializeField]
	 UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };

	 #region Properties
	 public Blend BlendMode {
		 get { return _blendMode; }
		 set { _blendMode = value; }
	 }

	 public UnityEngine.Gradient EffectGradient {
		 get { return _effectGradient; }
		 set { _effectGradient = value; }
	 }

	 public Type GradientType {
		 get { return _gradientType; }
		 set { _gradientType = value; }
	 }

	 public float Offset {
		 get { return _offset; }
		 set { _offset = value; }
	 }
	 #endregion

	 public override void ModifyMesh(VertexHelper helper) {
		 if(!IsActive() || helper.currentVertCount == 0)
			 return;

		 List<UIVertex> _vertexList = new List<UIVertex>();

		 helper.GetUIVertexStream(_vertexList);

		 int nCount = _vertexList.Count;
		 switch(GradientType) {
			 case Type.Horizontal: {
					 float left = _vertexList[0].position.x;
					 float right = _vertexList[0].position.x;
					 float x = 0f;

					 for(int i = nCount - 1; i >= 1; --i) {
						 x = _vertexList[i].position.x;

						 if(x > right) right = x;
						 else if(x < left) left = x;
					 }

					 float width = 1f / (right - left);
					 UIVertex vertex = new UIVertex();

					 for(int i = 0; i < helper.currentVertCount; i++) {
						 helper.PopulateUIVertex(ref vertex, i);

						 vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate((vertex.position.x - left) * width - Offset));

						 helper.SetUIVertex(vertex, i);
					 }
				 }
				 break;

			 case Type.Vertical: {
					 float bottom = _vertexList[0].position.y;
					 float top = _vertexList[0].position.y;
					 float y = 0f;

					 for(int i = nCount - 1; i >= 1; --i) {
						 y = _vertexList[i].position.y;

						 if(y > top) top = y;
						 else if(y < bottom) bottom = y;
					 }

					 float height = 1f / (top - bottom);
					 UIVertex vertex = new UIVertex();

					 for(int i = 0; i < helper.currentVertCount; i++) {
						 helper.PopulateUIVertex(ref vertex, i);

						 vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate((vertex.position.y - bottom) * height - Offset));

						 helper.SetUIVertex(vertex, i);
					 }
				 }
				 break;
		 }
	 }

	 Color BlendColor(Color colorA, Color colorB) {
		 switch(BlendMode) {
			 default: return colorB;
			 case Blend.Add: return colorA + colorB;
			 case Blend.Multiply: return colorA * colorB;
		 }
	 }

	 public enum Type {
		 Horizontal,
		 Vertical
	 }

	 public enum Blend {
		 Override,
		 Add,
		 Multiply
	 }
 }
}