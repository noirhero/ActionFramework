// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct TargetIdFollowComponent : IComponentData {
    public IdUtility.Id followId;
    public float speed;
}
