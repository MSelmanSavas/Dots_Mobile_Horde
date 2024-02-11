using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationsConfigSetter : MonoBehaviour
{
    private void Start()
    {
        SetApplicationsConfig();
    }

    void SetApplicationsConfig()
    {
        Application.targetFrameRate = 120;
    }
}
