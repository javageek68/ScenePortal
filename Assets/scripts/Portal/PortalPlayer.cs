using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlayer : MonoBehaviour {

    public PortalPlayerData portalPlayerData;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {

    }

    private void Update()
    {
        portalPlayerData.playerPosition = gameObject.transform.position;
    }

    public void SetScene(string strCurrentScene)
    {
        portalPlayerData.CurrentScene = strCurrentScene;
    }

    public void ApplySkyboxMaterial(Material newSkybox)
    {
        Skybox skybox = gameObject.GetComponentInChildren<Skybox>();
        skybox.material = newSkybox;
    }
}
