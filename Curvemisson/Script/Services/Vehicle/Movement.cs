using UnityEngine;

namespace Services.Vehicle
{
    public class Movement : MonoBehaviour
    {
        public void MoveTransform(Transform transfrom, Vector2 direction, float speed)
        {
            Vector3 positionVector = direction * speed * Time.deltaTime;
            transfrom.position += positionVector;
        }
    }
}