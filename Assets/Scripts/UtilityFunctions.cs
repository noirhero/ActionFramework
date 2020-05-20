// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;
using UnityEngine;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
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
    public static string ShowLog(int state) {
        var cachedStr = string.Empty;
        if (0 != (left & state)) {
            cachedStr += " Left";
        }
        if (0 != (right & state)) {
            cachedStr += " Right";
        }
        if (0 != (jump & state)) {
            cachedStr += " Jump";
        }
        if (0 != (attack & state)) {
            cachedStr += " Attack";
        }
        if (0 != (axis & state)) {
            cachedStr += " Axis";
        }
        return cachedStr;
    }
}
    

