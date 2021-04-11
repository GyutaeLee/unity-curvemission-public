using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VillageUIManager : MonoBehaviour
{
    private const float kVillageInitialXposition = 0.0f;
    private const float kVillageWidth = 180.0f;

    public class VillageUIInformation
    {
        public float villageMoveSpeed;

        public string userNickname;
        public int userCoin;
    }

    private VillageUIInformation info;

    private Text TXT_UserNickname;
    private Text TXT_UserCoin;
    private RectTransform RT_Villages;

    private bool isVillageMove;

    private void Awake()
    {
        this.info = new VillageUIInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitVillageUIManager();
    }

    private void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_UserNickname, mainCanvas, "TXT_UserNickname", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_UserCoin, mainCanvas, "TXT_UserCoin", true);
        CMObjectManager.CheckNullAndFindRectTransformInAllChild(ref this.RT_Villages, mainCanvas, "Villages", true);
    }

    private void InitVillageUIManager()
    {
        // 우측 마을에서 돌아올시 위치 이동
        string[] rightVillageSceneNames = { "security-related", "security-related", "security-related" };

        for (int i = 0; i < rightVillageSceneNames.Length; i++)
        {
            if (UserManager.instance.GetBeforeSceneName() == rightVillageSceneNames[i])
            {
                SetVillageToRight();
                break;
            }
        }

        this.info.userNickname = UserManager.instance.GetUserNickname();
        this.info.userCoin = UserManager.instance.GetUserCoin_1();

        this.info.villageMoveSpeed = 3.0f;

        // TEST CODE
        this.TXT_UserNickname.text = "NICKNAME : " + this.info.userNickname;
        this.TXT_UserCoin.text = "COIN : " + this.info.userCoin;
    }

    private void SetVillageToRight()
    {
        Vector2 v = new Vector2();

        v.x = this.RT_Villages.anchoredPosition.x - kVillageWidth;
        v.y = this.RT_Villages.anchoredPosition.y;

        this.RT_Villages.anchoredPosition = v;
    }

    public void MoveVillages(bool isLeft)
    {
        StartCoroutine(CoroutineMoveVillages(isLeft));
    }

    public void MoveVillagesToRight()
    {
        StartCoroutine(CoroutineMoveVillages(false));
    }

    public void MoveVillageToLeft()
    {
        StartCoroutine(CoroutineMoveVillages(true));
    }

    private bool IsValidVillagesPosition()
    {
        if (this.RT_Villages.anchoredPosition.x > kVillageInitialXposition)
        {
            return false;
        }

        if (this.RT_Villages.anchoredPosition.x < -kVillageWidth)
        {
            return false;
        }

        return true;
    }

    private IEnumerator CoroutineMoveVillages(bool isLeft)
    {
        if (this.RT_Villages == null || this.isVillageMove == true)
        {
            yield break;
        }

        WaitForSeconds WFS = new WaitForSeconds(0.01f);
        float currentTravelDistance = 0.0f;
        float targetTravelDistance = kVillageWidth;
        float moveWeight = (isLeft == true) ? -1.0f : 1.0f;
        Vector2 v = Vector2.zero;

        this.isVillageMove = true;
        moveWeight *= this.info.villageMoveSpeed;

        while (Math.Abs(currentTravelDistance) < targetTravelDistance)
        {            
            v.x = this.RT_Villages.anchoredPosition.x + moveWeight;
            v.y = this.RT_Villages.anchoredPosition.y;
            this.RT_Villages.anchoredPosition = v;

            currentTravelDistance += moveWeight;

            if (IsValidVillagesPosition() == false)
            {
                yield break;
            }
            else
            {
                yield return WFS;
            }
        }

        this.RT_Villages.anchoredPosition = MoveVillageErrorCorrection(this.RT_Villages.anchoredPosition, isLeft, currentTravelDistance, targetTravelDistance); ;
        this.isVillageMove = false;
    }

    private Vector2 MoveVillageErrorCorrection(Vector2 anchoredPosition, bool isLeft, float currentTravelDistance, float targetTravelDistance)
    {
        float lastWeight = Math.Abs(currentTravelDistance) - targetTravelDistance;
        lastWeight *= (isLeft == true) ? -1.0f : 1.0f;

        Vector2 v = new Vector2();
        v.x = anchoredPosition.x + lastWeight;
        v.y = anchoredPosition.y;

        return v;
    }

#if (CHEAT_MODE)
    public void CHEAT_AddCoin(int addCoinCount)
    {
        ServerManager.instance.PostUserCoinToFirebaseDB(addCoinCount);
    }
    public void CHEAT_UpdateCoin()
    {
        this.info.userCoin = UserManager.instance.GetUserCoin_1();
        this.TXT_UserCoin.text = "COIN : " + this.info.userCoin;
    }
#endif
}
