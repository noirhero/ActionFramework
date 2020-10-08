// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
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

                // load char
                timer.elapsedTick += Time.DeltaTime;
                if (timer.tick <= timer.elapsedTick) {
                    EntityManager.AddComponentData(entity, new CharacterLoadComponent {
                        id = timer.id,
                        pos = timer.pos
                    });
                    
                    // reset
                    timer.elapsedTick = 0.0f;
                    timer.effectPlayed = false;
                    
                    // done
                    if (false == timer.loop) {
                        EntityManager.RemoveComponent<CharacterLoadTimerComponent>(entity);
                    }
                }
                
                // load effect
                if (false == timer.effectPlayed && EffectUtility.Key.None != timer.effectId) {
                    if (timer.effectTick <= timer.elapsedTick) {
                        var effectSpawnEntity = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(effectSpawnEntity, new EffectSpawnComponent {
                            id = timer.effectId,
                            pos = timer.pos,
                            rot = quaternion.identity,
                            scale = new float3(1.0f)
                        });
                        timer.effectPlayed = true;
                    }
                }
            })
            .Run();
    }
}
