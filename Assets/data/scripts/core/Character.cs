using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay.Core;
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

        float maxBattleRnage = 0f; //最大交战距离
        float minBattleRnage = 0f; //最小交战距离

        void Awake()
        {

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
            staggersPoint = Mathf.Max(staggersPoint - staggersRecoverRate, 0);
            if (buffs == null || buffs.Count() == 0)
            {
                return;
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

        public bool inStaggerStatus()
        {
            return staggerStatus;
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
            if (staggersPoint >= maxStaggersPoint && GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetBool("getHit", true);
                staggerStatus = true;
            }
        }

        public void resetStaggers()
        {
            GetComponent<Animator>().SetBool("getHit", false);
            staggerStatus = false;
            staggersPoint = 0;
        }

        public void costMana(float cost)
        {
            manaPoint = Mathf.Max(manaPoint - cost, 0);
        }


        public float getMaxBattleRange()
        {
            return maxBattleRnage;
        }
        public float getMinBattleRange()
        {
            return minBattleRnage;
        }


        private void die()
        {
            if (dead) return;
            dead = true;
            GetComponent<Animator>().SetBool("die", true);
            GetComponent<ActionScheduler>().stopCurrentAction();
        }
    }
}