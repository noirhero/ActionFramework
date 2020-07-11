// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AttackCollisionComponent : IComponentData {
    public Rect bounds;
    public float pixelsPerUnit;
    public int animationFrame;
    public bool bShouldMultiCollide;
    public bool bIsConsumed;  // 이미 충돌 처리된 상태인지? (bShouldMultiCollide가 true인 경우 의미 없음)
}
