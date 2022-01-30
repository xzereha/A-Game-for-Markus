using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{

    public static WorldManager Instance
    {
        get
        {
            s_Instance ??= FindObjectOfType<WorldManager>();
            if(!s_Instance)
            {
                Debug.LogError("Can't find instance of WorldManager in the loaded scene");
            }
            return s_Instance;
        }
    }

    public static Vector3 CurrentCheckpoint => Instance.m_CurrentCheckpoint.Position;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private WorldDesc heaven;
    [SerializeField] private WorldDesc hell;
    [SerializeField] private Checkpoint m_CurrentCheckpoint;
    [SerializeField] private bool m_ShouldWorldChange;
    [SerializeField] private float m_TimeBeforeChange;
    private static WorldManager s_Instance;
    private bool firstCurrent;
    private Color32 color;
    private float m_timeToFlip;

    public void ToggleAutoCycling()
    {
        m_ShouldWorldChange ^= true;
    }

    public void SwitchWorld()
    {
        Debug.Log("Swap World");
        firstCurrent ^= true;
        color = firstCurrent? heaven.skyColor : hell.skyColor;
        heaven.gameObject.SetActive(firstCurrent);
        hell?.gameObject.SetActive(!firstCurrent);
    }

    private void Start()
    {
        SwitchWorld();
        if(m_ShouldWorldChange)
            StartCoroutine(WorldCycling());
    }

    private void Update()
    {

    }

    private void OnEnable() 
    {
        Checkpoint.OnActivate += OnCheckpointChanged;
    }

    private void OnDisable() 
    {
        Checkpoint.OnActivate -= OnCheckpointChanged;
        s_Instance = s_Instance == this? null : s_Instance;
    }

    private void OnCheckpointChanged(Checkpoint checkpoint)
    {
        m_CurrentCheckpoint = checkpoint;
    }

    private void Reset() 
    {
        m_Camera ??= FindObjectOfType<Camera>();
    }

    private IEnumerator WorldCycling()
    {
        while(true)
        {
            m_timeToFlip += Time.deltaTime;
            m_Camera.backgroundColor = color;
            color = firstCurrent ? LerpViaHSB(color, hell.skyColor, 0.005f) : LerpViaHSB(color, heaven.skyColor, 0.005f);
            if(m_timeToFlip >= m_TimeBeforeChange)
            {
                m_timeToFlip = 0;
                SwitchWorld();
            }
            yield return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Add Checkpoints")]
    private void AddCheckpoints()
    {
        List<Checkpoint> activeCheckpoints = FindObjectsOfType<Checkpoint>().Where(c => c.Active == true).ToList();
        if(activeCheckpoints.Count != 1) Debug.LogError("The world needs to contain excactly one active checkpoint!");
        m_CurrentCheckpoint = activeCheckpoints.First();
    }

    private void OnGUI() 
    {
        if(GUI.Button(new Rect(0, 0, 100, 40), "Swap World"))
        {
            SwitchWorld();
        }
    }
#endif

    public Color LerpViaHSB(Color a, Color b, float t)
    {
        return HSBColor.Lerp(HSBColor.FromColor(a), HSBColor.FromColor(b), t).ToColor();
    }
}