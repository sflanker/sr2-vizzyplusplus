Shader "Jundroo/Part Editor Testing" 
{
    Properties 
    {
        _ColorPrimary ("Primary", Color) = (1,1,1,1)
        _ColorTrim1("Trim 1", Color) = (1,1,1,1)
        _ColorTrim2("Trim 2", Color) = (1,1,1,1)
        _ColorTrim3("Trim 3", Color) = (1,1,1,1)
        _ColorTrim4("Trim 4", Color) = (1,1,1,1)
        [Space(10)]
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input 
        {
            float4 data : TEXCOORD0;
        };

        half4 _ColorPrimary;
        half4 _ColorTrim1;
        half4 _ColorTrim2;
        half4 _ColorTrim3;
        half4 _ColorTrim4;
        half _Glossiness;
        half _Metallic;

        void vert(inout appdata_full v, out Input o)
        {
            o.data = v.texcoord;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            half4 c = half4(0, 0, 0, 0);

            int matId = int(IN.data.w % 10.0);
            if (matId == 0) { c += _ColorPrimary; }
            if (matId == 1) { c += _ColorTrim1; }
            if (matId == 2) { c += _ColorTrim2; }
            if (matId == 3) { c += _ColorTrim3; }
            if (matId == 4) { c += _ColorTrim4; }
            
            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
}
