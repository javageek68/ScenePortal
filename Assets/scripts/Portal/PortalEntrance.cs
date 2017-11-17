using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalEntrance : MonoBehaviour {

    public PortalPlayerData portalPlayerData;
    public PortalData portalData;
    public TextMesh textMesh; 
    public float baseZoom = 120f;
    public float zoomFactor = 1f;

    private void Start()
    {
        textMesh.text = portalData.ExitSceneName;
    }

    private void Update()
    {
        //make sure only the portal entrances in the scene with the player are running
        if (portalPlayerData.CurrentScene != gameObject.scene.name) return;

        //give the portal exit camera the same perspective as the player to the entrance
        Vector3 vctLookDirection = gameObject.transform.position - portalPlayerData.playerPosition;
        portalData.ExitCameraLookDirection = vctLookDirection;

        //base the zoom of the exit camera on the distance to the entrance of the portal
        portalData.ExitCameraFieldOfView = baseZoom - vctLookDirection.magnitude * zoomFactor;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Entered the portal
        PortalController.instance.PlayerEnteredPortal(this, gameObject.scene.name, portalData.ExitSceneName);
    }

    public void AssignPortalSurfaceTexture()
    {
        //PortalSurface.
    }
}
