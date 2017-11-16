using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContainer : MonoBehaviour {

    public string SceneName;

    private void Awake()
    {
        this.SceneName = gameObject.scene.name;
        //GetPortalEntrances(gameObject.scene.name);
    }
    
    /// <summary>
    /// 
    /// </summary>
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<PortalEntrance> GetPortalEntrances()
    {
        Debug.Log("Entered GetPortalEntrances() - ");
        List<PortalEntrance> lstEntrances = new List<PortalEntrance>();
        PortalEntrance[] entrances = FindObjectsOfType(typeof(PortalEntrance)) as PortalEntrance[];
        Debug.Log("found entrances " + entrances.Length);
        foreach (PortalEntrance entrance in entrances)
        {
            Debug.Log("adding " + entrance.gameObject.name);
            lstEntrances.Add(entrance);
        }
        return lstEntrances;
    }

    public Material GetSkyboxMaterial()
    {
        //get the portal exit for this level
        PortalExit exit = gameObject.GetComponentInChildren<PortalExit>();
        //the exit camera is a child of the portal exit.  it has a skybox component
        Skybox skybox = exit.gameObject.GetComponentInChildren<Skybox>();
        //get the material from the skybox
        Material skyboxMaterial = skybox.material;

        return skyboxMaterial;
    }

}
