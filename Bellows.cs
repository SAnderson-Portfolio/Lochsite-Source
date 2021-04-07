using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bellows : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip forgeSound;
    [SerializeField] private AudioClip bellowSoundOne;
    [SerializeField] private AudioClip bellowSoundTwo;
    [SerializeField] private AudioClip ingotClip;
    private GameObject backgroundBar;
    private GameObject hitBar;
    private GameObject progressBar;
    private GameObject progressBarBackground;
    private GameObject forgeObject;
    private GameObject bellowsObject;
    private GameObject[] ores;
    private GameObject hitLocation;
    SpriteRenderer backgroundRenderer;//Sprite renderer for the knife scene background.
    private int progress;
    Vector2 backgroundBounds;
    bool movingUp;
    bool targetHit;
    float waitTimer;
    public GameObject fire;
    public GameObject smoke;
    ParticleSystem.EmissionModule fireEmission;
    ParticleSystem.EmissionModule smokeEmission;
    // Start is called before the first frame update
    void Start()
    {
        progress = 0;
        backgroundBar = new GameObject();
        hitBar = new GameObject();

        backgroundBar.AddComponent<SpriteRenderer>();
        backgroundBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Slider_Upright");
        backgroundBar.name = "Hit Background";
        backgroundBar.transform.localPosition = new Vector3(-6.6f, 0, 0);
        backgroundBar.GetComponent<SpriteRenderer>().sortingOrder = 0;

        hitBar.AddComponent<SpriteRenderer>();
        hitBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Slider_MovingBit");
        hitBar.name = "Hit Bar";
        hitBar.transform.parent = backgroundBar.transform;
        hitBar.transform.localPosition = Vector3.zero;
        hitBar.GetComponent<SpriteRenderer>().sortingOrder = 2;

        hitLocation = new GameObject();
        hitLocation.name = "Hit Location";
        hitLocation.AddComponent<BoxCollider2D>();
        hitLocation.GetComponent<BoxCollider2D>().size = new Vector2(1.3f, 1.6f);
        hitLocation.transform.parent = backgroundBar.transform;
        hitLocation.transform.localPosition = Vector3.zero;
        hitLocation.tag = "HitLocation";


        progressBarBackground = new GameObject();
        progressBarBackground.AddComponent<SpriteRenderer>();
        progressBarBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Ore_Case");
        progressBarBackground.name = "Progress Background";
        progressBarBackground.transform.localPosition = new Vector2(5, 0f);

        forgeObject = new GameObject();
        forgeObject.AddComponent<SpriteRenderer>();
        forgeObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_ForgeNoFaceFront");
        forgeObject.name = "Forge";
        forgeObject.transform.parent = gameObject.transform;
        forgeObject.transform.localPosition = new Vector2(0, -3f);
        forgeObject.GetComponent<SpriteRenderer>().sortingOrder = 4;

        GameObject forgeBack = new GameObject();
        forgeBack.AddComponent<SpriteRenderer>();
        forgeBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_ForgeNoFaceBack");
        forgeBack.name = "Forge Back";
        forgeBack.transform.parent = forgeObject.transform;
        forgeBack.transform.localPosition = Vector2.zero;


        bellowsObject = new GameObject();
        bellowsObject.AddComponent<SpriteRenderer>();
        bellowsObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Bellow_Open");
        //bellowsObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
        bellowsObject.name = "Bellows";
        bellowsObject.transform.parent = forgeObject.transform;
        bellowsObject.transform.localPosition = new Vector2(-3.5f, -1f);

        backgroundBounds = new Vector2(backgroundBar.GetComponent<SpriteRenderer>().bounds.extents.x * 2, backgroundBar.GetComponent<SpriteRenderer>().bounds.extents.y * 2);
        movingUp = false;
        waitTimer = 1f;
        targetHit = false;

        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/BackG_ExteriorMinigames");
        backgroundRenderer.name = "Background";

        //Vector2 orePosOne = new Vector2(1.05f, 0.3f);
        //Vector2 orePosTwo = new Vector2(0.63f, -0.9f);
        //Vector2 orePosThree = new Vector2(-0.63f, -0.9f);
        //Vector2 orePosFour = new Vector2(-1.05f, 0.3f);
        //Vector2 orePosFive = new Vector2(0f, 1.05f);
        Vector2[] orePositions = new Vector2[] {
            new Vector2(1.05f, 0.3f),
            new Vector2(0.63f, -0.9f),
            new Vector2(-0.63f, -0.9f),
            new Vector2(-1.05f, 0.3f),
            new Vector2(0f, 1.05f)};
        ores = new GameObject[5];
        for(int i = 0; i < 5; i++)
        {
            ores[i] = new GameObject();
            ores[i].AddComponent<SpriteRenderer>();
            ores[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Ore_Ball");
            ores[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
            ores[i].name = "Ore";
            ores[i].transform.parent = progressBarBackground.transform;
            ores[i].transform.localPosition = orePositions[i];
            ores[i].SetActive(false);
        }
       
        //Get the size of the background sprite and scale it to the viewport
        backgroundRenderer.transform.localScale = Vector3.one;
        double swidth = backgroundRenderer.sprite.bounds.size.x;
        double sheight = backgroundRenderer.sprite.bounds.size.y;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        backgroundRenderer.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));
        backgroundRenderer.transform.position = Vector3.zero;
        
        fireEmission = fire.GetComponent<ParticleSystem>().emission;
        fireEmission.rateOverTime = 0f;
        fire.GetComponent<ParticleSystem>().Play();
        smokeEmission = smoke.GetComponent<ParticleSystem>().emission;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsInTransition())
            SoundManager.instance.PlayMusic(forgeSound);
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit2D hit;
            hit = Physics2D.Raycast(hitBar.transform.position, Vector3.forward);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("HitLocation"))
                {
                    if(progress < 5 && !targetHit)
                    {
                        targetHit = true;
                        bellowsObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Bellow_Closed");
                        bellowsObject.transform.localPosition = new Vector2(-3.5f, -1.3f);
                        smokeEmission.rateOverTime = 0f;
                        fireEmission.rateOverTime = 60f;
                        SoundManager.instance.PlayClipOneShot(bellowSoundOne);
                    }
                    
                }
            }
        }

        if(movingUp)
        {
            if (hitBar.transform.position.y > -backgroundBar.GetComponent<SpriteRenderer>().bounds.extents.y + (hitBar.GetComponent<SpriteRenderer>().bounds.extents.y * 2))
            {
                hitBar.transform.position -= new Vector3(0, 10f, 0) * Time.deltaTime;
            }
            else
                movingUp = false;
        }
        else if(!movingUp)
        {
            if (hitBar.transform.position.y < backgroundBar.GetComponent<SpriteRenderer>().bounds.extents.y - (hitBar.GetComponent<SpriteRenderer>().bounds.extents.y * 2))
            {
                hitBar.transform.position += new Vector3(0, 10f, 0) * Time.deltaTime;
            }
            else
                movingUp = true;
        }

        if(targetHit)
        {
            waitTimer -= 1f * Time.deltaTime;
            ores[progress].SetActive(true);
            if (waitTimer <= 0)
            {
                SoundManager.instance.PlayClipOneShot(ingotClip);
                progress++;
                targetHit = false;
                waitTimer = 1f;
                bellowsObject.transform.localPosition = new Vector2(-3.5f, -1.0f);
                bellowsObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Forge/Asset_Bellow_Open");
                smokeEmission.rateOverTime = 5f;
                fireEmission.rateOverTime = 0f;
                SoundManager.instance.PlayClipOneShot(bellowSoundTwo);
            }
        }
        
        
        if(progress >= 5)
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
}
