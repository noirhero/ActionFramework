// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct CharacterLoadTimerComponent : IComponentData {
    public float tick;
    public bool loop;
    public EffectUtility.Key effectId;
    public float effectTick;
    [HideInInspector] public bool effectPlayed;
    [HideInInspector] public float elapsedTick;
    [HideInInspector] public IdUtility.Id id;
    [HideInInspector] public float3 pos;
}
