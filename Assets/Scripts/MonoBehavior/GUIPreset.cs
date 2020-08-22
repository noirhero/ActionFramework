// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GUIPreset : MonoBehaviour {
    public Text Text_Message;

    public Button Button_Start;
    public Text Text_Start;

    private EntityManager _entManager;

    public void Awake() {
        gameObject.SetActive(false);

        if (null != Button_Start) {
            Button_Start.onClick.AddListener(delegate { OnClickStart(); });
        }

        _entManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void GUIUpdate(IdUtility.GUIId inId) {
        gameObject.SetActive(true);
        switch (inId) {
            case IdUtility.GUIId.Title:
                Button_Start.gameObject.SetActive(true);
                Text_Start.text = "Start";
                Text_Message.gameObject.SetActive(true);
                Text_Message.text = "LuisZuno";
                break;
            case IdUtility.GUIId.InGame:
                Button_Start.gameObject.SetActive(false);
                Text_Message.gameObject.SetActive(false);
                break;
            case IdUtility.GUIId.Result:
                Button_Start.gameObject.SetActive(true);
                Text_Start.text = "Restart";
                Text_Message.gameObject.SetActive(true);
                Text_Message.text = "Playtime : ~ ";
                break;
        }
    }

    public void OnClickStart() {
        if (false == _entManager.HasComponent<GUIComponent>(Utility.SystemEntity)) {
            _entManager.AddComponentData(Utility.SystemEntity, new GUIComponent() {
                id = IdUtility.GUIId.InGame
            });
        }
    }
}
