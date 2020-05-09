// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

Shader "Hidden/FadeInOutEffect" {
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _Value;

    float4 Frag(VaryingsDefault i) : SV_Target {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        color.rgb = lerp(0, color.rgb, _Value);
        return color;
    }
    ENDHLSL

    SubShader {
        Cull Off
        ZWrite Off
        ZTest Off

        Pass {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
