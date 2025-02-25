using System;
using System.Collections.Generic;
using GamePlay.Core;
using UnityEngine;

namespace GamePlay.Combat
{

    public class Battle : MonoBehaviour, Core.IAction
    {
        public float attackInterval = 0.1f;

        public Equipment defaultEquipment = null;
        public Transform handTransfrom = null;
        public Transform handTransfromL = null;
        bool handRHasWeapon = false;
        String weaponRType = null;
        bool handLHasWeapon = false;
        String weaponLType = null;
        
        //判断攻击动画是否完成播放
        bool animDone = true;

        Character atkTarget;
        Character self;
        float lastAttackTime = Mathf.Infinity;
        Movement.Move move;
        Animator animator;

        void Start()
        {
            self = GetComponent<Character>();
            move = GetComponent<Movement.Move>();
            animator = GetComponent<Animator>();
            //左右手装备空拳
            equip(defaultEquipment);
            equip(defaultEquipment);
        }

        void Update()
        {
            //处于硬直状态时则跳过该帧其他动作
            if(self.inStaggerStatus()){
                attackDone();
                return;
            }
            //上一轮攻击动画未结束则不处理
            if(!animDone) return;
            //攻击间隔，可由增加攻速缩短
            lastAttackTime += Time.deltaTime;
            if(atkTarget == null) return;
            //目标死亡则不进行攻击
            if(atkTarget.isDead()) return;
            //处于攻击范围外时，调用移动组件移动
            if (!inRange(true))
                move.setMoveDestination(atkTarget.transform.position);
            else
            {
                move.stopAction();
                attackBehaviour();
            }
        }

        public Equipment getEquipmentByName(String name){
            return self.getEquipmentBySlot(name);
        }

        public List<Equipment> getEquipmentsActived(){
            return self.getEquipmentsActived();
        }

        public void equip(Equipment equipment)
        {
            //标识是否完成装备行为
            bool equiped = false;
            //存储装备到的槽位
            String slotName = null;
            //检查装备位是否有空缺
            for(int i = 1; i <= 4; i++){
                Equipment weapon = self.getEquipmentBySlot("weapon" + i);
                if(weapon == null || weapon.getEquipmentName().Equals("unArmed")){
                    self.setEquipmentBySlot("weapon" + i, equipment);
                    slotName = "weapon" + i;
                    equiped = true;
                    break;
                }
            }
            //没有空缺的情况下替换第一个装备
            if(!equiped) self.setEquipmentBySlot("weapon1", equipment);
            //根据激活槽位选择装备模型生成位置
            if(slotName.Equals("weapon1") || slotName.Equals("weapon3")){
                handRHasWeapon = true;
                weaponRType = equipment.getEquipmentType();
                equipment.equipWeaponShowOnPosition(handTransfrom, animator);
            }
            else if (slotName.Equals("weapon2") || slotName.Equals("weapon4")){
                handLHasWeapon = true;
                weaponLType = equipment.getEquipmentType();
                equipment.equipWeaponShowOnPosition(handTransfromL, animator);
            }
        }

        //处理攻击动画逻辑
        private void attackBehaviour()
        {
            transform.LookAt(atkTarget.transform);
            if(lastAttackTime !< attackInterval) return;
            //状态机开启攻击开关
            animator.SetBool("attack",true);
            if (weaponRType.Equals("wave"))
            {
                animator.SetBool("wave", true);
            }
            else if (weaponRType.Equals("shoot"))
            {
                animator.SetBool("shoot", true);
            }
            else if (weaponRType.Equals("throw"))
            {
                animator.SetBool("throw", true);
            }
            if (handRHasWeapon && !handLHasWeapon){//仅装备右手武器
                animator.SetBool("right",true);
            } else if(!handRHasWeapon && handLHasWeapon){//仅装备左手武器
                animator.SetBool("left",true);
            } else if(handRHasWeapon && handLHasWeapon){//双手都有装备
                animator.SetBool("bothhands",true);
            }
            lastAttackTime = 0;
        }

        private Vector3 getShootPosition(){
            return handTransfrom.position;
        }

        public void throwAmmo(){
            List<Equipment> equipmentsActived = self.getEquipmentsActived();
            //处于激活状态的武器检查是否是需要发射
            foreach(Equipment equipmentActived in equipmentsActived)
            {
                if (!equipmentActived.needAmmo())
                {
                    continue;
                }
                Ammo ammo = equipmentActived.GetAmmo();
                if (ammo == null) return;
                //send value of damage and let ammo shoot
                Ammo ammoInstance = Instantiate(ammo, getShootPosition(), Quaternion.identity);
                ammoInstance.setDamage(equipmentActived.getPhysicDamage());
                ammoInstance.shoot(atkTarget.transform.position + new Vector3(0, 1f, 0));
            }
        }

        private bool inRange(bool needMaxRange)
        {
            float battleRange = 0;
            if (needMaxRange)
                battleRange = self.getMaxBattleRange();
            else
                battleRange = self.getMinBattleRange();
            return Vector3.Distance(transform.position, atkTarget.transform.position) < battleRange;
        }

        //设定攻击对象
        public void setAttackTarget(GameObject combatTarget)
        {
            GetComponent<Core.ActionScheduler>().startAction(this);
            animator.SetBool("combatState",true);
            atkTarget = combatTarget.GetComponent<Core.Character>();
        }

        public void attackStart(){
            animDone = false;
        }

        public void attackDone()
        {
            stopAction();
        }

        public void stopAction(){
            GetComponent<Animator>().SetBool("attack",false);
            GetComponent<Animator>().SetBool("right",false);
            GetComponent<Animator>().SetBool("left",false);
            GetComponent<Animator>().SetBool("bothhands",false);
            GetComponent<Animator>().SetBool("wave",false);
            GetComponent<Animator>().SetBool("shoot",false);
            GetComponent<Animator>().SetBool("throw",false);
            animDone = true;
            atkTarget = null;
        }
    }
}