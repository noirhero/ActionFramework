// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Entities;
using System.Collections.Generic;

public class AutoDestroySystem : SystemBase {
    private EntityCommandBufferSystem _cmdBufSystem;

    protected override void OnCreate() {
        _cmdBufSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var deltaTime = Time.DeltaTime;
        var cmdBuf = _cmdBufSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithName("AutoDestroySystem_Find")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithStructuralChanges()
            .ForEach(
                (Entity entity, SpritePresetComponent preset, ref FindAutoDestroyComponent findDestroyComp) => {
                    if (0 == findDestroyComp.loopingCount) {
                        return;
                    }

                    if (false == preset.value.datas.TryGetValue(AnimUtility.AnimKey.Run, out var animData)) {
                        return;
                    }

                    EntityManager.RemoveComponent<FindAutoDestroyComponent>(entity);
                    EntityManager.AddComponentData(entity, new AutoDestroyComponent {
                        lifeTime = animData.length * findDestroyComp.loopingCount
                    });
                })
            .Run();

        Entities.WithName("AutoDestroySystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref AutoDestroyComponent autoDestroy) => {
                autoDestroy.lifeTime -= deltaTime;
                if (0.0f < autoDestroy.lifeTime) {
                    return;
                }

                cmdBuf.DestroyEntity(entityInQueryIndex, entity);
            })
            .ScheduleParallel();
        _cmdBufSystem.AddJobHandleForProducer(Dependency);
    }
}
