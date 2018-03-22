Shader "UI Extensions/Particles/Multiply" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

		_StencilComp ("Stencil Comparison", Float) = 8
    _Stencil ("Stencil ID", Float) = 0
    _StencilOp ("Stencil Operation", Float) = 0
    _StencilWriteMask ("Stencil Write Mask", Float) = 255
    _StencilReadMask ("Stencil Read Mask", Float) = 255

    _ColorMask ("Color Mask", Float) = 15

    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"  }
	Blend Zero SrcColor
	Cull Off Lighting Off ZWrite Off
	
	SubShader {

		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

                #pragma multi_compile __ UNITY_UI_ALPHACLIP

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t IN)
			{
				v2f v;
				v.vertex = UnityObjectToClipPos(IN.vertex);
				#ifdef SOFTPARTICLES_ON
				v.projPos = ComputeScreenPos (v.vertex);
				COMPUTE_EYEDEPTH(v.projPos.z);
				#endif
				v.color = IN.color;
				v.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(v,v.vertex);
				return v;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				half4 prev = i.color * tex2D(_MainTex, i.texcoord);
				fixed4 col = lerp(half4(1,1,1,1), prev, prev.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1,1,1,1)); // fog towards white due to our blend mode
				return col;
			}
			ENDCG 
		}
	}
}
}
