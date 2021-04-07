using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
/// <summary>
/// Selection of tools.
/// </summary>


public class Level : MonoBehaviour
{
    private enum Tool
    {
        Brush,
        Dredge,
        Saw,
        Mattock
    }
    /// <summary>
    /// Text file names.
    /// </summary>
    private enum TEXT_FILE_NAME
    {
        HELLO,
        TOOLNAMES
    }

    private enum TileLayers
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }

    private struct ArtefactNode
    {
        private GameObject nodeObject;
        private bool isOccupied;//If it is fully revealed

        public GameObject NodeObject
        {
            get { return nodeObject; }
            set { nodeObject = value; }

        }

        public bool IsOccupied
        {
            get { return isOccupied; }
            set { isOccupied = value; }
        }
    }

    private struct Artefact
    {
        private Vector2 position;//Layer artefact is on
        private bool isRevealed;//If it is fully revealed
        private int numTiles;//The number of tiles it takes up.

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsRevealed
        {
            get { return isRevealed; }
            set { isRevealed = value; }
        }

        public int NumTiles
        {
            get { return numTiles; }
            set { numTiles = value; }
        }
    }

    private struct TileBeingHit
    {
        private GameObject tileHitObject;
        private Vector3 arrayPosition;
        private int breakFrame;

        public GameObject TileObject
        {
            get { return tileHitObject; }
            set { tileHitObject = value; }
        }

        public Vector3 ArrayPosition
        {
            get { return arrayPosition; }
            set { arrayPosition = value; }
        }

        public int BreakFrame
        {
            get { return breakFrame; }
            set { breakFrame = value; }
        }
    }

    private List<TileBeingHit> tilesBeingHit;

    [Header("Map Size")]
    public int width;//Width of map
    public int height;//Height of map
    
    [Header("Sprites")]//The sprites required for the level.
    [SerializeField] private Sprite backgroundSprite = null;
    [SerializeField] private Sprite backgroundMask = null;
    [SerializeField] private Sprite shovelSprite = null;
    [SerializeField] private Sprite hammerSprite = null;
    [SerializeField] private Texture2D waterTexture = null;

    private GameObject tilesHeader;
    private GameObject layerOneGroup;
    private GameObject layerTwoGroup;
    private GameObject layerThreeGroup;
    private GameObject layerFourGroup;
    private GameObject layerFiveGroup;
    private GameObject[,,] tiles;//2D array of tiles.
    private int numLayers;
    private float hitTimer = 0.0f;//Timer used for displaying a tile has been hit.
    private TileBeingHit tileBeingHit;//Struct containing the GameObject of tile hit and it's position in the array.
    Vector3 bottomRightOrigin = new Vector3(2.86f, -3.48f, 0f);
    private Tool currentTool;//The current tool selected.
    private Tool previousTool;
    SpriteRenderer backgroundRenderer;//Sprite renderer for the level background.
    GameObject toolUsed;//A GameObject used to render sprite of tool being used.

    /// <summary>
    /// How many layers the player is able to dig down.
    /// </summary>
    [SerializeField] private int numberOfLayers;
    
    /// <summary>
    /// Stores the position the hammer goes on the screen.
    /// </summary>
    private Vector2 brushPosition;
    private Vector2 sawPosition;
    private Vector2 mattockPosition;
    private Vector2 dredgePosition;
    /// <summary>
    /// Stores the position the currently used tool goes on the screen.
    /// </summary>
    private Vector2 usedToolPosition;

    /// <summary>
    /// GameObject array that stores the tool GameObjects.
    /// </summary>
    GameObject[] toolBelt;
    GameObject toolBeltObject;
    GameObject artefactBeltObject;

    [Header("Sounds")]
    [SerializeField] private AudioClip brushSoundOne;
    [SerializeField] private AudioClip brushSoundTwo;
    [SerializeField] private AudioClip brushSoundThree;
    [SerializeField] private AudioClip brushSelect;
    [SerializeField] private AudioClip dredgeSoundOne;
    [SerializeField] private AudioClip dredgeSoundTwo;
    [SerializeField] private AudioClip dredgeSoundThree;
    [SerializeField] private AudioClip dredgeSelect;
    [SerializeField] private AudioClip handsawSoundOne;
    [SerializeField] private AudioClip handsawSoundTwo;
    [SerializeField] private AudioClip handsawSoundThree;
    [SerializeField] private AudioClip handsawSelect;
    [SerializeField] private AudioClip mattockSoundOne;
    [SerializeField] private AudioClip mattockSoundTwo;
    [SerializeField] private AudioClip mattockSoundThree;
    [SerializeField] private AudioClip mattockSelect;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioClip toolDamageClip;
    [SerializeField] private AudioClip toolBreakClip;
    [SerializeField] private AudioClip buttonSound;


    [Header("Values")]
    public float offset = 0f;

    Material shaderMat;
    GameObject waterObject;
    float drawRange;
    float rippleTimer;
    RenderTexture textRend;
    Texture2D testTex;
    float waterIntensity;
    float waterIntensityTwo;
    bool waterGoingUp;

    /// <summary>
    /// Durability of tools
    /// </summary>
    private int brushDurability;
    private int dredgDurability;
    private int sawDurability;
    private int mattockDurability;
    //The layer which tools are on.
    const int toolLayer = 14;
    //Belt positions;
    private Vector2 beltOpenPosition;
    private Vector2 beltClosedPosition;

    GameObject toolIcon;
    GameObject artefactIcon;
    private bool toolBeltActive;
    private bool artefactBeltActive;
    private bool toolBeltMoving;
    private bool artefactBeltMoving;

    [SerializeField] private float beltTravelSpeed;

    [Header("Artefact Positions")]
    private Vector2 artefactPositionOne;
    private Vector2 artefactPositionTwo;
    private Vector2 artefactPositionThree;
    private Vector2 artefactPositionFour;
    private int artefactPiecesCollected;
    private int totalArtefactPieces;
    private bool artefactSelected;
    private GameObject selectedArtefact;
    private Vector2 initialArtefactPosition;
    private int initialSortingOrder;
    private Transform initialParent;
    private Vector2[] artefactNodePositions;
    private ArtefactNode[] artefactNodes;

    private bool initialRootsSetup;

    private bool toolBroken;
    private GameObject failObject;
    private float bgAlpha;
    private int tutorialSection;
    private bool tutorialComplete;
    private GameObject tutorialObject;
    private int layerToRemove;
    /// <summary>
    /// Tutorial tracking booleans
    /// </summary>
    private bool delayFinished;
    private bool delayStarted;
    private bool layerRemoveStarted;
    private bool layerRemoveFinshed;
    private bool newSection;
    private GameObject exampleTile;
    // Start is called before the first frame update
    void Start()
    {
        toolBroken = false;
        initialRootsSetup = false;
        artefactPiecesCollected = 0;
        tutorialSection = 1;
        tutorialComplete = GameManager.instance.GetTutorialComplete();

        if (!tutorialComplete)
        {
            layerToRemove = 0;
            delayFinished = false;
            delayStarted = false;
            layerRemoveStarted = false;
            layerRemoveFinshed = false;
            newSection = false;
            tutorialObject = new GameObject();
            InitTutorial();
        }
        else
        {
            tutorialObject = null;
        }

        switch(GameManager.instance.GetCurrentArtefact())
        {
            case GameManager.Artefacts.Butterdish:
                totalArtefactPieces = 3;
                break;
            case GameManager.Artefacts.Ore:
                totalArtefactPieces = 3;
                break;
            case GameManager.Artefacts.Knife:
                totalArtefactPieces = 2;
                break;
            case GameManager.Artefacts.Textile:
                totalArtefactPieces = 4;
                break;
            case GameManager.Artefacts.Lyre:
                totalArtefactPieces = 2;
                break;
        }

        float startingPositionTemp = 1f;
        artefactNodePositions = new Vector2[totalArtefactPieces];
        artefactNodes = new ArtefactNode[totalArtefactPieces];

        initialArtefactPosition = Vector2.zero;
        artefactSelected = false;
        initialSortingOrder = 0;
        
        
        for(int i = 0; i < totalArtefactPieces; i++)
        {
            artefactNodePositions[i] = new Vector2(-0.05f, startingPositionTemp);
            startingPositionTemp -= 0.5f;
            artefactNodes[i].IsOccupied = false;
        }

        usedToolPosition = new Vector2(1.9f, 0);
        brushPosition = new Vector2(-0.1f, 0.8f);
        dredgePosition = new Vector2(-0.1f, 0.15f);
        sawPosition = new Vector2(-0.1f, -0.49f);
        mattockPosition = new Vector2(-0.1f, -1.13f);

        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = backgroundSprite;
        
        //Get the size of the background sprite and scale it to the viewport
        backgroundRenderer.transform.localScale = Vector3.one;
        double swidth = backgroundRenderer.sprite.bounds.size.x;
        double sheight = backgroundRenderer.sprite.bounds.size.y;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        if (worldScreenHeight > 10)
            worldScreenHeight = 10;
        if (worldScreenWidth > 17.77f)
            worldScreenWidth = 17.78f;
        backgroundRenderer.transform.localScale = new Vector2( (float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));

        beltOpenPosition = new Vector2(-2.1f, 0f);
        beltClosedPosition = new Vector2(-2.95f, 0f);

        artefactBeltActive = false;
        toolBeltActive = false;
        toolBeltMoving = false;
        artefactBeltMoving = false;

        InitBelts(new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight)));

        GameObject tileMask = new GameObject();
        tileMask.AddComponent<SpriteRenderer>();
        tileMask.name = "Background Overlay";
        tileMask.GetComponent<SpriteRenderer>().sprite = backgroundMask;
        tileMask.GetComponent<SpriteRenderer>().transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));
        tileMask.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        tileMask.GetComponent<SpriteRenderer>().sortingOrder = 1;
        //Initialise all the layer groups.
        InitLayerGroups();

        numLayers = 5;
        tiles = new GameObject[numLayers, width, height];//Init tile array
        generateLevel();//Generate the level

        brushDurability = 0;
        dredgDurability = 0;
        sawDurability = 0;
        mattockDurability = 0;
        currentTool = Tool.Brush;
        previousTool = Tool.Dredge;
        toolUsed = new GameObject
        {
            name = currentTool.ToString()
        };
        toolUsed.AddComponent<SpriteRenderer>();
        toolUsed.GetComponent<SpriteRenderer>().sprite = shovelSprite;
        toolUsed.GetComponent<SpriteRenderer>().sortingOrder = 3;
        toolUsed.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolUsed.transform.parent = gameObject.transform;
        toolUsed.GetComponent<SpriteRenderer>().transform.localPosition = usedToolPosition;
        InitTools();

        numberOfLayers = 5;

        Texture2D startingTexture = Resources.Load<Texture2D>("Sprites/TestingGround/Asset_DiggingUnderwater_WaterFilter");
        testTex = new Texture2D(Screen.width, Screen.height);
        textRend = new RenderTexture(Screen.width, Screen.height, 1080);
        textRend.Create();
        drawRange = 50f;
        rippleTimer = 0f;

        GameObject testQuad = GameObject.CreatePrimitive(PrimitiveType.Quad) as GameObject;
        testQuad.name = "Scene Texture";
        testQuad.GetComponent<Renderer>().material = Resources.Load<Material>("Sprites/TestingGround/Water");
        testQuad.GetComponent<Renderer>().material.SetTexture("_MainTex", textRend);
        testQuad.GetComponent<Renderer>().material.SetTexture("_WaterTex", Resources.Load<Texture2D>("Sprites/TestingGround/Asset_DiggingUnderwater_WaterFilter"));
        testQuad.GetComponent<Renderer>().material.SetTexture("_NoiseTex", Resources.Load<Texture2D>("Sprites/TestingGround/dudvWater"));
        var quadHeight = Camera.main.orthographicSize * 2.0f;
        var quadWidth = quadHeight * Screen.width / Screen.height;
        testQuad.transform.localScale = new Vector3(quadWidth, quadHeight, 1);
        testQuad.transform.localPosition = new Vector3(46, 0, 0);

        shaderMat = testQuad.GetComponent<Renderer>().material;
        shaderMat.SetFloat("_BlendRatioDistort", 0.1f);
        
        waterIntensity = -0.02f;
        waterIntensityTwo = 0.02f;
        waterGoingUp = true;

        tilesBeingHit = new List<TileBeingHit>();


        InitFailMessage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Vector2.zero;
#if UNITY_ANDROID//Run this section on Android.
            mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR//Run this section on PC build or in editor.
            mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
#endif
            shaderMat.SetFloat("_MousePositionX", mousePosition.x);
            shaderMat.SetFloat("_MousePositionY", mousePosition.y);
            shaderMat.SetInt("_DrawRipple", 1);
            shaderMat.SetFloat("_DrawRange", drawRange);

            if (rippleTimer <= 0)
                rippleTimer = 10f;
        }

        if (rippleTimer >= 0)//If the ripple is active.
        {
            rippleTimer -= 20f * Time.deltaTime;
            drawRange += 200f * Time.deltaTime;
            shaderMat.SetFloat("_DrawRange", drawRange);
        }

        if (rippleTimer <= 0f)//If the ripple is not active.
        {
            shaderMat.SetInt("_DrawRipple", 0);
            drawRange = 10f;
        }


        if (!tutorialComplete)
        {
            if (!GameManager.instance.IsInTransition())
                SoundManager.instance.PlayMusic(levelMusic);
            PerformTutorial();
        }

        if(!toolBroken && tutorialComplete)
        {
            if (!GameManager.instance.IsInTransition())
                SoundManager.instance.PlayMusic(levelMusic);
            RenderTexture.active = textRend;
            Camera.main.targetTexture = textRend;
        
            checkForInput();
            //Checks if a tile has been hit.
            TileHit();
            //Checks if tool has been changed
            CheckTool();
            
            if(toolBeltMoving)
            {
                if(toolBeltActive)
                {
                    toolBeltObject.transform.localPosition = Vector3.Lerp(toolBeltObject.transform.localPosition, beltOpenPosition, beltTravelSpeed * Time.deltaTime);
                    if (toolBeltObject.transform.localPosition.x > beltOpenPosition.x - 0.05f)
                    {
                        toolBeltMoving = false;
                    }
                }
                else if(!toolBeltActive)
                {
                    toolBeltObject.transform.localPosition = Vector3.Lerp(toolBeltObject.transform.localPosition, beltClosedPosition, beltTravelSpeed * Time.deltaTime);
                    if (toolBeltObject.transform.localPosition.x < beltClosedPosition.x + 0.05f)
                    {
                        toolBeltMoving = false;
                        artefactIcon.SetActive(true);
                        artefactBeltObject.SetActive(true);
                    }
                }
            }

            if(artefactBeltMoving)
            {
                if(artefactBeltActive)
                {
                    artefactBeltObject.transform.localPosition = Vector3.Lerp(artefactBeltObject.transform.localPosition, beltOpenPosition, beltTravelSpeed * Time.deltaTime);
                    if (artefactBeltObject.transform.localPosition.x > beltOpenPosition.x - 0.05f)
                    {
                        artefactBeltMoving = false;
                    }
                }
                else if(!artefactBeltActive)
                {
                    artefactBeltObject.transform.localPosition = Vector3.Lerp(artefactBeltObject.transform.localPosition, beltClosedPosition, beltTravelSpeed * Time.deltaTime);
                    if (artefactBeltObject.transform.localPosition.x < beltClosedPosition.x + 0.05f)
                    {
                        artefactBeltMoving = false;
                        toolIcon.SetActive(true);
                        toolBeltObject.SetActive(true);
                    }
                }
            }
        
            if(artefactPiecesCollected >= totalArtefactPieces)
            {
                SoundManager.instance.StopMusic();
                GameManager.instance.changeScene((int)GameManager.Scene.PuzzleMinigame);
            }
        }
        else
        {
            
           if (bgAlpha < 0.8f)
           {
                 bgAlpha += 0.5f * Time.deltaTime;
                failObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Vector4(0f, 0f, 0f, bgAlpha);
                if (bgAlpha >= 0.75f)
                {
                    failObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    shaderMat.SetInt("_DrawRipple", 0);
                }
           }
        }


    }

    private void FixedUpdate()
    {
        if(!toolBroken)
        {
            if (waterIntensity >= 0.02f)
            {
                waterGoingUp = false;
            }
            if (waterIntensity <= -0.02f)
                waterGoingUp = true;

            if (waterGoingUp)
            {
                waterIntensity += 0.05f * Time.deltaTime;
                waterIntensityTwo -= 0.05f * Time.deltaTime;

            }
            else if (!waterGoingUp)
            {
                waterIntensity -= 0.05f * Time.deltaTime;
                waterIntensityTwo += 0.05f * Time.deltaTime;

            }
            shaderMat.SetFloat("_Intensity", waterIntensity);
            
        }
        
    }

    /// <summary>
    /// Instantiates GameObjects and stores them within a 2D array based off numerical values read in from a text file.
    /// </summary>
    void generateLevel()
    {
        string tempData;
        if (tutorialComplete)
            tempData = GameManager.instance.GetLevelMap(GameManager.instance.GetCurrentArtefact().ToString() + "Map");//Gets the data from the desired text file
        else
            tempData = GameManager.instance.GetLevelMap("TutorialMap");

        tempData = tempData.Replace(System.Environment.NewLine, "");//Replaces the new lines with nothing for easier reading next step.
        string[] layers = tempData.Split(',');
        int piecesPlaced = 0;
        int x, y;
        x = 0;
        y = 0;
        int layerNumber = 0;
        for(int it = 0; it < numLayers; it++)
        {
            string[] currentLayout = layers[it].Split('-');//Splits the characters into an array based off the desired character.
            System.Array.Reverse(currentLayout);//Reverses the data for doing grid layout correct way.
            TileLayers currentTileLayer = (TileLayers)it + 1;
            for (int i = 0; i < currentLayout.Length; i++)
            {
                GameObject currentTile;

                if (int.Parse(currentLayout[i]) != 8)
                {
                    if (int.Parse(currentLayout[i]) == 9 && piecesPlaced <= totalArtefactPieces)
                    {
                        currentTile = Instantiate(GameManager.instance.GetArtefactFinal((int)GameManager.Tile.Artefact, piecesPlaced + 1), bottomRightOrigin + new Vector3(-x, y), Quaternion.identity) as GameObject;
                        piecesPlaced++;
                    }
                    else
                        currentTile = Instantiate(GameManager.instance.getTile(int.Parse(currentLayout[i]) - 1, currentTileLayer.ToString()), bottomRightOrigin + new Vector3(-x, y), Quaternion.identity) as GameObject;
                    //currentTile.transform.parent = backgroundRenderer.transform;
                    tiles[(numLayers - layerNumber) - 1, x, y] = currentTile;//Sets current array position to the current tile
                }
                else
                {
                    currentTile = null;
                    tiles[(numLayers - layerNumber) - 1, x, y] = null;
                }

                if(tiles[(numLayers - layerNumber) - 1, x, y] != null)
                {

                    switch (layerNumber)
                    {
                        case 0:
                            tiles[(numLayers - layerNumber) - 1, x, y].transform.parent = layerOneGroup.transform;
                            break;
                        case 1:
                            tiles[(numLayers - layerNumber) - 1, x, y].transform.parent = layerTwoGroup.transform;
                            break;
                        case 2:
                            tiles[(numLayers - layerNumber) - 1, x, y].transform.parent = layerThreeGroup.transform;
                            break;
                        case 3:
                            tiles[(numLayers - layerNumber) - 1, x, y].transform.parent = layerFourGroup.transform;
                            break;
                        case 4:
                            tiles[(numLayers - layerNumber) - 1, x, y].transform.parent = layerFiveGroup.transform;
                            break;
                        default:
                            Debug.LogError("Layer grouping default reached!");
                            break;
                    }
                }

                if(tiles[(numLayers - layerNumber) - 1, x, y] != null)
                tiles[(numLayers - layerNumber) - 1, x, y].transform.localPosition = new Vector3(tiles[(numLayers - layerNumber) - 1, x, y].transform.localPosition.x, tiles[(numLayers - layerNumber) - 1, x, y].transform.localPosition.y, layerNumber + 1);

                x++;//Increases x value to iterate along the 'width' of the array.
                if (x == width)
                {//Once at the end width of array it proceeds down the 'height' of the array.
                    y++;
                    x = 0;
                }
                
            }
            if(layerNumber < 4)
                layerNumber++;

            x = 0;
            y = 0;
        }

        ArrangeRoots();
    }

   

    ///<summary>Checks through various inputs.</summary>
    private void checkForInput()
    { 

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
            Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

            RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
            if (hit2D.collider != null)//Checks if ray has collided.
            {
                GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                if(toolBeltActive && clickedObject.layer == toolLayer)
                {
                    switch(clickedObject.name)
                    {
                        case "Brush":
                            if (currentTool != Tool.Brush)
                            {
                                previousTool = currentTool;
                                currentTool = Tool.Brush;
                                SoundManager.instance.PlayClipOneShot(brushSelect);
                            }
                            break;
                        case "Dredge":
                            if (currentTool != Tool.Dredge)
                            {
                                previousTool = currentTool;
                                currentTool = Tool.Dredge;
                                SoundManager.instance.PlayClipOneShot(dredgeSelect);
                            }
                            break;
                        case "Mattock":
                            if (currentTool != Tool.Mattock)
                            {
                                previousTool = currentTool;
                                currentTool = Tool.Mattock;
                                SoundManager.instance.PlayClipOneShot(mattockSelect);
                            }
                            break;
                        case "Saw":
                            if (currentTool != Tool.Saw)
                            {
                                previousTool = currentTool;
                                currentTool = Tool.Saw;
                                SoundManager.instance.PlayClipOneShot(handsawSelect);
                            }
                            break;
                        default:
                            Debug.LogError("Tool selection switch has been called with no valid tool present!");
                            break;
                    }
                    return;
                }

                if(clickedObject.name == "Tool Tab" || clickedObject.name == "Artefact Tab")
                {
                    if (clickedObject.name == "Tool Tab")
                    {
                        if (!toolBeltMoving)
                        {
                            toolBeltMoving = true;
                            toolBeltActive = !toolBeltActive;
                            if (toolBeltActive)
                            {
                                artefactIcon.SetActive(false);
                                artefactBeltObject.SetActive(false);
                                toolIcon.SetActive(true);
                            }
                        }
                    }
                    
                    if(clickedObject.name == "Artefact Tab")
                    {
                        if(!artefactBeltMoving)
                        {
                            artefactBeltMoving = true;
                            artefactBeltActive = !artefactBeltActive;
                            if (artefactBeltActive)
                            {
                                artefactIcon.SetActive(true);
                                toolIcon.SetActive(false);
                                toolBeltObject.SetActive(false);
                            }
                        }

                    }

                    return;
                }

                Vector3Int arrayPosition = GetObjectArrayPosition(clickedObject);
                bool toolDamaged = false;
                if (arrayPosition != Vector3Int.down)//If the game object is in the tiles array.
                {
                    //Checks for ray hit on Rock layer.
                    if (clickedObject.layer == LayerMask.NameToLayer("Stone"))
                    {
                        if(currentTool == Tool.Mattock)
                            PerformDig(arrayPosition, clickedObject);
                        else
                            toolDamaged = true;

                    }
                    //Checks for ray hit on Dirt layer
                    if (clickedObject.layer == LayerMask.NameToLayer("Dirt"))
                    {
                        if (currentTool == Tool.Brush)
                            PerformDig(arrayPosition, clickedObject);
                        else
                            toolDamaged = true;
                    }
                    //Checks for ray hit on Sand layer
                    if (clickedObject.layer == LayerMask.NameToLayer("Sand"))
                    {
                        if(currentTool == Tool.Dredge)
                            PerformDig(arrayPosition, clickedObject);
                        else
                            toolDamaged = true;
                    }
                    //Checks for ray hit on root layer
                    if (clickedObject.layer == LayerMask.NameToLayer("Root"))
                    {
                        if(currentTool == Tool.Saw)
                            PerformDig(arrayPosition, clickedObject);
                        else
                            toolDamaged = true;
                    }

                    if (clickedObject.layer == LayerMask.NameToLayer("Artefact"))
                    {
                        if(artefactBeltActive && selectedArtefact == null)
                        {
                            artefactSelected = true;
                            selectedArtefact = clickedObject;
                            initialArtefactPosition = selectedArtefact.transform.position;
                            initialSortingOrder = selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder;
                            selectedArtefact.GetComponent<SpriteRenderer>().sortingLayerName = "Artefact";
                            selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder = 7;
                            selectedArtefact.layer = 2;
                            initialParent = selectedArtefact.transform.parent;
                            selectedArtefact.transform.parent = null;
                        }
                        
                    }
                    if(toolDamaged)
                    {
                        DamageTool();
                    }
                }
                
            }
 
        }

        if(artefactSelected)
        {
            if(Input.GetMouseButton(0))
            {
                if(selectedArtefact != null)
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    selectedArtefact.transform.position = mousePos;
                   
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                if(artefactSelected)
                {
                    bool nodeHit = false;
                    if(selectedArtefact != null)
                    {
                        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);//Raycast from mouse position in a straight line.
                        if (hit2D.collider != null)//Checks if ray has collided.
                        {
                            for (int i = 0; i < totalArtefactPieces; i++)
                            {
                                if (hit2D.collider.gameObject == artefactNodes[i].NodeObject)
                                {
                                    if(!artefactNodes[i].IsOccupied)
                                    {
                                        selectedArtefact.transform.parent = artefactNodes[i].NodeObject.transform;
                                        selectedArtefact.transform.localPosition = Vector2.zero;
                                        artefactNodes[i].IsOccupied = true;
                                        selectedArtefact.GetComponent<BoxCollider2D>().enabled = false;
                                        artefactPiecesCollected++;
                                        nodeHit = true;
                                    }
                                }

                            }
                        }

                        if(!nodeHit)
                        {
                            selectedArtefact.transform.position = initialArtefactPosition;
                            selectedArtefact.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                            selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder = initialSortingOrder;
                            selectedArtefact.layer = LayerMask.NameToLayer("Artefact");
                            selectedArtefact.transform.parent = initialParent;
                        }

                        artefactSelected = false;
                        selectedArtefact = null;
                    }
                }
            }
        }
    }

    ///<summary>Takes the object hit by ray cast and checks if it matches any in
    ///the 2D tiles array then returns the index position if found</summary>
    ///<param name="clickedObject">The object that has been hit by the ray cast from mouse.</param>
    private Vector3Int GetObjectArrayPosition(GameObject clickedObject)
    {

        for(int it = 0; it < numLayers; it++)
        {

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (clickedObject == tiles[it, i, j])
                    {
                        return new Vector3Int(it, i, j);
                    }
                }
            }
        }

        return Vector3Int.down;

    }
    ///<summary>Sets the GameObject to the currently hit tile and records the position in array
    ///then changes the sprite to the breaking sprite and increases the current layer</summary>
    ///<param name="index">The array index position of the game object.</param>
    ///<param name="clickedObject">The currently hit GameObject to be destroyed.</param>
    private void PerformDig(Vector3Int index, GameObject clickedObject)
    {
        Vector2 tempPosition = clickedObject.transform.position;

        if (index.x < numberOfLayers)
        {
            if (tileBeingHit.TileObject == null)
            {
                tileBeingHit.TileObject = clickedObject;
                tileBeingHit.ArrayPosition = new Vector3(index.x, index.y, index.z);
                tiles[index.x, index.y, index.z].GetComponent<SpriteMask>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Hit/Asset_Tile_TileBreaking_Frame01");
                tiles[index.x, index.y, index.z].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                tiles[index.x, index.y, index.z].GetComponent<SpriteMask>().enabled = true;
                tileBeingHit.BreakFrame = 1;
                //tilesBeingHit.Add(tileBeingHit);

                switch(currentTool)
                {
                    case Tool.Mattock:
                        SoundManager.instance.RandomClipOneShot(mattockSoundOne, mattockSoundTwo, mattockSoundThree);
                        break;
                    case Tool.Saw:
                        SoundManager.instance.RandomClipOneShot(handsawSoundOne, handsawSoundTwo, handsawSoundThree);
                        break;
                    case Tool.Brush:
                        SoundManager.instance.RandomClipOneShot(brushSoundOne, brushSoundTwo, brushSoundThree);
                        break;
                    case Tool.Dredge:
                        SoundManager.instance.RandomClipOneShot(dredgeSoundOne, dredgeSoundTwo, dredgeSoundThree);
                        break;
                    default:
                        Debug.LogError("Reached default on tool sound switch!");
                        break;
                }
                
            }

        }
    }

    ///<summary>Initialises all the tools to either be the selected tool or the tool
    ///on the tool belt and adds components required to render the sprite</summary>
    void InitTools()
    {
        toolBelt = new GameObject[5];
        //Brush
        toolBelt[(int)Tool.Brush] = new GameObject();
        toolBelt[(int)Tool.Brush].name = Tool.Brush.ToString();
        toolBelt[(int)Tool.Brush].AddComponent<SpriteRenderer>();
        toolBelt[(int)Tool.Brush].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Brush/Asset_Tool_Side_Brush_Break0" + brushDurability.ToString());
        toolBelt[(int)Tool.Brush].GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolBelt[(int)Tool.Brush].GetComponent<SpriteRenderer>().sortingOrder = 3;
        toolBelt[(int)Tool.Brush].AddComponent<BoxCollider2D>();
        toolBelt[(int)Tool.Brush].transform.parent = toolBeltObject.transform;
        toolBelt[(int)Tool.Brush].transform.localPosition = brushPosition;
        toolBelt[(int)Tool.Brush].layer = toolLayer;
        //Hammer
        toolBelt[(int)Tool.Dredge] = new GameObject();
        toolBelt[(int)Tool.Dredge].name = Tool.Dredge.ToString();
        toolBelt[(int)Tool.Dredge].AddComponent<SpriteRenderer>();
        toolBelt[(int)Tool.Dredge].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Hose/Asset_Tool_Side_DredgeHose_Break0" + dredgDurability.ToString());
        toolBelt[(int)Tool.Dredge].GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolBelt[(int)Tool.Dredge].GetComponent<SpriteRenderer>().sortingOrder = 3;
        toolBelt[(int)Tool.Dredge].AddComponent<BoxCollider2D>();
        toolBelt[(int)Tool.Dredge].transform.parent = toolBeltObject.transform;
        toolBelt[(int)Tool.Dredge].transform.localPosition = dredgePosition;
        toolBelt[(int)Tool.Dredge].layer = toolLayer;
        //Saw
        toolBelt[(int)Tool.Saw] = new GameObject();
        toolBelt[(int)Tool.Saw].name = Tool.Saw.ToString();
        toolBelt[(int)Tool.Saw].AddComponent<SpriteRenderer>();
        toolBelt[(int)Tool.Saw].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Saw/Asset_Tool_Side_Handsaw_Break0" + sawDurability.ToString());
        toolBelt[(int)Tool.Saw].GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolBelt[(int)Tool.Saw].GetComponent<SpriteRenderer>().sortingOrder = 3;
        toolBelt[(int)Tool.Saw].AddComponent<BoxCollider2D>();
        toolBelt[(int)Tool.Saw].transform.parent = toolBeltObject.transform;
        toolBelt[(int)Tool.Saw].transform.localPosition = sawPosition;
        toolBelt[(int)Tool.Saw].layer = toolLayer;
        //Mattock
        toolBelt[(int)Tool.Mattock] = new GameObject();
        toolBelt[(int)Tool.Mattock].name = Tool.Mattock.ToString();
        toolBelt[(int)Tool.Mattock].AddComponent<SpriteRenderer>();
        toolBelt[(int)Tool.Mattock].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Asset_Tool_Side_Mattock");
        toolBelt[(int)Tool.Mattock].GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolBelt[(int)Tool.Mattock].GetComponent<SpriteRenderer>().sortingOrder = 3;
        toolBelt[(int)Tool.Mattock].AddComponent<BoxCollider2D>();
        toolBelt[(int)Tool.Mattock].transform.parent = toolBeltObject.transform;
        toolBelt[(int)Tool.Mattock].transform.localPosition = mattockPosition;
        toolBelt[(int)Tool.Mattock].layer = toolLayer;
    }

    ///<summary>If a tile is in the process of being destroyed increases timer
    ///until hitTimer is reached then destroys and replaces tile if there's more layers</summary>
    private void TileHit()
    {

        //if(tilesBeingHit.Count != 0)
        //{
        //    foreach (var tile in tilesBeingHit)
        //    {
        //    };
        //}
        if (tileBeingHit.TileObject != null)
        {
            hitTimer += 3f * Time.deltaTime;//Increments the timer.
            Vector3Int index = new Vector3Int((int)tileBeingHit.ArrayPosition.x, (int)tileBeingHit.ArrayPosition.y, (int)tileBeingHit.ArrayPosition.z);

            if (hitTimer > 1.0f)
            {//Once reached destroys and created a new tile in the hit tiles location.
                bool rootHit = (tileBeingHit.TileObject.layer == LayerMask.NameToLayer("Root"));
                Destroy(tiles[index.x, index.y, index.z]);
                tiles[index.x, index.y, index.z] = null;
                tileBeingHit.TileObject = null;
                tileBeingHit.ArrayPosition = Vector3.zero;
                hitTimer = 0.0f;//Reset the hit timer.
                if (rootHit)
                    ArrangeRoots();
            }
            else
            {
                if (hitTimer % 0.2f <= 0.01f)
                {
                    if(tileBeingHit.BreakFrame < 4)
                        tileBeingHit.BreakFrame++;
                    tileBeingHit.TileObject.GetComponent<SpriteMask>().sprite = Resources.Load<Sprite>("Tiles/TileSprites/Hit/Asset_Tile_TileBreaking_Frame0" + tileBeingHit.BreakFrame);
                }
            }
        }
    }

    ///<summary>Checks if the player has changed their tool then updates sprites based off this
    ///</summary>
    private void CheckTool()
    {
        //Checks if the curreent tool has been changed.
        if (currentTool != previousTool)
        {
            toolUsed.name = currentTool.ToString();
            switch (currentTool)
            {
                case Tool.Brush:
                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Brush_Break0"+ brushDurability);
                    toolBelt[(int)currentTool].SetActive(false);
                    toolBelt[(int)previousTool].SetActive(true);
                    currentTool = Tool.Brush;
                    break;
                case Tool.Dredge:
                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_DredgeHose_Break0" + dredgDurability.ToString());
                    toolBelt[(int)currentTool].SetActive(false);
                    toolBelt[(int)previousTool].SetActive(true);
                    currentTool = Tool.Dredge;
                    break;
                case Tool.Saw:
                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Handsaw_Break0" + sawDurability.ToString());
                    toolBelt[(int)currentTool].SetActive(false);
                    toolBelt[(int)previousTool].SetActive(true);
                    currentTool = Tool.Saw;
                    break;
                case Tool.Mattock:
                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Mattock_Break0" + mattockDurability.ToString());
                    toolBelt[(int)currentTool].SetActive(false);
                    toolBelt[(int)previousTool].SetActive(true);
                    currentTool = Tool.Mattock;
                    break;
                default:
                    Debug.LogError("Tool sprite default reached!");
                    break;
            }
            previousTool = currentTool;
        }
    }

    private void InitBelts(Vector2 scale)
    {
        toolBeltObject = new GameObject();
        toolBeltObject.name = "Tool Belt";
        toolBeltObject.transform.parent = gameObject.transform;
        toolBeltObject.transform.localPosition = beltClosedPosition;
        toolBeltObject.AddComponent<SpriteRenderer>();
        toolBeltObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_Belt_Toolbelt");
        toolBeltObject.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolBeltObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        toolBeltObject.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one;
        

        GameObject toolCover = new GameObject();
        toolCover.name = "Tool Cover";
        toolCover.AddComponent<SpriteRenderer>();
        toolCover.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_DiggingUnderwater_Toolbelt_NoShadow");
        toolCover.transform.parent = toolBeltObject.transform;
        toolCover.transform.localPosition = new Vector2(-0.37f, -0.15f);
        toolCover.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolCover.GetComponent<SpriteRenderer>().sortingOrder = 4;
        toolCover.GetComponent<SpriteRenderer>().transform.localScale = Vector2.one;

        toolIcon = new GameObject();
        toolIcon.name = "Tool Tab";
        toolIcon.AddComponent<SpriteRenderer>();
        toolIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_Button_Tool");
        toolIcon.transform.parent = toolBeltObject.transform;
        toolIcon.GetComponent<SpriteRenderer>().transform.localScale = Vector2.one;
        toolIcon.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        toolIcon.GetComponent<SpriteRenderer>().sortingOrder = 6;
        toolIcon.AddComponent<BoxCollider2D>();
        toolIcon.transform.localPosition = new Vector2(0.5f, 1.26f);


        artefactBeltObject = new GameObject();
        artefactBeltObject.name = "Artefact Belt";
        artefactBeltObject.transform.parent = gameObject.transform;
        artefactBeltObject.transform.localPosition = beltClosedPosition;
        artefactBeltObject.AddComponent<SpriteRenderer>();
        artefactBeltObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_Belt_ArtifactBelt");
        artefactBeltObject.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        artefactBeltObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
       artefactBeltObject.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one;

        for(int i = 0; i < totalArtefactPieces; i++)
        {
            artefactNodes[i].NodeObject = new GameObject();
            artefactNodes[i].NodeObject.name = "Node " + (i + 1);
            artefactNodes[i].NodeObject.transform.parent = artefactBeltObject.transform;
            artefactNodes[i].NodeObject.transform.localScale = Vector2.one;
            artefactNodes[i].NodeObject.transform.localPosition = artefactNodePositions[i];
            artefactNodes[i].NodeObject.AddComponent<SpriteRenderer>();
            artefactNodes[i].NodeObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_Belt_ArtifactNode");
            artefactNodes[i].NodeObject.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
            artefactNodes[i].NodeObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
            artefactNodes[i].NodeObject.AddComponent<BoxCollider2D>();
            artefactNodes[i].NodeObject.GetComponent<BoxCollider2D>().size = new Vector2(0.4f, 0.4f);
        }

        artefactIcon = new GameObject();
        artefactIcon.name = "Artefact Tab";
        artefactIcon.AddComponent<SpriteRenderer>();
        artefactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Main Level/Asset_Button_Artifact");
        artefactIcon.transform.parent = artefactBeltObject.transform;
        artefactIcon.GetComponent<SpriteRenderer>().transform.localScale = Vector2.one;
        artefactIcon.GetComponent<SpriteRenderer>().sortingLayerName = "Tool UI";
        artefactIcon.GetComponent<SpriteRenderer>().sortingOrder = 6;
        artefactIcon.AddComponent<BoxCollider2D>();
        artefactIcon.transform.localPosition = new Vector2(0.5f, 0.93f);
    }

    private void DamageTool()
    {
        SoundManager.instance.PlayClipOneShot(toolDamageClip);
        switch (currentTool)
        {
            case Tool.Brush:
                brushDurability++;
                toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Brush_Break0" + brushDurability);
                toolBelt[(int)Tool.Brush].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Brush/Asset_Tool_Side_Brush_Break0" + brushDurability.ToString());
                break;
            case Tool.Dredge:
                dredgDurability++;
                toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_DredgeHose_Break0" + dredgDurability.ToString());
                toolBelt[(int)Tool.Dredge].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Hose/Asset_Tool_Side_DredgeHose_Break0" + dredgDurability.ToString());
                break;
            case Tool.Mattock:
                mattockDurability++;
                toolBelt[(int)Tool.Mattock].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Asset_Tool_Side_Mattock");
                toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Mattock_Break0" + mattockDurability.ToString());
                break;
            case Tool.Saw:
                sawDurability++;
                toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Handsaw_Break0" + sawDurability.ToString());
                toolBelt[(int)Tool.Saw].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Saw/Asset_Tool_Side_Handsaw_Break0" + sawDurability.ToString());
                break;
            default:
                Debug.LogError("Damage tool default reached!");
                break;
        }

        if(brushDurability >= 4)
        {
            SoundManager.instance.PlayClipOneShot(toolBreakClip);
            toolBroken = true;
        }

        if(dredgDurability >=4)
        {
            SoundManager.instance.PlayClipOneShot(toolBreakClip);
            toolBroken = true;
        }

        if(mattockDurability >= 4)
        {
            SoundManager.instance.PlayClipOneShot(toolBreakClip);
            toolBroken = true;
        }

        if(sawDurability >= 4)
        {
            SoundManager.instance.PlayClipOneShot(toolBreakClip);
            toolBroken = true;
        }

        if (toolBroken)
            failObject.SetActive(true);
    }
    /// <summary>
    /// Method to arrange the roots according to their neighbours, the implementation is a mess so it's probably best ignore this unless it needs fixed.
    /// </summary>
    private void ArrangeRoots()
    {
        for (int it = 0; it < numLayers; it++)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool rootUp = false;
                    bool rootDown = false;
                    bool rootLeft = false;
                    bool rootRight = false;
                    if(tiles[it, i, j] != null)
                    {

                        if (tiles[it, i, j].layer == LayerMask.NameToLayer("Root"))
                        { 
                            int upIndex = j;
                            int downIndex = j;
                            int leftIndex = i;
                            int rightIndex = i;

                            if(i > 0)
                            {
                                rightIndex = i - 1;
                            }
                            if (i < width - 1)
                            {
                                leftIndex = i + 1;
                            }

                            if(j > 0)
                            {
                                downIndex = j - 1;
                            }

                            if(j < height - 1)
                            {
                                upIndex = j + 1;
                            }

                            if (rightIndex != i)
                            {
                                if (tiles[it, rightIndex, j] != null && tiles[it, rightIndex, j].layer == LayerMask.NameToLayer("Root"))
                                {
                                    rootRight = true;
                                }
                            }

                            if(leftIndex != i)
                            {
                                if (tiles[it, leftIndex, j] != null && tiles[it, leftIndex, j].layer == LayerMask.NameToLayer("Root"))
                                {
                                    rootLeft = true;
                                }
                            }

                            if(upIndex != j)
                            {
                                if (tiles[it, i, upIndex] != null && tiles[it, i, upIndex].layer == LayerMask.NameToLayer("Root"))
                                {
                                    rootUp = true;
                                }
                            }

                            if(downIndex != j)
                            {
                                if (tiles[it, i, downIndex] != null && tiles[it, i, downIndex].layer == LayerMask.NameToLayer("Root"))
                                {
                                    rootDown = true;
                                }
                            }

                            TileLayers currentLayer = 5 - ((TileLayers)it);
                            //Place a centre root.
                            if (rootUp && rootDown && rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(4, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                            }
                            //Place a straight root up and down.
                            if (rootUp && rootDown && !rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(1, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                            }
                            //Place a straight root right and left.
                            if (!rootUp && !rootDown && rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(1, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 90f);
                            }
                            //Place an end root going right.
                            if (!rootUp && !rootDown && !rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(5, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 90);
                            }
                            //Place an end root going up.
                            if (!rootUp && rootDown && !rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(5, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                            }
                            //Place an end root going left.
                            if (!rootUp && !rootDown && rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(5, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), -90);
                            }
                            //Place an end root going down.
                            if (rootUp && !rootDown && !rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(5, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 180);
                            }
                            //Place a corner root going up and left.
                            if (rootUp && !rootDown && rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(2, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), -90);
                            }
                            //Place a corner root going up and right.
                            if (rootUp && !rootDown && !rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(2, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 180);
                            }
                            //Place a corner root going down and left.
                            if (!rootUp && rootDown && rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(2, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                            }
                            //Place a corner root going down and right.
                            if (!rootUp && rootDown && !rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(2, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 90);
                            }
                            //Place a T root going up, left and right.
                            if (rootUp && !rootDown && rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(3, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 180);
                            }
                            //Place a T root going up, down and left.
                            if (rootUp && rootDown && rootLeft && !rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(3, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), -90);
                            }
                            //Place a T root going up, down and right.
                            if (rootUp && rootDown && !rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(3, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                                tiles[it, i, j].transform.Rotate(new Vector3(0, 0, 1), 90);
                            }
                            //Place a T root going down, left and right.
                            if (!rootUp && rootDown && rootLeft && rootRight)
                            {
                                tiles[it, i, j].GetComponent<SpriteRenderer>().sprite = GameManager.instance.GetRoot(3, currentLayer.ToString());
                                tiles[it, i, j].transform.rotation = Quaternion.identity;
                            }

                        }
                    }


                }
            }
        }

        if (!initialRootsSetup)
            initialRootsSetup = true;
    }

    private void InitFailMessage()
    {
        failObject = new GameObject();
        failObject.name = "Fail Message Canvas";
        failObject.AddComponent<Canvas>();
        failObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        failObject.AddComponent<CanvasScaler>();
        failObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        failObject.AddComponent<GraphicRaycaster>();

        GameObject backgroundObject = new GameObject();
        backgroundObject.name = "Background";
        backgroundObject.transform.parent = failObject.transform;
        backgroundObject.transform.localPosition = Vector3.zero;
        backgroundObject.AddComponent<Image>();
        backgroundObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Backgrounds/blackBackground");
        bgAlpha = 0f;
        backgroundObject.GetComponent<Image>().color = new Vector4(0f, 0f, 0f, 0f);
        backgroundObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);

        GameObject imageObject = new GameObject();
        imageObject.name = "Message Box";
        imageObject.transform.parent = backgroundObject.transform;
        imageObject.AddComponent<Image>();
        imageObject.GetComponent<Image>().transform.localPosition = Vector3.zero;
        imageObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI Assets/Asset_Button_Stan 1");
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(412f, 110f);
        imageObject.SetActive(false);

        GameObject textObject = new GameObject();
        textObject.name = "Fail Message Text";
        textObject.transform.parent = imageObject.transform;
        textObject.transform.localPosition = Vector3.zero;
        textObject.AddComponent<Text>();
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 90f);
        textObject.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/Pixeled");
        textObject.GetComponent<Text>().resizeTextForBestFit = true;
        textObject.GetComponent<Text>().resizeTextMinSize = 1;
        textObject.GetComponent<Text>().resizeTextMaxSize = 40;
        textObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        if(GameManager.instance.GetCurrentLanguage() == "Eng")
            textObject.GetComponent<Text>().text = "Looks like you've broken a tool.\nYou should really be more careful whilst excavating lost artefacts in order to preserve them.\nWould you like to try again?".ToUpper();
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
            textObject.GetComponent<Text>().text = "On dirait que vous avez cassé un outil.\nVous devez vraiment être plus prudent lors de l'excavation d'objets perdus afin de les conserver.\nVoudriez-vous essayer à nouveau?".ToUpper();
        GameObject firstButton = new GameObject();
        firstButton.name = "Restart Button";
        firstButton.transform.parent = imageObject.transform;
        firstButton.transform.localPosition = new Vector2(-140f, -60f);
        firstButton.AddComponent<Button>();
        firstButton.AddComponent<Image>();
        firstButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/WorldMap Assets/Asset_Button_Back");
        firstButton.GetComponent<RectTransform>().sizeDelta = new Vector2(64f, 64f);
        firstButton.GetComponent<Button>().onClick.AddListener(RestartLevel);

        GameObject secondButton = new GameObject();
        secondButton.name = "Exit Button";
        secondButton.transform.parent = imageObject.transform;
        secondButton.transform.localPosition = new Vector2(140f, -60f);
        secondButton.AddComponent<Button>();
        secondButton.AddComponent<Image>();
        secondButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/WorldMap Assets/Asset_Button_X");
        secondButton.GetComponent<RectTransform>().sizeDelta = new Vector2(64f, 64f);
        secondButton.GetComponent<Button>().onClick.AddListener(GoToWorldMap);
        failObject.SetActive(false);
    }

    private void InitLayerGroups()
    {
        
        tilesHeader = new GameObject();
        tilesHeader.name = "--Tiles--";

        //Layer one group
        layerOneGroup = new GameObject();
        layerOneGroup.name = "--Layer One--";
        layerOneGroup.AddComponent<SortingGroup>();
        layerOneGroup.GetComponent<SortingGroup>().sortingOrder = 4;
        layerOneGroup.transform.parent = tilesHeader.transform;
        //Layer two group
        layerTwoGroup = new GameObject();
        layerTwoGroup.name = "--Layer Two--";
        layerTwoGroup.AddComponent<SortingGroup>();
        layerTwoGroup.GetComponent<SortingGroup>().sortingOrder = 3;
        layerTwoGroup.transform.parent = tilesHeader.transform;
        //Layer three group
        layerThreeGroup = new GameObject();
        layerThreeGroup.name = "--Layer Three--";
        layerThreeGroup.AddComponent<SortingGroup>();
        layerThreeGroup.GetComponent<SortingGroup>().sortingOrder = 2;
        layerThreeGroup.transform.parent = tilesHeader.transform;
        //Layer four group
        layerFourGroup = new GameObject();
        layerFourGroup.name = "--Layer Four--";
        layerFourGroup.AddComponent<SortingGroup>();
        layerFourGroup.GetComponent<SortingGroup>().sortingOrder = 1;
        layerFourGroup.transform.parent = tilesHeader.transform;
        //Layer five group
        layerFiveGroup = new GameObject();
        layerFiveGroup.name = "--Layer Five--";
        layerFiveGroup.AddComponent<SortingGroup>();
        layerFiveGroup.GetComponent<SortingGroup>().sortingOrder = 0;
        layerFiveGroup.transform.parent = tilesHeader.transform;
    }
    
    private void RestartLevel()
    {
        if (!GameManager.instance.IsInTransition())
        {
            SoundManager.instance.PlayClip(buttonSound);
            SoundManager.instance.StopMusic();
            Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
            if (canvasCheck.Length > 0)
            {
                for (int i = 0; i < canvasCheck.Length; i++)
                {
                        canvasCheck[i].enabled = false;
                }
            }
            GameManager.instance.changeScene((int)GameManager.Scene.Level);
        }
    }

    private void GoToWorldMap()
    {
        if (!GameManager.instance.IsInTransition())
        {
            SoundManager.instance.PlayClip(buttonSound);
            SoundManager.instance.StopMusic();
            Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
            if (canvasCheck.Length > 0)
            {
                for (int i = 0; i < canvasCheck.Length; i++)
                {
                    canvasCheck[i].enabled = false;
                    GameManager.instance.changeScene((int)GameManager.Scene.WorldMapScene);
                }
            }
        }
    }

    private void PerformTutorial()
    {
        if(tutorialObject != null)
        {
            RenderTexture.active = textRend;
            Camera.main.targetTexture = textRend;
            
            if (tutorialSection == 2)
            {
                if (newSection)
                {
                    tutorialObject.transform.GetChild(0).gameObject.SetActive(true);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(1.2f, 1.2f);
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "This is your excavation site hidden under water, these sites are left from crumbled Crannogs from a time long ago.\nIt is suspected that there are hidden artefacts in each different location that we've uncovered.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                     tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "C'est votre site d'excavation caché sous l'eau, ces sites sont laissés par des Crannogs émiettés il y a longtemps.\nOn soupçonne qu'il y a des artefacts cachés dans chaque endroit différent que nous avons découvert.".ToUpper();
                    newSection = false;
                }

            }//Dig site section.
            else if (tutorialSection == 3)
            {
                if (newSection)
                {
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 55f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-30f, 131f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = Vector3.one / 2;
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-22f, 171f);
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Each site has multiple layers that artefacts can be hidden.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Chaque site possède plusieurs couches dont les artefacts peuvent être masqués.".ToUpper();
                    newSection = false;
                }
            }//Layer explanation section.
            else if (tutorialSection == 4)
            {
                if (!delayFinished)
                {
                    if (newSection)
                    {
                        if(GameManager.instance.GetCurrentLanguage() == "Eng")
                            tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Lets peel back these layers from top to bottom.".ToUpper();
                        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                            tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Décollons ces couches de haut en bas.".ToUpper();
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        tutorialObject.transform.GetChild(1).gameObject.SetActive(true);
                        newSection = false;
                    }

                    if (!delayStarted)
                        StartCoroutine(Delay(3f));
                }
                if (delayFinished)
                {

                    if (!layerRemoveStarted)
                    {
                        StartCoroutine(RemoveLayers());
                        tutorialObject.transform.GetChild(1).gameObject.SetActive(false);
                        tutorialObject.transform.GetChild(0).gameObject.SetActive(false);
                    }

                    switch (layerToRemove)
                    {
                        case 0:
                            break;
                        case 1:
                            tilesHeader.transform.GetChild(0).gameObject.SetActive(false);
                            break;
                        case 2:
                            tilesHeader.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                        case 3:
                            tilesHeader.transform.GetChild(2).gameObject.SetActive(false);
                            break;
                        case 4:
                            tilesHeader.transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        case 5:
                            tilesHeader.transform.GetChild(4).gameObject.SetActive(false);
                            break;
                        case 6:
                            tutorialSection++;
                            newSection = true;
                            break;
                        default:
                            Debug.LogError("Default reached in Level.cs tutorial method, section 4,");
                            break;
                    }
                }
            }//Layer peel section.
            else if (tutorialSection == 5)
            {
                if (newSection)
                {
                    tilesHeader.transform.GetChild(0).gameObject.SetActive(true);
                    tilesHeader.transform.GetChild(1).gameObject.SetActive(true);
                    tilesHeader.transform.GetChild(2).gameObject.SetActive(true);
                    tilesHeader.transform.GetChild(3).gameObject.SetActive(true);
                    tilesHeader.transform.GetChild(4).gameObject.SetActive(true);
                    tutorialObject.transform.GetChild(1).gameObject.SetActive(true);
                    tutorialObject.transform.GetChild(0).gameObject.SetActive(true);
                    delayStarted = false;
                    delayFinished = false;

                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-436f, 225f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.8f, 0.8f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-224f, 120f);
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "to find these artefacts you must remove the layers hiding them and to do that you'll need tools.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "pour trouver ces artefacts, vous devez supprimer les couches qui les cachent et pour ce faire, vous aurez besoin d'outils.".ToUpper();
                    currentTool = Tool.Saw;

                    newSection = false;
                }

                if (toolBeltMoving)
                {
                    if (toolBeltActive)
                    {
                        toolBeltObject.transform.localPosition = Vector3.Lerp(toolBeltObject.transform.localPosition, beltOpenPosition, beltTravelSpeed * Time.deltaTime);
                        if (toolBeltObject.transform.localPosition.x > beltOpenPosition.x - 0.05f)
                        {
                            toolBeltMoving = false;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        if (toolBeltActive && clickedObject.layer == toolLayer)
                        {
                            if (clickedObject.name == "Brush")
                            {
                                if (currentTool != Tool.Brush)
                                {
                                    previousTool = currentTool;
                                    currentTool = Tool.Brush;
                                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Brush_Break0" + brushDurability);
                                    toolBelt[(int)Tool.Brush].SetActive(false);
                                    SoundManager.instance.PlayClipOneShot(brushSelect);
                                    //Outline box
                                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 400f);
                                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(350f, 0f);
                                    //Text box
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.8f, 0.8f);
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(40f, -144f);
                                    //Text
                                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Your selected tool will be shown within this box so you don't forget.".ToUpper();
                                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Votre outil sélectionné sera affiché dans cette boîte afin que vous ne l'oubliez pas".ToUpper();
                                    //Button
                                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                                }
                            }

                        }

                        if (clickedObject.name == "Tool Tab" && !toolBeltActive)
                        {
                            if (!toolBeltMoving)
                            {
                                toolBeltMoving = true;
                                toolBeltActive = !toolBeltActive;
                                if (toolBeltActive)
                                {
                                    artefactIcon.SetActive(false);
                                    artefactBeltObject.SetActive(false);
                                    toolIcon.SetActive(true);

                                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 74f);
                                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-390f, 145f);
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-350f, 60f);
                                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "This is your tool belt, select the brush.".ToUpper();
                                    if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Ceci est votre ceinture à outils, sélectionnez la brosse.".ToUpper();
                                }
                            }
                        }


                    }
                }
            }//Tool belt and brush select section.
            else if (tutorialSection == 6)
            {
                if (newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(52f, 52f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-165f, 128f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-135f, 38f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Try removing this section, the brush can remove dirt.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Essayez de retirer cette section, la brosse peut enlever la saleté.".ToUpper();
                    //Button
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    newSection = false;
                    exampleTile = tiles[3, 6, 6];
                }

                TileHit();
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        if (clickedObject == exampleTile)
                        {
                            if (exampleTile == tiles[4, 5, 6])
                            {
                                if (brushDurability == 0)
                                {
                                    SoundManager.instance.PlayClip(toolDamageClip);
                                    brushDurability++;
                                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Brush_Break0" + brushDurability);
                                    //Outline box
                                    tutorialObject.transform.GetChild(0).gameObject.SetActive(false);
                                    //Text box
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(1f, 1f);
                                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(0f, 0f);
                                    //Text
                                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oops, see? If you're not careful whilst excavating you can damage your tools.\nIf you lose a tool then you will not be able to continue excavating".ToUpper();
                                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oups, tu vois? Si vous ne faites pas attention lors de l'excavation, vous pouvez endommager vos outils.\nSi vous perdez un outil, vous ne pourrez pas continuer à creuser".ToUpper();
                                    //Button
                                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);

                                    toolBelt[(int)Tool.Brush].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Belt/Brush/Asset_Tool_Side_Brush_Break0" + brushDurability);
                                }
                            }

                            Vector3Int arrayPosition = GetObjectArrayPosition(clickedObject);
                            if (exampleTile == tiles[3, 6, 6])
                            {
                                PerformDig(arrayPosition, clickedObject);
                                exampleTile = tiles[4, 5, 6];

                                //Outline box
                                tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(52f, 52f);
                                tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-110.7f, 131f);
                                //Text box
                                tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
                                tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-87f, 38f);
                                //Text
                                if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Nice, each tool can remove a certain type of section.\nNow try clearing this one".ToUpper();
                                else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Bien, chaque outil peut supprimer un certain type de section.\nMaintenant essayez d'effacer celui-ci".ToUpper();
                                //Button
                                tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                            }


                        }
                    }
                }
            }//Digging section.
            else if (tutorialSection == 7)
            {
                if (newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.SetActive(true);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 80f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-390f, 28f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-161f, 17f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "The dredge hose can remove sand.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Le tuyau de dragage peut éliminer le sable.".ToUpper();
                    newSection = false;
                }
            }//Dredge hose description section.
            else if (tutorialSection == 8)
            {
                if (newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 80f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-390f, -85f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-161f, -80f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "The saw can remove roots.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "La scie peut enlever les racines.".ToUpper();
                    newSection = false;
                }
            }//Saw description section.
            else if (tutorialSection == 9)
            {
                if (newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 104f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-390f, -205f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-161f, -183f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "The mattock can remove stone.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "La pioche peut enlever la pierre.".ToUpper();
                    newSection = false;
                }
            }//Mattock description section.
            else if (tutorialSection == 10)
            {
                if (newSection)
                {
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Try selecting the mattock.".ToUpper();
                    if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Essayez de sélectionner la pioche.".ToUpper();
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    newSection = false;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        if (toolBeltActive && clickedObject.layer == toolLayer)
                        {
                            if (clickedObject.name == "Mattock")
                            {
                                if (currentTool != Tool.Mattock)
                                {
                                    previousTool = currentTool;
                                    currentTool = Tool.Mattock;
                                    toolUsed.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Tools/Current/Asset_Tool_Mattock_Break0" + mattockDurability);
                                    toolBelt[(int)Tool.Brush].SetActive(true);
                                    toolBelt[(int)Tool.Mattock].SetActive(false);
                                    SoundManager.instance.PlayClipOneShot(mattockSelect);
                                    tutorialSection++;
                                    newSection = true;
                                }
                            }

                        }
                    }
                }

            }//Mattock select section.
            else if (tutorialSection == 11)
            {
                if (newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(52f, 52f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-110.7f, 131f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-87f, 38f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Now try hitting this section again.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Essayez à nouveau d'appuyer sur cette section".ToUpper();
                    //Button
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    newSection = false;
                    exampleTile = tiles[4, 5, 6];
                    Destroy(tiles[3, 5, 6]);
                    tiles[3, 5, 6] = Instantiate(GameManager.instance.GetArtefactFinal((int)GameManager.Tile.Artefact, 1), bottomRightOrigin + new Vector3(-5, 6, 3), Quaternion.identity) as GameObject;
                    tiles[3, 5, 6].transform.parent = tilesHeader.transform.GetChild(2).transform;
                }

                if(delayStarted && delayFinished)
                {
                    delayStarted = false;
                    delayFinished = false;
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oh, you've found an artefact piece.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oh, vous avez trouvé un objet façonné.".ToUpper();
                    tutorialObject.transform.GetChild(0).gameObject.SetActive(true);
                    //Button
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                }

                TileHit();
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        Vector3Int arrayPosition = GetObjectArrayPosition(clickedObject);
                        if (clickedObject == exampleTile)
                        {
                            
                            if (exampleTile == tiles[4, 5, 6])
                            {
                                PerformDig(arrayPosition, clickedObject);
                                if(!delayStarted)
                                {
                                    StartCoroutine(Delay(3f));
                                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "See? no problem whilst using the correct tool.".ToUpper();
                                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Voir? aucun problème lors de l'utilisation du bon outil.".ToUpper();

                                    tutorialObject.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                
                            }
                        }
                    }
                }
            }//Mattock dig and artefact reveal section.
            else if (tutorialSection == 12)
            {
                if (newSection)
                {
                    delayStarted = false;
                    delayFinished = false;

                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-295f, 225f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.8f, 0.8f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-224f, 120f);
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Now that you've found an artefact piece you'll need to store it in your bag until you can take it back to the museum, close your tool belt.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Maintenant que vous avez trouvé une pièce d'artéfact, vous devrez la stocker dans votre sac jusqu'à ce que vous puissiez la rapporter au musée, fermez votre ceinture à outils".ToUpper();
                    //Button
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    newSection = false;
                }

                if (toolBeltMoving)
                {
                    if (!toolBeltActive)
                    {
                        toolBeltObject.transform.localPosition = Vector3.Lerp(toolBeltObject.transform.localPosition, beltClosedPosition, beltTravelSpeed * Time.deltaTime);
                        if (toolBeltObject.transform.localPosition.x < beltClosedPosition.x + 0.05f)
                        {
                            toolBeltMoving = false;
                            artefactIcon.SetActive(true);
                            artefactBeltObject.SetActive(true);
                            tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                            tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-436.5f, 168f);
                            tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(0.5f, 0.5f);
                            tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(-289f, 80f);
                            if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Now open your artefact storage.".ToUpper();
                            else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Ouvrez maintenant votre stockage d'artefacts.".ToUpper();
                        }
                    }
                }

                if (artefactBeltMoving)
                {
                    if (artefactBeltActive)
                    {
                        artefactBeltObject.transform.localPosition = Vector3.Lerp(artefactBeltObject.transform.localPosition, beltOpenPosition, beltTravelSpeed * Time.deltaTime);
                        if (artefactBeltObject.transform.localPosition.x > beltOpenPosition.x - 0.05f)
                        {
                            artefactBeltMoving = false;
                            //Outline box
                            tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(52f, 52f);
                            tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-110.7f, 131f);
                            //Text box
                            tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(1f, 1f);
                            tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(0f, -150f);
                            //Text
                            if(GameManager.instance.GetCurrentLanguage() == "Eng")
                                tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "This is where you'll store your pieces until you've found all of them in an actual digsite.\nGo pick up the artefact and drag it onto one of the open slots.".ToUpper();
                            else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                                tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "C'est là que vous stockerez vos pièces jusqu'à ce que vous les ayez toutes trouvées dans un véritable site de fouille.\nAllez ramasser l'artefact et faites-le glisser sur l'un des emplacements ouverts".ToUpper();
                            newSection = true;
                            tutorialSection++;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        
                        if (clickedObject.name == "Tool Tab")
                        {
                            if (toolBeltActive)
                            {
                                toolBeltMoving = true;
                                toolBeltActive = false;
                                if (toolBeltActive)
                                {
                                    artefactIcon.SetActive(false);
                                    artefactBeltObject.SetActive(false);
                                    toolIcon.SetActive(true);
                                }
                            }
                        }

                        if (clickedObject.name == "Artefact Tab")
                        {
                            if (!artefactBeltActive)
                            {
                                artefactBeltMoving = true;
                                artefactBeltActive = !artefactBeltActive;
                                if (artefactBeltActive)
                                {
                                    artefactIcon.SetActive(true);
                                    toolIcon.SetActive(false);
                                    toolBeltObject.SetActive(false);
                                }
                            }

                        }

                    }

                }

                
            }//Belt switching sections.
            else if (tutorialSection == 13)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
                    Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

                    RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
                    if (hit2D.collider != null)//Checks if ray has collided.
                    {
                        GameObject clickedObject = hit2D.collider.gameObject;//Gets the object that has been hit.
                        
                        if (clickedObject.layer == LayerMask.NameToLayer("Artefact"))
                        {
                            if (artefactBeltActive && selectedArtefact == null)
                            {
                                artefactSelected = true;
                                selectedArtefact = clickedObject;
                                initialArtefactPosition = selectedArtefact.transform.position;
                                initialSortingOrder = selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder;
                                selectedArtefact.GetComponent<SpriteRenderer>().sortingLayerName = "Artefact";
                                selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder = 7;
                                selectedArtefact.layer = 2;
                                initialParent = selectedArtefact.transform.parent;
                                selectedArtefact.transform.parent = null;
                            }

                        }

                    }

                }

                if (artefactSelected)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (selectedArtefact != null)
                        {
                            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            selectedArtefact.transform.position = mousePos;

                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (artefactSelected)
                        {
                            bool nodeHit = false;
                            if (selectedArtefact != null)
                            {
                                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);//Raycast from mouse position in a straight line.
                                if (hit2D.collider != null)//Checks if ray has collided.
                                {
                                    for (int i = 0; i < totalArtefactPieces; i++)
                                    {
                                        if (hit2D.collider.gameObject == artefactNodes[i].NodeObject)
                                        {
                                            if (!artefactNodes[i].IsOccupied)
                                            {
                                                selectedArtefact.transform.parent = artefactNodes[i].NodeObject.transform;
                                                selectedArtefact.transform.localPosition = Vector2.zero;
                                                artefactNodes[i].IsOccupied = true;
                                                selectedArtefact.GetComponent<BoxCollider2D>().enabled = false;
                                                artefactPiecesCollected++;
                                                nodeHit = true;
                                                newSection = true;
                                                tutorialSection++;
                                            }
                                        }

                                    }
                                }

                                if (!nodeHit)
                                {
                                    selectedArtefact.transform.position = initialArtefactPosition;
                                    selectedArtefact.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                                    selectedArtefact.GetComponent<SpriteRenderer>().sortingOrder = initialSortingOrder;
                                    selectedArtefact.layer = LayerMask.NameToLayer("Artefact");
                                    selectedArtefact.transform.parent = initialParent;
                                }

                                artefactSelected = false;
                                selectedArtefact = null;
                            }
                        }
                    }
                }
            }//Artefact storage section.
            else if (tutorialSection == 14)
            {
                if(newSection)
                {
                    //Outline box
                    tutorialObject.transform.GetChild(0).gameObject.SetActive(false);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(46f, 46f);
                    tutorialObject.transform.GetChild(0).gameObject.GetComponent<Image>().transform.localPosition = new Vector2(-140f, 111f);
                    //Text box
                    tutorialObject.transform.GetChild(1).gameObject.transform.localScale = new Vector2(1.5f, 1.5f);
                    tutorialObject.transform.GetChild(1).gameObject.transform.localPosition = new Vector2(0f, 0f);
                    //Text
                    if(GameManager.instance.GetCurrentLanguage() == "Eng")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "You'll need to find all of the pieces to finish a real digsite but i'll let you off with it this time, go forward and find lost history hidden under the water.".ToUpper();
                    else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                        tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Vous aurez besoin de trouver toutes les pièces pour terminer un vrai site de fouille, mais je vais vous laisser partir cette fois, aller de l'avant et retrouver l'histoire perdue cachée sous l'eau.".ToUpper();
                    //Button
                    tutorialObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    newSection = false;
                }
            }//End section.
            else if(tutorialSection == 15)
            {
                if (!GameManager.instance.IsInTransition())
                {
                    SoundManager.instance.PlayClip(buttonSound);
                    SoundManager.instance.StopMusic();
                    Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
                    if (canvasCheck.Length > 0)
                    {
                        for (int i = 0; i < canvasCheck.Length; i++)
                        {
                            canvasCheck[i].enabled = false;
                        }
                    }
                    GameManager.instance.changeScene((int)GameManager.Scene.WorldMapScene);
                    GameManager.instance.SetTutorialComplete(1);
                }
            }//Go to world map.
        }
    }

    private void InitTutorial()
    {
        tutorialObject.name = "Tutorial Canvas";
        tutorialObject.AddComponent<Canvas>();
        tutorialObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        tutorialObject.AddComponent<CanvasScaler>();
        tutorialObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        tutorialObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
        tutorialObject.AddComponent<GraphicRaycaster>();

        GameObject outlineObject = new GameObject();
        outlineObject.name = "Outline Box";
        outlineObject.transform.parent = tutorialObject.transform;
        outlineObject.AddComponent<Image>();
        outlineObject.GetComponent<Image>().transform.localPosition = new Vector2(-25f, 0f);
        outlineObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Main Level/OutlineBox");
        outlineObject.GetComponent<RectTransform>().sizeDelta = new Vector2(420f, 420f);
        outlineObject.SetActive(false);


        GameObject imageObject = new GameObject();
        imageObject.name = "Message Box";
        imageObject.transform.parent = tutorialObject.transform;
        imageObject.AddComponent<Image>();
        imageObject.GetComponent<Image>().transform.localPosition = new Vector2(90f, -100f);
        imageObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI Assets/Asset_Button_Stan 1");
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(412f, 110f);

      
        GameObject textObject = new GameObject();
        textObject.name = "Tutorial Text";
        textObject.transform.parent = imageObject.transform;
        textObject.transform.localPosition = Vector3.zero;
        textObject.AddComponent<Text>();
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 90f);
        textObject.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/Pixeled");
        textObject.GetComponent<Text>().resizeTextForBestFit = true;
        textObject.GetComponent<Text>().resizeTextMinSize = 1;
        textObject.GetComponent<Text>().resizeTextMaxSize = 40;
        textObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        if(GameManager.instance.GetCurrentLanguage() == "Eng")
            textObject.GetComponent<Text>().text = "Looks like this is your first time at a dig site.\nLet me show you how this works.".ToUpper();
        if (GameManager.instance.GetCurrentLanguage() == "Fr")
            textObject.GetComponent<Text>().text = "On dirait que c'est votre première fois sur un site de fouille.\nPermettez-moi de vous montrer comment cela fonctionne.".ToUpper();
        GameObject firstButton = new GameObject();
        firstButton.name = "Continue Button";
        firstButton.transform.parent = imageObject.transform;
        firstButton.transform.localPosition = new Vector2(205f, 50f);
        firstButton.AddComponent<Button>();
        firstButton.AddComponent<Image>();
        firstButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/WorldMap Assets/Asset_Button_Back");
        firstButton.GetComponent<RectTransform>().sizeDelta = new Vector2(64f, 64f);
        firstButton.GetComponent<Button>().onClick.AddListener(ProgressTutorial);

        
    }

    private void ProgressTutorial()
    {
        
        if(tutorialSection < 15)
        {
            SoundManager.instance.PlayClip(buttonSound);
            tutorialSection++;
            newSection = true;
        }
    }

    private IEnumerator Delay(float delayTime)
    {
        delayStarted = true;

        yield return new WaitForSeconds(delayTime);

        delayFinished = true;
    }

    private IEnumerator RemoveLayers()
    {
        layerRemoveStarted = true;
        layerRemoveFinshed = false;

        yield return new WaitForSeconds(2f);

        if (layerToRemove < 6)
        {
            layerRemoveFinshed = true;
            layerRemoveStarted = false;
            layerToRemove++;
        }

    }
}
      