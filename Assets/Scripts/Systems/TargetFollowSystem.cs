// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TargetFollowSystem : SystemBase {
    protected override void OnUpdate() {
        var idList = new List<IdUtility.Id>();
        var posList = new List<float3>();
        Entities
            .WithName("TargetFollowSystem_Collecting")
            .WithoutBurst()
            .ForEach((in TargetIdComponent id, in Translation pos) => {
                idList.Add(id.value);
                posList.Add(pos.Value);
            })
            .Run();
        var count = idList.Count;
        if (0 == count) {
            return;
        }

        var idArray = new NativeArray<IdUtility.Id>(count, Allocator.TempJob);
        var posArray = new NativeArray<float3>(count, Allocator.TempJob);
        for (var i = 0; i < count; ++i) {
            idArray[i] = idList[i];
            posArray[i] = posList[i];
        }

        var deltaTime = Time.DeltaTime;
        Entities
            .WithName("TargetFollowSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((int nativeThreadIndex, ref Translation pos, in TargetIdFollowComponent follow) => {
                for (var i = 0; i < idArray.Length; ++i) {
                    if (follow.followId != idArray[i]) {
                        continue;
                    }

                    pos.Value = math.lerp(pos.Value, posArray[i], deltaTime * follow.speed);
                    break;
                }
            })
            .WithDeallocateOnJobCompletion(idArray)
            .WithDeallocateOnJobCompletion(posArray)
            .ScheduleParallel(Dependency)
            .Complete();
    }
}
