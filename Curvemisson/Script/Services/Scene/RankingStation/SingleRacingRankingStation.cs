using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Services.Item;
using Services.Character;
using Services.GameText;
using Services.Useful;

using Services.Enum.Item;
using Services.Enum.GameText;

namespace Services.Scene.RankingStation
{
    public class SingleRacingRankingStation : MonoBehaviour
    {
        private static class Constants
        {
            public const int ElseRankingFirstIndex = 3;
        }

        private enum UIState
        {
            None = 0,

            Main = 1,
            MapComboBox = 2,
            UserRanking = 3,
            TotalRanking = 4,

            Max
        }

        private class RankerRecordLine
        {
            public Character.Avatar Avatar;

            public Image BestLapCarImage;

            public Button RecordCardButton;

            public UnityEngine.UI.Text RankingNumberText;
            public UnityEngine.UI.Text BestLapTimeText;
            public UnityEngine.UI.Text NicknameText;
        }

        private class RecordCard
        {
            public AvatarUI AvatarUI;

            public Image BestLapCarImage;

            public UnityEngine.UI.Text MapNameText;
            public UnityEngine.UI.Text RankingNumberText;
            public UnityEngine.UI.Text BestLapTimeText;
            public UnityEngine.UI.Text NicknameText;
        }

        private UIState currentUIState;

        private int currentRankingStageID;
        private int currentElseRankingFirstIndex;

        private Dictionary<int, List<RankingData>> rankingInformationDictionary;

        private RecordCard userRecordCard;
        private GameObject userRecordCardObject;

        private RecordCard driverRecordCard;
        private GameObject driverRecordCardObject;

        private List<RankerRecordLine> topRankerRecordLines;
        private List<RankerRecordLine> elseRankerRecordLines;

        private GameObject singlePlayRankingStationCanvas;

        private GameObject mapComboBoxObject;
        private GameObject opendMapComboBoxObject;
        private GameObject totalRankingObject;
        private GameObject topRankingObject;
        private GameObject elseRankingObject;

        private Button mapComboxBoxButton;
        private Button closeMapComboBoxButton;
        private Button upMapComboBoxButton;
        private Button downMapComboBoxButton;

        private Button upElseRankingButton;
        private Button downElseRankingButton;

        private Button userRecordCardButton;
        private Button totalRankingButton;

        private Button backButton;
            
        public void Open(Dictionary<int, List<RankingData>> rankingInformationDictionary)
        {
            this.rankingInformationDictionary = rankingInformationDictionary;

            InstantiateCanvas();

            PrepareObjects();
            PrepareButtons();

            Initialize();
        }

        private void Close()
        {
            Destroy(this.singlePlayRankingStationCanvas);
            Destroy(this);
        }

        private void InstantiateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");

            this.singlePlayRankingStationCanvas = Resources.Load<GameObject>("Prefab/UI/Canvas/SingleRacingRankingStationCanvas");
            this.singlePlayRankingStationCanvas = Instantiate(this.singlePlayRankingStationCanvas, canvas.transform);
            this.singlePlayRankingStationCanvas.SetActive(true);
        }

        private void PrepareObjects()
        {
            ObjectFinder.FindGameObjectInAllChild(ref this.totalRankingObject, this.singlePlayRankingStationCanvas, "TotalRankingObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.mapComboBoxObject, this.singlePlayRankingStationCanvas, "MapComboBoxObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.opendMapComboBoxObject, this.singlePlayRankingStationCanvas, "OpendMapComboBoxObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.topRankingObject, this.totalRankingObject, "TopRankingObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.elseRankingObject, this.totalRankingObject, "ElseRankingObject", true);

            GameObject canvas = GameObject.Find("Canvas");
            ObjectFinder.FindGameObjectInAllChild(ref this.userRecordCardObject, canvas, "UserRecordCardObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.driverRecordCardObject, canvas, "DriverRecordCardObject", true);

            PrepareRecordCardObject(ref this.userRecordCard, this.userRecordCardObject);
            PrepareRecordCardObject(ref this.driverRecordCard, this.driverRecordCardObject);

            PrepareTotalTopRankerRecordLines();
            PrepareTotalElseRankerRecordLines();
        }

        private void PrepareRecordCardObject(ref RecordCard recordCard, GameObject parentObject)
        {
            GameObject avatarObject = null;
            ObjectFinder.FindGameObjectInAllChild(ref avatarObject, parentObject, "AvatarObject", true);

            recordCard = new RecordCard();
            recordCard.AvatarUI = new AvatarUI(avatarObject);

            ObjectFinder.FindComponentInAllChild(ref recordCard.BestLapCarImage, parentObject, "BestLapCarImage", true);
            ObjectFinder.FindComponentInAllChild(ref recordCard.MapNameText, parentObject, "MapNameText", true);
            ObjectFinder.FindComponentInAllChild(ref recordCard.NicknameText, parentObject, "NicknameText", true);
            ObjectFinder.FindComponentInAllChild(ref recordCard.BestLapTimeText, parentObject, "BestLapTimeText", true);
            ObjectFinder.FindComponentInAllChild(ref recordCard.RankingNumberText, parentObject, "RankingNumberText", true);
        }

        private void PrepareTotalTopRankerRecordLines()
        {
            this.topRankerRecordLines = new List<RankerRecordLine>(new RankerRecordLine[this.topRankingObject.transform.childCount]);

            for (int i = 0; i < this.topRankingObject.transform.childCount; i++)
            {
                GameObject rankerRecordLineObject = this.topRankingObject.transform.GetChild(i).gameObject;
                RankerRecordLine rankerRecordLine = new RankerRecordLine();

                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.BestLapCarImage, rankerRecordLineObject, "BestLapCarImage", true);
                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.NicknameText, rankerRecordLineObject, "NicknameText", true);
                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.BestLapTimeText, rankerRecordLineObject, "BestLapTimeText", true);

                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.RecordCardButton, rankerRecordLineObject, "RecordCardButton", true);
                AddOpenDriverRankingUIListener(rankerRecordLine.RecordCardButton, i);

                this.topRankerRecordLines[i] = rankerRecordLine;
            }
        }

        private void PrepareTotalElseRankerRecordLines()
        {
            this.elseRankerRecordLines = new List<RankerRecordLine>(new RankerRecordLine[this.elseRankingObject.transform.childCount]);

            for (int i = 0; i < this.elseRankingObject.transform.childCount; i++)
            {
                GameObject rankerRecordLineObject = this.elseRankingObject.transform.GetChild(i).gameObject;
                RankerRecordLine rankerRecordLine = new RankerRecordLine();

                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.BestLapCarImage, rankerRecordLineObject, "BestLapCarImage", true);
                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.NicknameText, rankerRecordLineObject, "NicknameText", true);
                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.BestLapTimeText, rankerRecordLineObject, "BestLapTimeText", true);
                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.RankingNumberText, rankerRecordLineObject, "RankingNumberText", true);

                ObjectFinder.FindComponentInAllChild(ref rankerRecordLine.RecordCardButton, rankerRecordLineObject, "RecordCardButton", true);
                AddOpenDriverRankingUIListener(rankerRecordLine.RecordCardButton, i + Constants.ElseRankingFirstIndex);

                this.elseRankerRecordLines[i] = rankerRecordLine;
            }
        }

        private void AddOpenDriverRankingUIListener(Button button, int index)
        {
            button.onClick.AddListener(() => { OpenDriverRecordCard(index); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void OpenDriverRecordCard(int index)
        {
            if (ChangeDriverRecordCard(index) == true)
            {
                this.driverRecordCardObject.SetActive(true);
            }
        }

        private bool ChangeDriverRecordCard(int index)
        {
            RankerRecordLine rankerRecordLine;

            if (index < Constants.ElseRankingFirstIndex)
            {
                rankerRecordLine = this.topRankerRecordLines[index];
            }
            else
            {
                rankerRecordLine = this.elseRankerRecordLines[index - Constants.ElseRankingFirstIndex];
            }

            if (rankerRecordLine.Avatar == null)
            {
                return false;
            }

            if (this.driverRecordCard.AvatarUI == null)
            {
                this.driverRecordCard.AvatarUI = new AvatarUI(rankerRecordLine.Avatar, this.driverRecordCardObject);
            }
            else
            {
               this.driverRecordCard.AvatarUI.ChangeAvatar(rankerRecordLine.Avatar);
            }

            this.driverRecordCard.BestLapCarImage.enabled = true;
            this.driverRecordCard.BestLapCarImage.sprite = rankerRecordLine.BestLapCarImage.sprite;

            this.driverRecordCard.MapNameText.text = Manager.Instance.GetText(TextType.Stage, this.currentRankingStageID);
            //this.sprDriverIDCard.TXT_RankingNumber.text = copy.TXT_RankingNumber.text;
            //this.sprDriverIDCard.TXT_BestLapCar
            this.driverRecordCard.BestLapTimeText.text = rankerRecordLine.BestLapTimeText.text;
            this.driverRecordCard.NicknameText.text = rankerRecordLine.NicknameText.text;

            return true;
        }

        private void PrepareButtons()
        {
            ObjectFinder.FindComponentInAllChild(ref this.mapComboxBoxButton, this.mapComboBoxObject, "MapComboxBoxButton", true);
            this.mapComboxBoxButton.onClick.AddListener(() => { ClickMapComboBoxButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.closeMapComboBoxButton, this.mapComboBoxObject, "CloseMapComboBoxButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.upMapComboBoxButton, this.mapComboBoxObject, "UpMapComboBoxButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.downMapComboBoxButton, this.mapComboBoxObject, "DownMapComboBoxButton", true);
            this.closeMapComboBoxButton.onClick.AddListener(() => { ClickBackButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.upMapComboBoxButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.downMapComboBoxButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.upElseRankingButton, this.totalRankingObject, "UpElseRankingButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.downElseRankingButton, this.totalRankingObject, "DownElseRankingButton", true);
            this.upElseRankingButton.onClick.AddListener(() => { ClickUpElseRankingButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.downElseRankingButton.onClick.AddListener(() => { ClickDownElseRankingButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.userRecordCardButton, this.singlePlayRankingStationCanvas, "UserRecordCardButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.totalRankingButton, this.singlePlayRankingStationCanvas, "TotalRankingButton", true);
            this.userRecordCardButton.onClick.AddListener(() => { OpenUserRecordCard(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.totalRankingButton.onClick.AddListener(() => { OpenTotalRanking(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.backButton, this.singlePlayRankingStationCanvas, "BackButton", true);
            this.backButton.onClick.AddListener(() => { ClickBackButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void ClickMapComboBoxButton()
        {
            if (this.currentUIState == UIState.MapComboBox)
            {
                CloseOpendMapComboBoxObject();
            }
            else
            {
                OpenOpendMapComboBoxObject();
            }
        }

        private void OpenOpendMapComboBoxObject()
        {
            this.opendMapComboBoxObject.SetActive(true);

            this.currentUIState = UIState.MapComboBox;
        }

        private void CloseOpendMapComboBoxObject()
        {
            this.opendMapComboBoxObject.SetActive(false);

            if (this.userRecordCardObject.activeSelf == true)
            {
                this.currentUIState = UIState.UserRanking;
            }
            else if (this.totalRankingObject.activeSelf == true)
            {
                this.currentUIState = UIState.TotalRanking;
            }
            else
            {
                this.currentUIState = UIState.Main;
            }
        }

        private void ClickBackButton()
        {
            switch (this.currentUIState)
            {
                case UIState.Main:
                    Close();
                    break;
                case UIState.MapComboBox:
                    ClickBackButtonAtMapComboBoxState();
                    break;
                case UIState.UserRanking:
                    ClickBackButtonAtUserRankingState();
                    break;
                case UIState.TotalRanking:
                    ClickBackButtonAtTotalRankingState();
                    break;
                default:
                    break;
            }
        }

        private void ClickBackButtonAtMapComboBoxState()
        {
            this.opendMapComboBoxObject.SetActive(false);

            if (this.userRecordCardObject.activeSelf == true)
            {
                this.currentUIState = UIState.UserRanking;
            }
            else if (this.totalRankingObject.activeSelf == true)
            {
                this.currentUIState = UIState.TotalRanking;
            }
            else
            {
                this.currentUIState = UIState.Main;
            }
        }
        
        private void ClickBackButtonAtUserRankingState()
        {
            this.userRecordCardObject.SetActive(false);
            this.currentUIState = UIState.Main;
        }

        private void ClickBackButtonAtTotalRankingState()
        {
            this.totalRankingObject.SetActive(false);

            this.currentUIState = UIState.Main;
            this.currentElseRankingFirstIndex = Constants.ElseRankingFirstIndex;
        }

        private void ClickUpElseRankingButton()
        {
            if (IsElseRankingIndexLowerRangeValid() == true)
            {
                this.currentElseRankingFirstIndex -= this.elseRankerRecordLines.Count;

                SetTotalElseRanking(this.rankingInformationDictionary[this.currentRankingStageID]);
            }
        }

        private bool IsElseRankingIndexLowerRangeValid()
        {
            if (this.currentElseRankingFirstIndex - this.elseRankerRecordLines.Count >= Constants.ElseRankingFirstIndex)
            {
                return true;
            }

            return false;
        }

        private void ClickDownElseRankingButton()
        {
            if (IsElseRankingIndexUpperRangeValid() == true)
            {
                this.currentElseRankingFirstIndex += this.elseRankerRecordLines.Count;

                SetTotalElseRanking(this.rankingInformationDictionary[this.currentRankingStageID]);
            }
        }

        private bool IsElseRankingIndexUpperRangeValid()
        {
            List<RankingData> infoList = this.rankingInformationDictionary[this.currentRankingStageID];

            if (infoList == null)
            {
                return false;
            }

            if (this.currentElseRankingFirstIndex + this.elseRankerRecordLines.Count < infoList.Count)
            {
                return true;
            }

            return false;
        }

        private void OpenUserRecordCard()
        {
            this.opendMapComboBoxObject.SetActive(false);
            this.userRecordCardObject.SetActive(true);
            this.totalRankingObject.SetActive(false);

            this.currentUIState = UIState.UserRanking;
        }

        private void OpenTotalRanking()
        {
            this.opendMapComboBoxObject.SetActive(false);
            this.userRecordCardObject.SetActive(false);
            this.totalRankingObject.SetActive(true);

            this.currentUIState = UIState.TotalRanking;
            this.currentElseRankingFirstIndex = Constants.ElseRankingFirstIndex;
        }

        private void Initialize()
        {
            this.currentUIState = UIState.Main;
            this.currentRankingStageID = Static.Stage.GetDefualtStageID();
            this.currentElseRankingFirstIndex = Constants.ElseRankingFirstIndex;

            SetUserRecordCard(this.currentRankingStageID);

            SetTotalTopRanking(this.rankingInformationDictionary[this.currentRankingStageID]);
            SetTotalElseRanking(this.rankingInformationDictionary[this.currentRankingStageID]);
        }

        private void SetUserRecordCard(int stageID)
        {
            float lapTime = User.User.Instance.GetSingleRacingRecords(stageID, "security-related");

            if (Static.Record.IsValidRecordValue(lapTime) == true)
            {
                Item.Avatar.Head head = new Item.Avatar.Head(User.User.Instance.CurrentAvatar.Head.InfoID);
                Item.Avatar.Top top = new Item.Avatar.Top(User.User.Instance.CurrentAvatar.Top.InfoID);
                Item.Avatar.Bottom bottom = new Item.Avatar.Bottom(User.User.Instance.CurrentAvatar.Bottom.InfoID);
                Character.Avatar avatar = new Character.Avatar(head, top, bottom);

                if (this.userRecordCard.AvatarUI == null)
                {
                    this.userRecordCard.AvatarUI = new AvatarUI(avatar, userRecordCardObject);
                    this.userRecordCard.AvatarUI.SetImage();
                }
                else
                {
                    this.userRecordCard.AvatarUI.ChangeAvatar(avatar); 
                }


                int recordCarInfoID = (int)User.User.Instance.GetSingleRacingRecords(stageID, "security-related");
                int recordPaintInfoID = (int)User.User.Instance.GetSingleRacingRecords(stageID, "security-related");
                Vehicle.Car car = new Vehicle.Car(new Item.Vehicle.Car(recordCarInfoID), new Item.Vehicle.Paint(recordPaintInfoID));
                this.userRecordCard.BestLapCarImage.sprite = car.Sprite;
                this.userRecordCard.BestLapCarImage.enabled = true;

                this.userRecordCard.MapNameText.text = Manager.Instance.GetText(TextType.Stage, stageID);
                this.userRecordCard.RankingNumberText.text = "-"; // TODO : 랭킹 작업 필요
                this.userRecordCard.BestLapTimeText.text = User.User.Instance.GetSingleRacingRecords(stageID, "security-related").ToString("F3");
                this.userRecordCard.NicknameText.text = User.User.Instance.GetUserNickname();
            }
            else
            {
                this.userRecordCard.BestLapCarImage.enabled = false;
                this.userRecordCard.BestLapCarImage.sprite = null;
                this.userRecordCard.MapNameText.text = Manager.Instance.GetText(TextType.Stage, stageID);
                this.userRecordCard.RankingNumberText.text = "-"; // TODO : 랭킹 작업 필요
                this.userRecordCard.BestLapTimeText.text = "-";
                this.userRecordCard.NicknameText.text = User.User.Instance.GetUserNickname();
            }
        }

        private void SetTotalTopRanking(List<RankingData> rankingInformations)
        {
            for (int i = 0; i < this.topRankerRecordLines.Count; i++)
            {
                if (rankingInformations != null && i < rankingInformations.Count)
                {
                    Item.Avatar.Head head = new Item.Avatar.Head(rankingInformations[i].Avatar["security-related"]);
                    Item.Avatar.Top top = new Item.Avatar.Top(rankingInformations[i].Avatar["security-related"]);
                    Item.Avatar.Bottom bottom = new Item.Avatar.Bottom(rankingInformations[i].Avatar["security-related"]);

                    if (this.topRankerRecordLines[i].Avatar == null)
                    {
                        this.topRankerRecordLines[i].Avatar = new Character.Avatar(head, top, bottom);
                    }
                    else
                    {
                        this.topRankerRecordLines[i].Avatar.ChangeHead(head);
                        this.topRankerRecordLines[i].Avatar.ChangeTop(top);
                        this.topRankerRecordLines[i].Avatar.ChangeBottom(bottom);
                    }

                    int recordCarInfoID = (int)rankingInformations[i].Record["security-related"];
                    int recordPaintInfoID = (int)rankingInformations[i].Record["security-related"];
                    Vehicle.Car car = new Vehicle.Car(new Item.Vehicle.Car(recordCarInfoID), new Item.Vehicle.Paint(recordPaintInfoID));
                    this.topRankerRecordLines[i].BestLapCarImage.sprite = car.Sprite;
                    this.topRankerRecordLines[i].BestLapCarImage.enabled = true;

                    this.topRankerRecordLines[i].BestLapTimeText.text = rankingInformations[i].Record["security-related"].ToString("F3");
                    this.topRankerRecordLines[i].NicknameText.text = rankingInformations[i].Nickname;
                }
                else
                {
                    this.topRankerRecordLines[i].BestLapCarImage.enabled = false;
                    this.topRankerRecordLines[i].BestLapCarImage.sprite = null;
                    this.topRankerRecordLines[i].BestLapTimeText.text = "-";
                    this.topRankerRecordLines[i].NicknameText.text = "-";
                }
            }
        }

        private void SetTotalElseRanking(List<RankingData> rankingInformations)
        {
            int rankingInfoIndex = this.currentElseRankingFirstIndex;

            for (int i = 0; i < this.elseRankerRecordLines.Count; i++, rankingInfoIndex++)
            {
                if (rankingInformations != null && rankingInfoIndex < rankingInformations.Count)
                {
                    Item.Avatar.Head head = new Item.Avatar.Head(rankingInformations[i].Avatar["security-related"]);
                    Item.Avatar.Top top = new Item.Avatar.Top(rankingInformations[i].Avatar["security-related"]);
                    Item.Avatar.Bottom bottom = new Item.Avatar.Bottom(rankingInformations[i].Avatar["security-related"]);

                    if (this.elseRankerRecordLines[i].Avatar == null)
                    {
                        this.elseRankerRecordLines[i].Avatar = new Character.Avatar(head, top, bottom);
                    }
                    else
                    {
                        this.elseRankerRecordLines[i].Avatar.ChangeHead(head);
                        this.elseRankerRecordLines[i].Avatar.ChangeTop(top);
                        this.elseRankerRecordLines[i].Avatar.ChangeBottom(bottom);
                    }

                    int recordCarInfoID = (int)rankingInformations[rankingInfoIndex].Record["security-related"];
                    int recordPaintInfoID = (int)rankingInformations[rankingInfoIndex].Record["security-related"];
                    Vehicle.Car car = new Vehicle.Car(new Item.Vehicle.Car(recordCarInfoID), new Item.Vehicle.Paint(recordPaintInfoID));
                    this.elseRankerRecordLines[i].BestLapCarImage.sprite = car.Sprite;
                    this.elseRankerRecordLines[i].BestLapCarImage.enabled = true;

                    this.elseRankerRecordLines[i].BestLapTimeText.text = rankingInformations[rankingInfoIndex].Record["security-related"].ToString("F3");
                    this.elseRankerRecordLines[i].RankingNumberText.text = rankingInfoIndex.ToString();
                    this.elseRankerRecordLines[i].NicknameText.text = rankingInformations[rankingInfoIndex].Nickname;
                }
                else
                {
                    this.elseRankerRecordLines[i].BestLapCarImage.enabled = false;
                    this.elseRankerRecordLines[i].BestLapCarImage.sprite = null;
                    this.elseRankerRecordLines[i].BestLapTimeText.text = "-";
                    this.elseRankerRecordLines[i].RankingNumberText.text = "-";
                    this.elseRankerRecordLines[i].NicknameText.text = "-";
                }
            }
        }
    }
}