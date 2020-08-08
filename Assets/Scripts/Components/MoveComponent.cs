// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct MoveComponent : IComponentData {
    public Vector2 value;
    public Vector2 impulseDir;
    public float impulseForce;
}
