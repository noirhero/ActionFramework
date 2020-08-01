// Copyright 2018-202 TAP, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpritePreset))]
[CanEditMultipleObjects]
public class SpritePresetInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Set animation clips and then click")) {
            InitializePresetData(target as SpritePreset);
        }
    }

    private void InitializePresetData(SpritePreset preset) {
        preset.datas.Clear();

        foreach (var clip in preset.clips) {
            var animData = new AnimData {
                length = clip.length,
                timelines = new List<AnimTimeline>()
            };

            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip)) {
                foreach (var frame in AnimationUtility.GetObjectReferenceCurve(clip, binding)) {
                    var sprite = (Sprite) frame.value;

                    animData.timelines.Add(new AnimTimeline {
                        start = frame.time,
                        sprite = sprite
                    });
                }
            }

            if (0 == animData.timelines.Count) {
                continue;
            }

            for (var i = 0; i < animData.timelines.Count - 1; ++i) {
                animData.timelines[i].end = animData.timelines[i + 1].start;
            }

            animData.timelines[animData.timelines.Count - 1].end = clip.length;

            if (false == Enum.TryParse(clip.name.Substring(clip.name.LastIndexOf(".", StringComparison.Ordinal) + 1),
                out AnimUtility.AnimKey animID)) {
                Debug.LogError("!!!! Check Anim Name");
            }
            else {
                preset.datas.Add(animID, animData);
            }
        }
    }
}
