using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageHandler : MonoBehaviour 
{
    public static MessageHandler Instance
    {
        get
        {
            if(!s_Instance)
            {
                s_Instance = FindObjectOfType<MessageHandler>();
                if(!s_Instance)
                {
                    Debug.LogError("Can't find instance of MessageHandler in the loaded scenes");
                }
                else
                {
                    s_Instance.Init();
                }
            }
            return s_Instance;
        }
    }
    private Dictionary<string, UnityEvent> m_Events;
    private static MessageHandler s_Instance;


    private void Init()
    {
        m_Events ??= new Dictionary<string, UnityEvent>();
    }

    public static void StartListen(string eventName, UnityAction callback)
    {
        UnityEvent thisEvent = null;
        if(Instance.m_Events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(callback);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(callback);
            Instance.m_Events.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction callback)
    {
        if(s_Instance == null) return;
        UnityEvent thisEvent = null;
        if(Instance.m_Events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(callback);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if(s_Instance == null) return;
        UnityEvent thisEvent = null;
        if(Instance.m_Events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}
