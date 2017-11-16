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

}
