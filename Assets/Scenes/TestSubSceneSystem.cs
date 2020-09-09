// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class TestSubSceneSystem : SystemBase {
    protected override void OnCreate() {
        //Enabled = false;
    }

    private float _accumTime = 0.0f;
    protected override void OnUpdate() {
        _accumTime += Time.DeltaTime;
        if (1.0f > _accumTime) {
            return;
        }
        _accumTime = 0.0f;

        var subSceneComp = EntityManager.GetSharedComponentData<SubSceneComponent>(Utility.SystemEntity);
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, SceneSectionData sectionData) => {
                if (subSceneComp.title.SceneGUID != sectionData.SceneGUID) {
                    return;
                }

                if (EntityManager.HasComponent<RequestSceneLoaded>(entity)) {
                    EntityManager.RemoveComponent<RequestSceneLoaded>(entity);
                    Debug.Log("Unload");
                }
                else {
                    EntityManager.AddComponentData(entity, new RequestSceneLoaded {
                    });
                    Debug.Log("Load");
                }
            })
            .Run();
    }
}
