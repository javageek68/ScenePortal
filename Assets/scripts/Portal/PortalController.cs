using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour {

    public static PortalController instance = null;

    public Vector3 PlayerLevel = Vector3.zero;
    public string strStartScene = "Main";
    public string strCurrentScene;

    private PortalPlayer portalPlayer = null;

    private List<PortalEntrance> lstPortalEntrances;
    private Vector3 vctTempLevelArea = new Vector3(0, 10000, 0);

    private void Awake()
    {
        PortalController.instance = this;

        strCurrentScene = strStartScene;
        //load the starting scene
        SceneManager.LoadScene(strCurrentScene, LoadSceneMode.Additive);

        Debug.Log("looking at scene");
        Scene s = SceneManager.GetSceneByName(strStartScene);
        Debug.Log("scene instance " + s.name);
        GameObject[] gos = s.GetRootGameObjects();
        foreach (GameObject go in gos)
        {
            Debug.Log("found " + go.name);
        }


        ////get a list of portal entrances for this scene
        //lstPortalEntrances = GetPortalEntrances(strCurrentScene);
        //LoadPortalScenes();
        //PortalPlayer[] plrs = FindObjectsOfType(typeof(PortalPlayer)) as PortalPlayer[];
        //Debug.Log("found plrs " + plrs.Length);

    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    public void PlayerEnteredPortal(PortalEntrance entrance, string strOldScene, string strNewScene)
    {
        Debug.Log(string.Format("player entered portal.  old scene {0} new scene {1}", strOldScene, strNewScene));

        //change to the new scene
        strCurrentScene = strNewScene;

        //get the scene container for the new scene
        GameObject newSceneContainer = GetSceneContainer(strNewScene);
        //move the new scene to the player level
        newSceneContainer.transform.position = PlayerLevel;

        //get a list of portal entrances for this scene
        lstPortalEntrances = GetPortalEntrances(strNewScene);

        //check to see if the old scene is a destination of the new scene
        if (SceneIsInPortalList(strOldScene))
        {
            //the scene we are leaving is in the list of the new scene's portals
            //don't bother unloading it
            //get the scene container for the new scene
            GameObject oldSceneContainer = GetSceneContainer(strOldScene);
            //move the scene to temp holding
            oldSceneContainer.transform.position = vctTempLevelArea;
        }
        else
        {
            //the scene we are leaving is NOT in the list of the new scene's portals
            //unload the current scene
            SceneManager.UnloadSceneAsync(strOldScene);
        }

        //unload scenes not contained in the list
        UnloadPortalScenes();

        //load scenes that are contained in the list that are not currently loaded
        LoadPortalScenes();
    }


    /// <summary>
    /// get all of the portals that are in the scene strSceneName
    /// </summary>
    /// <param name="strSceneName"></param>
    /// <returns></returns>
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
                Debug.Log("adding " + entrance.gameObject.name );
                lstEntrances.Add(entrance);
            }
        }
        return lstEntrances;
    }

    private GameObject GetSceneContainer(string strSceneName)
    {
        GameObject goSceneContainer = null;
        SceneContainer[] sceneContainers = FindObjectsOfType(typeof(SceneContainer)) as SceneContainer[];
        foreach (SceneContainer container in sceneContainers)
        {
            if (container.gameObject.scene.name == strSceneName)
            {
                goSceneContainer = container.gameObject;
            }
        }
        return goSceneContainer;
    }

    /// <summary>
    /// Unload any portals that are not pointed to by the current set of portals
    /// </summary>
    private void UnloadPortalScenes()
    {
        for (int i = SceneManager.sceneCount-1; i >= 0; i--)
        {
            string strTargetScene = SceneManager.GetSceneAt(i).name;
            //if the scene is not in the portal list, unload it
            if (!SceneIsInPortalList(strTargetScene)) SceneManager.UnloadSceneAsync(strTargetScene);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strTargetScene"></param>
    /// <returns></returns>
    private bool SceneIsInPortalList(string strTargetScene)
    {
        bool blnFound = false;
        foreach (PortalEntrance entrance in lstPortalEntrances)
        {
            if (strTargetScene == entrance.portalData.ExitSceneName)
            {
                blnFound = true;
                break;
            }
        }
        return blnFound;
    }

    /// <summary>
    /// Load any scenes that are pointed to by the current set of portals but are not already loaded
    /// </summary>
    private void LoadPortalScenes()
    {
        //iterate through the portals in this scene and load the corresponding scenes
        foreach (PortalEntrance  entrance in lstPortalEntrances)
        {
            string strTargetScene = entrance.portalData.ExitSceneName;
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
    

}
