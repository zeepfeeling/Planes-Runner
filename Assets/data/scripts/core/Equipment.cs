using System;
using UnityEngine;

namespace GamePlay.Interface
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "GamePlay/Equipment", order = 3)]
    public class Equipment : Item
    {
        public GameObject equipPrefab = null;
        public String equipmentType = "";
        public AnimatorOverrideController equipAnimatorOverrideController = null;
        public float physicDamage = 5f;
        public float battleRange = 2f;
        public Ammo ammo = null;

        //装备武器在对应位置显示
        public void equipWeaponShowOnPosition(Transform spawnPosition, Animator animator)
        {
            if (equipPrefab != null && spawnPosition != null){
                GameObject equipment = Instantiate(equipPrefab, spawnPosition);
            }
            // set animator
            if (equipAnimatorOverrideController != null)
                animator.runtimeAnimatorController = equipAnimatorOverrideController;
            else { // reset the animator to default status
                setDefaultAnimator(animator);
            }
        }

        //装备武器在对应位置消失
        public void equipWeaponDisappearOnPosition(Transform destoryPosition, Animator animator){
            Transform equipment = destoryPosition.Find(itemName);
            if(equipment == null) return;
            equipment.name = equipment.name + "_delete";
            Debug.Log(equipment.name);
            Destroy(equipment.gameObject);
            if(animator != null) setDefaultAnimator(animator);
        }

        private void setDefaultAnimator(Animator animator){
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if(overrideController == null) return;
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }        

        public float getPhysicDamage()
        {
            return physicDamage;
        }

        public float getRange()
        {
            return battleRange;
        }

        public bool needAmmo()
        {
            return ammo != null;
        }

        public Ammo GetAmmo()
        {
            return ammo;
        }

        public String getEquipmentType(){
            return equipmentType;
        }
        
        public String getEquipmentName(){
            return itemName;
        }
    }

}
