using System;
using GamePlay.Core;
using UnityEngine;
namespace GamePlay.Combat
{
    public class Ammo : MonoBehaviour
    {
        public float damage = 0;
        public float speed = 5f;
        public float maxDistance = 200f;
        public GameObject hitEffect = null;
        public String hitBuffName = null;
        public Buff[] buffs = null;
        float currentDistance = 0f;
        Vector3 direction;
        
        private void Update() {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            currentDistance += speed * Time.deltaTime;
            // be destoried when fly too far
            if(currentDistance >= maxDistance){
                Destroy(gameObject);
            }
        }

        public void setDamage(float damage){
            this.damage = damage;
        }

        public void shoot(Vector3 direction){
            transform.LookAt(direction);
        }

        private void OnTriggerEnter(Collider other) {
            // don't shoot yourself
            if (other.gameObject.layer != 3) return;
            //do damage
            Character target = other.transform.GetComponent<Character>();
            target.takeDamage(damage);
            if(hitEffect != null){
                Instantiate(hitEffect,transform.position,Quaternion.identity);
            }
            if(!target.isDead()){
                target.addBuffs(buffs);
            }
            Destroy(gameObject);
        }
    }
}