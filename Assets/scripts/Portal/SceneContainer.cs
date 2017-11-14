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

    private List<PortalEntrance> GetPortalEntrances(string strSceneName)
    {
        Debug.Log("Entered GetPortalEntrances() - " + strSceneName);
        List<PortalEntrance> lstEntrances = new List<PortalEntrance>();
        PortalEntrance[] entrances = FindObjectsOfType(typeof(PortalEntrance)) as PortalEntrance[];
        Debug.Log("found entrances " + entrances.Length);
        foreach (PortalEntrance entrance in entrances)
        {
            if (entrance.gameObject.scene.name == strSceneName)
            {
                Debug.Log("adding " + entrance.gameObject.name);
                lstEntrances.Add(entrance);
            }
        }
        return lstEntrances;
    }

}
