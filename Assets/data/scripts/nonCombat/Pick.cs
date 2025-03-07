using GamePlay.Interface;
using UnityEngine;

namespace GamePlay.NonCombat
{
    public class Pick : MonoBehaviour, IAction
    {
        PickTarget target;
        Movement.Move move;
        Battle battle;
        Inventory inventory;

        private void Start() {
            move = GetComponent<Movement.Move>();
            battle = GetComponent<Battle>();
            inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            if (target == null) return;
            if (!inRange()) move.setMoveDestination(target.transform.position);
            else
            {
                move.stopAction();
                if (!target.isPickable()) return;
                if(target.item != null){
                    if(inventory.addItemsToInventory(target.item,target.singleItemCount))
                        target.vanish();
                }
            }
        }

        public void pick(PickTarget pickTarget)
        {
            GetComponent<Interface.ActionScheduler>().startAction(this);
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