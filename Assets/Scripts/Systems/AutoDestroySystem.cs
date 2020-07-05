// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Entities;

public class AutoDestroySystem : SystemBase {
    private EntityCommandBufferSystem _cmdBufSystem;
    protected override void OnCreate() {
        _cmdBufSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var deltaTime = Time.DeltaTime;
        var cmdBuf = _cmdBufSystem.CreateCommandBuffer().ToConcurrent();
        Entities
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
