using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay.Combat;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Core
{

    public class Character : MonoBehaviour
    {
        public float hitPoint = 100f; //生命
        public float manaPoint = 50f;//法力
        float staggersPoint = 0;//眩晕值
        public Slider hitPointSlider;
        public Slider manaSlider;
        public Slider staggerSlider;
        List<Buff> buffs;
        List<String> buffNames;
        float maxHitPoint; //生命值上限
        float maxManaPoint;//法力值上限
        float maxStaggersPoint = 3f;//眩晕值上限
        bool staggerStatus;//是否处于眩晕状态
        float staggersRecoverRate = 0.001f;//眩晕值恢复速率
        bool dead;
        public Dictionary<String,Equipment> equipments = new Dictionary<string, Equipment>(); //装备目录
        Dictionary<String,float> equipValues = new Dictionary<string, float>(); //装备数值目录
        String[] activeSlots = new String[2]; //被激活的装备槽位
        List<String> frozenSlots = new List<string>(); //被冻结的装备槽位
        float maxBattleRnage = 0f; //最大交战距离
        float minBattleRnage = 0f; //最小交战距离

        void Awake()
        {
            //初始化装备目录信息
            initiateEquipSlots();
        }
        void Start()
        {
            buffs = new List<Buff>();
            buffNames = new List<String>();
            maxHitPoint = hitPoint;
            maxManaPoint = manaPoint;
            if (hitPointSlider != null)
            {
                hitPointSlider.maxValue = maxHitPoint;
                hitPointSlider.value = maxHitPoint;
            }
            if (manaSlider != null)
            {
                manaSlider.maxValue = maxManaPoint;
                manaSlider.value = maxManaPoint;
            }
            if (staggerSlider != null)
            {
                staggerSlider.maxValue = maxStaggersPoint;
                staggerSlider.value = maxStaggersPoint;
            }
        }

        private void initiateEquipSlots()
        {
            equipments.Add("weapon1", null);
            equipments.Add("weapon2", null);
            equipments.Add("weapon3", null);
            equipments.Add("weapon4", null);
        }

        void Update()
        {
            if (hitPointSlider != null)
            {
                hitPointSlider.value = hitPoint;
            }
            if (manaSlider != null)
            {
                manaSlider.value = manaPoint;
            }
            if (staggerSlider != null)
            {
                staggerSlider.value = staggersPoint;
            }
            //恢复眩晕值
            staggersPoint = Mathf.Max(staggersPoint - staggersRecoverRate,0);
            if (buffs == null || buffs.Count() == 0)
            {
                return;
            }
            dealWithBuffs();
        }

        private void dealWithBuffs()
        {
            List<Buff> deleteBuffs = new List<Buff>();
            foreach (Buff buff in buffs)
            {
                Debug.Log(buff.getBuffName());
                Debug.Log("buff last " + buff.getCurrentTime());
                if (buff.getCurrentTime() < buff.getDuration())
                {
                    buff.buffact(this);
                    buff.buffTimeForward();
                }
                else
                {
                    deleteBuffs.Add(buff);
                }
            }
            //delete timeout buffs
            foreach (Buff buff in deleteBuffs)
            {
                buffs.Remove(buff);
                buffNames.Remove(buff.getBuffName());
            }
        }

        public bool isDead()
        {
            return dead;
        }

        public float getHitPoint()
        {
            return hitPoint;
        }

        public void setHitPoint(float hitPoint)
        {
            this.hitPoint = hitPoint;
        }

        public float getMana()
        {
            return manaPoint;
        }

        public void setMana(float manaPoint)
        {
            this.manaPoint = manaPoint;
        }
        public float getMaxHitPoint()
        {
            return maxHitPoint;
        }

        public void setMaxHitPoint(float maxHitPoint)
        {
            this.maxHitPoint = maxHitPoint;
        }

        public float getMaxMana()
        {
            return maxManaPoint;
        }

        public void setMaxMana(float maxManaPoint)
        {
            this.maxManaPoint = maxManaPoint;
        }

        public bool inStaggerStatus(){
            return staggerStatus;
        }

        //add new and refresh buffs
        public void addBuffs(Buff[] buffs)
        {
            foreach(Buff buff in buffs){
                if(buffNames.Contains(buff.getBuffName())){
                    return;
                }
                this.buffs.Add(buff.buffInstance());
                buffNames.Add(buff.getBuffName());
            }
        }

        public void takeDamage(float damage)
        {
            hitPoint = Mathf.Max(hitPoint - damage, 0);
            print("got " + damage + " points damage, left " + hitPoint + " health.");
            if (hitPoint <= 0)
            {
                die();
                return;
            }
            staggersPoint += 1f;
            if(staggersPoint >= maxStaggersPoint && GetComponent<Animator>() != null){
                GetComponent<Animator>().SetBool("getHit",true);
                staggerStatus = true;
            }
        }

        public void resetStaggers(){
            GetComponent<Animator>().SetBool("getHit",false);
            staggerStatus = false;
            staggersPoint = 0;
        }

        public void costMana(float cost)
        {
            manaPoint = Mathf.Max(manaPoint - cost, 0);
        }

        public Dictionary<String,Equipment> getCurrentEquipments(){
            return equipments;
        }

        public Dictionary<String,float> getEquipValues(){
            return equipValues;
        }

        public Equipment getEquipmentBySlot(String slot){
            return equipments[slot];
        }

        public float getEquipValueBySlot(String slot){
            return equipValues[slot];
        }

        public void setEquipmentBySlot(String slot,Equipment equipment){
            equipments[slot] = equipment;
            activeEquipment(slot);
        }

        public List<Equipment> getEquipmentsActived(){
            List<Equipment> activeEquipments = new List<Equipment>();
            foreach(String slot in activeSlots){
                if(slot == null) continue;
                Equipment activeEquipment = equipments[slot];
                if(activeEquipment != null)
                    activeEquipments.Add(activeEquipment);
            }
            return activeEquipments;
        }

        public String[] getActiveSlots(){
            return activeSlots;
        }

        public void removeEquipment(String slot){
            equipments[slot] = null;
            inActiveEquipment(slot);
        }

        public float getMaxBattleRange(){
            return maxBattleRnage;
        }
        public float getMinBattleRange(){
            return minBattleRnage;
        }

        private void activeEquipment(String slot){
            if(slot.Equals("weapon1") || slot.Equals("weapon3"))
                activeSlots[0] = slot;
            else if(slot.Equals("weapon2") || slot.Equals("weapon4"))
                activeSlots[1] = slot;
            //重新计算人物最大和最小交战距离
            countBattleRange();
        }

        private void inActiveEquipment(String slot){
            if(slot.Equals(activeSlots[0])) activeSlots[0] = null;
            if(slot.Equals(activeSlots[1])) activeSlots[1] = null;
        }

        private void die()
        {
            if (dead) return;
            dead = true;
            GetComponent<Animator>().SetBool("die",true);
            GetComponent<ActionScheduler>().stopCurrentAction();
        }

        //计算人物最大和最小交战距离
        private void countBattleRange(){
            List<Equipment> equipmentsActived = getEquipmentsActived();
            foreach(Equipment equipmentActived in equipmentsActived)
            {   
                    maxBattleRnage = Mathf.Max(equipmentActived.getRange(),maxBattleRnage);
                    minBattleRnage = Mathf.Min(equipmentActived.getRange(),minBattleRnage);
            }
        }
    }
}