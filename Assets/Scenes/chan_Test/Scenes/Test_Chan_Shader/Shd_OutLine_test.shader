Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            }
    Pass {
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"
    
    struct appdata {
        float4 vertex : POSITION;
    };
    
    struct v2f {
        float4 vertex : SV_POSITION;
        float4 depth : TEXCOORD0;
    };
    
    v2f vert(appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.depth = ComputeGrabScreenPos(o.vertex);
        return o;
    }
    
    float _OutlineWidth;
    
    fixed4 frag(v2f i) : SV_Target {
        float4 col = tex2D(_MainTex, i.uv);
        float4 outlineCol = float4(0, 0, 0, 1);
        float4 depth = i.depth;
        depth.z += _OutlineWidth;
    
        float4 outline = tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(depth));
        if (outline.a > 0) {
            return outlineCol;
        }
    
        return col;
    }
    ENDCG
    }
    
            UNITY_INSTANCING_BUFFER_END(Props)
    
            void surf (Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
    }
        FallBack "Diffuse"
}