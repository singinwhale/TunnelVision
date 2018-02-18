// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/Fog" {
Properties {
    		_intensity ("Intensity", Float) = 1
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off


    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        // make fog work
        #pragma multi_compile_fog
        
        #include "UnityCG.cginc"
            
        float _intensity;
            
        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        struct v2f {
            float4 vertex : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            UNITY_FOG_COORDS(1)
        };
        
        v2f vert (appdata v)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o,o.vertex);
            return o;
        }
			
        fixed4 frag (v2f i) : SV_Target
        {
            float4 col = 1;
            //col.w = 0;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col * _intensity;
        }
			
        ENDCG
    }
}
}
