using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GamePlay.Core
{
    public class Inventory : MonoBehaviour
    {
        Dictionary<int,Item> itemSignInDictionary = new Dictionary<int, Item>();
        Dictionary<int,int> itemCountDictionary = new Dictionary<int,int>();
        int coins = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void addItemsToInventory(Item item,int count){
            itemSignInDictionary.Add(item.id, item);
            itemCountDictionary.Add(item.id, count);
            if(item.isCoin){
                coins = count;
            }
        }
    }
}
