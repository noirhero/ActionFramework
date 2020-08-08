// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

public class HitSystem : SystemBase {

    protected override void OnUpdate() {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, SpriteRenderer renderer, ref HitComponent hitComp, ref AnimationFrameComponent animComp) => {
                if (false == AnimUtility.HasState(animComp, AnimUtility.hit)) {
                    animComp.state |= AnimUtility.hit;
                    renderer.color = hitComp.hitEffectColor;
                    GamePause.Pause(0.15f);
                }

                hitComp.elapsedTime += Time.DeltaTime;
                if ((hitComp.elapsedTime >= hitComp.hitEffectTime) && (renderer.color == hitComp.hitEffectColor)) {
                    renderer.color = Color.white;
                }
                else if (hitComp.elapsedTime >= hitComp.godTime) {

                    /* TODO : 데미지 처리! */

                    animComp.state ^= AnimUtility.hit;
                    EntityManager.RemoveComponent<HitComponent>(entity);
                    EntityManager.RemoveComponent<MoveComponent>(entity);
                }
            }).Run();
    }
}
