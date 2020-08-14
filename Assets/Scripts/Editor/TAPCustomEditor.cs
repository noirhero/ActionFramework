// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEditor;

public class TAPCustomEditor : EditorWindow {
    public SpritePreset preset;
    public bool[] foldedClipDatas = null;

    private Vector2 _scrollPosition = Vector2.zero;
    private Vector2 _windowSize = new Vector2(450, 450);
    private GUIContent[] _locationContents = new GUIContent[] {new GUIContent("X"), new GUIContent("Y")};
    private GUIContent[] _sizeContents = new GUIContent[] {new GUIContent("Width"), new GUIContent("Height")};
    private Texture2D _currentSpriteTexture = null;
    private AnimUtility.AnimKey _currentKey;
    private int _currentTimelineIndex = -1;


    [MenuItem("Window/---- TAP Custom Editor ----")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TAPCustomEditor));
    }

    public void InitPresetData() {
        _currentSpriteTexture = null; // OnGUI 호출될 때마다 덮어씌워지는 것 방지
        foldedClipDatas = new bool[preset.datas.Count];
    }

    public void OnGUI() {
        minSize = _windowSize;

        GUILayout.BeginArea(new Rect(0, 0, _windowSize.x, _windowSize.y));
        GUILayout.Label("Animation Setting", EditorStyles.boldLabel);

        // preset 바뀔 때마다 초기화
        EditorGUI.BeginChangeCheck();
        preset = (SpritePreset) (EditorGUILayout.ObjectField("preset", preset, typeof(SpritePreset), true,
            GUILayout.Width(400), GUILayout.ExpandWidth(false)));
        if (EditorGUI.EndChangeCheck()) {
            if (preset) {
                InitPresetData();
            }
        }

        if (null == preset) {
            GUILayout.EndArea();
            return;
        }

        // 프리셋 전환 없이 코드 변경 후 OnGUI 호출됐을 때 null인 경우가 있음
        if (null == foldedClipDatas) {
            InitPresetData();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        _scrollPosition =
            GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Width(150), GUILayout.Height(410));
        EditorGUILayout.Separator();

        var i = 0;
        foreach (var data in preset.datas) {
            foldedClipDatas[i] = EditorGUILayout.Foldout(foldedClipDatas[i], data.Key.ToString(), true);
            if (foldedClipDatas[i]) {
                EditorGUI.indentLevel++;

                for (var j = 0; j < data.Value.timelines.Count; ++j) {
                    var isClicked = GUILayout.Button("Frame " + j, GUILayout.MinWidth(100), GUILayout.MaxWidth(100),
                        GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
                    if (isClicked) {
                        _currentKey = data.Key;
                        _currentTimelineIndex = j;
                        _currentSpriteTexture = data.Value.timelines[j].sprite.texture;
                    }
                }

                EditorGUI.indentLevel--;
            }

            ++i;
        }

        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        // 오른쪽 프리뷰 화면
        if (null != _currentSpriteTexture) {
            var textureLocation = new float[2] {_scrollPosition.x + 200, 100};
            var textureSize = 200.0f;
            EditorGUI.TextArea(
                new Rect(textureLocation[0], textureLocation[1] - 40, 200, EditorGUIUtility.singleLineHeight),
                _currentKey.ToString() + " / Frame " + _currentTimelineIndex);
            EditorGUI.TextArea(
                new Rect(textureLocation[0], textureLocation[1] - 20, 200, EditorGUIUtility.singleLineHeight),
                "Texture size : " + _currentSpriteTexture.width + " x " + _currentSpriteTexture.height);
            EditorGUI.DrawPreviewTexture(new Rect(textureLocation[0], textureLocation[1], textureSize, textureSize),
                _currentSpriteTexture);

            if (false == preset.datas.TryGetValue(_currentKey, out var animData) || 0 > _currentTimelineIndex) {
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
                return;
            }

            var currentRect = animData.timelines[_currentTimelineIndex].attackCollision;

            // 위치 조정
            var location = new float[2] {currentRect.x, currentRect.y};
            var uiPositionRect = new Rect(200, textureLocation[1] + textureSize + 20, 200,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.MultiFloatField(uiPositionRect, new GUIContent(), _locationContents, location);
            animData.timelines[_currentTimelineIndex].attackCollision.x = location[0];
            animData.timelines[_currentTimelineIndex].attackCollision.y = location[1];

            uiPositionRect.y += (EditorGUIUtility.singleLineHeight + 5);

            // 사이즈 조정
            var size = new float[2] {currentRect.width, currentRect.height};
            EditorGUI.MultiFloatField(uiPositionRect, new GUIContent(), _sizeContents, size);
            animData.timelines[_currentTimelineIndex].attackCollision.width = size[0];
            animData.timelines[_currentTimelineIndex].attackCollision.height = size[1];

            // 콜리전 사이즈가 있을 때 멀티플 체크
            if ((0 < size[0]) && (0 < size[1])) {
                uiPositionRect.y += (EditorGUIUtility.singleLineHeight + 5);

                var bUseMultiCollision = animData.timelines[_currentTimelineIndex].bUseMultiCollision;
                animData.timelines[_currentTimelineIndex].bUseMultiCollision = EditorGUI.ToggleLeft(uiPositionRect,
                    new GUIContent("Use Multi Collision", "do multi-check when colliding so that several objects can be hit in one frame"), bUseMultiCollision);
            }

            // 사운드 클립
            uiPositionRect.y += (EditorGUIUtility.singleLineHeight + 10);
            EditorGUI.LabelField(uiPositionRect, "Sound Clip", EditorStyles.boldLabel);

            uiPositionRect.y += EditorGUIUtility.singleLineHeight;
            animData.timelines[_currentTimelineIndex].soundClipKey = (SoundUtility.ClipKey)EditorGUI.EnumFlagsField(uiPositionRect, 
                animData.timelines[_currentTimelineIndex].soundClipKey);

            // 이펙트
            uiPositionRect.y += (EditorGUIUtility.singleLineHeight + 10);
            EditorGUI.LabelField(uiPositionRect, "Effect", EditorStyles.boldLabel);

            uiPositionRect.y += EditorGUIUtility.singleLineHeight;
            animData.timelines[_currentTimelineIndex].effectKey = (EffectUtility.Key) EditorGUI.EnumFlagsField
            (uiPositionRect,
                animData.timelines[_currentTimelineIndex].effectKey);

            // Reset
            var isClicked = GUILayout.Button("Reset this frame", GUILayout.MinWidth(110), GUILayout.MaxWidth(110),
                GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
            if (isClicked) {
                animData.timelines[_currentTimelineIndex].attackCollision = new Rect();
            }

            // Save
            isClicked = GUILayout.Button("Save All", GUILayout.MinWidth(80), GUILayout.MaxWidth(80),
                GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
            if (isClicked) {
                var presetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(preset.gameObject);
                PrefabUtility.SaveAsPrefabAsset(preset.gameObject, presetPath);
            }

            // Draw preview lines
            if (0 < size[0] && 0 < size[1]) {
                var ratio_x = (textureSize / _currentSpriteTexture.width);
                var ratio_y = (textureSize / _currentSpriteTexture.height);
                var scaled = new Rect(ratio_x * location[0], ratio_y * location[1], ratio_x * size[0],
                    ratio_y * size[1]);

                // 원점 = 캐릭터 발 밑 기준
                var result_x = textureLocation[0] + scaled.x + (textureSize * 0.5f);
                var result_y = textureLocation[1] - scaled.y + textureSize;

                var left_x = result_x - (scaled.width * 0.5f);
                var right_x = result_x + (scaled.width * 0.5f);
                var top_y = result_y - scaled.height;
                var bottom_y = result_y;

                // clockwise
                var lines = new Vector3[8] {
                    new Vector3(left_x, top_y), new Vector3(right_x, top_y),
                    new Vector3(right_x, top_y), new Vector3(right_x, bottom_y),
                    new Vector3(right_x, bottom_y), new Vector3(left_x, bottom_y),
                    new Vector3(left_x, bottom_y), new Vector3(left_x, top_y)
                };
                Handles.DrawLines(lines);
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
