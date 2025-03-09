using System;
using GamePlay.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace GamePlay.Core
{
    public class EquipedObject : MonoBehaviour
    {
        public Weapon equipment; //装备数据
        float physicDamage = 0; //物理伤害值，取自装备模板，会根据属性、技能、装备浮动计算;空手默认为5

        void Update()
        {
            if (equipment != null)
                physicDamage = equipment.getPhysicDamage();
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag != "Hostile") return;
            //do damage
            Character target = other.transform.GetComponent<Character>();
            target.takeDamage(physicDamage);
        }
    }
}
