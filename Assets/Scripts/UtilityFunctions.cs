// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;
using Unity.Entities;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
    }

    public static int INDEX_NONE = -1;

    // TODO : temporary constant -> status 
    public static readonly float jumpForce = 70.0f;
    public static readonly float gravity = 2.0f;
    public static readonly float terminalVelocity = -30.0f;
    public static readonly float speedX = 1.0f;
    public static readonly float speedY = 0.1f;
    public static readonly float skinWidth = 0.01f;
    public static readonly float stepOffset = 0.01f;

    public static Entity SystemEntity { get; private set; }

    public static void SetSystemEntity(Entity entity) {
        if (Entity.Null == SystemEntity) {
            SystemEntity = entity;
        }
    }
}

public static class IdUtility {
    public enum Id {
        Player,
        Enemy,
        NPC
    }
}

public static class EffectUtility {
    public enum Key {
        Hit = 1 << 0,
        FootStep = 1 << 1,
        Landing = 1 << 2,
    }
}

public static class SoundUtility {
    public enum SourceKey {
        GhostTown,
    }

    public enum ClipKey {
        GhostTown = 1 << 0,
        FootStep = 1 << 1,
        Sword = 1 << 2,
        Landing = 1 << 3,
        Hit = 1 << 4,
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

        if (HasState(inAnimComp, hit)) {
            return AnimKey.Hit;
        }

        return AnimKey.Idle;
    }

    public static bool IsChangeAnim(AnimationFrameComponent inAnimComp, int inState) {
        // 조건 정리
        if (0 != (run & inState)) {
            if (AnimKey.Attack == inAnimComp.currentAnim ||
                AnimKey.Hit == inAnimComp.currentAnim ||
                AnimKey.Crouch == inAnimComp.currentAnim) {
                return false;
            }
        }
        else if (0 != (crouch & inState)) {
            if (AnimKey.Jump == inAnimComp.currentAnim) {
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
