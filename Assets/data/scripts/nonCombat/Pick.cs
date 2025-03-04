using GamePlay.Core;
using UnityEngine;

namespace GamePlay.NonCombat
{
    public class Pick : MonoBehaviour, Core.IAction
    {
        PickTarget target;
        Movement.Move move;
        Battle battle;

        private void Start() {
            move = GetComponent<Movement.Move>();
            battle = GetComponent<Battle>();
        }

        private void Update()
        {
            if (target == null) return;
            if (!inRange()) move.setMoveDestination(target.transform.position);
            else
            {
                move.stopAction();
                if (!target.isPickable()) return;
                // if (target.isEquipable())
                // {
                //     goEquip();
                //     return;
                // }
                // if (target.isCastable() && target.GetSpell() != null){
                    
                // }

            }
        }

        private void goEquip()
        {

        }

        public void pick(PickTarget pickTarget)
        {
            GetComponent<Core.ActionScheduler>().startAction(this);
            target = pickTarget;          
        }
        public void stopAction()
        {
            target = null;
        }

        private bool inRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < target.getPickRange();
        }
    }

}