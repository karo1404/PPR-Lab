Shader "Hidden/Wirl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_intensity("Intenstity of effect", Range(0,1)) = 1
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

            sampler2D _MainTex;
			uniform float _intensity;
			uniform float _seed;
			uniform float _phase;
			uniform float _freq;
			uniform float _amp;
			uniform int _count;
			uniform float _xc[10];
			uniform float _yc[10];

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				float2 c = i.uv; //c.x, c.y
				float2 arg = float2(c.x - 0.5, c.y - 0.5);
                // just invert the colors
				float level = 0.35*col.r + 0.15*col.g + 0.5*col.b;
				float4 resultCol = float4(0, 0, 0, 0);
				
				//generowanie liczby losowej
				//float y = frac(sin(_seed+ dot(c, float2(12.9898, 78.233)))*43758.5453123);
				//float y =  _amp * pow(sin(pow(arg.x, 2) * _freq + pow(arg.y, 2)* _freq + _phase), 1);// / (1 + pow(arg.x, 6) + pow(arg.y, 6));
				
				float y = 0;
				for (int i = 0; i < _count; ++i) {
					float2 arg = 10 * float2(c.x - _xc[i], c.y - _yc[i]);
					y += _amp * pow(sin(pow(arg.x, 2) * _freq + pow(arg.y, 2)* _freq + _phase), 2) / (1 + pow(arg.x, 6) + pow(arg.y, 6));
				}
				y = y / _count;

				resultCol = tex2D(_MainTex, c + float2(y*_intensity, y*_intensity));
				resultCol.rgb = float3(resultCol.r*(1 - _intensity) + y * _intensity, resultCol.g*(1 - _intensity) + y * _intensity, resultCol.b*(1 - _intensity) + y * _intensity);//resultCol.rgb = float3(col.r*(1-_intensity)+_intensity*level, y, col.b*(1 - _intensity) + _intensity * level);
                return resultCol;
            }
            ENDCG
        }
    }
}
