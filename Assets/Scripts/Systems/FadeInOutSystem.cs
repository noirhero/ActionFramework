// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FadeInOutSystem : SystemBase {
    public FloatParameter valueParam;

    protected override void OnCreate() {
        valueParam = new FloatParameter { value = 1.0f };
    }

    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAny<FadeInComponent, FadeOutComponent>()
            .ForEach((Entity entity) => {
                if (EntityManager.HasComponent<FadeInComponent>(entity)) {
                    var fadeInComp = EntityManager.GetComponentData<FadeInComponent>(entity);
                    fadeInComp.elapsedTime += Time.DeltaTime;

                    // done
                    if (fadeInComp.time <= fadeInComp.elapsedTime) {
                        valueParam.value = 0.0f;
                        EntityManager.RemoveComponent<FadeInComponent>(entity);
                        return;
                    }

                    float time = fadeInComp.elapsedTime / fadeInComp.time;
                    valueParam.value = Mathf.Lerp(1.0f, 0.0f, time);
                    EntityManager.SetComponentData<FadeInComponent>(entity, fadeInComp);
                }
                else {
                    var fadeOutComp = EntityManager.GetComponentData<FadeOutComponent>(entity);
                    fadeOutComp.elapsedTime += Time.DeltaTime;

                    // done
                    if (fadeOutComp.time <= fadeOutComp.elapsedTime) {
                        valueParam.value = 1.0f;
                        EntityManager.RemoveComponent<FadeOutComponent>(entity);
                        return;
                    }

                    float time = fadeOutComp.elapsedTime / fadeOutComp.time;
                    valueParam.value = Mathf.Lerp(0.0f, 1.0f, time);
                    EntityManager.SetComponentData<FadeOutComponent>(entity, fadeOutComp);
                }

                var volume = Camera.main.GetComponent<PostProcessVolume>();
                var fadeInOutEffect = volume.profile.GetSetting<FadeInOutEffect>();
                fadeInOutEffect.value.value = valueParam.value;
            }).Run();
    }
}
