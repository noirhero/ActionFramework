// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

public class CharacterLoadSystem : SystemBase {
    protected override void OnUpdate() {
        var prefabs = new List<CharacterPrefabComponent>();
        Entities
            .WithName("CharacterLoadSystem_Collecting")
            .WithoutBurst()
            .ForEach((in CharacterPrefabComponent prefab) => {
                prefabs.Add(prefab);
            })
            .Run();

        Entities
            .WithName("CharacterLoadSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in CharacterLoadComponent load) => {
                foreach (var prefab in prefabs) {
                    if (prefab.id == load.id) {
                        var loadEntity = EntityManager.Instantiate(prefab.prefab);
                        EntityManager.SetComponentData(loadEntity, new Translation {
                            Value = load.pos
                        });
                        break;
                    }
                }

                if (EntityManager.HasComponent<CharacterLoadTimerComponent>(entity)) {
                    EntityManager.RemoveComponent<CharacterLoadComponent>(entity);
                }
                else {
                    EntityManager.DestroyEntity(entity);
                }
            })
            .Run();
    }
}
