// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;

public class AutoDestroyAttachSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithName("AutoDestroyAttachSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .WithNone<FindSpritePresetComponent>()
            .ForEach((Entity entity, in SpritePresetComponent preset, in FindAutoDestroyComponent findDestroyComp) => {
                if (0 == findDestroyComp.loopingCount) {
                    EntityManager.RemoveComponent<FindAutoDestroyComponent>(entity);
                    return;
                }

                if (false == preset.value.datas.TryGetValue(AnimUtility.AnimKey.Run, out var animData)) {
                    return;
                }

                EntityManager.AddComponentData(entity, new AutoDestroyComponent {
                    lifeTime = animData.length * findDestroyComp.loopingCount
                });
                EntityManager.RemoveComponent<FindAutoDestroyComponent>(entity);
            })
            .Run();
    }
}
