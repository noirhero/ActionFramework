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

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in SceneSectionData subScene) => {
                if (EntityManager.HasComponent<RequestSceneLoaded>(entity)) {
                    EntityManager.RemoveComponent<RequestSceneLoaded>(entity);
                    Debug.Log("Unload");
                }
                else {
                    EntityManager.AddComponentData(entity, new RequestSceneLoaded {
                        //LoadFlags = SceneLoadFlags.NewInstance
                    });
                    Debug.Log("Load");
                }
            })
            .Run();
    }
}
