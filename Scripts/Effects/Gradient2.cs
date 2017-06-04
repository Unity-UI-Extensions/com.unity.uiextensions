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
		 set 
		 { 
			 _blendMode = value;
			 graphic.SetVerticesDirty(); 
		 }
	 }

	 public UnityEngine.Gradient EffectGradient {
		 get { return _effectGradient; }
		 set 
		 {
			  _effectGradient = value;
			  graphic.SetVerticesDirty(); 
		 }
	 }

	 public Type GradientType {
		 get { return _gradientType; }
		 set 
		 { 
			 _gradientType = value;
			 graphic.SetVerticesDirty(); 
		 }
	 }

	 public float Offset {
		 get { return _offset; }
		 set 
		 { 
			 _offset = value;
			 graphic.SetVerticesDirty(); 
		 }
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

			 case Type.Diamond: {
				
					float bottom = _vertexList[0].position.y;
					float top = _vertexList[0].position.y;
					float y = 0f;

					for(int i = nCount - 1; i >= 1; --i) {
						y = _vertexList[i].position.y;

						if(y > top) top = y;
						else if(y < bottom) bottom = y;
					}

					float height = 1f / (top - bottom);

					helper.Clear();
					for (int i = 0; i < nCount; i++) helper.AddVert(_vertexList[i]);

					float center = (bottom + top) / 2f;
					UIVertex centralVertex = new UIVertex();
					centralVertex.position = (Vector3.right + Vector3.up) * center + Vector3.forward * _vertexList[0].position.z;
					centralVertex.normal = _vertexList[0].normal;
					centralVertex.color = Color.white;
					helper.AddVert(centralVertex);

					for (int i = 1; i < nCount; i++) helper.AddTriangle(i-1,i,nCount);
					helper.AddTriangle(0,nCount-1,nCount);

					UIVertex vertex = new UIVertex();

					for(int i = 0; i < helper.currentVertCount; i++) {
						helper.PopulateUIVertex(ref vertex, i);

						vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate(
							Vector3.Distance(vertex.position, centralVertex.position) * height - Offset));
						
						helper.SetUIVertex(vertex, i);
					}
				}
				break;

			case Type.Radial: {

					float left = _vertexList[0].position.x;
					float right = _vertexList[0].position.x;
					float bottom = _vertexList[0].position.y;
					float top = _vertexList[0].position.y;

					float x = 0f;
					float y = 0f;

					for(int i = nCount - 1; i >= 1; --i) {
						x = _vertexList[i].position.x;

						if(x > right) right = x;
						else if(x < left) left = x;

						y = _vertexList[i].position.y;

						if(y > top) top = y;
						else if(y < bottom) bottom = y;
					}

					float width = 1f / (right - left);
					float height = 1f / (top - bottom);

					helper.Clear();

					float centerX = (right + left) / 2f;
					float centerY = (bottom + top) / 2f;
					float radiusX = (right - left) / 2f;
					float radiusY = (top - bottom) / 2f;
					UIVertex centralVertex = new UIVertex();
					centralVertex.position = Vector3.right * centerX + Vector3.up * centerY + Vector3.forward * _vertexList[0].position.z;
					centralVertex.normal = _vertexList[0].normal;
					centralVertex.color = Color.white;

					int steps = 64;
					for (int i = 0; i < steps; i++)
					{
						UIVertex curVertex = new UIVertex();
						float angle = (float)i * 360f / (float)steps;
						float curX = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusX;
						float curY = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusY;

						curVertex.position = Vector3.right * curX + Vector3.up * curY + Vector3.forward * _vertexList[0].position.z;
						curVertex.normal = _vertexList[0].normal;
						curVertex.color = Color.white;
						helper.AddVert(curVertex);
					}

					helper.AddVert(centralVertex);

					for (int i = 1; i < steps; i++) helper.AddTriangle(i-1,i,steps);
					helper.AddTriangle(0,steps-1,steps);

					UIVertex vertex = new UIVertex();

					for(int i = 0; i < helper.currentVertCount; i++) {
						helper.PopulateUIVertex(ref vertex, i);

						vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate(
							Mathf.Sqrt(
								Mathf.Pow(Mathf.Abs(vertex.position.x - centerX) * width, 2f) +
								Mathf.Pow(Mathf.Abs(vertex.position.y - centerY) * height, 2f)) * 2f - Offset));

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
		 Vertical,
		 Radial,
		 Diamond
	 }

	 public enum Blend {
		 Override,
		 Add,
		 Multiply
	 }
 }
}