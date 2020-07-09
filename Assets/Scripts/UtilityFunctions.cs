// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
    }

    public static bool bShowInputLog = false;
    public static int INDEX_NONE = -1;
    
    // TODO : temporary constant -> status 
    public const float force = 50.0f;
    public const float gravity = 2.0f;
    public const float terminalVelocity = -30.0f;
    public const float speedX = 0.03f;
    public const float speedY = 0.1f;
    public const float skinWidth = 0.01f;
    public const float stepOffset = 0.01f;
}

public static class AnimState {
    public enum AnimKey {
        Idle,
        Run,
        Jump,
        Attack,
        Hit,
    }
    public const int run = 0x1;
    public const int jump = 0x2;
    public const int attack = 0x4;
    public const int hit = 0x8;

    public static bool HasState(AnimationFrameComponent inAnimComp, int insState) {
        return 0 != (inAnimComp.state & insState);
    }

    public static AnimKey GetAnimKey(AnimationFrameComponent inAnimComp) {
        if (HasState(inAnimComp, AnimState.jump)) {
            return AnimKey.Jump;
        }
        if (HasState(inAnimComp, AnimState.attack)) {
            return AnimKey.Attack;
        }
        if (HasState(inAnimComp, AnimState.run)) {
            return AnimKey.Run;
        }
        if (HasState(inAnimComp, AnimState.hit)) {
            return AnimKey.Hit;
        }

        return AnimKey.Idle;
    }

    public static bool IsLooping(AnimationFrameComponent inAnimComp) {
        if (inAnimComp.currentAnim == AnimKey.Run ||
            inAnimComp.currentAnim == AnimKey.Idle) {
            return true;
        }

        return false;
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

        return cachedStr;
    }
}

public static class InputState {
    public const int left = 0x1;
    public const int right = 0x2;
    public const int jump = 0x4;
    public const int attack = 0x8;
    public const int axis = 0x10;

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

        return cachedStr;
    }
}
