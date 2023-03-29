// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shd_Dissolve_OpacityMask"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Noise_Texture("Noise_Texture", 2D) = "white" {}
		_Dissolve("Dissolve", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Noise_Texture;
		uniform float _Dissolve;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color11 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			o.Emission = color11.rgb;
			o.Alpha = 1;
			float ifLocalVar1 = 0;
			if( tex2D( _Noise_Texture, i.uv_texcoord ).r > _Dissolve )
				ifLocalVar1 = 1.0;
			else if( tex2D( _Noise_Texture, i.uv_texcoord ).r < _Dissolve )
				ifLocalVar1 = 0.0;
			clip( ifLocalVar1 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Shd_Dissolve_OpacityMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ConditionalIfNode;1;-546.4479,119.9439;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-404.7867,-118.0191;Inherit;False;Constant;_Main_Color;Main_Color;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-929.8063,-23.57705;Inherit;True;Property;_Noise_Texture;Noise_Texture;1;0;Create;True;0;0;0;False;0;False;-1;bdc3aadbc87209c4ab2e0ccc461befd1;bdc3aadbc87209c4ab2e0ccc461befd1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1301.802,-704.3752;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;15;-639.8919,-650.5883;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-1013.889,-583.3527;Inherit;True;Property;_Noise_Texture1;Noise_Texture;2;0;Create;True;0;0;0;False;0;False;-1;79b28d4d15df6c8479f30603c87d48e3;bdc3aadbc87209c4ab2e0ccc461befd1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1032.521,-347.1492;Inherit;False;Property;_Lerp_Control;Lerp_Control;4;0;Create;True;0;0;0;False;0;False;0.4909582;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1186.304,-103.5793;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-975.0786,407.1473;Inherit;False;Constant;_White;White;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-953.7775,518.4363;Inherit;False;Constant;_Black;Black;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1081.125,276.243;Inherit;False;Property;_Dissolve;Dissolve;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
WireConnection;0;2;11;0
WireConnection;0;10;1;0
WireConnection;1;0;3;1
WireConnection;1;1;4;0
WireConnection;1;2;8;0
WireConnection;1;4;9;0
WireConnection;3;1;10;0
WireConnection;15;0;16;2
WireConnection;15;1;18;1
WireConnection;15;2;17;0
WireConnection;18;1;16;0
ASEEND*/
//CHKSM=35B200CCAA500D709B584093DE23183D57EA5EB2