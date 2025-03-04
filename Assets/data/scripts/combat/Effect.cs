using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GamePlay.Core
{

    public class Effect : MonoBehaviour
    {
        ParticleSystem[] particleSystems;
        void Start(){
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }
        // Update is called once per frame
        void Update()
        {
            foreach(ParticleSystem particleSystem in particleSystems){
                if(particleSystem.IsAlive())
                    return;
            }
            Destroy(gameObject);
        }
    }
}
