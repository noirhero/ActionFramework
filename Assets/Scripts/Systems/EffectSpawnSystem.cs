// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EffectSpawnSystem : SystemBase {
    private EndSimulationEntityCommandBufferSystem _cmdBufSystem;

    protected override void OnCreate() {
        _cmdBufSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var effectGuidList = new List<EffectGuidComponent>();
        Entities
            .WithName("EffectSpawnSystem_Collecting")
            .WithoutBurst()
            .ForEach((in EffectGuidComponent effectGuid) => { effectGuidList.Add(effectGuid); })
            .Run();

        var effectGuidArray = new NativeArray<EffectGuidComponent>(effectGuidList.Count, Allocator.Temp);
        for (int i = 0; i < effectGuidArray.Length; ++i) {
            effectGuidArray[i] = effectGuidList[i];
        }

        var oneUniformScale = new float3(1.0f, 1.0f, 1.0f);
        var cmdBuf = _cmdBufSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in EffectSpawnComponent spawnInfo) => {
                foreach (var effectGuid in effectGuidArray) {
                    if (spawnInfo.id != effectGuid.id) {
                        continue;
                    }
        
                    var spawnEntity = cmdBuf.Instantiate(entityInQueryIndex, effectGuid.prefab);
        
                    cmdBuf.SetComponent(entityInQueryIndex, spawnEntity, new Translation {
                        Value = spawnInfo.pos
                    });
        
                    if (false == quaternion.identity.Equals(spawnInfo.rot)) {
                        cmdBuf.SetComponent(entityInQueryIndex, spawnEntity, new Rotation {
                            Value = spawnInfo.rot
                        });
                    }
        
                    if (false == oneUniformScale.Equals(spawnInfo.scale)) {
                        cmdBuf.SetComponent(entityInQueryIndex, spawnEntity, new NonUniformScale {
                            Value = spawnInfo.scale
                        });
                    }
        
                    break;
                }
        
                cmdBuf.DestroyEntity(entityInQueryIndex, entity);
            })
            .WithDeallocateOnJobCompletion(effectGuidArray)
            .ScheduleParallel();
        
        _cmdBufSystem.AddJobHandleForProducer(Dependency);
    }
}
