// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI Extensions/SoftMaskShader"
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

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		LOD 0

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float2 maskTexcoord : TEXCOORD1;
			};

			inline float UnityGet2DClipping (in float2 position, in float4 clipRect)
			{
				float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
				return inside.x * inside.y;
			}

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			sampler2D _MainTex;

			bool _UseAlphaClip;
			int _FlipAlphaMask = 0;
			float4 _AlphaMask_ST;
			sampler2D _AlphaMask;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				float4 wolrdPos = IN.vertex;
				OUT.maskTexcoord = TRANSFORM_TEX(wolrdPos.xy, _AlphaMask);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			float _CutOff;
			bool _HardBlend = false;
			bool _NoOuterClip = false;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				float4 inMask = float4( 
					step(float2(0.0f, 0.0f), IN.maskTexcoord), 
					step(IN.maskTexcoord, float2(1.0f, 1.0f)) );

				// Do we want to clip the image to the Mask Rectangle?
				if (_NoOuterClip == false && all(inMask) == false )
				{
					color.a = 0;
				}
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{

					float a = tex2D(_AlphaMask, IN.maskTexcoord).a;

					if (a <= _CutOff)
						a = 0;
					else
					{
						if(_HardBlend)
							a = 1;
					}

					if (_FlipAlphaMask == 1)
						a = 1 - a;

					color.a *= a;
				}

				if (_UseAlphaClip)
					clip(color.a - 0.001);
				return color;
			}
			ENDCG
		}
	}
}