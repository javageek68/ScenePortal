using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalEntrance : MonoBehaviour {

    public string TargetSceneName;
    public GameObject exitSceneContainer;
    public GameObject player;
    public GameObject portalExitCamera;
    public GameObject PortalSurface;
    private Camera exitCamera;
    public float baseZoom = 120f;
    public float zoomFactor = 1f;


    /// <summary>
    /// 
    /// </summary>
    void Start () {
        PortalController.instance.RegisterEntrance(this);
	}

    private void Update()
    {
        if (player == null ||  portalExitCamera == null) return;

        //give the portal exit camera the same perspective as the player to the entrance
        Vector3 vctLookDirection = gameObject.transform.position - player.transform.position;
        portalExitCamera.transform.forward = vctLookDirection;

        //base the zoom of the exit camera on the distance to the entrance of the portal
        if (exitCamera == null) exitCamera = portalExitCamera.GetComponent<Camera>();
        exitCamera.fieldOfView = baseZoom - vctLookDirection.magnitude * zoomFactor;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Entered the portal
        PortalController.instance.PlayerEnteredPortal(this, gameObject.scene.name);
    }

    public void AssignPortalSurfaceTexture()
    {
        //PortalSurface.
    }
}
