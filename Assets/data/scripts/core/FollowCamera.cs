using UnityEngine;

namespace GamePlay.Interface
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = target.position;
            transform.position = new Vector3(target.position.x - 6, target.position.y + 8, target.position.z - 6);
        }
    }

}
