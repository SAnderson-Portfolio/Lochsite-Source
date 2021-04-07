using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loom : MonoBehaviour
{
    public Texture2D threadWhite;
    public Texture2D threadRed;
    private GameObject needle;
    private GameObject[,] verticalWool;
    private GameObject[,] horizontalWool;
    private bool goingRight;
    private Vector2Int horzIndex;
    private GameObject horzWoolHeader;
    private Vector2 horzPosition;
    private bool swipeNeeded;
    private int maxRows;
    private int numCols;
    private GameObject loom;
    private float swipeMovement;
    private float needleMovement;
    private float loomMovement;
    private char[] desiredInput;
    private int patternPosition;
    private const int totalInputsNeeded = 42;
    private bool initialSetup;
    private bool wrongInput;
    GameObject buttonCanvas;
    private Vector3 initialTouchPosition;
    [Header("Variables")]
    [SerializeField] private float swipeDistanceNeeded;
    [SerializeField] private AudioClip threadSoundUp;
    [SerializeField] private AudioClip threadSoundDown;
    [SerializeField] private AudioClip musicClip;
    private bool startMessageShown;
    // Start is called before the first frame update
    void Start()
    {
        initialTouchPosition = new Vector3(-1f, -1f, -1f);
        swipeDistanceNeeded = 100f;
        startMessageShown = false;
        swipeMovement = 0.25f;
        needleMovement = 0.2f;
        loomMovement = 0.2f;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
       

        loom = new GameObject();
        loom.name = "Loom";
        loom.AddComponent<SpriteRenderer>();
        loom.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/loom/Assets_Textile_Loom");
        loom.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(2.87f, 2.3f);
        loom.GetComponent<SpriteRenderer>().sortingOrder = 3;
        loom.transform.position = new Vector2(0.461f, 0.135f);
        maxRows = 18;
        numCols = 14;
        verticalWool = new GameObject[numCols, 5];
        horizontalWool = new GameObject[numCols, maxRows];
        needle = new GameObject();

        needle.AddComponent<SpriteRenderer>();
        needle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/loom/Asset_Textile_Needle");
        needle.name = "Needle";
        needle.transform.position = new Vector2(-2.5f, 1.5f);
        needle.transform.Rotate(new Vector3(0, 0, 1), 180);
        needle.transform.localScale = new Vector2(1, 0.5f);
        GameObject woolHeader = new GameObject();
        woolHeader.name = "--Vertical Wool--";
        horzWoolHeader = new GameObject();
        horzWoolHeader.name = "--Horizontal Wool--";
        Vector2 startingPosition = new Vector2(-2.8f, 2.5f);
        horzPosition = new Vector2(-2.8f, 1.5f);
        for(int i = 0; i < numCols; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                verticalWool[i, j] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                verticalWool[i, j].name = "Vertical " + i + "," + j;
                verticalWool[i, j].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/Wool");
                verticalWool[i, j].transform.parent = woolHeader.transform;
                verticalWool[i, j].transform.localScale = new Vector3(0.5f, 1, 1);
                verticalWool[i, j].transform.localPosition = startingPosition;
                startingPosition.y -= 1f;
            }
            startingPosition.y = 2.5f;
            startingPosition.x += 0.5f;
        }

        
        goingRight = true;
        horzIndex = Vector2Int.zero;
        SpriteRenderer  backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/loom/Backg_CrannogInterior_NoFire");

        //Get the size of the background sprite and scale it to the viewport
        backgroundRenderer.transform.localScale = Vector3.one;
        double swidth = backgroundRenderer.sprite.bounds.size.x;
        double sheight = backgroundRenderer.sprite.bounds.size.y;
        backgroundRenderer.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));

        

        InitCanvas();

        swipeNeeded = false;
        initialSetup = true;
        for (int it = 0; it < numCols - 2; it++)
        {
            for (int it2 = 0; it2 < maxRows; it2++)
            {
                if(horzIndex.y < maxRows - 3)
                {

                    if (it2 % 3 == 0)
                    {
                        PlaceWoolUnder();
                    }
                    else
                    {
                        PlaceWoolOver();
                    }

                    if (swipeNeeded)
                        SwipeWool();
                }
            }

        }
        initialSetup = false;

        desiredInput = new char[totalInputsNeeded];
        for(int i = 0; i < desiredInput.Length; i++)
        {
            if (i % 3 == 0)
            {
                desiredInput[i] = '0';
            }
            else
                desiredInput[i] = '1';
        }

        patternPosition = 0;
        wrongInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsInTransition())
            SoundManager.instance.PlayMusic(musicClip);
        if (!initialSetup && swipeNeeded)
        {
            buttonCanvas.transform.GetChild(2).gameObject.SetActive(true);
            if(GameManager.instance.GetCurrentLanguage() == "Eng")
                buttonCanvas.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Looks like you completed a row, now you need to tightly compact the weave.\nTry swiping up.".ToUpper();
            else if(GameManager.instance.GetCurrentLanguage() == "Fr")
                buttonCanvas.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>().text = "On dirait que vous avez terminé une rangée, maintenant vous devez compacter étroitement le tissage.\nEssayez de glisser vers le haut.".ToUpper();
            
            if(Input.GetMouseButtonDown(0))
            {
                initialTouchPosition = Input.mousePosition;
            }

            if(initialTouchPosition != new Vector3(-1f, -1f, -1f) && Input.GetMouseButton(0))
            {
                Vector3 currentMousePos = Input.mousePosition;
                if(currentMousePos.y > initialTouchPosition.y + swipeDistanceNeeded)
                {
                    SwipeWool();
                    swipeNeeded = false;
                    buttonCanvas.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }

        if(!startMessageShown)
        {
            StartCoroutine(DisplayStartMessage());
        }

        if(patternPosition == totalInputsNeeded  && !swipeNeeded)
        {
            if (!GameManager.instance.IsInTransition())
            {
                SoundManager.instance.StopMusic();
                Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
                if (canvasCheck.Length > 0)
                {
                    for (int i = 0; i < canvasCheck.Length; i++)
                    {
                        canvasCheck[i].enabled = false;
                    }
                }
                GameManager.instance.changeScene((int)GameManager.Scene.Museum);
            }
        }
    }

    private void PlaceWoolOver()
    {
        if(!wrongInput && !swipeNeeded)
        {
            if(!initialSetup && desiredInput[patternPosition] == '1')
            {
                if(horzIndex.x < numCols && horzIndex.y < maxRows)
                {
                    SoundManager.instance.PlayClip(threadSoundUp);
                    horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
                    horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
                    horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = 1;
                    horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
                    horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
                    MoveNeedle();
                    if(patternPosition < totalInputsNeeded)
                        patternPosition++;
                    return;
                }
            }
            else if(!initialSetup && desiredInput[patternPosition] == '0')
            {
                wrongInput = true;
                WrongInput('1');
                return;
            }
        }

        if (initialSetup)
        {
            horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
            horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
            horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = 1;
            horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
            horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
            horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
            MoveNeedle();
        }
    }

    private void PlaceWoolUnder()
    {
        if(!wrongInput && !swipeNeeded)
        {

            if(!initialSetup && desiredInput[patternPosition] == '0')
            {
                if (horzIndex.x < numCols && horzIndex.y < maxRows)
                {
                    SoundManager.instance.PlayClip(threadSoundDown);
                    horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
                    horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
                    horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = -1;
                    horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
                    horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
                    MoveNeedle();
                    if (patternPosition < totalInputsNeeded)
                        patternPosition++;
                    return;
                }
            }
            else if(!initialSetup && desiredInput[patternPosition] == '1')
            {
                wrongInput = true;
                WrongInput('0');
                return;
            }
        }

        if(initialSetup)
        {
            horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
            horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
            horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = -1;
            horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
            horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
            horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
            MoveNeedle();
        }
    }

    private void MoveNeedle()
    {
        if (goingRight)
        {
            needle.transform.position += new Vector3(0.5f, 0f, 0f);
            if (horzIndex.x < numCols - 1)
            {
                horzIndex.x += 1;
                horzPosition.x += 0.5f;
            }

        }
        else
        {
            needle.transform.position -= new Vector3(0.5f, 0f, 0f);
            if (horzIndex.x > 0)
            {
                horzIndex.x -= 1;
                horzPosition.x -= 0.5f;
            }

        }
        RotateNeedle();
    }
    /// <summary>
    /// Moves the wool upwards when a row is completed.
    /// </summary>
    private void SwipeWool()
    {
        for (int i = 0; i < numCols; i++)
        {
            horizontalWool[i, horzIndex.y - 1].transform.position += new Vector3(0, swipeMovement, 0);
        }
        swipeNeeded = false;
    }
    /// <summary>
    /// Rotates the needle when the end of a row has been reached to continue back the other way.
    /// </summary>
    private void RotateNeedle()
    {
        if (needle.transform.position.x == 4.5f && goingRight)
        {
            needle.transform.Rotate(new Vector3(0, 0, 1), 180);
            needle.transform.position -= new Vector3(1f, needleMovement, 0);
            goingRight = false;
            horzIndex.y += 1;
            horzPosition.y -= loomMovement;
            swipeNeeded = true;
        }

        if (needle.transform.position.x == -3.5f && !goingRight)
        {
            needle.transform.Rotate(new Vector3(0, 0, 1), -180);
            needle.transform.position -= new Vector3(-1f, needleMovement, 0);
            goingRight = true;
            horzIndex.y += 1;
            horzPosition.y -= loomMovement;
            swipeNeeded = true;
        }
    }

    private void WrongInput(char buttonPressed)
    {
        if(wrongInput && horzIndex.x < numCols && horzIndex.y < maxRows)
        {
            //0 = Down, 1 = Up
            if (buttonPressed == '0')
            {
                SoundManager.instance.PlayClip(threadSoundDown);
                horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
                horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
                horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = -1;
                horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
                horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
            }
            else if(buttonPressed == '1')
            {
                SoundManager.instance.PlayClip(threadSoundUp);
                horizontalWool[horzIndex.x, horzIndex.y] = GameObject.CreatePrimitive(PrimitiveType.Quad);
                horizontalWool[horzIndex.x, horzIndex.y].name = "Horizontal " + horzIndex.x + "," + horzIndex.y;
                horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Sprites/loom/Materials/WoolHorz");
                horizontalWool[horzIndex.x, horzIndex.y].GetComponent<MeshRenderer>().sortingOrder = 1;
                horizontalWool[horzIndex.x, horzIndex.y].transform.parent = horzWoolHeader.transform;
                horizontalWool[horzIndex.x, horzIndex.y].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                horizontalWool[horzIndex.x, horzIndex.y].transform.localPosition = horzPosition;
            }
            StartCoroutine(DisplayFailMessage());
        }
    }

    private void InitCanvas()
    {
        buttonCanvas = new GameObject();
        buttonCanvas.name = "Button Canvas";
        buttonCanvas.AddComponent<Canvas>();
        buttonCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        buttonCanvas.AddComponent<CanvasScaler>();
        buttonCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        buttonCanvas.AddComponent<GraphicRaycaster>();

        GameObject upButton = new GameObject();
        GameObject downButton = new GameObject();

        upButton.AddComponent<Button>();
        upButton.name = "Up Button";
        upButton.transform.parent = buttonCanvas.transform;
        upButton.transform.localPosition = new Vector2(290, 90f);
        upButton.AddComponent<Image>();
        upButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/loom/Asset_Textile_UpArrow");
        upButton.GetComponent<Button>().onClick.AddListener(PlaceWoolOver);

        downButton.AddComponent<Button>();
        downButton.name = "Down Button";
        downButton.transform.parent = buttonCanvas.transform;
        downButton.transform.localPosition = new Vector2(290f, -50f);
        downButton.AddComponent<Image>();
        downButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/loom/Asset_Textile_DownArrow");
        downButton.GetComponent<Button>().onClick.AddListener(PlaceWoolUnder);

        GameObject imageObject = new GameObject();
        imageObject.name = "Message Box";
        imageObject.transform.parent = buttonCanvas.transform;
        imageObject.AddComponent<Image>();
        imageObject.GetComponent<Image>().transform.localPosition = new Vector2(26f, -160f);
        imageObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI Assets/Asset_Button_Stan 1");
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(412f, 110f);
        //imageObject.SetActive(false);

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
            textObject.GetComponent<Text>().text = "Use the up and down buttons to complete the pattern.\nThe up arrow places the thread above and the down arrow place the thread below".ToUpper();
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
            textObject.GetComponent<Text>().text = "Utilisez les boutons haut et bas pour terminer le motif.\nLa flèche vers le haut place le fil au - dessus et la flèche vers le bas place le fil en dessous".ToUpper();
        
    }

    private IEnumerator DisplayFailMessage()
    {
        if(goingRight)
            needle.transform.position += new Vector3(0.5f, 0f, 0f);
        else
            needle.transform.position -= new Vector3(0.5f, 0f, 0f);

        if(GameManager.instance.GetCurrentLanguage() == "Eng")
            buttonCanvas.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oops, that doesn't look right.\nPerhaps try looking at the pattern again.".ToUpper();
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
            buttonCanvas.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>().text = "Oups, ça n'a pas l'air bien.\nEssayez peut-être de revoir le motif.".ToUpper();
        buttonCanvas.transform.GetChild(2).gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        buttonCanvas.transform.GetChild(2).gameObject.SetActive(false);
        wrongInput = false;
        if (goingRight)
            needle.transform.position -= new Vector3(0.5f, 0f, 0f);
        else
            needle.transform.position += new Vector3(0.5f, 0f, 0f);
        Destroy(horizontalWool[horzIndex.x, horzIndex.y]);
    }

    private IEnumerator DisplayStartMessage()
    {
        startMessageShown = true;
        yield return new WaitForSeconds(6f);

        buttonCanvas.transform.GetChild(2).gameObject.SetActive(false);
        
    }
    
}
