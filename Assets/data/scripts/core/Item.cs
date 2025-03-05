using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Core
{
    [CreateAssetMenu(fileName = "Item", menuName = "GamePlay/Item", order = 0)]
    public class Item : ScriptableObject
    {
        public String itemName;
        public Sprite icon;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
