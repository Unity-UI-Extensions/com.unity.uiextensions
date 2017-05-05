// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/// Credit 00christian00
/// Sourced from - http://forum.unity3d.com/threads/any-way-to-show-part-of-an-image-without-using-mask.360085/#post-2332030

Shader "UI Extensions/UI Image Crop" {
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_XCrop ("X Crop", Range(0.0,1.0)) = 1
		_YCrop ("Y Crop", Range(0.0,1.0)) = 1
	}
	
	SubShader {

	ZWrite Off
	Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}
		Blend SrcAlpha OneMinusSrcAlpha 
	
    Pass {

      CGPROGRAM
	 
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
    
      struct v2f {
          float4 pos : POSITION;
          fixed4 color : COLOR;
		  float2 uv 	: TEXCOORD0; //UV1 coord
      };
      
	  uniform sampler2D _MainTex;
	  float4 _MainTex_ST;
	  uniform float _XCrop;
	  uniform float _YCrop;
	  
      v2f vert (v2f v)
      {
	  
          v2f o;
		  o.color=v.color;
		  o.color.a=0.1;
		  o.pos = UnityObjectToClipPos (v.pos);
		  
		  o.uv=TRANSFORM_TEX(v.uv, _MainTex); 
		 
          return o;
      }
      fixed4 frag (v2f i) : COLOR
	  { 
	  
	  //return fixed4(0.25,0,0,1);
	  i.color.a=step(i.uv.x,_XCrop);
	  //I don't like the bottom up ordering,so I reverse it
	  i.color.a=i.color.a*step(1-i.uv.y,_YCrop);
	  return i.color * tex2D (_MainTex, i.uv);
	   }
      ENDCG
    }
  } 
}
