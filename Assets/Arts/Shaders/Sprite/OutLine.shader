Shader "Unlit/OutLine"
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
            Cull Off
            Stencil {
                Ref 2        //参考值为2，stencilBuffer值默认为0
            	Comp greater
            	Pass replace
            	ZFail keep
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
    		    o.vertex = UnityObjectToClipPos(v.vertex*1.2);
    		    UNITY_TRANSFER_FOG(o,o.vertex);
    		    return o;
    		}
    		fixed4 frag (v2f i) : SV_Target
    		{
    		    return half4(1,0,0,1);
    		}
    		ENDCG
        }
    }
}
