// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEditor;

public class TAPUtilityEditor : EditorWindow {
    public bool bClearSaveData;

    [MenuItem("Window/---- TAP Utility Editor ----")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TAPUtilityEditor));
    }

    public void OnGUI() {
        GUILayout.Label("TAP Utility Setting", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        Utility.jumpForce = EditorGUILayout.Slider("jumpForce", Utility.jumpForce, 0.0f, 1000.0f);
        Utility.gravity = EditorGUILayout.Slider("gravity", Utility.gravity, 0.0f, 100.0f);
        Utility.speedX = EditorGUILayout.Slider("speedX", Utility.speedX, 0.0f, 10.0f);
        Utility.speedY = EditorGUILayout.Slider("speedY", Utility.speedY, 0.0f, 10.0f);
        
        bClearSaveData = EditorGUILayout.Toggle("Clear Score", bClearSaveData);
        if (EditorGUI.EndChangeCheck()) {
            if (bClearSaveData) {
                bClearSaveData = false;
                PlayerPrefs.DeleteKey(Utility.SaveDataName);
            }
        }
    }
}
