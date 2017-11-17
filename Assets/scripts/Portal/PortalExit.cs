using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExit : MonoBehaviour {

    public Camera portalExitCamera;
    public PortalData portalData;

    private void Update()
    {
        if (portalData.ExitCameraLookDirection != Vector3.zero) portalExitCamera.transform.forward = portalData.ExitCameraLookDirection;
        portalExitCamera.fieldOfView = portalData.ExitCameraFieldOfView;
    }
}
