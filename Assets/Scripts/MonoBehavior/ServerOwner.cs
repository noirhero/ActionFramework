// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

public class ServerOwner : MonoBehaviour {
    void Start() {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ServerSystem>().Enabled = true;
    }
}
