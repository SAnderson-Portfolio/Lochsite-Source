using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
   
    struct Nodes
    {
        GameObject nodeObject { get; set; }
        bool isHit { get; set; }

        public GameObject gameObject
        {
            get { return nodeObject; }
            set { nodeObject = value; }
        }

        public bool IsHIt
        {
            get { return isHit; }
            set { isHit = value; }
        }
    }

    Nodes[] nodes;

    GameObject stone;
    GameObject dagger;
    float sharpness;
    bool daggerHit;
    GameObject sharpnessBarProgress;
    GameObject sharpnessBarBackground;
    GameObject sharpnessMarkings;
    GameObject sharpnessMask;
    SpriteRenderer backgroundRenderer;//Sprite renderer for the knife scene background.
    int edgesSharp;
    Vector3 startPosition;

    [Header("Sounds")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip sharpenClip;
    [SerializeField] private AudioClip progressClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip winClip;
    // Start is called before the first frame update
    void Start()
    {
        sharpness = 0f;
        stone = new GameObject();
        stone.name = "Stone";
        stone.AddComponent<SpriteRenderer>();
        stone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_WhetstoneFront");
        stone.GetComponent<SpriteRenderer>().sortingOrder = 0;
        stone.transform.position = new Vector3(1.35f, 0, 0);

        dagger = new GameObject();
        dagger.name = "Dagger";
        dagger.AddComponent<SpriteRenderer>();
        dagger.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_Dagger_Demonstration");
        dagger.GetComponent<SpriteRenderer>().sortingOrder = 2;
        dagger.AddComponent<BoxCollider2D>();
        startPosition = new Vector3(-6.77f, 0, 0);
        dagger.transform.position = startPosition;
        nodes = new Nodes[6];
        for (int i = 0; i < 6; i++)
        {
            nodes[i].gameObject = new GameObject();
            nodes[i].gameObject.name = "Node" + i;
            nodes[i].gameObject.AddComponent<SpriteRenderer>();
            nodes[i].gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/eightNode");
            nodes[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            nodes[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            nodes[i].gameObject.AddComponent<BoxCollider2D>();
            nodes[i].gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 4);
            nodes[i].gameObject.transform.parent = stone.transform;
            nodes[i].gameObject.transform.localScale = Vector2.one;
            nodes[i].IsHIt = false;
            //nodes[i].gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        nodes[0].gameObject.transform.localPosition = new Vector2(-2.79f, 0f);
        nodes[1].gameObject.transform.localPosition = new Vector2(-1.52f, 0f);
        nodes[2].gameObject.transform.localPosition = new Vector2(-0.30f, 0f);
        nodes[3].gameObject.transform.localPosition = new Vector2(0.81f, 0f);
        nodes[4].gameObject.transform.localPosition = new Vector2(1.95f, 0f);
        nodes[5].gameObject.transform.localPosition = new Vector2(3.08f, 0f);

        daggerHit = false;

        sharpnessBarBackground = new GameObject();
        sharpnessBarBackground.name = "Sharpness Gauge";
        sharpnessBarBackground.AddComponent<SpriteRenderer>();
        sharpnessBarBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_Slider_Base");
        sharpnessBarBackground.transform.position = new Vector2(-3f, 3.70f);

        sharpnessBarProgress = new GameObject();
        sharpnessBarProgress.name = "Progress";
        sharpnessBarProgress.AddComponent<SpriteRenderer>();
        sharpnessBarProgress.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_Slider_Colours");
        sharpnessBarProgress.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        sharpnessBarProgress.transform.parent = sharpnessBarBackground.transform;
        sharpnessBarProgress.transform.localPosition = Vector2.zero;
        sharpnessBarProgress.GetComponent<SpriteRenderer>().sortingOrder = 1;

        GameObject sharpnessMarkings;
        sharpnessMarkings = new GameObject();
        sharpnessMarkings.name = "Markings";
        sharpnessMarkings.AddComponent<SpriteRenderer>();
        sharpnessMarkings.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_Slider_Markings");
        sharpnessMarkings.transform.parent = sharpnessBarBackground.transform;
        sharpnessMarkings.transform.localPosition = Vector2.zero;
        sharpnessMarkings.GetComponent<SpriteRenderer>().sortingOrder = 2;

        sharpnessMask = new GameObject();
        sharpnessMask.name = "Mask";
        sharpnessMask.AddComponent<SpriteMask>();
        sharpnessMask.GetComponent<SpriteMask>().sprite = Resources.Load<Sprite>("Sprites/Knife/Asset_Slider_Colours");
        sharpnessMask.transform.parent = sharpnessBarBackground.transform;
        sharpnessMask.transform.localPosition = Vector2.zero;
        sharpnessMask.transform.localScale = new Vector3(0, 1f, 1f);

        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/Knife/Backg_KnifeGame");
        backgroundRenderer.name = "Background";
        //Get the size of the background sprite and scale it to the viewport
        backgroundRenderer.transform.localScale = Vector3.one;
        double swidth = backgroundRenderer.sprite.bounds.size.x;
        double sheight = backgroundRenderer.sprite.bounds.size.y;
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        backgroundRenderer.transform.localScale = new Vector2((float)(worldScreenWidth / swidth), (float)(worldScreenHeight / sheight));
        backgroundRenderer.transform.position = Vector3.zero;
        edgesSharp = 0;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            RaycastHit2D hit;
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name == "Dagger")
                {
                    daggerHit = true;
                    SoundManager.instance.PlayClipOneShot(pickupClip);
                }

                if(daggerHit)
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (hitObject == nodes[i].gameObject && !nodes[i].IsHIt)
                        {
                            if (i > 0)
                            {
                                if (nodes[i - 1].IsHIt)
                                {
                                    hitObject.GetComponent<SpriteRenderer>().color = Color.green;
                                    nodes[i].IsHIt = true;
                                    if(sharpness < 1f)
                                    {
                                        sharpness += (1f / nodes.Length) / 2f;
                                        SoundManager.instance.PlayClipOneShot(sharpenClip);
                                    }
                                }
                                else
                                {
                                    DeactivateAllNodes();
                                }
                            }
                            else if (i == 0)
                            {
                                hitObject.GetComponent<SpriteRenderer>().color = Color.green;
                                nodes[i].IsHIt = true;
                                if(sharpness < 1f)
                                    sharpness += (1f / nodes.Length) / 2f;
                            }

                        }


                    }
                }
                
            }
        }
        else
        {
            daggerHit = false;
            dagger.layer = 0;
            dagger.transform.position = startPosition;
            
        }
            
        if(Input.GetMouseButtonUp(0))
        {
            if (edgesSharp < 2)
                SoundManager.instance.PlayClipOneShot(dropClip);
        }
        if (daggerHit)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dagger.transform.localPosition = mousePos;
            dagger.layer = 2;
        }
        
        if (CheckAllNodes())
        {
            DeactivateAllNodes();
        }

        sharpnessMask.transform.localScale = new Vector3(sharpness, 1f, 1f);
        if(sharpness >= 0.99f)
        {
            sharpness = 0f;
            dagger.transform.Rotate(new Vector3(0, 180, 0));
            edgesSharp++;
            SoundManager.instance.PlayClipOneShot(progressClip);
        }

        if (edgesSharp >= 2)
        {
            edgesSharp = 0;
            daggerHit = false;
            SoundManager.instance.PlayClipOneShot(winClip);
            GameManager.instance.changeScene((int)GameManager.Scene.Museum);
        }
    }

    bool CheckAllNodes()
    {
        int nodesHit = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].IsHIt)
                nodesHit++;
            else
                return false;
        }
        if (nodesHit == nodes.Length)
            return true;

        return false;
    }

    void DeactivateAllNodes()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].IsHIt = false;
            nodes[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
