using GamePlay.Combat;
using GamePlay.Core;
using GamePlay.Movement;
using GamePlay.NonCombat;
using UnityEngine;

namespace GamePlay.Control
{
    public class PlayerControl : MonoBehaviour
    {
        Ray clickRay;
        Character charactor;
        Battle battle;
        Cast cast;
        Move move;
        Pick pick;

        private void Start()
        {
            //initiate components player will use
            charactor = GetComponent<Character>();
            battle = GetComponent<Battle>();
            cast = GetComponent<Cast>();
            pick = GetComponent<Pick>();
            move = GetComponent<Move>();
        }
        void Update()
        {
            if (charactor.isDead()) return;
            //every frame should do
            //if (interactWithSpell()) return;
            //if (interactWithCombat()) return;
            //if (interactWithPickUp()) return;
            if (interactWithMovement()) return;
        }

        private bool interactWithCombat()
        {
            if (!Input.GetMouseButton(0))
            {
                return false;
            }
            return combatBehaviour();
        }

        private bool combatBehaviour()
        {
            RaycastHit[] hits = Physics.RaycastAll(getRayByCursor());
            foreach (RaycastHit hit in hits)
            {
                HitTarget target = hit.transform.GetComponent<HitTarget>();
                if (target == null) continue;
                //dead guy should be ignored
                if (charactor.isDead()) continue;
                //you cannot beat yourself
                if (hit.transform.tag == "Player") continue;
                battle.attack(target.gameObject);
                return true;
            }
            return false;
        }

        private bool interactWithSpell()
        {
            //前一法术或技能未完成时，当前法术和技能无法发动
            if(!cast.getLastSpellDone()){
                return false;
            }
            // 检查法术技能列表是否存在数据
            Spell[] spells = cast.getSpells();
            if (spells == null) return false;
            // 检测按键是否为法术技能按键
            if (!Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.R) && !Input.GetKeyDown(KeyCode.F))
            {
                return false;
            }
            Debug.Log("spell key pressed");
            KeyCode[] spellKeys = cast.getSpellKeys();
            for (int index = 0; index < spells.Length; index++)
            {
                if (Input.GetKeyDown(spellKeys[index]) && spells[index] != null)
                {
                    Debug.Log("current spell:" + spells[index].getName());
                    cast.setCurrentSpell(spells[index]);
                    break;
                }
            }
            if (cast.getCurrentSpell() == null) return false;
            return castBehavior();
        }

        private bool castBehavior()
        {
            clickRay = getRayByCursor();
            RaycastHit rayhit;
            if (!Physics.Raycast(clickRay, out rayhit))
            {
                return false;
            }
            // reset the height of direction
            Vector3 hitpoint = rayhit.point;
            hitpoint.y = transform.position.y;
            cast.castBydirection(hitpoint);

            RaycastHit[] hits = Physics.RaycastAll(clickRay);
            foreach (RaycastHit hit in hits)
            {
                Character target = hit.transform.GetComponent<Character>();
                if (target == null) continue;
                if (hit.transform.tag == "Player") continue;
                cast.setTarget(target);
            }
            return true;
        }

        private bool interactWithMovement()
        {
            if (!Input.GetMouseButton(0))
            {
                return false;
            }
            clickRay = getRayByCursor();
            RaycastHit rayhit;
            if (!Physics.Raycast(clickRay, out rayhit))
            {
                return false;
            }
            move.moveTo(rayhit.point);
            return true;
        }

        private bool interactWithPickUp()
        {
            if (!Input.GetMouseButton(0))
            {
                return false;
            }
            RaycastHit[] hits = Physics.RaycastAll(getRayByCursor());
            foreach (RaycastHit hit in hits)
            {
                PickTarget target = hit.transform.GetComponent<PickTarget>();
                if (target == null) continue;
                pick.pick(target);
                return true;
            }
            return false;
        }

        private static Ray getRayByCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

