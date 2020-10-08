// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PointerDownEvent : UnityEvent { }
public class PointerUpEvent : UnityEvent { }

public class ButtonEx : Button, IPointerDownHandler, IPointerUpHandler {
    public PointerDownEvent OnPressEvent = new PointerDownEvent();
    public PointerUpEvent OnReleaseEvent = new PointerUpEvent();

    public override void OnPointerDown(PointerEventData ped) {
        OnPressEvent.Invoke();
    }
    public override void OnPointerUp(PointerEventData ped) {
        OnReleaseEvent.Invoke();
    }
}
