using UnityEngine;

namespace Services.Game.Collision
{
    public class Lap : Collision
    {
        [SerializeField]
        private Enum.Collision.Lap lap;

        public new Enum.Collision.Lap Collide()
        {
            return this.lap;
        }
    }
}