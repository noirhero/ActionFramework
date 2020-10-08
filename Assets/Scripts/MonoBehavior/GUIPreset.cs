﻿// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class GUIPreset : MonoBehaviour {
    public Text Text_Message;

    public Button Button_Start;
    public Text Text_Start;
    
    public Button Button_Attack;
    public Button Button_Jump;
    public ButtonEx Button_Left;
    public ButtonEx Button_Right;
    public ButtonEx Button_Crouch;

    private EntityManager _entManager;

    private long _playTime;

    public void Awake() {
        gameObject.SetActive(false);

        if (null != Button_Start) {
            Button_Start.onClick.AddListener(delegate { OnClickStart(); });
        }

        if (null != Button_Attack) {
            Button_Attack.onClick.AddListener(delegate { OnClickAttack(); });
        }

        if (null != Button_Jump) {
            Button_Jump.onClick.AddListener(delegate { OnClickJump(); });
        }

        if (null != Button_Left) {
            Button_Left.OnPressEvent.AddListener(delegate { OnPressedLeft(true); });
            Button_Left.OnReleaseEvent.AddListener(delegate { OnPressedLeft(false); });
        }

        if (null != Button_Right) {
            Button_Right.OnPressEvent.AddListener(delegate { OnPressedRight(true); });
            Button_Right.OnReleaseEvent.AddListener(delegate { OnPressedRight(false); });
        }

        if (null != Button_Crouch) {
            Button_Crouch.OnPressEvent.AddListener(delegate { OnPressedCrouch(true); });
            Button_Crouch.OnReleaseEvent.AddListener(delegate { OnPressedCrouch(false); });
        }

        _entManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private IEnumerator DelayEnableGUI() {
        yield return new WaitForSeconds(5);

        if (false == _entManager.HasComponent<GUIComponent>(Utility.SystemEntity)) {
            _entManager.AddComponentData(Utility.SystemEntity, new GUIComponent() {
                id = IdUtility.GUIId.Result
            });
        }
    }

    private void ResetPressedButtonState() {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        dataComp.dir = 0.0f;
        dataComp.state = 0;
        _entManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void GUIUpdate(IdUtility.GUIId inId) {
        gameObject.SetActive(true);
        switch (inId) {
            case IdUtility.GUIId.Title: {
                Button_Start.gameObject.SetActive(true);
                Text_Start.text = "Start";
                
                Button_Attack.gameObject.SetActive(false);
                Button_Jump.gameObject.SetActive(false);
                Button_Left.gameObject.SetActive(false);
                Button_Right.gameObject.SetActive(false);
                Button_Crouch.gameObject.SetActive(false);

                Text_Message.gameObject.SetActive(true);
                Text_Message.text = "LuisZuno";
            }
                break;
            case IdUtility.GUIId.InGame: {
                Button_Start.gameObject.SetActive(false);
                
                Button_Attack.gameObject.SetActive(true);
                Button_Jump.gameObject.SetActive(true);
                Button_Left.gameObject.SetActive(true);
                Button_Right.gameObject.SetActive(true);
                Button_Crouch.gameObject.SetActive(true);
                
                Text_Message.gameObject.SetActive(false);
                _playTime = System.DateTime.Now.Ticks;
            }
                break;
            case IdUtility.GUIId.Over: {
                var time = System.DateTime.Now.Ticks - _playTime;
                var result = math.trunc(System.TimeSpan.FromTicks(time).TotalSeconds * 10) / 10;
                
                ScoreData scoreData;
                if (PlayerPrefs.HasKey(Utility.SaveDataName)) {
                    scoreData = JsonUtility.FromJson<ScoreData>(PlayerPrefs.GetString(Utility.SaveDataName));
                    scoreData.RecordScore(result);
                }
                else {
                    scoreData = new ScoreData(result);
                }
                PlayerPrefs.SetString(Utility.SaveDataName, JsonUtility.ToJson(scoreData));
                
                Button_Start.gameObject.SetActive(false);
                Button_Attack.gameObject.SetActive(false);
                Button_Jump.gameObject.SetActive(false);
                Button_Left.gameObject.SetActive(false);
                Button_Right.gameObject.SetActive(false);
                Button_Crouch.gameObject.SetActive(false);
                Text_Message.gameObject.SetActive(false);

                ResetPressedButtonState();
                StartCoroutine(DelayEnableGUI());

            }
                break;
            case IdUtility.GUIId.Result: {
                string scoreMsg = string.Empty;
                if (PlayerPrefs.HasKey(Utility.SaveDataName)) {
                    ScoreData scoreData = JsonUtility.FromJson<ScoreData>(PlayerPrefs.GetString(Utility.SaveDataName));

                    scoreMsg += "HighScore : ";
                    scoreMsg += scoreData.bNewHighScore ? "<color=#63D46A>" : "<color=white>";
                    scoreMsg += scoreData.HighScore.ToString();
                    scoreMsg += "sec \n";
                    scoreMsg += "</color>";
                    
                    scoreMsg += scoreData.bNewHighScore ? "<color=white>" : "<color=#B0B0B0>";
                    scoreMsg += "Score : ";
                    scoreMsg += "</color>";
                    scoreMsg += scoreData.bNewHighScore ? "<color=#63D46A>" : "<color=#B0B0B0>";
                    scoreMsg += scoreData.lastScore.ToString();
                    scoreMsg += "sec";
                    scoreMsg += "</color>";
                }

                Text_Message.text = scoreMsg;
                    
                Text_Start.text = "Restart";
                
                Button_Start.gameObject.SetActive(true);
                Button_Attack.gameObject.SetActive(false);
                Button_Jump.gameObject.SetActive(false);
                Button_Left.gameObject.SetActive(false);
                Button_Right.gameObject.SetActive(false);
                Button_Crouch.gameObject.SetActive(false);
                Text_Message.gameObject.SetActive(true);
            }
                break;
        }
    }

    public void OnClickStart() {
        if (false == _entManager.HasComponent<ConfirmComponent>(Utility.SystemEntity)) {
            _entManager.AddComponentData(Utility.SystemEntity, new ConfirmComponent());
        }
    }

    public void OnClickAttack() {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (false == InputUtility.HasState(dataComp, InputUtility.attack)) {
            dataComp.state |= InputUtility.attack;
            _entManager.SetComponentData(Utility.SystemEntity, dataComp);
        }
    }

    public void OnClickJump() {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        if (false == InputUtility.HasState(dataComp, InputUtility.jump)) {
            dataComp.state |= InputUtility.jump;
            _entManager.SetComponentData(Utility.SystemEntity, dataComp);
        }
    }

    public void OnPressedLeft(bool pressed) {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        var hasState = InputUtility.HasState(dataComp, InputUtility.left);
        if (pressed && (false == hasState)) {
            dataComp.state |= InputUtility.left;
            dataComp.dir -= 1.0f;
        }
        else if (hasState) {
            dataComp.state ^= InputUtility.left;
            dataComp.dir += 1.0f;
        }

        _entManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnPressedRight(bool pressed) {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        var hasState = InputUtility.HasState(dataComp, InputUtility.right);
        if (pressed && (false == hasState)) {
            dataComp.state |= InputUtility.right;
            dataComp.dir += 1.0f;
        }
        else if (hasState) {
            dataComp.state ^= InputUtility.right;
            dataComp.dir -= 1.0f;
        }
        _entManager.SetComponentData(Utility.SystemEntity, dataComp);
    }

    public void OnPressedCrouch(bool pressed) {
        var dataComp = _entManager.GetComponentData<InputDataComponent>(Utility.SystemEntity);
        var hasState = InputUtility.HasState(dataComp, InputUtility.crouch);

        if (pressed && (false == hasState)) {
            dataComp.state |= InputUtility.crouch;
        }
        else if (hasState) {
            dataComp.state ^= InputUtility.crouch;
        }
        _entManager.SetComponentData(Utility.SystemEntity, dataComp);
    }
}
