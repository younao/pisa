Shader "Custom/quan"
{
	Properties
	{
		_BGTex("Background Texture",2D) = "white"{}
	//[PerRendererData] 
	_MaskTex("Mask Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
	_Rate("Rate", range(0,100)) = 100
	_Center("Center", vector) = (.5,.5,0,0)
	_Width("Width", float) = 1
		//[Toggle(CLOCK_WISE)]
		_ColorMask("Color Mask", Float) = 15
		_MaskDetail("Mask Detail",range(0,1)) = 0.3
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
		 #pragma shader_feature CLOCK_WISE

		 #include "UnityCG.cginc"

		 float _Rate;
		 float4 _Center;
		 half _Width;

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
		   half2 mask_uv   : TEXCOORD0;
		   half2 bg_uv     : TEXCOORD1;
		  };

		  fixed4 _Color;
		  float4 _ClipRect;
		  bool _UseClipRect;
		  bool _UseAlphaClip;
		  fixed _MaskDetail;
		  sampler2D _MaskTex;
		  sampler2D _BGTex;
		  float4 _MaskTex_ST;
		  float4 _BGTex_ST;

		  v2f vert(appdata_t IN)
		  {
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.mask_uv = TRANSFORM_TEX(IN.texcoord,_MaskTex);
			OUT.bg_uv = TRANSFORM_TEX(IN.texcoord,_BGTex);
			OUT.color = IN.color * _Color;
			return OUT;
		  }

		  fixed4 frag(v2f IN) : SV_Target
		  {
			half4 color = tex2D(_MaskTex, IN.mask_uv)* IN.color;
			float2 pos = IN.mask_uv.xy - _Center.xy;
			float ang = degrees(atan2(pos.x, -pos.y)) + 180;
			float angle = _Rate * 3.6;
			color.a = color.a * saturate((ang - angle) / _Width);
			half4 bgcolor = tex2D(_BGTex, IN.bg_uv) * IN.color;
			float mask_a = step(_MaskDetail,color.a);
			color = mask_a * color + (1 - mask_a)*bgcolor;
			return color;
		   }
		ENDCG
	   }
	}
}