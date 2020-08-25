// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct SubSceneUnLoadComponent : IComponentData {
    public IdUtility.GUIId id;
    public float delay;
}
