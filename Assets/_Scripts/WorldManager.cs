using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchWorld();
        }
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
