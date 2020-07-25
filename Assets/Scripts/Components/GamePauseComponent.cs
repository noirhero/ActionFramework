// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

[Serializable]
public struct GamePauseComponent : IComponentData {
    public float time;

    public GamePauseComponent(float pauseTime) {
        time = pauseTime;
    }
}
