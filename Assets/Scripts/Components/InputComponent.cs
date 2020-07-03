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
}

[Serializable]
public struct JumpComponent : IComponentData {
}

[Serializable]
public struct AttackComponent : IComponentData {
    public float accumTime;
}