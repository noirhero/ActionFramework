// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(CharacterLoadSystem))]
public class CharacterLoadTimerSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
            .WithName("CharacterLoadTimerSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref CharacterLoadTimerComponent timer) => {
                if (0.0f >= timer.tick) {
                    EntityManager.RemoveComponent<CharacterLoadTimerComponent>(entity);
                    return;
                }

                // initialize once
                if (EntityManager.HasComponent<CharacterLoadComponent>(entity)) {
                    var initialLoad = EntityManager.GetComponentData<CharacterLoadComponent>(entity);
                    timer.id = initialLoad.id;
                    timer.pos = initialLoad.pos;
                    EntityManager.RemoveComponent<CharacterLoadComponent>(entity);
                }

                timer.elapsedTick += Time.DeltaTime;
                if (timer.tick <= timer.elapsedTick) {
                    EntityManager.AddComponentData(entity, new CharacterLoadComponent {
                        id = timer.id,
                        pos = timer.pos
                    });
                    timer.elapsedTick = 0.0f;

                    if (false == timer.loop) {
                        EntityManager.RemoveComponent<CharacterLoadTimerComponent>(entity);
                    }
                }
            })
            .Run();
    }
}
