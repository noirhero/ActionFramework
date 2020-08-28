// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Unity.Entities;

public class CharacterUnLoadSystem : SystemBase {
    protected override void OnUpdate() {
        var removeIds = new List<IdUtility.Id>();
        Entities
            .WithName("CharacterUnLoadSystem_Collecting")
            .WithoutBurst()
            .ForEach((in CharacterUnLoadComponent unload) => {
                removeIds.Add(unload.id);
            })
            .Run();

        Entities
            .WithName("CharacterUnLoadSystem")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in TargetIdComponent id) => {
                foreach (var removeId in removeIds) {
                    if (removeId == id.value) {
                        EntityManager.DestroyEntity(entity);
                        break;
                    }
                }
            })
            .Run();
    }
}
