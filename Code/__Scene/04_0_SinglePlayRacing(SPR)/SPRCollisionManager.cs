using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPRCollisionManager : MonoBehaviour
{   
    // Parent : CollisionInformation
    public class CollisionBase
    {
        public SPRCollisionManager sprCollisionManager;
        public GameObject collisionObject;

        public bool isActive;

        public CollisionBase(GameObject o)
        {
            this.collisionObject = o;
            this.sprCollisionManager = collisionObject.GetComponent<SPRCollisionManager>();

            this.isActive = true;
        }

        public virtual void Init()
        {

        }

        public virtual void Collide()
        {

        }

        public void PlaySound()
        {
            ESoundType e = ESoundType.None;
            int index = 0;

            GetSoundInfo(ref e, ref index);
            SoundManager.instance.PlaySound(e, index);
        }

        public virtual void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {

        }
    }

    public class CollisionDirection : CollisionBase
    {
        public ESPRCollisionDirection eSPRCollisionDirection;
        private GameObject childObject;

        public CollisionDirection(GameObject o, ESPRCollisionDirection e) : base(o)
        {
            this.eSPRCollisionDirection = e;
        }

        public override void Init()
        {
            if (this.childObject == null)
            {
                if (this.collisionObject.transform.childCount > 0)
                {
                    this.childObject = this.collisionObject.transform.GetChild(0).gameObject;
                }
            }
        }

        public new ESPRCollisionDirection Collide()
        {
            if (this.childObject != null)
            {
                this.sprCollisionManager.StartCoroutineBlinkSpriteByColor(this.childObject.GetComponent<SpriteRenderer>(), new Color(0.3f, 0.3f, 0.3f, 0.3f), 3.0f);
            }

            return this.eSPRCollisionDirection;
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {

        }
    }

    public class CollisionWall : CollisionBase
    {
        public ESPRCollisionWall eSPRCollisionWall;
        public Vector3 resetPosition;

        public CollisionWall(GameObject o, ESPRCollisionWall e, Vector3 p) : base(o)
        {
            this.eSPRCollisionWall = e;
            this.resetPosition = p;
        }

        public override void Init()
        {

        }

        public new ESPRCollisionWall Collide()
        {
            return this.eSPRCollisionWall;
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {
            switch (this.eSPRCollisionWall)
            {
                case ESPRCollisionWall.Normal:
                    eSoundType = ESoundType.Collision;
                    soundIndex = (int)ESoundCollision.Rock_1;
                    break;
                default:
                    eSoundType = ESoundType.None;
                    soundIndex = 0;
                    break;
            }
        }
    }

    public class CollisionItem : CollisionBase
    {
        public ESPRCollisionItem eSPRCollisionItem;
        public int quantity;

        public CollisionItem(GameObject o, ESPRCollisionItem e, int q) : base(o)
        {
            this.eSPRCollisionItem = e;
            this.quantity = q;
        }

        public override void Init()
        {

        }

        public override void Collide()
        {
            switch (this.eSPRCollisionItem)
            {
                case ESPRCollisionItem.Coin:
                    SPRGameManager.instance.AddCoinQuantity(this.quantity);
                    SPRGameManager.instance.RefreshUICoin();

                    this.isActive = false;
                    this.collisionObject.GetComponent<Animator>().SetBool("security-related", true);

                    this.sprCollisionManager.StartCoroutineActiveGameObject(this.collisionObject, false, 0.8f);
                    break;
                default:
                    break;
            }
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {
            switch (this.eSPRCollisionItem)
            {
                case ESPRCollisionItem.Coin:
                    eSoundType = ESoundType.Collision;
                    soundIndex = (int)ESoundCollision.Coin_1;
                    break;
                default:
                    eSoundType = ESoundType.None;
                    soundIndex = 0;
                    break;
            }
        }

    }

    public class CollisionBooster : CollisionBase
    {
        public GameObject childObject;
        public ESPRCollisionBooster eSPRCollisionBooster;
        public List<Sprite> SPT_Booster;    // [0] : origin, [1] : blink

        public CollisionBooster(GameObject o, ESPRCollisionBooster e, List<Sprite> s) : base(o)
        {
            this.eSPRCollisionBooster = e;
            this.SPT_Booster = s;
        }

        public override void Init()
        {
            if (this.childObject == null)
            {
                if (this.collisionObject.transform.childCount > 0)
                {
                    this.childObject = this.collisionObject.transform.GetChild(0).gameObject;
                }
            }
        }

        public new ESPRCollisionBooster Collide()
        {
            if (this.childObject != null)
            {
                this.sprCollisionManager.StartCoroutineBlinkSpriteBySprite(this.childObject.GetComponent<SpriteRenderer>(), this.SPT_Booster[0], this.SPT_Booster[1], 1.5f);
            }

            return this.eSPRCollisionBooster;
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {
            switch (this.eSPRCollisionBooster)
            {
                case ESPRCollisionBooster.Booster_1:
                    eSoundType = ESoundType.Collision;
                    soundIndex = (int)ESoundCollision.Booster_1;
                    break;
                case ESPRCollisionBooster.Booster_2:
                    eSoundType = ESoundType.Collision;
                    soundIndex = (int)ESoundCollision.Booster_2;
                    break;
                default:
                    eSoundType = ESoundType.None;
                    soundIndex = 0;
                    break;
            }
        }
    }

    public class CollisionObstacle : CollisionBase
    {
        public ESPRCollisionObstacle eSPRCollisionObstacle;

        public CollisionObstacle(GameObject o, ESPRCollisionObstacle e) : base(o)
        {
            this.eSPRCollisionObstacle = e;
        }


        public new ESPRCollisionObstacle Collide()
        {
            return this.eSPRCollisionObstacle;
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {
            switch (this.eSPRCollisionObstacle)
            {
                case ESPRCollisionObstacle.Obstacle_1:
                    eSoundType = ESoundType.Collision;
                    soundIndex = (int)ESoundCollision.Rock_1;
                    break;
                case ESPRCollisionObstacle.Obstacle_2:
                    break;
                default:
                    eSoundType = ESoundType.None;
                    soundIndex = 0;
                    break;
            }
        }
    }

    public class CollisionLap : CollisionBase
    {
        public ESPRCollisionLap eSPRCollisionLap;

        public CollisionLap(GameObject o, ESPRCollisionLap e) : base(o)
        {
            this.eSPRCollisionLap = e;
        }

        public override void Init()
        {

        }

        public new ESPRCollisionLap Collide()
        {
            return this.eSPRCollisionLap;
        }

        public override void GetSoundInfo(ref ESoundType eSoundType, ref int soundIndex)
        {

        }
    }

    //////////////////
    /* End of Class */
    //////////////////

    public ESPRCollision eSPRCollision;

    public int enumValue;
    public List<float> param;
    public List<Sprite> spts;

    public CollisionBase info;

    private void Start()
    {
        InitSPRCollisionManager();
    }

    private void InitSPRCollisionManager()
    {
        switch (this.eSPRCollision)
        {
            case ESPRCollision.Direction:
                this.info = new CollisionDirection(this.gameObject, (ESPRCollisionDirection)this.enumValue);
                break;

            case ESPRCollision.Wall:
                Vector3 resetPosition = Vector3.zero;

                if (this.param.Count >= 2)
                {
                    resetPosition.x = this.param[0];
                    resetPosition.y = this.param[1];
                }

                this.info = new CollisionWall(this.gameObject, (ESPRCollisionWall)this.enumValue, resetPosition);
                break;

            case ESPRCollision.Item:
                int quantity = 0;

                if (this.param.Count > 0)
                {
                    quantity = (int)this.param[0];
                }

                this.info = new CollisionItem(this.gameObject, (ESPRCollisionItem)this.enumValue, quantity);
                break;

            case ESPRCollision.Booster:
                this.info = new CollisionBooster(this.gameObject, (ESPRCollisionBooster)this.enumValue, spts);
                break;

            case ESPRCollision.Obstacle:
                this.info = new CollisionObstacle(this.gameObject, (ESPRCollisionObstacle)this.enumValue);
                break;

            case ESPRCollision.Lap:
                this.info = new CollisionLap(this.gameObject, (ESPRCollisionLap)this.enumValue);
                break;
            default:
                break;
        }

        this.info.Init();
    }

    /* Collide */
    public void Collide()
    {
        this.info.Collide();
    }

    public void StartCoroutineBlinkSpriteByColor(SpriteRenderer sr, Color c, float blinkTerm)
    {
        StartCoroutine(CoroutineBlinkSpriteByColor(sr, c, blinkTerm));
    }

    public void StartCoroutineBlinkSpriteBySprite(SpriteRenderer sr, Sprite originSprite, Sprite blinkSprite, float blinkTerm)
    {
        StartCoroutine(CoroutineBlinkSpriteBySprite(sr, originSprite, blinkSprite, blinkTerm));
    }

    public void StartCoroutineActiveGameObject(GameObject o, bool isEnabled, float activeTerm)
    {
        StartCoroutine(CoroutineActiveGameObject(o, isEnabled, activeTerm));
    }

    private IEnumerator CoroutineBlinkSpriteByColor(SpriteRenderer sr, Color c, float blinkTerm)
    {
        if (sr.color == c)
        {
            yield break;
        }

        Color originColor = sr.color;

        sr.color = c;

        yield return new WaitForSeconds(blinkTerm);

        sr.color = originColor;
    }

    private IEnumerator CoroutineBlinkSpriteBySprite(SpriteRenderer sr, Sprite originSprite, Sprite blinkSprite, float blinkTerm)
    {
        if (sr.sprite == blinkSprite)
        {
            yield break;
        }

        sr.sprite = blinkSprite;

        yield return new WaitForSeconds(blinkTerm);

        sr.sprite = originSprite;
    }

    private IEnumerator CoroutineActiveGameObject(GameObject o, bool bActive, float activeTerm)
    {
        yield return new WaitForSeconds(activeTerm);

        o.SetActive(bActive);
    }
}
