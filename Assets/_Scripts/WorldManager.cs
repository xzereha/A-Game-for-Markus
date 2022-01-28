using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private WorldDesc heaven;
    [SerializeField] private WorldDesc hell;

    private bool firstCurrent;

    private void Start()
    {
        SwitchWorld();
    }

    private void SwitchWorld()
    {
        firstCurrent ^= true;
        cam.backgroundColor = firstCurrent? heaven.skyColor : hell.skyColor;
        heaven.gameObject.SetActive(firstCurrent);
        hell.gameObject.SetActive(!firstCurrent);
    }

    private void OnGUI() 
    {
        if(GUI.Button(new Rect(0, 0, 100, 40), "Swap World"))
        {
            SwitchWorld();
        }
    }
}
