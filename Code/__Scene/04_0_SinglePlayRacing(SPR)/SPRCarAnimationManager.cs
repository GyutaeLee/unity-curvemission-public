using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ESPRCollisionBooster와 맞추어서 가야함
public enum EBoosterLevel
{
    LEVEL_1,
    LEVEL_2,

    Max
}

public class SPRCarAnimationManager : MonoBehaviour, ICMInterface
{
    public class SPRCarAnimationInformation
    {
        public int currentBoosterLevel;
        public List<List<GameObject>> boosterObject;

        public Dictionary<string, Sprite> spriteSheet;
        public string loadedSpriteSheetName;

        public float animationSpeed;
        public float collisionAnimationTerm;

        public bool isBoosterAnimEnabled;
        public int boosterAnimIndex;

        public int collisionAnimationNumberOfTime;
    }

    private SPRCarAnimationInformation info;

    private SPRCarManager sprCarManager;

    private GameObject carObject;
    private SpriteRenderer _SpriteRenderer;
    private Animator _Animator;

    private void Awake()
    {
        this.info = new SPRCarAnimationInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRCarAnimationManager();
    }

    private void FixedUpdate()
    {
        UpdateAnimationSpeed();
    }

    private void LateUpdate()
    {
        LateUpdateCarAnimationSprite();
    }

    public void PrepareBaseObjects()
    {
        if (this.carObject == null)
        {
            this.carObject = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Game"), "SPRCar", true);
        }

        if (this.info.boosterObject == null)
        {
            GameObject boosterObject = CMObjectManager.FindGameObjectInAllChild(this.carObject, "BOOSTER_OBJECT", true);
            int childCount01 = boosterObject.transform.childCount;

            this.info.boosterObject = new List<List<GameObject>>(new List<GameObject>[childCount01]);
            for (int i = 0; i < childCount01; i++)
            {
                GameObject boosterLevelObject = boosterObject.transform.GetChild(i).gameObject;
                int childCount02 = boosterLevelObject.transform.childCount;

                this.info.boosterObject[i] = new List<GameObject>();
                for (int j = 0; j < childCount02; j++)
                {
                    this.info.boosterObject[i].Add(boosterLevelObject.transform.GetChild(j).gameObject);
                }
            }
        }

        if (this._SpriteRenderer == null)
        {
            this._SpriteRenderer = CMObjectManager.FindGameObjectInAllChild(this.carObject, "SPRCarAnimation", true).GetComponent<SpriteRenderer>();
        }

        if (this._Animator == null)
        {
            this._Animator = CMObjectManager.FindGameObjectInAllChild(this.carObject, "SPRCarAnimation", true).GetComponent<Animator>();
        }

        this.info.collisionAnimationTerm = 0.2f;
        this.info.collisionAnimationNumberOfTime = 5;

        if (this.sprCarManager == null)
        {
            this.sprCarManager = this.carObject.GetComponent<SPRCarManager>();
        }
    }

    /* Init Function */

    private void InitSPRCarAnimationManager()
    {
        int carInfoID = SecurityPlayerPrefs.GetInt("security-related", 0);
        int carPaintID = SecurityPlayerPrefs.GetInt("security-related", 0);
        LoadCarAnimationResource(carInfoID, carPaintID);

        // 시작 차량에 맞게 애니메이션 준비
        SetCaraAnimationStateByCarState((int)sprCarManager.GetCurrentCarState());
    }

    private void LoadCarAnimationResource(int carInfoID, int carPaintID)
    {
        string carSpriteSheetName = "security-related" + carInfoID + "/" + carPaintID;

        // Load the sprites from a sprite sheet file (png).
        var sprites = Resources.LoadAll<Sprite>(carSpriteSheetName);
        this.info.spriteSheet = sprites.ToDictionary(x => x.name, x => x);

        this.info.loadedSpriteSheetName = carSpriteSheetName;
    }

    /* Update Function */

    private void UpdateAnimationSpeed()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            this._Animator.speed = 0;
            return;
        }

        if (this._Animator.GetCurrentAnimatorStateInfo(0).IsTag("security-related") == true)
        {
            // TO DO : 0 ~ 3 사이가 애니메이션 속도가 적당하다?
            this._Animator.speed = sprCarManager.GetCurrentSpeed();
        }
        else if (this._Animator.GetCurrentAnimatorStateInfo(0).IsTag("security-related") == true)
        {
            this._Animator.speed = 1.0f;
        }
        else
        {
            this._Animator.speed = 1.0f;
        }
    }

    private void LateUpdateCarAnimationSprite()
    {
        // Swap out the sprite to be rendered by its name
        // Important: The name of the sprite must be the same!
        this._SpriteRenderer.sprite = this.info.spriteSheet[this._SpriteRenderer.sprite.name];
    }

    /* Animation */

    public void SetCaraAnimationStateByCarState(int carState)
    {
        this._Animator.SetInteger("security-related", carState);
    }

    public void PlayCarBoosterAniamtion(EBoosterLevel eBoosterLevel, bool isBoosterAnimEnabled)
    {
        if (eBoosterLevel >= EBoosterLevel.Max || eBoosterLevel < 0)
        {
            return;
        }

        ECarState eCurrentCarState = this.sprCarManager.GetCurrentCarState();

        // 이전 부스터 애니메이션이 켜져 있는 경우 끄기 위해
        if (isBoosterAnimEnabled == true)
        {
            this.info.boosterObject[this.info.boosterAnimIndex][(int)eCurrentCarState - 1].SetActive(false);         
        }

        this.info.boosterAnimIndex = (int)eBoosterLevel;
        this.info.isBoosterAnimEnabled = isBoosterAnimEnabled;

        this.info.boosterObject[this.info.boosterAnimIndex][(int)eCurrentCarState - 1].SetActive(isBoosterAnimEnabled);
        this._Animator.SetBool("security-related", isBoosterAnimEnabled);
    }

    public void StopCarBoosterAnimation()
    {
        if (this.info.isBoosterAnimEnabled == false)
        { 
            return;
        }

        ECarState eCurrentCarState = this.sprCarManager.GetCurrentCarState();

        this.info.boosterObject[this.info.boosterAnimIndex][(int)eCurrentCarState - 1].SetActive(false);

        this.info.isBoosterAnimEnabled = false;
        this.info.boosterAnimIndex = 0;

        this._Animator.SetBool("security-related", false);
    }

    public void PlayCarCollisionAnimation()
    {
        StartCoroutine(CoroutineCarCollisionAnimation(this.info.collisionAnimationTerm, this.info.collisionAnimationNumberOfTime));
    }

    public void PlayCarCollisionAnimationWithTerm(float collisionAnimationTerm, int collisionAnimationNumberOfTime)
    {
        StartCoroutine(CoroutineCarCollisionAnimation(collisionAnimationTerm, collisionAnimationNumberOfTime));
    }

    private IEnumerator CoroutineCarCollisionAnimation(float collisionAnimationTerm, int collisionAnimationNumberOfTime)
    {
        WaitForSeconds WFS = new WaitForSeconds(collisionAnimationTerm);

        for (int count = 0; count < collisionAnimationNumberOfTime; count++)
        {
            this._SpriteRenderer.color = Color.gray;
            yield return WFS;

            this._SpriteRenderer.color = Color.white;
            yield return WFS;
        }
    }
    
}
