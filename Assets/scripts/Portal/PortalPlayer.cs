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
        portalPlayerData.transform = gameObject.transform;
    }
}
