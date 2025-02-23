using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Control
{

    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(getWayPoint(i), radius: 0.1f);
                Gizmos.DrawLine(getWayPoint(i), getWayPoint(getNextIndex(i)));
            }
        }

        public int getNextIndex(int index){
            if(index + 1 == transform.childCount){
                return 0;
            }
            return index + 1;
        }

        public Vector3 getWayPoint(int index)
        {
            return transform.GetChild(index).position;
        }
    }
}
