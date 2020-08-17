// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct HitComponent : IComponentData {
    public int damage;
    public float godTime; // 무적 시간
    public Color hitEffectColor;
    public float hitEffectTime;
    [HideInInspector] public float elapsedTime;
}
