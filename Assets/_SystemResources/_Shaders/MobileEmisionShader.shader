Shader "Custom/CustomShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        fixed4 _EmissionColor;

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Emission = _EmissionColor.rgb;
        }
        ENDCG
    }
    // Fallback "Diffuse"
}
