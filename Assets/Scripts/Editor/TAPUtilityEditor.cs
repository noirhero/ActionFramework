// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEditor;

public class TAPUtilityEditor : EditorWindow {
    public bool bClearSaveData;

    private static float jumpForce;
    private static float gravity;
    private static float speedX;
    private static float speedY;
    private static float touchDelta;
    
    [MenuItem("Window/---- TAP Utility Editor ----")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TAPUtilityEditor));
    }
    
    public void OnGUI() {
        GUILayout.Label("TAP Utility Setting", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        jumpForce = EditorGUILayout.Slider(Utility.StatJumpForceName, Utility.jumpForce, 0.0f, 1000.0f);
        gravity = EditorGUILayout.Slider(Utility.StatGravityName, Utility.gravity, 0.0f, 100.0f);
        speedX = EditorGUILayout.Slider(Utility.StatSpeedXName, Utility.speedX, 0.0f, 10.0f);
        speedY = EditorGUILayout.Slider(Utility.StatSpeedYName, Utility.speedY, 0.0f, 10.0f);
        touchDelta = EditorGUILayout.Slider(Utility.StatTouchDeltaName, Utility.touchDelta, 0.0f, 10.0f);

        bClearSaveData = EditorGUILayout.Toggle("Clear Score", bClearSaveData);
        if (EditorGUI.EndChangeCheck()) {
            if (bClearSaveData) {
                bClearSaveData = false;
                PlayerPrefs.DeleteKey(Utility.SaveDataName);
            }

            PlayerPrefs.SetFloat(Utility.StatJumpForceName, jumpForce);
            PlayerPrefs.SetFloat(Utility.StatGravityName, gravity);
            PlayerPrefs.SetFloat(Utility.StatSpeedXName, speedX);
            PlayerPrefs.SetFloat(Utility.StatSpeedYName, speedY);
            PlayerPrefs.SetFloat(Utility.StatTouchDeltaName, touchDelta);
        }
    }
}
