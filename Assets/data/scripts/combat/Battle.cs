using System;
using System.Collections.Generic;
using GamePlay.Interface;
using UnityEngine;

namespace GamePlay.Interface
{

    public class Battle : MonoBehaviour, Interface.IAction
    {
        public float attackInterval = 0.1f;

        public Equipment defaultEquipment = null;
        public Transform handTransfrom = null;
        public Transform handTransfromL = null;
        bool handRHasWeapon = false;
        String weaponRType = null;
        bool handLHasWeapon = false;
        String weaponLType = null;
        GameObject weaponR = null;
        GameObject weaponL = null;
        
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
            //所有装备槽装备空拳
            equip(defaultEquipment);
            equip(defaultEquipment);
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
            //获取装备类型
            String weaponType = equipment.getEquipmentType();
            //存储装备到的槽位
            String slotName = null;
            //检查装备位是否有空缺
            for(int i = 1; i <= 4; i++){
                Equipment weapon = self.getEquipmentBySlot("weapon" + i);
                //判断槽位是否空手
                if(weapon == null || weapon.getEquipmentName().Equals(defaultEquipment.getEquipmentName())){
                    //弓类型必须装在左手槽位
                    if(weaponType == "bow" && i != 2 && i !=4){
                        continue;
                    }
                    self.setEquipmentBySlot("weapon" + i, equipment);
                    slotName = "weapon" + i;
                    equiped = true;
                    break;
                }
            }
            //没有空缺的情况下替换第一个装备
            if(!equiped){
                if(weaponType == "bow") self.setEquipmentBySlot("weapon2", equipment);
                else self.setEquipmentBySlot("weapon1", equipment);
            } 
            //根据激活槽位选择装备模型生成位置
            if(slotName.Equals("weapon1") || slotName.Equals("weapon3")){
                //判断是否不是空拳,空拳状态不标记为有装备
                if(!equipment.getEquipmentName().Equals(defaultEquipment.getEquipmentName())){
                    handRHasWeapon = true;
                    weaponRType = weaponType;
                }
                equipment.equipWeaponShowOnPosition(handTransfrom, animator);
            }
            else if (slotName.Equals("weapon2") || slotName.Equals("weapon4")){
                //判断是否不是空拳,空拳状态不标记为有装备
                if(!equipment.getEquipmentName().Equals(defaultEquipment.getEquipmentName())){
                    handLHasWeapon = true;
                    weaponLType = weaponType;
                }
                equipment.equipWeaponShowOnPosition(handTransfromL, animator);
            }
            getEquipObj();
        }

        //处理攻击动画逻辑
        private void attackBehaviour()
        {
            transform.LookAt(atkTarget.transform);
            if(lastAttackTime !< attackInterval) return;
            //状态机开启攻击开关
            animator.SetBool("attack",true);
            if(!handRHasWeapon && !handLHasWeapon){//赤手空拳播放拳击动画
                animator.SetBool("punch", true);
            }
            if(handRHasWeapon && !handLHasWeapon) {//仅装备右手武器
                animator.SetBool("right",true);
                if (weaponRType.Equals("onehanded"))
                {
                    animator.SetBool("wave", true);
                }
                else if (weaponRType.Equals("onehandedCrossBow"))
                {
                    animator.SetBool("shoot", true);
                }
                else if (weaponRType.Equals("ammo"))
                {
                    animator.SetBool("throw", true);
                }
            } else if(!handRHasWeapon && handLHasWeapon){//仅装备左手武器
                animator.SetBool("left",true);
                if (weaponLType.Equals("onehanded"))
                {
                    animator.SetBool("wave", true);
                }
                else if (weaponLType.Equals("onehandedCrossBow"))
                {
                    animator.SetBool("shoot", true);
                }
                else if (weaponLType.Equals("ammo"))
                {
                    animator.SetBool("throw", true);
                }
                else if (weaponLType.Equals("bow"))
                {
                    animator.SetBool("bow", true);
                }
            } else if(handRHasWeapon && handLHasWeapon){//双手都有装备
                animator.SetBool("bothhands",true);
                if(weaponRType == weaponLType){//双手武装类型相同时，播放特殊双持攻击动画
                    if (weaponRType.Equals("onehanded"))
                    {
                        animator.SetBool("wave", true);
                    }
                    else if (weaponLType.Equals("onehandedCrossBow"))
                    {
                        animator.SetBool("shoot", true);
                    }
                    else if (weaponLType.Equals("ammo"))
                    {
                        animator.SetBool("throw", true);
                    }
                }else{//双手武装类型不同时，根据和目标距离切换合适范围的武器动画

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
            if (needMaxRange)
                battleRange = self.getMaxBattleRange();
            else
                battleRange = self.getMinBattleRange();
            return Vector3.Distance(transform.position, atkTarget.transform.position) < battleRange;
        }

        //设定攻击对象
        public void setAttackTarget(GameObject combatTarget)
        {
            GetComponent<Interface.ActionScheduler>().startAction(this);
            animator.SetBool("combatState",true);
            atkTarget = combatTarget.GetComponent<Interface.Character>();
        }

        public void attackStart(){
            animDone = false;
        }

        public void activeWeaponR(){
            if(weaponR != null && weaponR.GetComponent<Collider> ()!= null) {
                Collider weaponRColloder = weaponR.GetComponent<Collider>();
                weaponRColloder.enabled = true;
            }
        }

        public void activeWeaponL(){
            if(weaponL != null && weaponR.GetComponent<Collider> ()!= null) {
                Collider weaponLColloder = weaponR.GetComponent<Collider>();
                weaponLColloder.enabled = true;
            }
        }
        public void inActiveWeaponR(){
            if(weaponR != null && weaponR.GetComponent<Collider> ()!= null) {
                Collider weaponRColloder = weaponR.GetComponent<Collider>();
                weaponRColloder.enabled = false;
            }
        }

        public void inActiveWeaponL(){
            if(weaponL != null && weaponR.GetComponent<Collider> ()!= null) {
                Collider weaponLColloder = weaponR.GetComponent<Collider>();
                weaponLColloder.enabled = false;
            }
        }

        public void attackDone()
        {
            stopAction();
        }

        public void stopAction(){
            //状态机取消动作
            GetComponent<Animator>().SetBool("attack",false);
            GetComponent<Animator>().SetBool("right",false);
            GetComponent<Animator>().SetBool("left",false);
            GetComponent<Animator>().SetBool("bothhands",false);
            GetComponent<Animator>().SetBool("wave",false);
            GetComponent<Animator>().SetBool("shoot",false);
            GetComponent<Animator>().SetBool("throw",false);
            GetComponent<Animator>().SetBool("punch",false);
            //碰撞取消
            inActiveWeaponR();
            inActiveWeaponL();
            animDone = true;
            atkTarget = null;
        }

        public void getEquipObj(){
            try
            {
                weaponR = handTransfrom.GetChild(0).gameObject;
                weaponL = handTransfromL.GetChild(0).gameObject;
            }
            catch (Exception)
            {
                Debug.Log("双手武器位获取为空");
                return;
            }

        }
    }
}