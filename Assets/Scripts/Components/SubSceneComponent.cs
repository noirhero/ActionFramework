// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Scenes;

[Serializable]
public struct SubSceneComponent : ISharedComponentData, IEquatable<SubSceneComponent> {
    public SubScene title;
    public SubScene inGame;
    public SubScene result;

    public bool Equals(SubSceneComponent other) {
        return ReferenceEquals(title, other.title) && ReferenceEquals(inGame, other.inGame) && ReferenceEquals(result, other.result);
    }

    public override int GetHashCode() {
        return (ReferenceEquals(null, title) ? 0 : title.GetHashCode()) + (ReferenceEquals(null, inGame) ? 0 : inGame.GetHashCode()) + (ReferenceEquals(null, result) ? 0 : result.GetHashCode());
    }
}
