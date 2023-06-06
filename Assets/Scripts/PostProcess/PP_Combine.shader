Shader "Hidden/PP_Combine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor",color) = (0,0,0,0)
        _BackgroundColor("BackgroundColor",color) = (0,0,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _CameraGBufferTexture0;//Set certain Object Color to black as a mask
            sampler2D _CameraDepthTexture;
            sampler2D _OutlineTex;
            sampler2D _OutlineBlurTex;
            sampler2D _MaskTex;

            float4 _BackgroundColor;
            float4 _OutlineColor;

            fixed4 frag (v2f i) : SV_Target
            {
                float Outline_Clear = tex2D(_OutlineTex, i.uv).r;
                float Outline_Blur = tex2D(_OutlineBlurTex, i.uv).r;
                
                float ObjectMask = step(tex2D(_CameraGBufferTexture0, i.uv).r,0.0001f);
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                float depthMask = 1.0f - step(depth,0.0001f);
                ObjectMask *= depthMask;
                float Mask = 1.0f - tex2D(_MaskTex, i.uv).r;
                float CombineMask = saturate(Mask + ObjectMask);
                float4 Outline_Combine = lerp(Outline_Blur * _OutlineColor,Outline_Clear * _OutlineColor, CombineMask);
                float4 outlineMask = Outline_Combine * CombineMask;
                return outlineMask + (1.0f - outlineMask) * _BackgroundColor;
            }
            ENDCG
        }
    }
}
