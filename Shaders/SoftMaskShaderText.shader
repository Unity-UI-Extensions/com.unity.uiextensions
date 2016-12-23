Shader "UI Extensions/SoftMaskShaderText"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
		_AlphaMask("AlphaMask - Must be Wrapped",2D) = "white"{}
		_CutOff("CutOff",Float) = 0
		[MaterialToggle]
		_HardBlend("HardBlend",Float) = 0
		_FlipAlphaMask("Flip Alpha Mask",int) = 0
		_NoOuterClip("Outer Clip",int) = 0
	}
	FallBack "UI Extensions/SoftMaskShader"
}