using System;
using UnityEngine;


public static class EventHandler
{
    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int gameWeek)
    {
        StartNewGameEvent?.Invoke(gameWeek);
    }

    public static event Action<string, GameObject> ItemPickUpEvent;
    public static void CallItemPickUpEvent(string item, GameObject gameObj)
    {
        ItemPickUpEvent?.Invoke(item, gameObj);
    }

    public static event Action<string,GameObject> ItemHoverOnEvent;
    public static void CallItemHoverOnEvent(string item, GameObject gameObj)
    {
        ItemHoverOnEvent?.Invoke(item, gameObj);
    }

    public static event Action<string, GameObject> ItemHoverOffEvent;
    public static void CallItemHoverOffEvent(string item, GameObject gameObj)
    {
        ItemHoverOffEvent?.Invoke(item, gameObj);
    }

    public static event Action<string, GameObject> ItemInteractEvent;

    public static void CallItemInteractEvent(string item, GameObject gameObj)
    {
        ItemInteractEvent?.Invoke(item, gameObj);
    }

    public static event Action<string,GameObject> ItemFinishInteractEvent;

    public static void CallItemFinishInteractEvent(string item, GameObject gameObj)
    {
        ItemFinishInteractEvent?.Invoke(item, gameObj);
    }

    public static event Action<string, GameObject> ItemDropEvent;

    public static void CallItemDropEvent(string item, GameObject gameObj)
    {
        ItemDropEvent?.Invoke(item, gameObj);
    }

    public static event Action<string, uint,GameObject> TasteEvent;

    public static void CallTasteEvent(string type, uint degree,GameObject obj)
    {
        TasteEvent?.Invoke(type, degree, obj);
    }

    public static event Action<string, uint,GameObject> SmellEvent;

    public static void CallSmellEvent(string type, uint degree, GameObject obj)
    {
        SmellEvent?.Invoke(type, degree, obj);
    }

    public static event Action<string> ChangeUIEvent;
    public static void CallChangeUIEvent(string str)
    {
        ChangeUIEvent?.Invoke(str);
    }

   

    public static event Action<GameObject, GameObject, string, uint> InteractiveEvent_Internal;

    public static void CallInteractEvent_Internal(GameObject from, GameObject to, string type, uint value)
    {
        InteractiveEvent_Internal?.Invoke(from,to,type,value);
    }
    
    public static event Action<GameObject, GameObject> InteractiveEvent;

    public static void CallInteractEvent(GameObject from, GameObject to)
    {
        InteractiveEvent?.Invoke(from,to);
    }

    public static event Action<IndicateState> ChangeIndicatorEvent;

    public static void CallChangeIndicatorEvent(IndicateState nextState)
    {
        ChangeIndicatorEvent?.Invoke(nextState);
    }

    public static event Action<GameObject> InteruptEvent;

    public static void CallInteruptEvent(GameObject obj)
    {
        InteruptEvent?.Invoke(obj);
    }
}
