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

    [SerializeField] private Camera cam;
    [SerializeField] private WorldDesc heaven;
    [SerializeField] private WorldDesc hell;
    [SerializeField] private Checkpoint m_CurrentCheckpoint;
    private static WorldManager s_Instance;
    private bool firstCurrent;

    private void Start()
    {
        // SwitchWorld();
    }

    private void OnEnable() 
    {
        Checkpoint.OnActivate += UpdateCheckpoint;
    }

    private void OnDisable() 
    {
        Checkpoint.OnActivate -= UpdateCheckpoint;
        s_Instance = s_Instance == this? null : s_Instance;
    }

    private void SwitchWorld()
    {
        firstCurrent ^= true;
        cam.backgroundColor = firstCurrent? heaven.skyColor : hell.skyColor;
        heaven.gameObject.SetActive(firstCurrent);
        hell.gameObject.SetActive(!firstCurrent);
    }

    private void UpdateCheckpoint(Checkpoint checkpoint)
    {
        m_CurrentCheckpoint = checkpoint;
    }

    [ContextMenu("Add Checkpoints")]
    private void AddCheckpoints()
    {
        List<Checkpoint> activeCheckpoints = FindObjectsOfType<Checkpoint>().Where(c => c.Active == true).ToList();
        if(activeCheckpoints.Count != 1)
            Debug.LogError("The world needs to contain excactly one active checkpoint!");
        m_CurrentCheckpoint = activeCheckpoints.First();
    }

    private void OnGUI() 
    {
        if(GUI.Button(new Rect(0, 0, 100, 40), "Swap World"))
        {
            SwitchWorld();
        }
    }
}
