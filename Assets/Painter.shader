Shader "Hidden/Painter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            uniform float4 _color;
            uniform float _brushSize;
            uniform StructuredBuffer<float> _pathX;
            uniform StructuredBuffer<float> _pathY;
            uniform int _pathPoints;


            fixed4 frag(v2f i) : COLOR
            {
                float2 c = i.uv; //c.x, c.y
                float4 resultCol = float4(0, 0, 0, 0);

                resultCol = tex2D(_MainTex, c);


                for (int i = 1; i < _pathPoints; i++)
                {
                    if (_pathPoints < 2) {
                        break;
                    }

                    // calculating line equation y = Ax + C
                    float A = (_pathY[i - 1] - _pathY[i]) / (_pathX[i - 1] - _pathX[i]);
                    float C = _pathY[i] - (A * _pathX[i]);
                    
                    bool HorizontalLine = true;
                    if (abs(A) > 1) {
                        HorizontalLine = false;
                    }


                    bool inRange = false;

                    if (HorizontalLine) {
                        if (_pathX[i - 1] < _pathX[i]) {
                            if (c.x > _pathX[i - 1] && c.x < _pathX[i]) {
                                inRange = true;
                            }
                        }
                        else {
                            if (c.x < _pathX[i - 1] && c.x > _pathX[i]) {
                                inRange = true;
                            }
                        }
                    }
                    else {
                        if (_pathY[i - 1] < _pathY[i]) {
                            if (c.y > _pathY[i - 1] && c.y < _pathY[i]) {
                                inRange = true;
                            }
                        }
                        else {
                            if (c.y < _pathY[i - 1] && c.y > _pathY[i]) {
                                inRange = true;
                            }
                        }
                    }

                    if (inRange) 
                    {
                        float dist = abs(A * c.x - c.y + C) / sqrt(pow(A, 2) + 1);
                        if (abs(dist) < _brushSize) {
                            resultCol.rgb = float3(resultCol.r * (1 - _color.a) + _color.r * _color.a, resultCol.g * (1 - _color.a) + _color.g * _color.a, resultCol.b * (1 - _color.a) + _color.b * _color.a);
                            break;
                        }
                    } 

                    // circle brush
                    //float dist = sqrt(pow(c.x - _pathX[i], 2) + pow(c.y - _pathY[i], 2));
                    //if (dist < _brushSize)
                    //{
                    //    resultCol.rgb = float3(resultCol.r * (1 - _color.a) + _color.r * _color.a, resultCol.g * (1 - _color.a) + _color.g * _color.a, resultCol.b * (1 - _color.a) + _color.b * _color.a);
                    //    break;
                    //}
                }
                return resultCol;
            }
            ENDCG
        }
    }
}
