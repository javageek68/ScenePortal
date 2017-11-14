using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContainer : MonoBehaviour {

    public string SceneName;

    private void Awake()
    {
        this.SceneName = gameObject.scene.name;
    }
    
    /// <summary>
    /// 
    /// </summary>
    void Start () {
        PortalController.instance.RegisterSceneContainer(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
