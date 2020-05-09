// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(GrayEffectRenderer), PostProcessEvent.AfterStack, "Custom/GrayEffect")]
public sealed class GrayEffect : PostProcessEffectSettings {
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter {value = 0.0f};

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
        return enabled.value && blend.value > 0f;
    }
}

public sealed class GrayEffectRenderer : PostProcessEffectRenderer<GrayEffect> {
    private const string StrBlend = "_Blend";
    private static readonly int Blend = Shader.PropertyToID(StrBlend);

    private readonly Shader _grayShader = Shader.Find("Hidden/GrayEffect");

    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(_grayShader);
        sheet.properties.SetFloat(Blend, settings.blend);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
