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
        heaven.gameObject.SetActive(true);
        hell.gameObject.SetActive(false);
        firstCurrent = true;
    }

    public void OnWorldSwitch(InputAction.CallbackContext ctx)
    {
        Debug.Log("AAAAAA");
        SwitchWorld();
    }

    private void SwitchWorld()
    {
        firstCurrent = !firstCurrent;

        if (firstCurrent)
        {
            cam.backgroundColor = heaven.skyColor;
            heaven.gameObject.SetActive(true);
            hell.gameObject.SetActive(false);
        }
        else
        {
            cam.backgroundColor = hell.skyColor;
            heaven.gameObject.SetActive(false);
            hell.gameObject.SetActive(true);
        }
        
    }
}
