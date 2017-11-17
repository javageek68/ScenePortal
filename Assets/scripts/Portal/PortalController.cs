using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour {

    public static PortalController instance = null;

    public Text txtCurrentScene;
    public Vector3 PlayerLevel = Vector3.zero;
    public string strStartScene = "Main";
    public string strCurrentScene;
    public int intPortalSceneVerticalSpacing = 1000;

    public PortalPlayer portalPlayer = null;

    private List<PortalEntrance> lstPortalEntrances;
    private Vector3 vctTempLevelArea = new Vector3(0, 10000, 0);

    private void Awake()
    {
        PortalController.instance = this;

        SetScene(strStartScene);

        StartCoroutine(LoadStartScene());
    }

    IEnumerator LoadStartScene()
    {
        //load the starting scene
        SceneManager.LoadScene(strCurrentScene, LoadSceneMode.Additive);

        Debug.Log("looking at scene");
        Scene scene = SceneManager.GetSceneByName(strStartScene);

        Debug.Log("scene instance " + scene.name + "  " + scene.isLoaded.ToString());

        //wait for main scene to finish loading
        while (scene.isLoaded == false)
        {
            yield return 0;
        }

        //get the start scene's container
        SceneContainer container = GetSceneContainer(strStartScene);

        Material skyboxMaterial = container.GetSkyboxMaterial();
        portalPlayer.ApplySkyboxMaterial(skyboxMaterial);


        //get the portals from the start scene's container
        lstPortalEntrances = container.GetPortalEntrances(strStartScene);

        //load the scenes that the portals point to
        foreach (PortalEntrance entrance in lstPortalEntrances)
        {
            string strPortalExitScene = entrance.portalData.ExitSceneName;
            SceneManager.LoadScene(strPortalExitScene, LoadSceneMode.Additive);
        }

        yield return 0;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void SetScene(string strScene)
    {
        this.strCurrentScene = strScene;
        this.txtCurrentScene.text = strScene;
        portalPlayer.SetScene(strScene);
    }

    public void PlayerEnteredPortal(PortalEntrance entrance, string strOldScene, string strNewScene)
    {

        Debug.Log(string.Format("player entered portal.  old scene {0} new scene {1}", strOldScene, strNewScene));

        //change to the new scene
        //the exit portal is in the new scene
        SetScene(strNewScene);
        SceneContainer oldScene = GetSceneContainer(strOldScene);
        SceneContainer newScene = GetSceneContainer(strNewScene);

        Debug.Log(string.Format("old scene container {0} new scene container {1}", oldScene.gameObject.scene.name, newScene.gameObject.scene.name));


        Debug.Log(string.Format("placing new scene {0} at player level {1}", newScene.gameObject.scene.name, PlayerLevel));
        //move the new scene to the player level
        newScene.gameObject.transform.position = PlayerLevel;

        //move the player to the portal exit position
        //PortalExit portalExit = newScene.gameObject.GetComponentInChildren<PortalExit>();
        //portalPlayer.transform.position = portalExit.gameObject.transform.position;

        //get the new scene's skybox material and apply it to the player camera
        Material skyboxMaterial = newScene.GetSkyboxMaterial();
        portalPlayer.ApplySkyboxMaterial(skyboxMaterial);
        Debug.Log(string.Format("set skybox material to {0}", skyboxMaterial.name));

        //get a list of portal entrances for this scene
        lstPortalEntrances = newScene.GetPortalEntrances(strNewScene);
        Debug.Log(string.Format("found {0} portal entrances in the new level", lstPortalEntrances.Count));

        //check to see if the old scene is a destination of the new scene
        if (SceneIsInPortalList(strOldScene))
        {
            Debug.Log(string.Format("the new level contains a portal back to the previous level. moving it to {0}", vctTempLevelArea));
            //the scene we are leaving is in the list of the new scene's portals
            //don't bother unloading it
            //move the scene to temp holding
            oldScene.transform.position = vctTempLevelArea;
        }
        else
        {
            Debug.Log(string.Format("the new level DOES NOT contain a portal back to the previous scene {0} . unloading scene", strOldScene));
            //the scene we are leaving is NOT in the list of the new scene's portals
            //unload the current scene
            SceneManager.UnloadSceneAsync(strOldScene);
        }

        Debug.Log("calling UnloadPortalScenes()");
        //unload scenes not contained in the list
        UnloadPortalScenes(strNewScene);

        Debug.Log("calling LoadPortalScenes()");
        //load scenes that are contained in the list that are not currently loaded
        LoadPortalScenes();

        //Move each portal to its vertical slot
        PlacePortalScenesInSlots();

    }

private SceneContainer GetSceneContainer(string strSceneName)
    {
        SceneContainer container = null;
        Scene scene = SceneManager.GetSceneByName(strSceneName);
        GameObject[] gos = scene.GetRootGameObjects();
        foreach (GameObject go in gos)
        {
            container = go.GetComponent<SceneContainer>();
            if (container != null) break;
        }
        return container;
    }



    /// <summary>
    /// Unload any portals that are not pointed to by the current set of portals
    /// </summary>
    private void UnloadPortalScenes(string strCurrScene)
    {
        Debug.Log("entered UnloadPortalScenes()");

        string strDirectorScene = gameObject.scene.name.Trim().ToLower();
        strCurrScene = strCurrScene.Trim().ToLower();

        for (int i = SceneManager.sceneCount-1; i >= 0; i--)
        {
            string strTargetScene = SceneManager.GetSceneAt(i).name.Trim().ToLower();
            Debug.Log("checking " + strTargetScene);
            //if the scene is not in the portal list, unload it
            //unless it is the scene the Director is in5
            if (strTargetScene != strDirectorScene &&
                strTargetScene != strCurrScene && 
                !SceneIsInPortalList(strTargetScene))
            {
                Debug.Log("Unloading " + strTargetScene);
                SceneManager.UnloadSceneAsync(strTargetScene);
            }

            Debug.Log("leaving UnloadPortalScenes()");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strTargetScene"></param>
    /// <returns></returns>
    private bool SceneIsInPortalList(string strTargetScene)
    {
        strTargetScene = strTargetScene.Trim().ToLower();
        Debug.Log("entered SceneIsInPortalList() " + strTargetScene);
        bool blnFound = false;
        foreach (PortalEntrance entrance in lstPortalEntrances)
        {
            Debug.Log(string.Format("checking {0} against {1}", strTargetScene, entrance.portalData.ExitSceneName));
            if (strTargetScene == entrance.portalData.ExitSceneName.Trim().ToLower())
            {
                Debug.Log("We have a match");
                blnFound = true;
                break;
            }
        }
        Debug.Log("leaving SceneIsInPortalList()");
        return blnFound;
    }

    /// <summary>
    /// Load any scenes that are pointed to by the current set of portals but are not already loaded
    /// </summary>
    private void LoadPortalScenes()
    {
        Debug.Log("entered LoadPortalScenes()");
        //iterate through the portals in this scene and load the corresponding scenes
        foreach (PortalEntrance  entrance in lstPortalEntrances)
        {
            string strTargetScene = entrance.portalData.ExitSceneName.Trim().ToLower();
            Debug.Log("checking " + strTargetScene);
            bool blnAlreadyLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name.Trim().ToLower() == strTargetScene)
                {
                    blnAlreadyLoaded = true;
                    Debug.Log(string.Format("{0} is already loaded", strTargetScene));
                    break;
                }
            }
            //load the scene if we don't already have it
            if (!blnAlreadyLoaded)
            {
                Debug.Log("loading " + strTargetScene);
                SceneManager.LoadScene(strTargetScene, LoadSceneMode.Additive);
            }
        }
        Debug.Log("leaving LoadPortalScenes()");
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlacePortalScenesInSlots()
    {
        Debug.Log("entered PlacePortalScenesInSlots()");
        //iterate through the portals in this scene 
        for (int i = 0; i < lstPortalEntrances.Count; i++)
        {
            PortalEntrance entrance = lstPortalEntrances[i];
           //get a ref to the actual scene container
           SceneContainer sceneContaier = GetSceneContainer(entrance.portalData.ExitSceneName);

            int intVerticalLevel = intPortalSceneVerticalSpacing * (i+1) * -1;
            Debug.Log(string.Format("Moving {0} to level {1}", entrance.portalData.ExitSceneName, intVerticalLevel));
            sceneContaier.gameObject.transform.position = new Vector3(0, intVerticalLevel, 0);
        }
        Debug.Log("leaving PlacePortalScenesInSlots()");

    }


}
