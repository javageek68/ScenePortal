using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour {

    public static PortalController instance = null;

    public Vector3 PlayerLevel = Vector3.zero;
    public string strStartScene = "Main";

    public SceneContainer currentScene;

    private PortalPlayer portalPlayer = null;
    private Dictionary<string, PortalEntrance> dctEntrances = new Dictionary<string, PortalEntrance>();
    private Dictionary<string, PortalExit> dctExits = new Dictionary<string, PortalExit>();
    private Dictionary<string, SceneContainer> dctSceneContainers = new Dictionary<string, SceneContainer>();

    private bool blnPortalScenesLoaded = false;

    private void Awake()
    {
        PortalController.instance = this;
        //load the starting scene
        //SceneManager.LoadScene(strStartScene, LoadSceneMode.Additive);
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (blnPortalScenesLoaded == false)
        {
            //LoadPortalScenes();
            blnPortalScenesLoaded = true;
        }
	}

    public void PlayerEnteredPortal(PortalEntrance entrance, string strCurrentScene)
    {
        Debug.Log("player entered portal.  Current scene " + strCurrentScene);
        //this.currentScene = entrance.exitSceneContainer;
        //move the next scene to the player level
        //entrance.exitSceneContainer.transform.position = PlayerLevel;

        //Does one of the new portals point to the old scene?
        //if so, then move it to an open slot.
        //otherwise, unload the scene

        //unload the current scene
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(strCurrentScene);

    }

    private void ChangeScenes()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    private void LoadPortalScenes()
    {
        //iterate through the portals in this scene and load the corresponding scenes
        foreach (KeyValuePair<string, PortalEntrance> kvpEntrance in dctEntrances)
        {
            string strTargetScene = kvpEntrance.Value.TargetSceneName;
            bool blnAlreadyLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == strTargetScene)
                {
                    blnAlreadyLoaded = true;
                    break;
                }
            }
            //load the scene if we don't already have it
            if (!blnAlreadyLoaded) SceneManager.LoadScene(strTargetScene, LoadSceneMode.Additive);
        }
    }

    private void ConnectPortals()
    {
        //iterate through list of entrances
        //match each one with its exit and the exit's scene container
        //get the portal exit camera's target texture
        //assign the target textture of each camera to the portal entrance
        foreach (KeyValuePair<string, PortalEntrance> kvpEntrance in dctEntrances)
        {
            string strTargetScene = kvpEntrance.Value.TargetSceneName;
            if (dctExits.ContainsKey(strTargetScene))
            {
                PortalExit exit = dctExits[strTargetScene];

            }
        }
    }

    public void RegisterPortalPlayer(PortalPlayer player)
    {
        this.portalPlayer = player;
    }

    public void RegisterEntrance(PortalEntrance portalEntrance)
    {
        if (dctEntrances.ContainsKey(portalEntrance.TargetSceneName))
        {
            Debug.LogWarning("Found multiple portals for " + portalEntrance.TargetSceneName);
        }
        else
        {
            dctEntrances.Add(portalEntrance.TargetSceneName, portalEntrance);
        }
    }

    public void RegisterExit(PortalExit portalExit)
    {
        if (dctExits.ContainsKey(portalExit.SceneName))
        {
            Debug.LogWarning("Found multiple portals exits for " + portalExit.SceneName);
        }
        else
        {
            dctExits.Add(portalExit.SceneName, portalExit);
        }

    }

    public void RegisterSceneContainer(SceneContainer sceneContainer)
    {
        if (dctSceneContainers.ContainsKey(sceneContainer.SceneName))
        {
            Debug.LogWarning("Found multiple portals exits for " + sceneContainer.SceneName);
        }
        else
        {
            dctSceneContainers.Add(sceneContainer.SceneName, sceneContainer);
        }

    }
}
