// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct InputComponent : IComponentData {
}

[Serializable]
public struct InputDataComponent : IComponentData {
    public int state;
    public float dir;
}

[Serializable]
public struct MoveComponent : IComponentData {
    public Vector2 value;
    public float accumTime;
}

[Serializable]
public struct JumpComponent : IComponentData {
    public float force;
    public float accumTime;

    public JumpComponent(float inForce) {
        force = inForce;
        accumTime = 0.0f;
    }
}

[Serializable]
public struct FallComponent : IComponentData {
    public float force;
    public float accumTime;

    public FallComponent(float inForce) {
        force = inForce;
        accumTime = 0.0f;
    }
}

[Serializable]
public struct AttackComponent : IComponentData {
    public float accumTime;
}