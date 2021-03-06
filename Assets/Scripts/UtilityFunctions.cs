﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
    }

    public static int INDEX_NONE = -1;

    public static readonly float terminalVelocity = -30.0f;
    public static readonly float skinWidth = 0.01f;
    public static readonly float stepOffset = 0.01f;
    
    public static readonly string SaveDataName = "Score";
    public static readonly string StatJumpForceName = "jumpForce";
    public static readonly string StatGravityName = "gravity";
    public static readonly string StatSpeedXName = "speedX";
    public static readonly string StatSpeedYName = "speedY";
    public static readonly string StatTouchDeltaName = "touchDelta";
    
    public static float jumpForce {
        get {
            return PlayerPrefs.HasKey(Utility.StatJumpForceName) ? PlayerPrefs.GetFloat(Utility.StatJumpForceName) : 65.0f;
        }
        private set{}
    }
    public static float gravity {
        get {
            return PlayerPrefs.HasKey(Utility.StatGravityName) ? PlayerPrefs.GetFloat(Utility.StatGravityName) : 2.0f;
        }
        private set{}
    }
    public static float speedX {
        get {
            return PlayerPrefs.HasKey(Utility.StatSpeedXName) ? PlayerPrefs.GetFloat(Utility.StatSpeedXName) : 2.0f;
        }
        private set{}
    }
    public static float speedY {
        get {
            return PlayerPrefs.HasKey(Utility.StatSpeedYName) ? PlayerPrefs.GetFloat(Utility.StatSpeedYName) : 0.1f;
        }
        private set{}
    }
    public static float touchDelta {
        get {
            return PlayerPrefs.HasKey(Utility.StatTouchDeltaName) ? PlayerPrefs.GetFloat(Utility.StatTouchDeltaName) : 1.0f;
        }
        private set{}
    }
    
    public static Entity SystemEntity { get; private set; }

    public static void SetSystemEntity(Entity entity) {
        if (Entity.Null == SystemEntity) {
            SystemEntity = entity;
        }
    }

    // TODO : temporary 
    public static IdUtility.GameStateId GetGameState() {
        GUISystem guiSystem = null;
        
        foreach (var system in World.DefaultGameObjectInjectionWorld.Systems) {
            if (system.GetType() == typeof(GUISystem)) {
                guiSystem = (GUISystem) system;
            }
        }

        if (null == guiSystem) {
            return IdUtility.GameStateId.Unknown;
        }

        switch (guiSystem.uiState) {
            case IdUtility.GUIId.Title: {
                return IdUtility.GameStateId.Title;
            }
            case IdUtility.GUIId.InGame: {
                if (GameStart.bIsPlayed) {
                    return IdUtility.GameStateId.Play;
                }
                else {
                    return IdUtility.GameStateId.Pause;
                }
            }
            case IdUtility.GUIId.Result: {
                return IdUtility.GameStateId.Result;
            }
            case IdUtility.GUIId.Over: {
                return IdUtility.GameStateId.Over;
            }
            default: {
                return IdUtility.GameStateId.Unknown;
            }
        }
    } 
}

public static class IdUtility {
    public enum Id {
        Player,
        Enemy,
        NPC
    }
    
    public enum GUIId {
        None,
        Title,
        InGame,
        Over,
        Result,
    }
    
    public enum GameStateId {
        Unknown,
        Title,
        Play,
        Pause,
        Over,
        Result,
    }
}

public static class EffectUtility {
    public enum Key {
        None = 0,
        Hit = 1 << 0,
        FootStep = 1 << 1,
        Landing = 1 << 2,
        Spawn = 1 << 3,
    }
}

public static class SoundUtility {
    public enum SourceKey {
        Title,
        GhostTown,
        End1,
        End2,
    }

    public enum ClipKey {
        GhostTown = 1 << 0,
        FootStep = 1 << 1,
        Sword = 1 << 2,
        Landing = 1 << 3,
        Hit = 1 << 4,
        Button = 1 << 5,
        Damage = 1 << 6,
        GhostSpawn = 1 << 7,
    }
}

public static class AnimUtility {
    public enum AnimKey {
        Idle,
        Run,
        Jump,
        Attack,
        Crouch,
        Block,
        Dash,
        Hit,
    }

    public const int run = 1 << 0;
    public const int jump = 1 << 1;
    public const int attack = 1 << 2;
    public const int hit = 1 << 3;
    public const int crouch = 1 << 4;

    public static bool HasState(AnimationFrameComponent inAnimComp, int insState) {
        return 0 != (inAnimComp.state & insState);
    }

    public static bool IsLooping(AnimationFrameComponent inAnimComp) {
        if (inAnimComp.currentAnim == AnimKey.Run ||
            inAnimComp.currentAnim == AnimKey.Idle) {
            return true;
        }

        return false;
    }

    public static AnimKey GetAnimKey(AnimationFrameComponent inAnimComp) {
        // 우선 순위별
        if (HasState(inAnimComp, hit)) {
            return AnimKey.Hit;
        }

        if (HasState(inAnimComp, crouch)) {
            return AnimKey.Crouch;
        }

        if (HasState(inAnimComp, jump)) {
            return AnimKey.Jump;
        }

        if (HasState(inAnimComp, attack)) {
            return AnimKey.Attack;
        }

        if (HasState(inAnimComp, run)) {
            return AnimKey.Run;
        }

        return AnimKey.Idle;
    }

    public static bool IsChangeAnim(AnimationFrameComponent inAnimComp, int inState) {
        // 조건 정리
        if (0 != (run & inState)) {
            if (AnimKey.Attack == inAnimComp.currentAnim ||
                AnimKey.Crouch == inAnimComp.currentAnim) {
                return false;
            }
        }
        else if (0 != (crouch & inState)) {
            if (AnimKey.Jump == inAnimComp.currentAnim ||
                AnimKey.Attack == inAnimComp.currentAnim ) {
                return false;
            }
        }
        else if (0 != (jump & inState)) {
            if (AnimKey.Crouch == inAnimComp.currentAnim ||
                AnimKey.Attack == inAnimComp.currentAnim ||
                AnimKey.Jump == inAnimComp.currentAnim) {
                return false;
            }
        }
        else {
            if (AnimKey.Attack == inAnimComp.currentAnim ||
                AnimKey.Hit == inAnimComp.currentAnim ||
                AnimKey.Jump == inAnimComp.currentAnim ||
                AnimKey.Crouch == inAnimComp.currentAnim) {
                return false;
            }
        }

        return true;
    }

    public static string ShowLog(AnimationFrameComponent animComp) {
        var cachedStr = string.Empty;

        if (0 != (run & animComp.state)) {
            cachedStr += " run";
        }

        if (0 != (jump & animComp.state)) {
            cachedStr += " jump";
        }

        if (0 != (attack & animComp.state)) {
            cachedStr += " attack";
        }

        if (0 != (hit & animComp.state)) {
            cachedStr += " hit";
        }

        if (0 != (crouch & animComp.state)) {
            cachedStr += " crouch";
        }

        return cachedStr;
    }
}

public static class InputUtility {
    public const int left = 1 << 0;
    public const int right = 1 << 1;
    public const int jump = 1 << 2;
    public const int attack = 1 << 3;
    public const int axis = 1 << 4;
    public const int crouch = 1 << 5;

    public static bool IsNone(int state) {
        return 0 == state;
    }

    public static bool HasState(InputDataComponent inputComp, int compareState) {
        return 0 != (inputComp.state & compareState);
    }

    public static string ShowLog(InputDataComponent inputComp) {
        var cachedStr = string.Empty;

        if (0 != (left & inputComp.state)) {
            cachedStr += " Left";
        }

        if (0 != (right & inputComp.state)) {
            cachedStr += " Right";
        }

        if (0 != (jump & inputComp.state)) {
            cachedStr += " Jump";
        }

        if (0 != (attack & inputComp.state)) {
            cachedStr += " Attack";
        }

        if (0 != (axis & inputComp.state)) {
            cachedStr += " Axis";
        }

        if (0 != (crouch & inputComp.state)) {
            cachedStr += " Crouch";
        }

        return cachedStr;
    }
}

public struct ScoreData {
    public double lastScore;
    public double HighScore;
    public bool bNewHighScore;
    
    public ScoreData(double inScore) {
        lastScore = inScore;
        HighScore = inScore;
        bNewHighScore = true;
    }

    public void RecordScore(double inTime) {
        bNewHighScore = inTime > HighScore;
        lastScore = inTime;
        HighScore = bNewHighScore ? lastScore : HighScore;
    }
}
