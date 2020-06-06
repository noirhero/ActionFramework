// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEditor;
using Unity.Physics.Authoring;

public struct AnimClipData {
    public bool isFolded;
    public Rect[] boxCollision;
}

public class TAPCustomEditor : EditorWindow {
    public SpritePreset preset;
    public AnimClipData[] foldedClipDatas = null;

    private Vector2 _scrollPosition = Vector2.zero;
    private Vector2 _windowSize = new Vector2(450, 450);
    private GUIContent[] _locationContents = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
    private GUIContent[] _sizeContents = new GUIContent[] { new GUIContent("Width"), new GUIContent("Height") };
    private Texture2D _currentSpriteTexture = null;
    private int _currentIndex = -1;


    [MenuItem("Window/---- TAP Custom Editor ----")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TAPCustomEditor));
    }

    public void InitPresetData() {
        _currentSpriteTexture = null;   // OnGUI 호출될 때마다 덮어씌워지는 것 방지
        foldedClipDatas = new AnimClipData[preset.datas.Count];

        var i = 0;
        foreach (var data in preset.datas) {
            foldedClipDatas[i].isFolded = false;
            foldedClipDatas[i++].boxCollision = new Rect[data.Value.timelines.Count];
        }
    }

    public void OnGUI() {
        minSize = _windowSize;

        GUILayout.BeginArea(new Rect(0, 0, _windowSize.x, _windowSize.y));
        GUILayout.Label("Animation Setting", EditorStyles.boldLabel);
        preset = (SpritePreset)(EditorGUILayout.ObjectField("preset", preset, typeof(SpritePreset), true, GUILayout.Width(400), GUILayout.ExpandWidth(false)));
        if (null == preset) {
            GUILayout.EndArea();
            return;
        }

        if (null == foldedClipDatas) {
            InitPresetData();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Width(150), GUILayout.Height(410));
        EditorGUILayout.Separator();

        var i = 0;
        foreach (var data in preset.datas) {
            foldedClipDatas[i].isFolded = EditorGUILayout.Foldout(foldedClipDatas[i].isFolded, data.Key.ToString(), true);
            if (foldedClipDatas[i].isFolded) {
                EditorGUI.indentLevel++;

                for (var j = 0; j < data.Value.timelines.Count; ++j) {
                    var isClicked = GUILayout.Button("Frame " + j, GUILayout.MinWidth(100), GUILayout.MaxWidth(100), GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
                    if (isClicked) {
                        _currentSpriteTexture = data.Value.timelines[j].sprite.texture;
                        _currentIndex = (i * 10) + j;   // 현재 선택된 메뉴를 보여주기 위한 간단 인덱싱
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
            var textureLocation = new float[2] { _scrollPosition.x + 200, 70 };
            var textureSize = 200.0f;
            EditorGUI.TextArea(new Rect(textureLocation[0], textureLocation[1] - 20, 200, EditorGUIUtility.singleLineHeight), "Texture size : " + _currentSpriteTexture.width + " x " + _currentSpriteTexture.height);

            EditorGUI.DrawPreviewTexture(new Rect(textureLocation[0], textureLocation[1], textureSize, textureSize), _currentSpriteTexture);

            var idx = (int)(_currentIndex * 0.1f);
            var timelineIndex = (_currentIndex % 10);
            var currentRect = foldedClipDatas[idx].boxCollision[timelineIndex];

            // 위치 조정
            var location = new float[2] { currentRect.x, currentRect.y };
            EditorGUI.MultiFloatField(new Rect(200, textureLocation[1] + textureSize + 20, 200, EditorGUIUtility.singleLineHeight), new GUIContent(), _locationContents, location);
            foldedClipDatas[idx].boxCollision[timelineIndex].x = location[0];
            foldedClipDatas[idx].boxCollision[timelineIndex].y = location[1];

            // 사이즈 조정
            var size = new float[2] { currentRect.width, currentRect.height };
            EditorGUI.MultiFloatField(new Rect(200, textureLocation[1] + textureSize + 30 + EditorGUIUtility.singleLineHeight, 200, EditorGUIUtility.singleLineHeight), new GUIContent(), _sizeContents, size);
            foldedClipDatas[idx].boxCollision[timelineIndex].width = size[0];
            foldedClipDatas[idx].boxCollision[timelineIndex].height = size[1];

            // Draw preview lines
            if (0 < size[0] && 0 < size[1]) {
                var ratio_x = (textureSize / _currentSpriteTexture.width);
                var ratio_y = (textureSize / _currentSpriteTexture.height);
                var scaled = new Rect(ratio_x * location[0], ratio_y * location[1], ratio_x * size[0], ratio_y * size[1]);

                // 원점 = 캐릭터 발 밑 기준
                var result_x = textureLocation[0] + scaled.x + (textureSize * 0.5f);
                var result_y = textureLocation[1] - scaled.y + textureSize;

                var left_x = result_x - (scaled.width * 0.5f);
                var right_x = result_x + (scaled.width * 0.5f);
                var top_y = result_y - scaled.height;
                var bottom_y = result_y;

                // clockwise
                var lines = new Vector3[8] { new Vector3(left_x, top_y), new Vector3(right_x, top_y),
                                             new Vector3(right_x, top_y), new Vector3(right_x, bottom_y),
                                             new Vector3(right_x, bottom_y), new Vector3(left_x, bottom_y),
                                             new Vector3(left_x, bottom_y), new Vector3(left_x, top_y) };
                Handles.DrawLines(lines);
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
