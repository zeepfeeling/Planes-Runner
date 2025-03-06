using GamePlay.Interface;
using UnityEngine;
namespace GamePlay.Control
{

    public class AIControl : MonoBehaviour
    {
        public float chaseDistance = 5f;
        public float suspicionTime = 3f;
        public PatrolPath patrolPath;
        public float wayPointMaxOffset = 1f;
        public float wayPointstayTime = 3f;


        Interface.Battle battle;
        GameObject player;
        Interface.Character health;
        Movement.Move move;
        Vector3 aiPosition;
        float timeAfterEncount = Mathf.Infinity;
        float timeAfterPatrol = Mathf.Infinity;
        int currentWayPointIndex = 0;

        private void Start()
        {
            health = GetComponent<Interface.Character>();
            battle = GetComponent<Interface.Battle>();
            move = GetComponent<Movement.Move>();
            player = GameObject.FindWithTag("Player");
            aiPosition = transform.position;
        }
        private void Update()
        {
            if (health.isDead()) return;
            if (player == null) return;
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= chaseDistance && !player.GetComponent<Interface.Character>().isDead())
            {
                //检测进入战斗
                battle.setAttackTarget(player);
                timeAfterEncount = 0;
            }
            else if (timeAfterEncount < suspicionTime)
            {
                //脱离范围后警戒
                GetComponent<ActionScheduler>().stopCurrentAction();
            }
            else
            {
                //警戒时限超过后返回巡逻
                if (patrolPath != null)
                {
                    if (atWayPoint())
                    {
                        currentWayPointIndex = patrolPath.getNextIndex(currentWayPointIndex);
                    }
                    aiPosition = getCurrentWayPoint();
                }
                if(timeAfterPatrol >= wayPointstayTime){
                    move.setMoveDestination(aiPosition);
                    timeAfterPatrol = 0;
                }
            }
            //重置时间
            timeAfterEncount += Time.deltaTime;
            timeAfterPatrol += Time.deltaTime;
        }

        private bool atWayPoint()
        {
            float distance = Vector3.Distance(transform.position, getCurrentWayPoint());
            return distance <= wayPointMaxOffset;

        }

        private Vector3 getCurrentWayPoint()
        {
            return patrolPath.getWayPoint(currentWayPointIndex);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}