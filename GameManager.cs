using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    public enum Artefacts
    {
        Butterdish,
        Ore,
        Knife,
        Lyre,
        Textile
    }
    /// <summary>
    /// Enum of the available tiles in the game.
    /// </summary>
    public enum Tile
    {
        Grass,
        Stone,
        Root,
        Sand,
        Dirt,
        Hit,
        Artefact

    }
    /// <summary>
    /// Enum of available scenes in the game.
    /// </summary>
    public enum Scene
    {
        MainMenu = 0,
        WorldMapScene = 1,
        Level = 2,
        PuzzleMinigame = 3,
        Museum = 4,
        Dredge = 5,
        Music = 6,
        TestingScene = 7,
        Knife = 8,
        Row = 9,
        Bellows = 10,
        Loom = 11,
        Butter = 12,
        Options = 13
    }
    
    public static GameManager instance = null;
    private Scene currentScene = Scene.MainMenu;

    public GameObject[] tiles = null;

    private int[] artefactsUnlocked;

    [Header("Artefact")]
    [SerializeField]private int numberOfArtefacts;

    [Header("Tooltips")]
    public bool[] has_Dismissed_Tutorial;
    /* Tracks whether the player has dismised the tooltips between scenes as to avoid showing them again.
    0 = Worldmap
    1 = Core Game
    2 = Assembly
    3 = Knife
    4 = Lyre
    5 = Dish
    6 = Ore
    7 = Textile
    8 = Museum
    */

    [Header("Files")]//The text files required for the level.
    public TextAsset helloTextFile = null;
    public TextAsset levelMap = null;
    public TextAsset artefactInfo = null;
    
    private GameObject transitionLeft;
    private GameObject transitionRight;
    private GameObject transitionCentre;
    private GameObject transitionAnimator;
    bool inTransition;
    public float playbackTime;

    
    private Artefacts currentArtefact;
    private int tutorialComplete;
    private string currentLang;
    void Awake()
    {
        //By defualt, show all tutorial hints.
        has_Dismissed_Tutorial = new bool[9];

        if (instance == null)
        {
            instance = this;
            GameObject temp = new GameObject();
            temp.name = "--Prefabs--";
            tiles = new GameObject[7];
            Init();
            DontDestroyOnLoad(temp);
            for (int i = 0; i < 7; i++)
            {
                tiles[i].transform.parent = GameObject.Find("--Prefabs--").transform;
            }

            artefactsUnlocked = new int[5];
            artefactsUnlocked[(int)Artefacts.Butterdish] = PlayerPrefs.GetInt(Artefacts.Butterdish.ToString());
            artefactsUnlocked[(int)Artefacts.Ore] = PlayerPrefs.GetInt(Artefacts.Ore.ToString());
            artefactsUnlocked[(int)Artefacts.Knife] = PlayerPrefs.GetInt(Artefacts.Knife.ToString());
            artefactsUnlocked[(int)Artefacts.Lyre] = PlayerPrefs.GetInt(Artefacts.Lyre.ToString());
            artefactsUnlocked[(int)Artefacts.Textile] = PlayerPrefs.GetInt(Artefacts.Textile.ToString());
            
            transitionAnimator = new GameObject();
            transitionAnimator.name = "Transition Animation";
            transitionAnimator.AddComponent<Animator>();
            transitionAnimator.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/TransitionStart");
            transitionAnimator.GetComponent<Animator>().speed = 1f;
            transitionAnimator.transform.localPosition = Vector3.zero;


            transitionLeft = new GameObject();
            transitionLeft.name = "TransitionLeft";
            transitionLeft.transform.parent = transitionAnimator.transform;
            transitionCentre = new GameObject();
            transitionCentre.name = "TransitionCentre";
            transitionCentre.transform.parent = transitionAnimator.transform;
            transitionRight = new GameObject();
            transitionRight.name = "TransitionRight";
            transitionRight.transform.parent = transitionAnimator.transform;

            transitionLeft.AddComponent<SpriteRenderer>();
            transitionLeft.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Animations/TransitionLeft");
            transitionLeft.GetComponent<SpriteRenderer>().sortingOrder = 0;
            transitionLeft.GetComponent<SpriteRenderer>().sortingLayerName = "Transition";
            transitionCentre.AddComponent<SpriteRenderer>();
            transitionCentre.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Animations/TransitionCentre");
            transitionCentre.GetComponent<SpriteRenderer>().sortingOrder = 1;
            transitionCentre.GetComponent<SpriteRenderer>().sortingLayerName = "Transition";
            transitionRight.AddComponent<SpriteRenderer>();
            transitionRight.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Animations/TransitionRight");
            transitionRight.GetComponent<SpriteRenderer>().sortingOrder = 0;
            transitionRight.GetComponent<SpriteRenderer>().sortingLayerName = "Transition";

            transitionAnimator.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            transitionAnimator.gameObject.SetActive(false);

            DontDestroyOnLoad(transitionAnimator);

            DontDestroyOnLoad(gameObject);
            
            currentLang = PlayerPrefs.GetString("Lang", "Eng");
            tutorialComplete = PlayerPrefs.GetInt("Tutorial Complete", 0);
            currentArtefact = Artefacts.Butterdish;
            //if (PlayerPrefs.GetString("Lang") != "Eng" || PlayerPrefs.GetString("Lang") != "Fr")
            //    currentLang = "Eng";
            //tutorialComplete = 1;
        }
        else if(instance != this)
        {//Destroys any new game managers that try to be created to keep it singleton.
            Destroy(gameObject);
        }
        
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        //ResetSave();
    }
    // Start is called before the first frame update
    void Start()
    {
        inTransition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();//Exits application
        }

        if (!inTransition)
        {
            transitionAnimator.gameObject.SetActive(false);
        }
    }

    ///<summary>Initialises an array with available tile prefabs</summary>
    void Init()
    {
        tiles[(int)Tile.Grass] = CreateTile(Tile.Grass);
        tiles[(int)Tile.Stone] = CreateTile(Tile.Stone);
        tiles[(int)Tile.Root] = CreateTile(Tile.Root);
        tiles[(int)Tile.Sand] = CreateTile(Tile.Sand);
        tiles[(int)Tile.Dirt] = CreateTile(Tile.Dirt);
        tiles[(int)Tile.Hit] = CreateTile(Tile.Hit);
        tiles[(int)Tile.Artefact] = CreateTile(Tile.Artefact);
    }

    ///<summary>This method creates a new GameObject to represent a tile
    ///then returns it to the tile array</summary>
    ///<param name="type">The type of the tile you want to create.</param>
    GameObject CreateTile(Tile type)
    {
        
        GameObject tileTemp = new GameObject();

        tileTemp.name = type.ToString();
        tileTemp.AddComponent<SpriteRenderer>();
        tileTemp.GetComponent<SpriteRenderer>().sortingOrder = 0;
        tileTemp.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        tileTemp.AddComponent<SpriteMask>();
        tileTemp.GetComponent<SpriteMask>().enabled = false;
        tileTemp.AddComponent<BoxCollider2D>();
        tileTemp.GetComponent<BoxCollider2D>().size = Vector2.one;
        if (type != Tile.Hit)
            tileTemp.layer = LayerMask.NameToLayer(type.ToString());
        else
            tileTemp.layer = LayerMask.NameToLayer("Default");

        //Hides first objects off screen to be used as 'prefabs'
        tileTemp.transform.localPosition = new Vector3(2000, 2000, -20);
        return tileTemp;
    }

    ///<summary>This method sets a random sprite to the tile prefab 
    ///then returns the prefab</summary>
    ///<param name="tileNum">The array index of the required prefab you want.</param>
    public GameObject getTile(int tileNum, string layerNumber)
    {
        Tile chosenTile = (Tile)tileNum;

        if (chosenTile == Tile.Root)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Root/LayerOne/Asset_Tile_LayerOne_Root01");
            return tiles[tileNum];
        }
        else if(chosenTile == Tile.Hit)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Hit/Asset_Tile_TileBreaking_Frame01");
            return tiles[tileNum];
        }
        else
        {

            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/" + chosenTile.ToString() + "/Layer" + layerNumber + "/Asset_Tile_Layer" + layerNumber + "_" + chosenTile.ToString() + "0" + Random.Range(1, 4));
           
            return tiles[tileNum];
        }

    }

    public Sprite GetRoot(int type, string layerNumber)
    {
        tiles[(int)Tile.Root].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Root/Layer" + layerNumber +"/Asset_Tile_Layer" + layerNumber +"_Root0" + type);
        return tiles[(int)Tile.Root].GetComponent<SpriteRenderer>().sprite;
    }

    ///<summary>This method sets a random sprite to the tile prefab 
    ///then returns the prefab</summary>
    ///<param name="tileNum">The array index of the required prefab you want.</param>
    public GameObject getTile(int tileNum)
    {
        Tile chosenTile = (Tile)tileNum;

        if (chosenTile != Tile.Artefact)
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/" + chosenTile.ToString() + "/Asset_Tile_" + chosenTile.ToString() + "0" + Random.Range(1, 4));


        if (chosenTile == Tile.Hit)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Hit/Asset_Tile_TileBreaking_Frame01");
        }

        if (chosenTile == Tile.Root)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Root/Asset_Tile_Root01");
        }
        return tiles[tileNum];

    }
   //Kept in to not remove anything that may be used by Annabelle as to not destroy her work, can be deleted in final build.
    public GameObject GetArtefact(int tileNum, string artefactName)
    {
        if ((Tile)tileNum == Tile.Artefact)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Artefact/Asset_Tile_" + artefactName);
        }

        return tiles[tileNum];
    }

    /// <summary>
    /// Gets an artefact of the desired name.
    /// </summary>
    /// <param name="tileNum">The number of tile you want.</param>
    /// <param name="artefactName">The name of the artefact you want.</param>
    /// <returns>Tile game object</returns>
    public GameObject GetArtefactFinal(int tileNum, int pieceNumber)
    {
        if ((Tile)tileNum == Tile.Artefact)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Artefact/" + currentArtefact + "/Asset_Tile_" + currentArtefact + "0" + pieceNumber);
            return tiles[tileNum];
        }

        return null;
        
    }

    /// <summary>
    /// Gets an artefact of the desired name.
    /// </summary>
    /// <param name="tileNum">The number of tile you want.</param>
    /// <param name="pieceNumber">The number of the piece of artefact you want.</param>
    /// <returns>Tile game object</returns>
    public GameObject GetArtefact(int tileNum, int pieceNumber)
    {
        if ((Tile)tileNum == Tile.Artefact)
        {
            tiles[tileNum].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Artefact/"+ currentArtefact +"/Asset_Tile_" + currentArtefact + "0" + pieceNumber);
        }

        return tiles[tileNum];
    }

    ///<summary>This method uses a switch statement to load different scenes
    ///based off the parameter</summary>
    ///<param name="newScene">The scene number you want to load.</param>
    public void changeScene(int newScene)
    {
        if(!inTransition)
        StartCoroutine(SceneTransition(newScene));
    }

    ///<summary>Reads text from a .txt file
    ///</summary>
    ///<param name="fileName">The file name you wish to retrieve text from.</param>
    public string readTextFile(string fileName)
    {
        switch (fileName)
        {
            case "Hello":
                return helloTextFile.text;
            case "LevelOne":
                return levelMap.text;
            default:
                return "I shouldn't be reaching here! you've broken me.";
        }
    }

    public string GetLevelMap(string artefactName)
    {
        TextAsset mapText = Resources.Load<TextAsset>("Text Files/LevelMaps/" + artefactName);
        if (mapText != null)
            return mapText.text;
        else
            return null;
        
    }
    public string ReadInfoFile(string fileName)
    {
        if(currentLang == "Fr")
        return Resources.Load<TextAsset>("Text Files/Info/French(Demo Only)/Artefact_" + fileName).text;
            else
        return Resources.Load<TextAsset>("Text Files/Info/Artefact_" + fileName).text;
    }

    ///<summary>Returns what the current scene is.</summary>
    public Scene getCurrentScene()
    {
        return currentScene;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="artefactNum">The artefact you wish to check the status of.</param>
    /// <returns>Whether an artefact is unlocked</returns>
    public int IsArtefactUnlocked(int artefactNum)
    {
        return artefactsUnlocked[artefactNum];
    }

    public int[] GetArtefactUnlockList()
    {
        return artefactsUnlocked;
    }


    private void SaveGame()
    {
        PlayerPrefs.SetInt(Artefacts.Butterdish.ToString(), artefactsUnlocked[(int)Artefacts.Butterdish]);
        PlayerPrefs.SetInt(Artefacts.Ore.ToString(), artefactsUnlocked[(int)Artefacts.Ore]);
        PlayerPrefs.SetInt(Artefacts.Knife.ToString(), artefactsUnlocked[(int)Artefacts.Knife]);
        PlayerPrefs.SetInt(Artefacts.Lyre.ToString(), artefactsUnlocked[(int)Artefacts.Lyre]);
        PlayerPrefs.SetInt(Artefacts.Textile.ToString(), artefactsUnlocked[(int)Artefacts.Textile]);
        PlayerPrefs.SetInt("Tutorial Complete", tutorialComplete);
        PlayerPrefs.SetString("Lang", currentLang);
    }

    public void ResetSave()
    {
        PlayerPrefs.SetInt(Artefacts.Butterdish.ToString(), 0);
        PlayerPrefs.SetInt(Artefacts.Ore.ToString(), 0);
        PlayerPrefs.SetInt(Artefacts.Knife.ToString(), 0);
        PlayerPrefs.SetInt(Artefacts.Lyre.ToString(), 0);
        PlayerPrefs.SetInt(Artefacts.Textile.ToString(), 0);
        PlayerPrefs.SetInt("Tutorial Complete", 0);
        PlayerPrefs.SetString("Lang", "Eng");
    }

    public void UnlockArtefact(int artefact)
    {
        Artefacts toUnlock = (Artefacts)artefact;

        switch(toUnlock)
        {
            case Artefacts.Butterdish:
                artefactsUnlocked[(int)Artefacts.Butterdish] = 1;
                break;
            case Artefacts.Ore:
                artefactsUnlocked[(int)Artefacts.Ore] = 1;
                break;
            case Artefacts.Knife:
                artefactsUnlocked[(int)Artefacts.Knife] = 1;
                break;
            case Artefacts.Lyre:
                artefactsUnlocked[(int)Artefacts.Lyre] = 1;
                break;
            case Artefacts.Textile:
                artefactsUnlocked[(int)Artefacts.Textile] = 1;
                break;
            default:
                Debug.LogWarning("Artefact does not exist to unlock!");
                break;
        }
    }

    IEnumerator SceneTransition(int newScene)
    {
        inTransition = true;
        transitionAnimator.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/TransitionStart");
        transitionAnimator.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        StartCoroutine(SceneChanger(newScene));
    }

    IEnumerator SceneChanger(int newScene)
    {
        Scene goToScene = (Scene)newScene;
        transitionAnimator.gameObject.SetActive(false);
        SceneManager.LoadScene("Assets/Resources/Scenes/" + goToScene + ".unity");
        
        currentScene = goToScene;
        transitionAnimator.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/TransitionEnd");
        transitionAnimator.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        inTransition = false;

    }

    public Artefacts GetCurrentArtefact()
    {
        return currentArtefact;
    }

    public void SetCurrentArtefact(Artefacts newArtefact)
    {
        currentArtefact = newArtefact;
    }

    public bool IsInTransition()
    {
        return inTransition;
    }

    public bool GetTutorialComplete()
    {
        if (tutorialComplete == 0)
            return false;
        else
            return true;
    }

    public void SetTutorialComplete(int newComplete)
    {
        if(newComplete == 0 || newComplete == 1)
            tutorialComplete = newComplete;
    }

    public string GetCurrentLanguage()
    {
        return currentLang;
    }

    public void SetCurrentLanguage(string newLang)
    {
        currentLang = newLang;
    }
    
    public void ResetArtefacts()
    {
        artefactsUnlocked[(int)Artefacts.Butterdish] = 0;
        artefactsUnlocked[(int)Artefacts.Ore] = 0;
        artefactsUnlocked[(int)Artefacts.Knife] = 0;
        artefactsUnlocked[(int)Artefacts.Lyre] = 0;
        artefactsUnlocked[(int)Artefacts.Textile] = 0;
    }
}


