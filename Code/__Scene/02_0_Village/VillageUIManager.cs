using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageUIManager : MonoBehaviour, ICMInterface
{
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

    private void Awake()
    {
        this.info = new VillageUIInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitVillageUIManager();
    }

    public void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        if (this.TXT_UserNickname == null)
        {
            this.TXT_UserNickname = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "TXT_UserNickname", true).GetComponent<Text>();
        }

        if (this.TXT_UserCoin == null)
        {
            this.TXT_UserCoin = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "TXT_UserCoin", true).GetComponent<Text>();
        }

        if (this.RT_Villages == null)
        {
            this.RT_Villages = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "Villages", true).GetComponent<RectTransform>();
        }
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

        // TO DO : test
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

    private IEnumerator CoroutineMoveVillages(bool isLeft)
    {
        if (this.RT_Villages == null)
        {
            yield break;
        }

        WaitForSeconds WFS = new WaitForSeconds(0.01f);
        float currentTravelDistance = 0.0f;
        float targetTravelDistance = kVillageWidth;
        float moveWeight = (isLeft == true) ? -1.0f : 1.0f;
        Vector2 v = Vector2.zero;

        moveWeight *= this.info.villageMoveSpeed;

        while (true)
        {            
            v.x = this.RT_Villages.anchoredPosition.x + moveWeight;
            v.y = this.RT_Villages.anchoredPosition.y;
            this.RT_Villages.anchoredPosition = v;

            // 현재 이동 거리 갱신
            currentTravelDistance += moveWeight;

            // 목표 이동 거리를 이동했을 경우 break
            if (Math.Abs(currentTravelDistance) >= targetTravelDistance)
            {
                break;
            }

            yield return WFS;
        }

        // 오차 보정
        float lastWeight = (isLeft == true) ? -1.0f : 1.0f;
        lastWeight *= Math.Abs(currentTravelDistance) - targetTravelDistance;

        v.x = this.RT_Villages.anchoredPosition.x + lastWeight;
        v.y = this.RT_Villages.anchoredPosition.y;
        this.RT_Villages.anchoredPosition = v;
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
