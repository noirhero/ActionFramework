// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FadeInOutRenderer), PostProcessEvent.AfterStack, "Custom/FadeInOutEffect")]
public sealed class FadeInOutEffect : PostProcessEffectSettings {
    [Range(0f, 1f), Tooltip("Fade in out effect intensity.")]
    public FloatParameter value = new FloatParameter {value = 1.0f};

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
        return enabled.value && value.value < 1.0f;
    }
}

public sealed class FadeInOutRenderer : PostProcessEffectRenderer<FadeInOutEffect> {
    private const string StrValue = "_Value";
    private static readonly int Value = Shader.PropertyToID(StrValue);

    private readonly Shader _fadeInOutShader = Shader.Find("Hidden/FadeInOutEffect");

    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(_fadeInOutShader);
        sheet.properties.SetFloat(Value, settings.value);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
