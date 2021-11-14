using UnityEngine;

namespace Services.Scene.SingleRacing
{
    public class Map : Util.Singleton<Map>
    {
        private GameObject halfOfFinishLap;
        private GameObject finishLapObject;

        private void Start()
        {
            GameObject game = GameObject.Find("Game");
            GameObject mapPrefab = Resources.Load<GameObject>("Prefab/Map/" + User.User.Instance.CurrentStageID);

            GameObject mapObject = Instantiate(mapPrefab, game.transform);

            this.halfOfFinishLap = Useful.ObjectFinder.GetGameObjectInAllChild(mapObject, "HalfOfFinishLap", true);
            this.finishLapObject = Useful.ObjectFinder.GetGameObjectInAllChild(mapObject, "FinishLapObject", true);

            GameObject coins = Useful.ObjectFinder.GetGameObjectInAllChild(mapObject, "Coins", true);            
            for (int i = 0; i < coins.transform.childCount; i++)
            {
                coins.transform.GetChild(i).GetComponent<Services.Game.Collision.FieldItem>().collideAction = () => { GameLogic.Instance.AcquiredCoinQuantity++; };
            }
        }

        public void ActiveHalfOfFinishLap(bool isActive)
        {
            this.halfOfFinishLap.SetActive(isActive);
        }

        public void ActiveFinishLap(bool isActive)
        {
            this.finishLapObject.SetActive(isActive);
        }
    }
}