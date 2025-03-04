using System;
using GamePlay.Core;
using UnityEngine;
namespace GamePlay.Core{
    
    [CreateAssetMenu(fileName = "Buff", menuName = "GamePlay/Buff", order = 2)]
    public class Buff : ScriptableObject {
        public String buffName = "";
        public String desc = "";
        public float duration = 1f;
        public float value = 1f;
        float currentTime = 0f;

        Character attributes = null;

        public void buffact(Character attributes)
        {
            switch(buffName){
                case "jump_slash_powerUp":{
                    jumpSlashPowerUp();
                }
                return;
            }
        }

        private void jumpSlashPowerUp()
        {
            throw new NotImplementedException();
        }

        public void buffTimeForward(){
            currentTime += 1f;
        }

        public float getCurrentTime(){
            return currentTime;
        }

        public float getDuration(){
            return duration;
        }

        public String getBuffName(){
            return buffName;
        }

        public Buff buffInstance(){
            Buff newBuff = new Buff();
            newBuff.buffName = this.buffName;
            newBuff.desc = this.desc;
            newBuff.duration = this.duration;
            newBuff.currentTime = this.currentTime;
            return newBuff;
        }
    }
}