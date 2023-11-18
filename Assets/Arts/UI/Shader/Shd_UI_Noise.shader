// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/Shd_UI_Noise"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _MainTex1("MainTex", 2D) = "white" {}
        _Fast("Fast", Float) = 5
        _Power("Power", Float) = 1
        _AVoronoiScale("A Voronoi Scale", Float) = 7
        _BVoronoiScale("B Voronoi Scale", Float) = 4

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform sampler2D _MainTex1;
            uniform float _AVoronoiScale;
            uniform float _Fast;
            uniform float _BVoronoiScale;
            uniform float _Power;
            		float2 voronoihash13( float2 p )
            		{
            			
            			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
            			return frac( sin( p ) *43758.5453);
            		}
            
            		float voronoi13( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
            		{
            			float2 n = floor( v );
            			float2 f = frac( v );
            			float F1 = 8.0;
            			float F2 = 8.0; float2 mg = 0;
            			for ( int j = -1; j <= 1; j++ )
            			{
            				for ( int i = -1; i <= 1; i++ )
            			 	{
            			 		float2 g = float2( i, j );
            			 		float2 o = voronoihash13( n + g );
            					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            					float d = 0.5 * dot( r, r );
            			 		if( d<F1 ) {
            			 			F2 = F1;
            			 			F1 = d; mg = g; mr = r; id = o;
            			 		} else if( d<F2 ) {
            			 			F2 = d;
            			
            			 		}
            			 	}
            			}
            			return F1;
            		}
            
            		float2 voronoihash16( float2 p )
            		{
            			
            			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
            			return frac( sin( p ) *43758.5453);
            		}
            
            		float voronoi16( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
            		{
            			float2 n = floor( v );
            			float2 f = frac( v );
            			float F1 = 8.0;
            			float F2 = 8.0; float2 mg = 0;
            			for ( int j = -1; j <= 1; j++ )
            			{
            				for ( int i = -1; i <= 1; i++ )
            			 	{
            			 		float2 g = float2( i, j );
            			 		float2 o = voronoihash16( n + g );
            					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            					float d = 0.5 * dot( r, r );
            			 		if( d<F1 ) {
            			 			F2 = F1;
            			 			F1 = d; mg = g; mr = r; id = o;
            			 		} else if( d<F2 ) {
            			 			F2 = d;
            			
            			 		}
            			 	}
            			}
            			return F1;
            		}
            

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float2 texCoord38 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float mulTime10 = _Time.y * _Fast;
                float time13 = mulTime10;
                float2 voronoiSmoothId13 = 0;
                float2 coords13 = IN.texcoord.xy * _AVoronoiScale;
                float2 id13 = 0;
                float2 uv13 = 0;
                float voroi13 = voronoi13( coords13, time13, id13, uv13, 0, voronoiSmoothId13 );
                float mulTime18 = _Time.y * _Fast;
                float time16 = mulTime18;
                float2 voronoiSmoothId16 = 0;
                float2 coords16 = IN.texcoord.xy * _BVoronoiScale;
                float2 id16 = 0;
                float2 uv16 = 0;
                float voroi16 = voronoi16( coords16, time16, id16, uv16, 0, voronoiSmoothId16 );
                

                half4 color = tex2D( _MainTex1, ( texCoord38 + ( ( voroi13 * voroi16 ) * _Power ) ) ) * _Color;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.VoronoiNode;13;-1355.137,-246.2308;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.VoronoiNode;16;-1351.536,95.37601;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleTimeNode;18;-1676.34,92.55552;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1608.536,194.376;Inherit;False;Property;_BVoronoiScale;B Voronoi Scale;4;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1980.387,10.43479;Inherit;False;Property;_Fast;Fast;1;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-862.6766,163.2217;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1125.182,-44.35752;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-451.3619,-8.972288;Inherit;True;Property;_MainTex1;MainTex;0;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-108.1518,-4.351486;Float;False;True;-1;2;ASEMaterialInspector;0;3;UI/Shd_UI_Noise;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;2;5;False;;10;False;;2;5;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;-936.422,-224.2674;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1662.137,-118.2308;Inherit;False;Property;_AVoronoiScale;A Voronoi Scale;3;0;Create;True;0;0;0;False;0;False;7;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1657.941,-225.0513;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-624.3955,-101.6696;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1143.677,257.2217;Inherit;False;Property;_Power;Power;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
WireConnection;13;1;10;0
WireConnection;13;2;14;0
WireConnection;16;1;18;0
WireConnection;16;2;17;0
WireConnection;18;0;26;0
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;22;0;13;0
WireConnection;22;1;16;0
WireConnection;25;1;37;0
WireConnection;0;0;25;0
WireConnection;10;0;26;0
WireConnection;37;0;38;0
WireConnection;37;1;23;0
ASEEND*/
//CHKSM=A4ADD0AC06C670F2701226521343313E368DC94A