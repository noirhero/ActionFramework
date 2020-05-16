// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct InputDataComponent : IComponentData {
    public int state;
}

[Serializable]
public struct InputMoveComponent : IComponentData {
    public Vector2 value;
    public float accumTime;
}

[Serializable]
public struct InputJumpComponent : IComponentData {
    public float accumTime;
}

[Serializable]
public struct InputAttackComponent : IComponentData {
    public float accumTime;
}
