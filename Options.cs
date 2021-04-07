using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Options : MonoBehaviour
{
    [Header("Canvas Objects")]
    [SerializeField]private Slider volumeSlider;
    [SerializeField] private Slider efxSlider;
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private Text volumeText;
    [SerializeField] private Text efxText;
    [SerializeField] private Text langText;
    private SpriteRenderer backgroundRenderer;
    private GameObject bgBack;
    [Header("Sounds")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip efxClip;
    
    // Start is called before the first frame update
    void Start()
    {
       
        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/Backg_LevelSelect_03NightSkyFront");
        backgroundRenderer.sortingOrder = 2;
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
        backgroundRenderer.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));
        gameObject.transform.Translate(new Vector3(0, 0, -2));
        bgBack = new GameObject();
        bgBack.name = "Sky";
        bgBack.AddComponent<SpriteRenderer>();
        bgBack.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Background");
        bgBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Backgrounds/Backg_LevelSelect_03NightSkyBack");
        bgBack.GetComponent<SpriteRenderer>().sortingOrder = 0;
        bgBack.transform.localScale = Vector3.one;
        bgBack.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));

        volumeSlider.value = SoundManager.instance.GetMusicVolume();
        if (GameManager.instance.GetCurrentLanguage() == "Eng")
            volumeText.text = "Music Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        else if (GameManager.instance.GetCurrentLanguage() == "Fr")
            volumeText.text = "Volume de la musique: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";

        efxSlider.value = SoundManager.instance.GetEFXVolume();
        efxText.text = "EFX Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetEFXVolume() * 100) + "%";

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsInTransition())
            SoundManager.instance.PlayMusic(musicClip);

        if(volumeSlider.value == 1)
        {
            if (GameManager.instance.GetCurrentLanguage() == "Eng")
                volumeText.text = "Music Volume: 100%".ToUpper();
            else if (GameManager.instance.GetCurrentLanguage() == "Fr")
                volumeText.text = "Volume de la musique: 100%".ToUpper();
        }

        if(efxSlider.value == 1)
        {
            efxText.text = "EFX Volume: 100%".ToUpper();
        }
    }

    public void UpdateMusicVolume(float value)
    {
        if (GameManager.instance.GetCurrentLanguage() == "Eng")
            volumeText.text = "Music Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        else if (GameManager.instance.GetCurrentLanguage() == "Fr")
            volumeText.text = "Volume de la musique: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        SoundManager.instance.SetMusicVolume(value);
    }

    public void UpdateEFXVolume(float value)
    {
        efxText.text = "EFX Volume: ".ToUpper() + Mathf.RoundToInt(value * 100) + "%";
        SoundManager.instance.SetEFXVolume(value);
    }

    public void GoToMenu()
    {
        if (!GameManager.instance.IsInTransition())
        {
            SoundManager.instance.PlayClipOneShot(efxClip);
            SoundManager.instance.StopMusic();
            Canvas[] canvasCheck = FindObjectsOfType<Canvas>();
            if (canvasCheck.Length > 0)
            {
                for (int i = 0; i < canvasCheck.Length; i++)
                {
                    canvasCheck[i].enabled = false;
                }
            }
            GameManager.instance.changeScene((int)GameManager.Scene.MainMenu);
        }
    }

    public void DragEnded()
    {
        SoundManager.instance.PlayClip(efxClip);
    }

    public void ChangeToEnglish()
    {
        if (GameManager.instance.GetCurrentLanguage() != "Eng")
            GameManager.instance.SetCurrentLanguage("Eng");

        volumeText.text = "Music Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        langText.text = "Language".ToUpper();
    }

    public void ChangeToFrench()
    {
        if (GameManager.instance.GetCurrentLanguage() != "Fr")
            GameManager.instance.SetCurrentLanguage("Fr");

        volumeText.text = "Volume de la musique: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        langText.text = "Langue".ToUpper();
    }

    public void ResetSave()
    {
        GameManager.instance.ResetSave();
        GameManager.instance.SetCurrentLanguage("Eng");
        GameManager.instance.SetTutorialComplete(0);
        GameManager.instance.ResetArtefacts();
        SoundManager.instance.SetEFXVolume(1.0f);
        SoundManager.instance.SetMusicVolume(1.0f);
        volumeText.text = "Music Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetMusicVolume() * 100) + "%";
        volumeSlider.value = 1.0f;
        efxText.text = "EFX Volume: ".ToUpper() + Mathf.RoundToInt(SoundManager.instance.GetEFXVolume() * 100) + "%";
        efxSlider.value = 1.0f;
        langText.text = "Language".ToUpper();
    }
}
