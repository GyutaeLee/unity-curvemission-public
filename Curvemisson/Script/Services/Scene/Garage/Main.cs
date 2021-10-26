using System.Collections;
using UnityEngine;

namespace Services.Scene.Garage
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private GameObject carItemGarageObject;
        [SerializeField]
        private GameObject avatarItemClosetObject;

        public void OpenCarItemGarage()
        {
            CarItemGarage carItemGarage = this.carItemGarageObject.AddComponent<CarItemGarage>();
            carItemGarage.Open();
        }

        public void OpenAvatarItemCloset()
        {
            AvatarItemCloset avatarItemCloset = this.avatarItemClosetObject.AddComponent<AvatarItemCloset>();
            avatarItemCloset.Open();
        }
    }
}