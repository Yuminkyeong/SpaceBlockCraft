Shader "CWDecal" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_DecalTex("Decal (RGBA)", 2D) = "black" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}

		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 250

		CGPROGRAM
#pragma surface surf Lambert vertex:vert alphatest:_Cutoff
#pragma target 3.0


	sampler2D _MainTex;
	sampler2D _DecalTex;
	fixed4 _Color;
	half herox;
	half heroz;
	half radius;


	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
		float3 vertexColor; // Vertex color stored here by vert() method
	};
	struct v2f
	{
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
	}


	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		float2 uv2 ;
		uv2.x = (128+IN.worldPos.x)/256;
		uv2.y = (128+IN.worldPos.z) / 256;
		half4 decal = tex2D(_DecalTex, uv2);
		half rr = (IN.worldPos.x - herox)*(IN.worldPos.x - herox) + (IN.worldPos.z - heroz)*(IN.worldPos.z - heroz);
		half rate = min(1, rr / (radius*radius));

		c.rgb = lerp(c.rgb, decal.rgb, decal.a*rate);
		
		c *= (_Color);

		o.Albedo = c.rgb* IN.vertexColor;
		o.Alpha = c.a;

	}
	ENDCG
	}

		Fallback "Diffuse"
}
