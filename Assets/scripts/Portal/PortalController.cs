using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour {

    public static PortalController instance = null;

    public Vector3 PlayerLevel = Vector3.zero;
    public string strStartScene = "Main";
    public string strCurrentScene;

    public PortalPlayer portalPlayer = null;

    private List<PortalEntrance> lstPortalEntrances;
    private Vector3 vctTempLevelArea = new Vector3(0, 10000, 0);

    private void Awake()
    {
        PortalController.instance = this;

        strCurrentScene = strStartScene;

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
        lstPortalEntrances = container.GetPortalEntrances();

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

    public void PlayerEnteredPortal(PortalEntrance entrance, string strOldScene, string strNewScene)
    {

        Debug.Log(string.Format("player entered portal.  old scene {0} new scene {1}", strOldScene, strNewScene));

        //change to the new scene
        //the exit portal is in the new scene
        strCurrentScene = strNewScene;
        SceneContainer oldScene = GetSceneContainer(strOldScene);
        SceneContainer newScene = GetSceneContainer(strNewScene);


        //move the new scene to the player level
        newScene.gameObject.transform.position = PlayerLevel;

        //get the new scene's skybox material and apply it to the player camera
        Material skyboxMaterial = newScene.GetSkyboxMaterial();
        portalPlayer.ApplySkyboxMaterial(skyboxMaterial);

        //get a list of portal entrances for this scene
        lstPortalEntrances = newScene.GetPortalEntrances();

        //check to see if the old scene is a destination of the new scene
        if (SceneIsInPortalList(strOldScene))
        {
            //the scene we are leaving is in the list of the new scene's portals
            //don't bother unloading it
            //move the scene to temp holding
            oldScene.transform.position = vctTempLevelArea;
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
    private void UnloadPortalScenes()
    {
        for (int i = SceneManager.sceneCount-1; i >= 0; i--)
        {
            string strTargetScene = SceneManager.GetSceneAt(i).name;
            //if the scene is not in the portal list, unload it
            //unless it is the scene the Director is in5
            if (!SceneIsInPortalList(strTargetScene) && strTargetScene != gameObject.scene.name)  SceneManager.UnloadSceneAsync(strTargetScene);
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
