using UnityEngine;
using UnityEngine.AI;

namespace GamePlay.Movement
{
    /**
        处理单位移动逻辑
    */
    public class Move : MonoBehaviour, Core.IAction
    {

        NavMeshAgent navMeshAgent;
        Core.Character character;
        Animator animator;
        float speedBase = 5f;

        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            character = GetComponent<Core.Character>();
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !character.isDead();
            updateAnimator();
        }

        public void moveTo(Vector3 destination)
        {
            GetComponent<Core.ActionScheduler>().startAction(this);
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void stopAction()
        {
            navMeshAgent.isStopped = true;
        }

        //update the velocity to change state of pc's walking
        private void updateAnimator()
        {
            //获取导航器中设置的速度，转换后设置到动作器中
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("speed", speed);
        }
    }
}
