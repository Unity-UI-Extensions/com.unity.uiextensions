Shader "UI/Particles/Hidden"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Cull Off Lighting Off ZWrite Off Fog { Mode Off }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
 
            v2f vert ()
            {
                v2f o;
                o.vertex = fixed4(0, 0, 0, 0);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                discard;
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}