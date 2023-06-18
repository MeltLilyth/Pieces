shader "custom/TexAttr"{
    properties{
        _MainTexArray("Main Texture2D Array", 2DArray) = ""{}
        _Index("Texture Array Index", Range(0,3)) = 0
    }
    subshader{
        Tags{"RenderType"="Opaque"}
        Cull off ZWrite off
        Pass{
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            int _Index;
            UNITY_DECLARE_TEX2DARRAY(_MainTexArray);

            struct VertexInput{
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct VertexOutput{
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            VertexOutput vert(VertexInput i){
                VertexOutput o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                o.color = i.color;
                return o;
            }

            half4 frag(VertexOutput i) : COLOR{
                return UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(i.uv.xy, _Index));
            }

            ENDCG
        }
    }
}