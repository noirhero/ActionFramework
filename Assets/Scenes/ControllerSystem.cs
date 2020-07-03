// Copyright 2018-20202 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ControllerSystem : JobComponentSystem {
    private BuildPhysicsWorld _buildPhysSystem;
    private EndFramePhysicsSystem _endPhysSystem;

    protected override void OnCreate() {
        _buildPhysSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _endPhysSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
    }

    private JobHandle _jobHandle;
    protected override unsafe JobHandle OnUpdate(JobHandle inputDeps) {
        _jobHandle.Complete();

        var deltaTime = UnityEngine.Time.fixedDeltaTime;
        var physWorld = _buildPhysSystem.PhysicsWorld;

        inputDeps = JobHandle.CombineDependencies(inputDeps, _buildPhysSystem.GetOutputDependency());
        _jobHandle = Entities
            .ForEach((ref Translation translation, in Rotation rotation, in ControllerAuthoringComponent controller, in PhysicsCollider collider) => {
                var newPos = translation.Value;
                newPos.y -= controller.gravity * deltaTime;

                if (physWorld.CastCollider(new ColliderCastInput {
                    Collider = collider.ColliderPtr,
                    Orientation = rotation.Value,
                    Start = translation.Value,
                    End = newPos
                }, out var hit)) {
                    translation.Value.y = math.lerp(translation.Value.y, newPos.y, hit.Fraction);
                }
                else {
                    translation.Value = newPos;
                }
            })
            .Schedule(inputDeps);
        _endPhysSystem.AddInputDependency(_jobHandle);

        return _jobHandle;
    }
}
