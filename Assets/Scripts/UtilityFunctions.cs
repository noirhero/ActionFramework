// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using System.Linq;

public static class Utility {
    public static int GetHashCode(in string path) {
        return path.Sum(Convert.ToInt32);
    }
}
