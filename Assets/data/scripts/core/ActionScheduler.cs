using UnityEngine;

namespace GamePlay.Interface
{
    public class ActionScheduler : MonoBehaviour {

        Interface.IAction currentAction;
        public void startAction(IAction action){
            if(currentAction == action) return;
            if(currentAction != null){
                print("stop " + currentAction);
                currentAction.stopAction();
            }
            currentAction = action;
        }

        public void stopCurrentAction(){
            startAction(null);
        }
    }
}