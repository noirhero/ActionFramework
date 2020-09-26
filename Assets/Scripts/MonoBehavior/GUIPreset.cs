// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

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

    private EntityManager _entManager;

    private long _playTime;

    public void Awake() {
        gameObject.SetActive(false);

        if (null != Button_Start) {
            Button_Start.onClick.AddListener(delegate { OnClickStart(); });
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

    public void GUIUpdate(IdUtility.GUIId inId) {
        gameObject.SetActive(true);
        switch (inId) {
            case IdUtility.GUIId.Title: {
                Button_Start.gameObject.SetActive(true);
                Text_Start.text = "Start";
                Text_Message.gameObject.SetActive(true);
                Text_Message.text = "LuisZuno";
            }
                break;
            case IdUtility.GUIId.InGame: {
                Button_Start.gameObject.SetActive(false);
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
                Text_Message.gameObject.SetActive(false);
                
                StartCoroutine(DelayEnableGUI());

            }
                break;
            case IdUtility.GUIId.Result: {
                string scoreMsg = string.Empty;
                if (PlayerPrefs.HasKey(Utility.SaveDataName)) {
                    ScoreData scoreData = JsonUtility.FromJson<ScoreData>(PlayerPrefs.GetString(Utility.SaveDataName));

                    scoreMsg = "HighScore : ";
                    scoreMsg += scoreData.HighScore.ToString();
                    scoreMsg += "sec \n";
                    
                    if (scoreData.bNewHighScore) {
                        string MakeMsg= string.Empty;
                        for (int i = 0; i < scoreMsg.Length; ++i) {
                            MakeMsg += "<color=#";
                            MakeMsg += ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f,1f), Random.Range(0, 1f), Random.Range(0, 1f)));
                            MakeMsg += ">";
                            MakeMsg += scoreMsg[i];
                            MakeMsg += "</color>";
                        }
                        scoreMsg = MakeMsg;
                    }
                    
                    scoreMsg += "Score : ";
                    scoreMsg += "<color=#"+ColorUtility.ToHtmlStringRGB(scoreData.bNewHighScore ? Color.yellow : Color.white)+">";
                    scoreMsg += scoreData.lastScore.ToString();
                    scoreMsg += " </color> sec";
                }

                Text_Message.text = scoreMsg;
                    
                Text_Start.text = "Restart";
                
                Button_Start.gameObject.SetActive(true);
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
}
