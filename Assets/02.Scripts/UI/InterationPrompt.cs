using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterationPrompt : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        transform.forward = mainCam.transform.forward;
    }
}
