// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEditor;

public class HitSystem : SystemBase {

    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref HitComponent hitComp, ref AnimationFrameComponent animComp) => {
                if (false == AnimUtility.HasState(animComp, AnimUtility.hit)) {
                    animComp.state |= AnimUtility.hit;
                }

                hitComp.elapsedTime += Time.DeltaTime;
                if (hitComp.elapsedTime >= hitComp.godTime) {

                    /* TODO : 데미지 처리! */

                    animComp.state ^= AnimUtility.hit;
                    EntityManager.RemoveComponent<HitComponent>(entity);
                }
            }).Run();
    }
}