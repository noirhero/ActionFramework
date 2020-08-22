// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public class InputSystemEX : ComponentSystem {
    private bool IsLocked() {
        if (Entity.Null == Utility.SystemEntity) {
            return true;
        }

        if (false == EntityManager.HasComponent<InputDataComponent>(Utility.SystemEntity)) {
            return true;
        }

        // TODO : other condition
        return false;
    }

    protected override void OnUpdate() {
        if (IsLocked()) {
            return;
        }

        Entities.ForEach((Entity entity, ref ControlComponent controlComp, ref AnimationFrameComponent animComp,
            ref MoveComponent moveComp) => {
            var inputDataComp = EntityManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);

#region Run
            moveComp.value.x = AnimUtility.IsChangeAnim(animComp, AnimUtility.run) ? inputDataComp.dir : 0.0f;
#endregion

#region Jump
            if (AnimUtility.IsChangeAnim(animComp, AnimUtility.jump) &&
                InputUtility.HasState(inputDataComp, InputUtility.jump)) {
                moveComp.value.y += Utility.jumpForce;

                // should be once play
                inputDataComp.state ^= InputUtility.jump;

                EntityManager.AddComponentData(entity, new JumpComponent());
            }
#endregion

#region Attack
            if (AnimUtility.IsChangeAnim(animComp, AnimUtility.attack) &&
                InputUtility.HasState(inputDataComp, InputUtility.attack)) {
                if (false == EntityManager.HasComponent<AttackComponent>(entity)) {
                    EntityManager.AddComponentData(entity, new AttackComponent());
                }

                // should be once play
                inputDataComp.state ^= InputUtility.attack;
            }
#endregion

#region Crouch
            if (AnimUtility.IsChangeAnim(animComp, AnimUtility.crouch) &&
                InputUtility.HasState(inputDataComp, InputUtility.crouch)) {
                if (false == EntityManager.HasComponent<CrouchComponent>(entity)) {
                    EntityManager.AddComponentData(entity, new CrouchComponent());

                    var colliderComponent = EntityManager.GetComponentData<PhysicsCollider>(entity);
                    unsafe {
                        var grabCollider = (CapsuleCollider*) colliderComponent.ColliderPtr;
                        var at = grabCollider->Vertex1 - grabCollider->Vertex0;

                        var geometry = grabCollider->Geometry;
                        geometry.Vertex1 = geometry.Vertex0 + at * 0.5f;
                        grabCollider->Geometry = geometry;
                    }
                }
            }
            else {
                if (EntityManager.HasComponent<CrouchComponent>(entity)) {
                    EntityManager.RemoveComponent<CrouchComponent>(entity);

                    var colliderComponent = EntityManager.GetComponentData<PhysicsCollider>(entity);
                    unsafe {
                        var grabCollider = (CapsuleCollider*) colliderComponent.ColliderPtr;
                        var at = grabCollider->Vertex1 - grabCollider->Vertex0;

                        var geometry = grabCollider->Geometry;
                        geometry.Vertex1 = geometry.Vertex0 + at * 2.0f;
                        grabCollider->Geometry = geometry;
                    }
                }
            }
#endregion

#region Gravity
            moveComp.value.y = math.max(moveComp.value.y - Utility.gravity, Utility.terminalVelocity);
#endregion

            EntityManager.SetComponentData(Utility.SystemEntity, inputDataComp);
        });
    }
}
