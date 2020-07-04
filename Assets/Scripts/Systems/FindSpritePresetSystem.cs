// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;

public class FindSpritePresetSystem : SystemBase {
    protected override void OnUpdate() {
        Dictionary<int, SpritePresetComponent> presets = new Dictionary<int, SpritePresetComponent>();
        Entities
            .WithName("FindSpritePresetSystem_Collecting")
            .WithoutBurst()
            .ForEach((SpritePresetComponent preset, in GuidComponent guid) => {
                presets.Add(guid.value, preset);
            })
            .Run();

        Entities
            .WithName("FindSpritePresetSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in FindSpritePresetComponent findGuid) => {
                if (false == presets.TryGetValue(findGuid.value, out var preset)) {
                    return;
                }

                EntityManager.RemoveComponent<FindSpritePresetComponent>(entity);
                EntityManager.AddSharedComponentData(entity, preset);

                if (false == EntityManager.HasComponent<AnimationFrameComponent>(entity)) {
                    EntityManager.AddComponentData(entity, new AnimationFrameComponent() {
                        setId = Utility.AnimState.Idle,
                        currentId = Utility.AnimState.Idle,
                        bLooping = true
                    });
                }
            })
            .Run();
    }
}
