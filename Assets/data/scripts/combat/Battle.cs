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

        void Start()
        {
            self = GetComponent<Character>();
            move = GetComponent<Movement.Move>();
            equip(defaultEquipment);
        }

        void Update()
        {
            //硬直，跳过该帧其他动作
            if(self.inStaggerStatus()){
                attackDone();
                return;
            }
            lastAttackTime += Time.deltaTime;
            if(atkTarget == null) return;
            if(atkTarget.isDead()) return;
            if(!animDone) return;
            if (!inRange(true))
                move.moveTo(atkTarget.transform.position);
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
            Animator animator = GetComponent<Animator>();
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

        private void attackBehaviour()
        {
            transform.LookAt(atkTarget.transform);
            if(lastAttackTime !< attackInterval) return;
            if(handRHasWeapon && !handLHasWeapon){//仅装备右手武器
                if(weaponRType.Equals("melee"))
                    GetComponent<Animator>().SetBool("attack_r",true);
                else if(weaponRType.Equals("range"))
                    GetComponent<Animator>().SetBool("shoot_r",true);
            } else if(!handRHasWeapon && handLHasWeapon){//仅装备左手武器
                if(weaponLType.Equals("melee"))
                    GetComponent<Animator>().SetBool("attack_l",true);
                else if(weaponLType.Equals("range"))
                    GetComponent<Animator>().SetBool("shoot_l",true);
            } else if(handRHasWeapon && handLHasWeapon){//双手都有装备
                if(weaponLType.Equals(weaponRType)){//双手武器类型相同时
                    if(weaponRType.Equals("melee")){
                        GetComponent<Animator>().SetBool("attack_r",true);
                        GetComponent<Animator>().SetBool("melee_dual",true);
                    }else if(weaponRType.Equals("range")){
                        GetComponent<Animator>().SetBool("shoot_r",true);
                        GetComponent<Animator>().SetBool("shoot_dual",true);
                    }
                }else{
                    if(weaponRType.Equals("melee")){//双手武器类型不同时，根据距离目标远近决定播放哪个手的动画
                        if(inRange(false)) GetComponent<Animator>().SetBool("attack_r",true);
                        else GetComponent<Animator>().SetBool("shoot_l",true);
                    }else if(weaponRType.Equals("range")){
                        if(inRange(false)) GetComponent<Animator>().SetBool("attack_l",true);
                        else GetComponent<Animator>().SetBool("shoot_r",true);
                    }
                }
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
            List<Equipment> equipmentsActived = self.getEquipmentsActived();
            foreach(Equipment equipmentActived in equipmentsActived)
            {   
                if(needMaxRange)
                    battleRange = Mathf.Max(equipmentActived.getRange(),battleRange);
                else{
                    if(battleRange == 0) battleRange = equipmentActived.getRange();
                    else battleRange = Mathf.Min(equipmentActived.getRange(),battleRange);
                }
            }
            return Vector3.Distance(transform.position, atkTarget.transform.position) < battleRange;
        }

        public void attack(GameObject combatTarget)
        {
            GetComponent<Core.ActionScheduler>().startAction(this);
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
            GetComponent<Animator>().SetBool("attack_r",false);
            GetComponent<Animator>().SetBool("attack_l",false);
            GetComponent<Animator>().SetBool("shoot_r",false);
            GetComponent<Animator>().SetBool("shoot_l",false);
            GetComponent<Animator>().SetBool("melee_dual",false);
            GetComponent<Animator>().SetBool("shoot_dual",false);
            animDone = true;
            atkTarget = null;
        }
    }
}