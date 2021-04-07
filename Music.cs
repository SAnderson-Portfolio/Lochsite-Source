using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Music : MonoBehaviour
{
    struct Strings
    {
        GameObject stringObject;
        bool isHit;
        float hitTime;

        public GameObject StringObject
        {
            get { return stringObject; }
            set { stringObject = value; }
        }

        public bool IsHit
        {
            get { return isHit; }
            set { isHit = value; }
        }

        public float HitTime
        {
            get { return hitTime; }
            set { hitTime = value; }
        }
    }

    [Header("String Audio")]
    public AudioClip stringOne;
    public AudioClip stringTwo;
    public AudioClip stringThree;
    public AudioClip stringFour;
    public AudioClip stringFive;
    public AudioClip stringSix;
    public AudioClip stringSeven;
    public AudioClip buttonSound;
    private SpriteRenderer backgroundRenderer;
    Strings[] stringList;
    // Start is called before the first frame update
    void Start()
    {
        stringList = new Strings[7];
        GameObject musicBox = new GameObject();
        GameObject stringOne = new GameObject();
        GameObject stringTwo = new GameObject();
        GameObject stringThree = new GameObject();
        GameObject stringFour = new GameObject();
        GameObject stringFive = new GameObject();
        GameObject stringSix = new GameObject();
        GameObject stringSeven = new GameObject();

        musicBox.AddComponent<SpriteRenderer>();
        musicBox.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Lyre/Asset_lyre");
        musicBox.name = "Lyre";

        stringOne.AddComponent<BoxCollider2D>();
        stringTwo.AddComponent<BoxCollider2D>();
        stringThree.AddComponent<BoxCollider2D>();
        stringFour.AddComponent<BoxCollider2D>();
        stringFive.AddComponent<BoxCollider2D>();
        stringSix.AddComponent<BoxCollider2D>();
        stringSeven.AddComponent<BoxCollider2D>();

        stringOne.transform.parent = musicBox.transform;
        stringTwo.transform.parent = musicBox.transform;
        stringThree.transform.parent = musicBox.transform;
        stringFour.transform.parent = musicBox.transform;
        stringFive.transform.parent = musicBox.transform;
        stringSix.transform.parent = musicBox.transform;
        stringSeven.transform.parent = musicBox.transform;

        stringOne.name = "String One";
        stringOne.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringOne.transform.localPosition = new Vector3(0.4f, 0.95f, 0f);
        stringOne.transform.Rotate(new Vector3(0, 0, 85.57f));

        stringTwo.name = "String Two";
        stringTwo.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringTwo.transform.localPosition = new Vector3(0.08f, 0.65f, 0);
        stringTwo.transform.Rotate(new Vector3(0, 0, 87f));

        stringThree.name = "String Three";
        stringThree.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringThree.transform.localPosition = new Vector3(0.08f, 0.3f, 0);
        stringThree.transform.Rotate(new Vector3(0, 0, 88.73f));

        stringFour.name = "String Four";
        stringFour.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringFour.transform.localPosition = new Vector3(0.07f, 0f, 0);
        stringFour.transform.Rotate(new Vector3(0, 0, 89.59f));

        stringFive.name = "String Five";
        stringFive.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringFive.transform.localPosition = new Vector3(0.08f, -0.39f, 0);
        stringFive.transform.Rotate(new Vector3(0, 0, 91.7f));

        stringSix.name = "String Six";
        stringSix.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringSix.transform.localPosition = new Vector3(0.09f, -0.69f, 0);
        stringSix.transform.Rotate(new Vector3(0, 0, 93.3f));

        stringSeven.name = "String Seven";
        stringSeven.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 12);
        stringSeven.transform.localPosition = new Vector3(0.11f, -1f, 0);
        stringSeven.transform.Rotate(new Vector3(0, 0, 94f));
        
        //String one
        stringList[0].StringObject = stringOne;
        stringList[0].IsHit = false;
        stringList[0].HitTime = -1f;
        //String two
        stringList[1].StringObject = stringTwo;
        stringList[1].IsHit = false;
        stringList[1].HitTime = -1f;
        //String three
        stringList[2].StringObject = stringThree;
        stringList[2].IsHit = false;
        stringList[2].HitTime = -1f;
        //String four
        stringList[3].StringObject = stringFour;
        stringList[3].IsHit = false;
        stringList[3].HitTime = -1f;
        //String five
        stringList[4].StringObject = stringFive;
        stringList[4].IsHit = false;
        stringList[4].HitTime = -1f;
        //String six
        stringList[5].StringObject = stringSix;
        stringList[5].IsHit = false;
        stringList[5].HitTime = -1f;
        //String seven
        stringList[6].StringObject = stringSeven;
        stringList[6].IsHit = false;
        stringList[6].HitTime = -1f;

        backgroundRenderer = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        backgroundRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        backgroundRenderer.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/Backg_CrannogInterior");

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert the screen point to world point.
            Vector3 mousePos2D = new Vector2(mousePos.x, mousePos.y);//Remove the Z-axis for 2D position.

            RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero);//Raycast from mouse position in a straight line.
            if(hit2D.collider != null)
            {
                //String one
                if (hit2D.collider.gameObject == stringList[0].StringObject && !stringList[0].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringOne);
                    stringList[0].HitTime = Time.time;
                    stringList[0].IsHit = true;
                }
                //String two
                if (hit2D.collider.gameObject == stringList[1].StringObject && !stringList[1].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringTwo);
                    stringList[1].HitTime = Time.time;
                    stringList[1].IsHit = true;
                }
                //String three
                if (hit2D.collider.gameObject == stringList[2].StringObject && !stringList[2].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringThree);
                    stringList[2].HitTime = Time.time;
                    stringList[2].IsHit = true;
                }
                //String four
                if (hit2D.collider.gameObject == stringList[3].StringObject && !stringList[3].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringFour);
                    stringList[3].HitTime = Time.time;
                    stringList[3].IsHit = true;
                }
                //String five
                if (hit2D.collider.gameObject == stringList[4].StringObject && !stringList[4].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringFive);
                    stringList[4].HitTime = Time.time;
                    stringList[4].IsHit = true;

                }
                //String six
                if (hit2D.collider.gameObject == stringList[5].StringObject && !stringList[5].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringSix);
                    stringList[5].HitTime = Time.time;
                    stringList[5].IsHit = true;
                }
                //String seven
                if (hit2D.collider.gameObject == stringList[6].StringObject && !stringList[6].IsHit)
                {
                    SoundManager.instance.PlayClipOneShot(stringSeven);
                    stringList[6].HitTime = Time.time;
                    stringList[6].IsHit = true;
                }
            }
        }
        else
        {
            ResetHits();
        }
       
        if(stringList[0].IsHit && stringList[1].IsHit && stringList[2].IsHit && stringList[3].IsHit && stringList[4].IsHit && stringList[5].IsHit && stringList[6].IsHit)
        {
            ResetHits();
        }

        for(int i = 0; i < stringList.Length; i++)
        {
            if(stringList[i].IsHit)
            {
                if (stringList[i].HitTime != -1f)
                {
                    if (Time.time > stringList[i].HitTime + 0.3f)
                    {
                        stringList[i].HitTime = -1f;
                        stringList[i].IsHit = false;
                    }
                }
               
            }
        }
        
    }

    void ResetHits()
    {
        for (int i = 0; i < stringList.Length; i++)
            stringList[i].IsHit = false;
    }

    public void GoToMuseum()
    {
        if (!GameManager.instance.IsInTransition())
        {
            SoundManager.instance.PlayClipOneShot(buttonSound);
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
