Shader "Unlit/ParticleAlphaBlend_Dissolve"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" } //! Alpha Blended로 세팅
		Blend SrcAlpha OneMinusSrcAlpha //! Alpha Blended로 세팅
		ZWrite off //! ZBuffer 사용하지 않음(Alpha Blend 쉐이더에서는 보통 ZBuffer사용하지 않음)

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata //! Mesh나 파티클시스템에서 입력되는 정보 구조체
			{
				float4 vertex : POSITION; //! Vertex 좌표 데이터
				float3 uv : TEXCOORD0;	  //! Mesh UV 정보
				fixed4 color : COLOR;	  //! Particle System에서 받는 색상값(파티클 시스템에서 설정한 색이 해당 변수로 입력됨)
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;        //! 텍스처 UV 데이터
				fixed4 color : COLOR;		  //! Particle System에서 받는 색상값
				float4 vertex : SV_POSITION;  //! Vertex 좌표 데이터

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;	//! 텍스처 Tiling Offset 정보

			v2f vert(appdata v)
			{	//! 이곳은 Vertex 쉐이더 함수 Fragment 쉐이더 함수로 정보 넘기는 과정
				v2f o;
				o.color = v.color;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
				o.uv.z = v.uv.z;		//! CustomData로 입력된 Z값을 전달
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{//! 이곳은 Fragment쉐이더, 픽셀을 담당하는 부분

				float fCustomData1 = i.uv.z;	//! uv.z로 받은 CustomData를 변수를 만들어서 할당

				fixed4 col = tex2D(_MainTex, i.uv);
			
				float4 fFinalColor;
				fFinalColor.rgb = i.color.rgb;
				fFinalColor.a = ceil(col.r - fCustomData1) * i.color.a;	//! 간단한 Dissolve공식 * ParticleSystem의 알파색상 곱하기
				return fFinalColor;
			}
			ENDCG
		}
	}
}
