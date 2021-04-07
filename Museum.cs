using UnityEngine;
using UnityEngine.UI;

public class Museum : MonoBehaviour
{
    /// <summary>
    /// The artefact being examined.
    /// </summary>
    private GameObject currentArtefact;
    /// <summary>
    /// If there's an artefact being examined.
    /// </summary>
    private bool isExamine;
    /// <summary>
    /// The speed that the artefact can rotate.
    /// </summary>
    public float rotationSpeed;

    /// <summary>
    /// Position artefact one goes on the screen.
    /// </summary>
    private Vector2 positionOne;
    /// <summary>
    /// Position artefact two goes on the screen.
    /// </summary>
    private Vector2 positionTwo;
    /// <summary>
    /// Position artefact three goes on the screen.
    /// </summary>
    private Vector2 positionThree;
    /// <summary>
    /// Position artefact four goes on the screen.
    /// </summary>
    private Vector2 positionFour;
    /// <summary>
    /// Position artefact five goes on the screen.
    /// </summary>
    private Vector2 positionFive;

    /// <summary>
    /// The initial position the artefact started at, i.e, positionOne
    /// </summary>
    private Vector3 initialPosition;
    /// <summary>
    /// The initial scale of an artefact before being examined.
    /// </summary>
    private Vector3 initialScale;
    /// <summary>
    /// The initial rotation of an artefact before being examined.
    /// </summary>
    private Quaternion initialRotation;
    /// <summary>
    /// The location artefacts go to be examined.
    /// </summary>
    private Vector3 inspectLocation;

    private float animationScale = 1.0f;
    bool goingUp = false;
    bool animationPaused = false;

    [Header("Sprites")]//The sprites required for the level.
    [SerializeField] private Sprite backgroundSprite = null;
    [SerializeField] private Sprite backgroundSpriteSelected = null;
    [SerializeField] private Sprite backButtonSprite = null;
    [SerializeField] private Sprite demoButtonSprite = null;
    [SerializeField] private Sprite worldButtonSprite = null;

    [Header("Sounds")]
    [SerializeField] private AudioClip buttonSound = null;
    [SerializeField] private AudioClip backgroundMusic = null;
    //Button objects
    GameObject worldButton;
    GameObject backButton;
    GameObject demoButton;
    /// <summary>
    /// The text that displays artefact information.
    /// </summary>
    private GameObject artefactInformation;
    private Vector2 informationStartPosition;
    private float maxScroll;
    private GameObject artefactName;
    private GameObject animationObject_Knife;
    private GameObject animationObject_Lyre;
    private GameObject animationObject_Forge;
    private GameObject animationObject_Butterdish;
    private GameObject animationObject_Loom;
    bool renderersDisabled;
    Vector3 initialTouchLocation;
    bool scrollingThroughText;

    private Vector2 nameTextPosition;
    private Vector2 infoTextPosition;
    [SerializeField]
    private float scrollSpeed;
    // Start is called before the first frame update
    void Start()
    {
        isExamine = false;

        rotationSpeed = 20f;
        initialPosition = Vector3.zero;
        initialScale = Vector3.one;
        initialRotation = Quaternion.identity;
        inspectLocation = new Vector3(-5, 0, 1);
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Background");
        gameObject.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
        gameObject.GetComponent<SpriteRenderer>().transform.localPosition = new Vector3(0, 0, 5f);

        //Get the size of the background sprite and scale it to the viewport
        gameObject.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one;
        double swidth = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        double sheight = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        gameObject.GetComponent<SpriteRenderer>().transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));

        positionOne = new Vector2(-1.92f, -0.5f);
        positionTwo = new Vector2(-0.65f, -0.67f);
        positionThree = new Vector2(0.64f, -0.5f);
        positionFour = new Vector2(1.92f, -0.5f);
        positionFive = new Vector2(1.92f, 0.8f);
        
       // initArtefact("Skull", "Models/skull", positionTwo, 0.2f);


        Canvas levelCanvas;
        GameObject myObject;
        //Canvas
        myObject = new GameObject
        {
            name = "Canvas"
        };
        myObject.AddComponent<Canvas>();

        levelCanvas = myObject.GetComponent<Canvas>();
        levelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        levelCanvas.GetComponent<RectTransform>().position = Vector3.zero;
        myObject.AddComponent<CanvasScaler>();
        myObject.AddComponent<GraphicRaycaster>();
        levelCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        levelCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        
        if (GameManager.instance.IsArtefactUnlocked((int)GameManager.Artefacts.Knife) == 1)
        {
            if (GameManager.instance.GetCurrentLanguage() == "Eng")
                animationObject_Knife = InitAnimationObject("Knife", "Sprites/Museum/Knife Animation/Dagger_Animation_Frame01", positionOne, 0.3f);
            else if (GameManager.instance.GetCurrentLanguage() == "Fr")
                animationObject_Knife = InitAnimationObject("Couteau", "Sprites/Museum/Knife Animation/Dagger_Animation_Frame01", positionOne, 0.3f);
        }

        if (GameManager.instance.IsArtefactUnlocked((int)GameManager.Artefacts.Butterdish) == 1)
        {
            if(GameManager.instance.GetCurrentLanguage() == "Eng")
                animationObject_Butterdish = InitAnimationObject("Butterdish", "Sprites/Museum/Butterdish Animation/Butterdish_Animation_Frame01", positionTwo, 0.3f);
            else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                animationObject_Butterdish = InitAnimationObject("Beurrier", "Sprites/Museum/Butterdish Animation/Butterdish_Animation_Frame01", positionTwo, 0.3f);
        }

        if (GameManager.instance.IsArtefactUnlocked((int)GameManager.Artefacts.Textile) == 1)
        {
            if (GameManager.instance.GetCurrentLanguage() == "Eng")
                animationObject_Loom = InitAnimationObject("Loom", "Sprites/Museum/Textile Animation/Textile_Animation_Frame01", positionThree, 0.3f);
            else if (GameManager.instance.GetCurrentLanguage() == "Fr")
                animationObject_Loom = InitAnimationObject("Métier à tisser", "Sprites/Museum/Textile Animation/Textile_Animation_Frame01", positionThree, 0.3f);
        }
        
        if (GameManager.instance.IsArtefactUnlocked((int)GameManager.Artefacts.Ore) == 1)
        {
            if (GameManager.instance.GetCurrentLanguage() == "Eng")
                animationObject_Forge = InitAnimationObject("Forge", "Sprites/Museum/Ores Animation/Ores_Animation_Frame01", positionFour, 0.3f);
            else if (GameManager.instance.GetCurrentLanguage() == "Fr")
                animationObject_Forge = InitAnimationObject("Minerai", "Sprites/Museum/Ores Animation/Ores_Animation_Frame01", positionFour, 0.3f);
        }
        
        if (GameManager.instance.IsArtefactUnlocked((int)GameManager.Artefacts.Lyre) == 1)
        {
            animationObject_Lyre = InitAnimationObject("Lyre", "Sprites/Museum/Lyre Animation/Lyre_Animation_Frame01", positionFive, 0.3f);
        }

        renderersDisabled = false;
        //Setup the text box for information.
        artefactInformation = new GameObject();
        artefactInformation.AddComponent<Text>();
        artefactInformation.name = "Artefact Info";
        artefactInformation.gameObject.transform.SetParent(levelCanvas.transform, true);
       
        artefactInformation.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(840, 900);
        artefactInformation.gameObject.GetComponent<Text>().text = GameManager.instance.readTextFile("");
        artefactInformation.gameObject.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/din1451alt");
        artefactInformation.gameObject.GetComponent<Text>().color = Color.white;
        if(GameManager.instance.GetCurrentLanguage() == "Eng")
        {
            artefactInformation.gameObject.GetComponent<Text>().fontSize = 55;
            artefactInformation.gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
            artefactInformation.gameObject.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            artefactInformation.gameObject.GetComponent<Text>().resizeTextForBestFit = true;
            artefactInformation.gameObject.GetComponent<Text>().resizeTextMaxSize = 60;
            artefactInformation.gameObject.GetComponent<Text>().resizeTextMinSize = 55;

        }
        else
        {
            artefactInformation.gameObject.GetComponent<Text>().fontSize = 42;
            artefactInformation.gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
            artefactInformation.gameObject.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            artefactInformation.gameObject.GetComponent<Text>().resizeTextForBestFit = false;
        }

        artefactInformation.gameObject.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
        infoTextPosition = new Vector2(503, -97);
        artefactInformation.transform.position = infoTextPosition;
        artefactInformation.SetActive(false);
        artefactInformation.AddComponent<BoxCollider>();
        artefactInformation.GetComponent<BoxCollider>().size = artefactInformation.gameObject.GetComponent<RectTransform>().sizeDelta;
        //Setup the text box for name.
        //Setup the text box.
        artefactName = new GameObject();
        artefactName.AddComponent<Text>();
        artefactName.name = "Artefact Name";
        artefactName.gameObject.transform.SetParent(levelCanvas.transform, true);
        artefactName.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(841, 189);
        artefactName.gameObject.GetComponent<Text>().text = GameManager.instance.readTextFile("");
        artefactName.gameObject.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/din1451alt");
        artefactName.gameObject.GetComponent<Text>().color = Color.white;
        artefactName.gameObject.GetComponent<Text>().fontSize = 200;
        artefactName.gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
        artefactName.gameObject.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Truncate;
        artefactName.gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        artefactName.gameObject.GetComponent<Text>().resizeTextForBestFit = true;
        artefactName.gameObject.GetComponent<Text>().resizeTextMaxSize = 100;
        artefactName.gameObject.GetComponent<Text>().resizeTextMinSize = 10;
        nameTextPosition = new Vector2(503, 440);
        artefactName.transform.position = nameTextPosition;
        artefactName.SetActive(false);

        initialTouchLocation = Vector3.zero;
        scrollingThroughText = false;
        scrollSpeed = 5.0f;
        maxScroll = 500f;
        worldButton = new GameObject();
        worldButton.transform.parent = levelCanvas.transform;
        worldButton.AddComponent<Button>();
        worldButton.name = "World Button";
        worldButton.gameObject.SetActive(true);
        worldButton.gameObject.AddComponent<Image>();
        worldButton.GetComponent<Image>().sprite = this.worldButtonSprite;
        worldButton.transform.localPosition = new Vector2(-851, 430);
        worldButton.GetComponent<RectTransform>().sizeDelta = new Vector2(this.worldButtonSprite.texture.width, this.worldButtonSprite.texture.height);
        worldButton.GetComponent<Button>().onClick.AddListener(GoToLevelSelect);
        worldButton.SetActive(true);
        worldButton.AddComponent<ButtonScript>();

        demoButton = new GameObject();
        demoButton.transform.parent = levelCanvas.transform;
        demoButton.AddComponent<Button>();
        demoButton.name = "Demo Button";
        demoButton.gameObject.SetActive(true);
        demoButton.gameObject.AddComponent<Image>();
        demoButton.GetComponent<Image>().sprite = this.demoButtonSprite;
        demoButton.transform.localPosition = new Vector2(-40f, -430f);
        demoButton.GetComponent<RectTransform>().sizeDelta = new Vector2(this.demoButtonSprite.texture.width, this.demoButtonSprite.texture.height);
        demoButton.GetComponent<Button>().onClick.AddListener(GoToDemo);
        demoButton.SetActive(false);
        demoButton.AddComponent<ButtonScript>();

        backButton = new GameObject();
        backButton.transform.parent = levelCanvas.transform;
        backButton.AddComponent<Button>();
        backButton.name = "Back Button";
        backButton.gameObject.SetActive(true);
        backButton.gameObject.AddComponent<Image>();
        backButton.GetComponent<Image>().sprite = this.backButtonSprite;
        backButton.transform.localPosition = new Vector2(-851, 430);
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(this.backButtonSprite.texture.width, this.backButtonSprite.texture.height);
        backButton.GetComponent<Button>().onClick.AddListener(CloseInfo);
        backButton.SetActive(false);
        backButton.AddComponent<ButtonScript>();

        
    }

    
    void Update()
    {
        if (!GameManager.instance.IsInTransition())
            SoundManager.instance.PlayMusic(backgroundMusic);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
            RaycastHit hit;
            Physics.Raycast(mousePos, Vector3.forward, out hit);

            if (hit.collider != null)
            {
                if (currentArtefact == null)
                {//If an artefact has been hit and there is not already an artefact being examined.
                    currentArtefact = hit.collider.gameObject;
                    isExamine = true;
                    initialPosition = currentArtefact.transform.localPosition;
                    initialScale = currentArtefact.transform.localScale;
                    initialRotation = currentArtefact.transform.rotation;

                    currentArtefact.transform.position = inspectLocation;
                    currentArtefact.transform.localScale = Vector3.one / 1.8f;
                    currentArtefact.GetComponent<Animator>().speed = 0.5f;
                    gameObject.GetComponent<SpriteRenderer>().sprite = backgroundSpriteSelected;//Change background sprite to focus on the artefact.
                    artefactInformation.SetActive(true);//Activates the text box when examining.
                    artefactInformation.gameObject.GetComponent<Text>().text = GameManager.instance.ReadInfoFile(currentArtefact.name);//Gets the information for the artefact.

                    artefactName.SetActive(true);//Activates the text box when examining.
                    artefactName.gameObject.GetComponent<Text>().text = currentArtefact.name.ToUpper();

                    demoButton.SetActive(true);
                    backButton.SetActive(true);
                    worldButton.SetActive(false);
                }
            }

            if (!scrollingThroughText && isExamine)
            {
                initialTouchLocation = mousePos;
                scrollingThroughText = true;
            }


        }
        

        if (Input.GetMouseButton(0))
        {
            if (scrollingThroughText)
            {
                if(initialTouchLocation != Vector3.zero)
                {
                    Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if(currentPosition.y > initialTouchLocation.y)
                    {
                        if(artefactInformation.transform.localPosition.y < infoTextPosition.y + maxScroll)
                        {
                            artefactInformation.transform.localPosition += new Vector3(0, scrollSpeed, 0);
                            artefactName.transform.localPosition += new Vector3(0, scrollSpeed, 0);
                        }
                       
                    }
                    else if(currentPosition.y < initialTouchLocation.y)
                    {
                        if(artefactInformation.transform.localPosition.y > infoTextPosition.y)
                        {
                            artefactInformation.transform.localPosition -= new Vector3(0, scrollSpeed, 0);
                            artefactName.transform.localPosition -= new Vector3(0, scrollSpeed, 0);
                        }
                    }
                        
                }
               
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            initialTouchLocation = Vector3.zero;
            scrollingThroughText = false;
        }

        if (currentArtefact != null)
        {//Checks if the mouse has been scrolled up or scrolled down
            //TODO: Implement mobile inputs here such as the typical two finger zoom on most mobile apps.
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (currentArtefact.GetComponent<SpriteRenderer>().bounds.extents.y < 4.5f)
                    currentArtefact.transform.localScale = currentArtefact.transform.localScale * 1.2f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentArtefact.GetComponent<SpriteRenderer>().bounds.extents.y > 1.5f)
                    currentArtefact.transform.localScale = currentArtefact.transform.localScale * 0.8f;
            }
                
        }

        if (isExamine)
        {//If one artefact is being examined then don't render the others.
            if(!renderersDisabled)
             DisableRenderers();
        }

        if (currentArtefact == null)
        {//If there is no currently examined artefact make all artefacts visible again.
            if (renderersDisabled)
                EnableRenderers();
        }

    }

    ///<summary>Initialises an artefact GameObject into the scene
    ///</summary>
    ///<param name="name">The name of the artefact.</param>
    ///<param name="filePath">The file path to the model, can be found like so (Right click model in editor -> "Copy Path" (Alt+Ctrl+C)).</param>
    ///<param name="position">The position on the screenthe model will be.</param>
    ///<param name="scale">The initial scale of the model in the scene.</param>
    void initArtefact(string name, string filePath, Vector2 position, float scale)
    {

        GameObject artefact;
        artefact = new GameObject();
        artefact.name = name;

        Mesh artefactMesh;
        artefactMesh = Resources.Load<Mesh>(filePath);
        artefact.gameObject.AddComponent<MeshFilter>();
        artefact.AddComponent<MeshRenderer>();
        artefact.GetComponent<MeshFilter>().sharedMesh = artefactMesh;

        artefact.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Models/Materials/Default");
        artefact.AddComponent<BoxCollider>();

        artefact.transform.localPosition = new Vector2(-2.19f, -2.68f);
        artefact.transform.localScale = new Vector3(scale, scale, scale);
        artefact.layer = LayerMask.NameToLayer("Artefact");
    }

    GameObject InitAnimationObject(string name, string filePath, Vector2 position, float scale)
    {
        GameObject animationObject = new GameObject();
        animationObject.name = name;
        animationObject.AddComponent<SpriteRenderer>();
        animationObject.AddComponent<Animator>();
        animationObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(filePath);
        animationObject.transform.parent = gameObject.GetComponent<SpriteRenderer>().transform;
        animationObject.transform.localPosition = position;
        animationObject.transform.localScale = new Vector3(scale, scale, scale);
        animationObject.GetComponent<Animator>().speed = 0f;
        animationObject.AddComponent<BoxCollider>();
        animationObject.GetComponent<BoxCollider>().size = new Vector3(3, 3, 1);
        animationObject.layer = LayerMask.NameToLayer("Artefact");
        return animationObject;
    }

    private void GoToLevelSelect()
    {
        if(!GameManager.instance.IsInTransition())
        {
            SoundManager.instance.PlayClip(buttonSound);
            SoundManager.instance.StopMusic();
            Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
            if (canvasCheck.Length > 0)
            {
                for(int i = 0; i < canvasCheck.Length; i++)
                {
                        canvasCheck[i].enabled = false;
                }
            }
            GameManager.instance.changeScene((int)GameManager.Scene.WorldMapScene);
        }
       

    }

    private void GoToDemo()
    {
        if (!GameManager.instance.IsInTransition())
        {

             if (currentArtefact != null)
             {
                Canvas canvasCheck = FindObjectOfType<Canvas>();
                if (canvasCheck != null)
                {
                        canvasCheck.enabled = false;
                }
                if (GameManager.instance.GetCurrentLanguage() == "Eng")
                {
                    switch (currentArtefact.name)
                    {
                        case "Knife":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Knife);
                            break;
                        case "Lyre":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Music);
                            break;
                        case "Loom":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Loom);
                            break;
                        case "Forge":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Bellows);
                            break;
                        case "Butterdish":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Butter);
                            break;
                        default:
                            break;
                    }
                }
                else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                {
                    switch (currentArtefact.name)
                    {
                        case "Couteau":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Knife);
                            break;
                        case "Lyre":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Music);
                            break;
                        case "Métier à tisser":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Loom);
                            break;
                        case "Minerai":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Bellows);
                            break;
                        case "Beurrier":
                            SoundManager.instance.PlayClip(buttonSound);
                            SoundManager.instance.StopMusic();
                            GameManager.instance.changeScene((int)GameManager.Scene.Butter);
                            break;
                        default:
                            break;
                    }
                }
           
             }
        }
        
    }

    private void CloseInfo()
    {
        if (currentArtefact != null)
        {
            isExamine = false;
            currentArtefact.transform.localPosition = initialPosition;//Store the starting position.
            currentArtefact.transform.localScale = initialScale;//Store the starting scale.
            currentArtefact.transform.rotation = initialRotation;//Store the starting rotation.
            currentArtefact.GetComponent<Animator>().speed = 0f;
            currentArtefact.GetComponent<Animator>().Play(currentArtefact.name + " Animation", 0, 0f);
            currentArtefact = null;
            gameObject.GetComponent<SpriteRenderer>().sprite = backgroundSprite;
            artefactInformation.SetActive(false);
            artefactName.SetActive(false);
            artefactInformation.transform.localPosition = infoTextPosition;
            artefactName.transform.localPosition = nameTextPosition;

            demoButton.SetActive(false);
            backButton.SetActive(false);
            worldButton.SetActive(true);

            SoundManager.instance.PlayClip(buttonSound);
        }
    }

    private void DisableRenderers()
    {
        if (GameManager.instance.GetCurrentLanguage() == "Eng")//English
        {
            switch (currentArtefact.name)
            {
                case "Knife":
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;
                case "Butterdish":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Loom":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Forge":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Lyre":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;
                default:
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = true;
                    break;
            }
        }
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")//French
        {
            switch (currentArtefact.name)
            {
                case "Couteau":
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;
                case "Beurrier":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Métier à tisser":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Minerai":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;

                case "Lyre":
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = false;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = false;
                    break;
                default:
                    if (animationObject_Knife != null)
                        animationObject_Knife.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Butterdish != null)
                        animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Forge != null)
                        animationObject_Forge.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Lyre != null)
                        animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = true;
                    if (animationObject_Loom != null)
                        animationObject_Loom.GetComponent<SpriteRenderer>().enabled = true;
                    break;
            }
        }//French

        renderersDisabled = true;
    }

    private void EnableRenderers()
    {
        if (animationObject_Knife != null)
            animationObject_Knife.GetComponent<SpriteRenderer>().enabled = true;
        if (animationObject_Butterdish != null)
            animationObject_Butterdish.GetComponent<SpriteRenderer>().enabled = true;
        if (animationObject_Forge != null)
            animationObject_Forge.GetComponent<SpriteRenderer>().enabled = true;
        if (animationObject_Lyre != null)
            animationObject_Lyre.GetComponent<SpriteRenderer>().enabled = true;
        if (animationObject_Loom != null)
            animationObject_Loom.GetComponent<SpriteRenderer>().enabled = true;
        renderersDisabled = false;
    }
}
