Shader "Unlit/Outline2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass{
            Name "Outline"
            Stencil{
                Ref 3        //参考值为2，stencilBuffer值默认为0
            	Comp always
            	Pass replace
            	ZFail replace
            }
        	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
    		#pragma multi_compile_fog
    		#include "UnityCG.cginc"
    		struct v2f
    		{
    		    UNITY_FOG_COORDS(0)
    		    float4 vertex : SV_POSITION;
    		};
        	
    		fixed4 _OutlineCol;
    		float _OutlineFactor;
    		
    		v2f vert (appdata_base v)
    		{
    		    v2f o;
    		    o.vertex = UnityObjectToClipPos(v.vertex);
    		    UNITY_TRANSFER_FOG(o,o.vertex);
    		    return o;
    		}
    		fixed4 frag (v2f i) : SV_Target
    		{
    		    return half4(0,1,0,1);
    		}
    		ENDCG
        }
    }
}
