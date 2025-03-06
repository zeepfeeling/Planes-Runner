using System;
using GamePlay.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Interface
{
    [CreateAssetMenu(fileName = "Spell", menuName = "GamePlay/Spell", order = 1)]
    public class Spell : ScriptableObject
    {
        public String mandatoryEquipment = "";//法术或技能是否依赖于某种装备
        public String spellName = "";//法术或技能名称，为界面显示
        public String spellAnimationType;//法术或技能动作类型，用于决定发动时播放动画
        public Image icon = null;//图标
        public float castCostMana = 2f;
        public float meleeRangeRite = 1f;//近身范围，某些技能的伤害或者效果需要在近身一定范围才能起效
        public float meleeDamage = 0f;//近身造成的伤害
        public bool meleeWide = false;//近身伤害是否为范围伤害
        public float moveSpeed = 0f;//位移速度，默认为0
        public Buff[] targetBuffs = null;//目标状态类效果
        public Buff[] selfBuffs = null;//自身状态类效果
        public Ammo ammo = null;//发射物，默认无
        public float ammoDamage = 5f;//发射物伤害

        //法术生效方法
        public void effectOnce(Character caster, Character castTarget, Vector3 direction)
        {
            Character casterAttributes = caster.GetComponent<Character>();
            if (casterAttributes == null) return;
            //发动者支付资源
            if (castCostMana > casterAttributes.getMana()) return;
            else casterAttributes.costMana(castCostMana);

            // 法术拥有发射物
            if (ammo != null)
            {
                //生成发射物，并将法术伤害传递给发射物
                Ammo ammoInstance = Instantiate(ammo, caster.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                ammoInstance.setDamage(ammoDamage);
                ammoInstance.shoot(direction + new Vector3(0, 1f, 0));
            }
            // 法术存在目标状态
            if (targetBuffs != null && castTarget != null) castTarget.addBuffs(targetBuffs);
            // 法术存在自身状态
            if (selfBuffs != null) casterAttributes.addBuffs(selfBuffs);
        }

        public String getName()
        {
            return spellName;
        }

        public String getSpellAnimationType()
        {
            return spellAnimationType;
        }

        public float getMoveSpeed()
        {
            return moveSpeed;
        }

        //判断是否在近身范围内
        private bool inMeleeRange(Character caster, Character castTarget)
        {
            //判断和目标直线距离是否在范围内
            bool inRange = Vector3.Distance(caster.transform.position, castTarget.transform.position) < meleeRangeRite;
            if(!inRange) return false;
            //判断是否正对目标
            Ray ray = new Ray(caster.transform.position + new Vector3(0,1f,0), caster.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
             if (Physics.Raycast(ray, out RaycastHit hit, meleeRangeRite))
            {
                if(hit.collider != null) return true;
            }
            return false;
        }
    }
}
