using UnityEngine;

namespace GamePlay.NonCombat
{
    public class PickTarget : MonoBehaviour
    {
        public bool equipable = true;
        public bool castable = false;
        public bool pickable = true;
        public Combat.Equipment equipment = null;
        public Combat.Spell spell = null;

        public float pickRange = 1f;

        public bool isEquipable(){
            return equipable;
        }

        public bool isCastable(){
            return castable;
        }

        public bool isPickable(){
            return pickable;
        }

        public Combat.Equipment GetEquipment(){
            return equipment;
        }

        public Combat.Spell GetSpell(){
            return spell;
        }

        public float getPickRange(){
            return pickRange;
        }

        public void vanish(){
            Destroy(gameObject);
        }
    }

}