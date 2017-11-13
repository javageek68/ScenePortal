using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlayer : MonoBehaviour {

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        PortalController.instance.RegisterPortalPlayer(this);
    }
}
