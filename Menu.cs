using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    Button playButton;
    public Sprite buttonClicked;
    public Sprite buttonUnclicked;
    SpriteRenderer backgroundRenderer;//Sprite renderer for the level background.
    GameObject myButton;
    GameObject optionsButton;
    float buttonAppearTimer;
    bool buttonsShown;

    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip menuMusic;
    void Start()
    {
        Canvas levelCanvas;
        GameObject myObject;
        
        GameObject buttonText;
        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/blackBackground");
        backgroundRenderer.transform.localPosition = new Vector2(0, 0);
        backgroundRenderer.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().clip = Resources.Load<UnityEngine.Video.VideoClip>("Video/Video_OpeningCinematic");
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().targetMaterialRenderer = backgroundRenderer;
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().targetMaterialProperty = "_MainTex";
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = false;
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().Prepare();
        
        //Get the size of the background sprite and scale it to the viewport
        backgroundRenderer.transform.localScale = Vector3.one;
        double swidth = backgroundRenderer.sprite.bounds.size.x;
        double sheight = backgroundRenderer.sprite.bounds.size.y;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        backgroundRenderer.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));
        //Canvas
        myObject = new GameObject();
        myButton = new GameObject();
        buttonText = new GameObject();
        optionsButton = new GameObject();
        myObject.name = "Canvas";
        myObject.AddComponent<Canvas>();

        levelCanvas = myObject.GetComponent<Canvas>();
        levelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myObject.AddComponent<CanvasScaler>();
        myObject.AddComponent<GraphicRaycaster>();


        myButton.transform.parent = levelCanvas.transform;
        myButton.transform.localPosition = new Vector2(-29, -43);
        myButton.AddComponent<Button>();
        myButton.name = "PlayButton";
        myButton.gameObject.SetActive(true);
        myButton.gameObject.AddComponent<Image>();
        myButton.GetComponent<Image>().sprite = buttonUnclicked;
       // myButton.transform.localPosition = new Vector2(-630, -10);
        myButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonUnclicked.texture.width, buttonUnclicked.texture.height);
        myButton.GetComponent<Button>().onClick.AddListener(MoveToWorldMap);
        myButton.SetActive(false);
        myButton.AddComponent<ButtonScript>();

        buttonText.transform.SetParent(myButton.transform, true);
        buttonText.transform.localPosition = new Vector2(0f, 0f);
        buttonText.AddComponent<Text>();
        buttonText.name = "Text";
        if(GameManager.instance.GetCurrentLanguage() == "Eng")
            buttonText.GetComponent<Text>().text = "Play".ToUpper();
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
            buttonText.GetComponent<Text>().text = "Jouer".ToUpper();
        buttonText.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/Pixeled");
        buttonText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        buttonText.GetComponent<Text>().color = Color.white;
        buttonText.GetComponent<Text>().fontSize = 32;
        buttonText.GetComponent<Text>().resizeTextForBestFit = true;
        buttonText.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(310f, 98f);

        optionsButton.transform.parent = levelCanvas.transform;
        optionsButton.transform.localPosition = new Vector2(-29, -160f);
        optionsButton.AddComponent<Button>();
        optionsButton.name = "Options Button";
        optionsButton.gameObject.SetActive(true);
        optionsButton.gameObject.AddComponent<Image>();
        optionsButton.GetComponent<Image>().sprite = buttonUnclicked;
        // myButton.transform.localPosition = new Vector2(-630, -10);
        optionsButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonUnclicked.texture.width, buttonUnclicked.texture.height);
        optionsButton.GetComponent<Button>().onClick.AddListener(MoveToOptions);
        optionsButton.SetActive(false);
        optionsButton.AddComponent<ButtonScript>();

        GameObject optionsText = new GameObject();
        optionsText.transform.SetParent(optionsButton.transform, true);
        optionsText.transform.localPosition = new Vector2(0f, 0f);
        optionsText.AddComponent<Text>();
        optionsText.name = "Text";
        if(GameManager.instance.GetCurrentLanguage() == "Eng")
            optionsText.GetComponent<Text>().text = "Options".ToUpper();
        else if(GameManager.instance.GetCurrentLanguage() == "Fr")
            optionsText.GetComponent<Text>().text = "Les options".ToUpper();
        optionsText.GetComponent<Text>().font = Resources.Load<Font>("UI Assets/Pixeled");
        optionsText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        optionsText.GetComponent<Text>().color = Color.white;
        optionsText.GetComponent<Text>().fontSize = 32;
        optionsText.GetComponent<Text>().resizeTextForBestFit = true;
        optionsText.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(310f, 98f);

        buttonAppearTimer = 0f;
        buttonsShown = false;
        StartCoroutine(ButtonCoroutine());

       
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsInTransition())
            SoundManager.instance.PlayMusic(menuMusic);
    }
    

    void MoveToWorldMap()
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
                    if (canvasCheck[i].name != "Transition Animation")
                        canvasCheck[i].enabled = false;
                }
            }

            if(GameManager.instance.GetTutorialComplete())
                GameManager.instance.changeScene((int)GameManager.Scene.WorldMapScene);
            else
                GameManager.instance.changeScene((int)GameManager.Scene.Level);
        }
        
    }

    private void MoveToOptions()
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
                    if (canvasCheck[i].name != "Transition Animation")
                        canvasCheck[i].enabled = false;
                }
            }

            GameManager.instance.changeScene((int)GameManager.Scene.Options);
        }
    }

    IEnumerator ButtonCoroutine()
    {
        backgroundRenderer.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().Play();
        yield return new WaitForSecondsRealtime(10);

        myButton.SetActive(true);
        optionsButton.SetActive(true);
    }
}
