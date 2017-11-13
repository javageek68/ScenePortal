using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExit : MonoBehaviour {

    public string SceneName;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        this.SceneName = gameObject.scene.name;
    }
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        PortalController.instance.RegisterExit(this);
    }
}
