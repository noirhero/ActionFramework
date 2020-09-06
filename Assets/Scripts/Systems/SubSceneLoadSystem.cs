// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using SuperTiled2Unity;
using Unity.Entities;
using Unity.Scenes;

public class SubSceneLoadSystem : SystemBase {
    private SubScene GetIdBySubScene(IdUtility.GUIId id, SubSceneComponent subSceneComp) {
        switch (id) {
            case IdUtility.GUIId.Title: return subSceneComp.title;
            case IdUtility.GUIId.InGame: return subSceneComp.inGame;
            case IdUtility.GUIId.Result: return subSceneComp.result;
        }

        return null;
    }

    protected override void OnUpdate() {
        if (Entity.Null == Utility.SystemEntity) {
            return;
        }

        if (false == EntityManager.HasComponent<SubSceneComponent>(Utility.SystemEntity)) {
            return;
        }

        var deltaTime = Time.DeltaTime;
        var requestLoadIds = new List<IdUtility.GUIId>();
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref SubSceneLoadComponent subSceneControl) => {
                subSceneControl.delay -= deltaTime;
                if (0.0f < subSceneControl.delay) {
                    return;
                }

                requestLoadIds.Add(subSceneControl.id);
                EntityManager.DestroyEntity(entity);
            })
            .Run();
        if (requestLoadIds.IsEmpty()) {
            return;
        }

        var subSceneComp = EntityManager.GetSharedComponentData<SubSceneComponent>(Utility.SystemEntity);
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .WithAll<SceneSectionData>()
            .ForEach((Entity subSceneEntity, SubScene subScene) => {
                foreach (var id in requestLoadIds) {
                    var checkSubScene = GetIdBySubScene(id, subSceneComp);
                    if (checkSubScene == subScene) {
                        if (EntityManager.HasComponent<RequestSceneLoaded>(subSceneEntity)) {
                            return;
                        }

                        EntityManager.AddComponentData(subSceneEntity, new RequestSceneLoaded {
                        });
                        return;
                    }
                }
            })
            .Run();
    }
}
