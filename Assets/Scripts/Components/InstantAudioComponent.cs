// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct InstantAudioComponent : IComponentData {
    public SoundUtility.ClipKey id;
    public Vector3 pos;
}
