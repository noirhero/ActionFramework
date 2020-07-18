// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraFollowSystem : SystemBase {
    private Transform _cameraTransform;
    protected override void OnStartRunning() {
        if (false == ReferenceEquals(null, Camera.main)) {
            _cameraTransform = Camera.main.transform;
        }
    }

    protected override void OnUpdate() {
        var camPos = _cameraTransform.position;
        var deltaTime = Time.DeltaTime;
        Entities
            .WithoutBurst()
            .WithAll<CameraFollowComponent>()
            .ForEach((in Translation pos) => {
                camPos.x = math.lerp(camPos.x, pos.Value.x, deltaTime);
            })
            .Run();
        _cameraTransform.position = camPos;
    }
}
