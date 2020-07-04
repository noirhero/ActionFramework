// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
    }

    public static bool bShowInputLog = false;
    public static int INDEX_NONE = -1;

    public enum AnimState {
        Idle,
        Run,
        Jump,
        Attack
    }

    public static bool IsPossibleAnimChange(AnimState inState, in AnimationFrameComponent inAnimComp) {
        // TODO : 조건 정리 필욘\요
        switch (inState) {
            case AnimState.Run:
                return AnimState.Attack != inAnimComp.currentId;
            case AnimState.Attack:
                return AnimState.Idle == inAnimComp.currentId;
            case AnimState.Jump:
                return AnimState.Jump != inAnimComp.currentId;
            case AnimState.Idle:
                return inAnimComp.bDone;
            default: return true;
        }
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