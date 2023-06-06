Shader "Hidden/PP_BlindEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Softness("Softness", float) = 0.5
        _Scale("Scale",float)=1.0
        _DepthThreshold("DepthThreshold",float)=1.0
        _DepthNormalThreshold("DepthNormalThreshold",float)=1.0
        _DepthNormalThresholdScale("DepthNormalThresholdScale",float)=1.0
        _NormalThreshold("NormalThreshold",float)=1.0
        _OutlineColor("OutlineColor",color)=(0,0,0,0)
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
                float3 viewSpaceDir : TEXCOORD1;
            };
            float4x4 _InvVP;
            float4x4 _ClipToView;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
                return o;
            }


            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;
            sampler2D _CameraDepthNormalsTexture;
            //Sphere Mask Param
            float4 _Position1;
            half _Radius1;
            float4 _Position2;
            half _Radius2;
            half _Softness;

            //Outline Param
            float _Scale;
            float4 _BackgroundColor;
            float4 _OutlineColor;

            float _DepthThreshold;
            float _DepthNormalThreshold;
            float _DepthNormalThresholdScale;

            float4 _MainTex_TexelSize;

            float _NormalThreshold;
            fixed4 frag (v2f i) : SV_Target
            {

                float halfScaleFloor = floor(_Scale * 0.5);
                float halfScaleCeil = ceil(_Scale * 0.5);
                float2 bottomLeftUV = i.uv - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 topRightUV = i.uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
                float2 bottomRightUV = i.uv + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 topLeftUV = i.uv + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);


                float3 normal0 = tex2D(_CameraDepthNormalsTexture, bottomLeftUV).rgb;
                float3 normal1 = tex2D(_CameraDepthNormalsTexture, topRightUV).rgb;
                float3 normal2 = tex2D(_CameraDepthNormalsTexture, bottomRightUV).rgb;
                float3 normal3 = tex2D(_CameraDepthNormalsTexture, topLeftUV).rgb;
 
                float depth0 = tex2D(_CameraDepthTexture, bottomLeftUV).r;
                float depth1 = tex2D(_CameraDepthTexture, topRightUV).r;
                float depth2 = tex2D(_CameraDepthTexture, bottomRightUV).r;
                float depth3 = tex2D(_CameraDepthTexture, topLeftUV).r;
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                float3 viewNormal = normal0 * 2 - 1;
                float NdotV = 1 - dot(viewNormal, normalize(- i.viewSpaceDir));
                
                float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
                float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;
                float depthThreshold = _DepthThreshold * depth0 * normalThreshold;

                float depthFiniteDifference0 = depth1 - depth0;
                float depthFiniteDifference1 = depth3 - depth2;
                float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
                edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

                float3 normalFiniteDifference0 = normal1 - normal0;
                float3 normalFiniteDifference1 = normal3 - normal2;
                float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
                edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;
                float edge = max(edgeDepth, edgeNormal);

 
                return edge.xxxx;
                //return finalColor;

            }
            ENDCG
        }

        Pass//SphereMaskPass
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
                float3 viewSpaceDir : TEXCOORD1;
            };
            float4x4 _InvVP;
            float4x4 _ClipToView;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
                return o;
            }


            sampler2D _NoiseTex;
            sampler2D _CameraDepthTexture;
            //Sphere Mask Param
            float4 _Position1;
            half _Radius1;
            float4 _Position2;
            half _Radius2;
            half _Softness;

            //Outline Param

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = tex2D(_NoiseTex, i.uv).r;

                //Sphere Mask Calculate
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                float4 proj = float4(i.uv * 2 - 1, depth,1);
                float4 worldPos = mul(_InvVP,proj);
                worldPos /= worldPos.w;
                half d1 = distance(_Position1.xyz, worldPos.xyz);
                half sum1 = saturate((d1 - _Radius1) / _Softness);
                half d2 = distance(_Position2.xyz, worldPos.xyz);
                half sum2 = saturate((d2 - _Radius2) / _Softness);
                half sum = sum1 * sum2;
                //sum-=noise*0.5;
                sum=saturate(sum);
                return sum;

            }
            ENDCG
        }

        Stencil{
            Ref 2
            ReadMask 2
            Comp Equal
        }
        Pass//CombinePass
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


            v2f vert(appdata v)
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

            fixed4 frag(v2f i) : SV_Target
            {
                float Outline_Clear = tex2D(_OutlineTex, i.uv).r;
                //return Outline_Clear.xxxx;
                float Outline_Blur = tex2D(_OutlineBlurTex, i.uv).r;
                float ObjectMask= step(tex2D(_CameraGBufferTexture0, i.uv).r,0.0001f);
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                float depthMask=1.0f-step(depth,0.0001f);
                ObjectMask*=depthMask;
                float Mask = 1.0f-tex2D(_MaskTex, i.uv).r;
                float CombineMask=saturate(Mask+ ObjectMask);
                //return ObjectMask;
                float4 Outline_Combine=lerp(Outline_Blur* _OutlineColor,Outline_Clear* _OutlineColor, CombineMask);
                //return Outline_Combine*CombineMask;
                float4 outlineMask= Outline_Combine * CombineMask;
                //float outlineMask= lerp(Outline_Combine* CombineMask, float4(0.0f, 0.0f, 0.0f, 0.0f), CombineMask);
                //return outlineMask;
                return outlineMask+(1.0f-outlineMask)* _BackgroundColor;
            }
        ENDCG
        }

    }
}
