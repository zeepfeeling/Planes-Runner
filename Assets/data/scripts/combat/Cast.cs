using System;
using GamePlay.Interface;
using TMPro;
using UnityEngine;

namespace GamePlay.Interface
{

    public class Cast : MonoBehaviour, IAction
    {
        public Spell[] spells = null;
        KeyCode[] spellKeys;
        Spell currentSpell = null;
        bool animDone = true;
        Character self;
        Character target;
        Vector3 castDirction;
        Movement.Move move;
        //施法动作类型，默认为cast的通用施法动作
        String castAnimationType = "cast";
        bool spellMoving = false;

        // Start is called before the first frame update
        void Start()
        {
            self = GetComponent<Character>();
            move = GetComponent<Movement.Move>();
            spellKeys = new KeyCode[] {KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.F};
        }

        // Update is called once per frame
        void Update()
        {
            //硬直，跳过该帧其他动作
            if (self.inStaggerStatus())
            {
                castDone();
                return;
            }
            if (currentSpell == null)
            {
                return;
            }

            if(spellMoving){
                transform.Translate(Vector3.forward * currentSpell.getMoveSpeed() * Time.deltaTime);
            }
            
            if (!animDone) return;
            castAnimationType = currentSpell.getSpellAnimationType();
            transform.LookAt(castDirction);
            startAction();
        }

        //法术释放动作开始节点调用
        public void castStart(){
            animDone = false;
            if(currentSpell.getMoveSpeed() != 0){
                spellMoving = true;
            }
        }

         //法术释放动作起效节点调用，适用于一次动作中单次生效的场景
        public void release(){
            currentSpell.effectOnce(self,target,castDirction);
            spellMoving = false;
            //释放后重置当前法术为空
            currentSpell = null;
        }
        //法术释放动作结束节点调用
        public void castDone(){
            stopAction();
        }

        //通用施法行为开始
        public void startAction()
        {
            if (castAnimationType.Equals("cast"))
            {
                GetComponent<Animator>().SetBool("cast",true);
            }
            else
                GetComponent<Animator>().SetBool(castAnimationType,true);
        }

        //通用施法行为结束
        public void stopAction()
        {
            if (castAnimationType.Equals("cast"))
            {
                GetComponent<Animator>().SetBool("cast",false);
            }
            else
                GetComponent<Animator>().SetBool(castAnimationType,false);
            animDone = true;
        }

        //指定方向的施放，由战斗组件调用
        public void castBydirection(Vector3 position)
        {
            GetComponent<ActionScheduler>().startAction(this);
            castDirction = position; 
        }

        public Spell getCurrentSpell(){
            return currentSpell;
        }

        public void setCurrentSpell(Spell spell){
            currentSpell = spell;
        }

        public Spell[] getSpells(){
            return spells;
        }

        public KeyCode[] getSpellKeys(){
            return spellKeys;
        }

        public void setTarget(Character target){
            this.target = target;
        }

        public bool getLastSpellDone(){
            return animDone;
        }
    }
}
