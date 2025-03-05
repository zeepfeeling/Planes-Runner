using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Core
{
    public class Inventory : MonoBehaviour
    {
        public GameObject inventoryInterface;
        public GameObject slotIconSample;
        int coins = 0;
        int slotCopunt = 40;
        Dictionary<int, InventorySlot> inventorySlots = new Dictionary<int, InventorySlot>();
        Transform inventorySlotInterface;
        bool needUpdateInv = false;

        //初始化背包格子以及对应数据结构
        void Awake()
        {
            if(inventoryInterface != null){
                inventorySlotInterface = inventoryInterface.transform.Find("items/Slots");
                for(int index = 0; index < 40; index ++){
                    GameObject slotIcon = Instantiate(slotIconSample);
                    slotIcon.name = slotIconSample.name + index; // 可选：更改复制对象的名称
                    slotIcon.transform.SetParent(inventorySlotInterface, false);
                }
            }
            for(int index = 0; index < 40; index ++){
                inventorySlots.Add(index, null);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            updateInventoryImages();
        }

        // Update is called once per frame
        void Update()
        {
            if(needUpdateInv){
                needUpdateInv = false;
                updateInventoryImages();
            }

        }

        public void addItemsToInventory(Item item,int singleItemCount){
            //货币类处理
            if(item.GetType() == typeof(Coin)){
                coins += singleItemCount;
                return;
            }
            InventorySlot inventorySlot = new InventorySlot(item.itemName, item, singleItemCount);
            //将物品设置到格子中
            inventorySlots[checkValidSlotNum()] = inventorySlot;
            Debug.Log("拾取了 " + item.itemName + " " + singleItemCount + "个");
            needUpdateInv = true;
        }
        
        public void removeItemFromInventory(int slotIndex, int removeCount){
            InventorySlot inventorySlot = inventorySlots[slotIndex];
            if(removeCount >= inventorySlot.getItemCount()){
                inventorySlots[slotIndex] = null;
            } else {
                inventorySlot.setItemCount(inventorySlot.getItemCount() - removeCount);
            }
            needUpdateInv = true;
        }

        void updateInventoryImages(){
            if(inventoryInterface == null) return;
            foreach (KeyValuePair<int, InventorySlot> kvp in inventorySlots)
            {
                Image slotIcon = inventorySlotInterface.GetChild(kvp.Key).Find("item_icon").GetComponent<Image>();
                Text itemCount = inventorySlotInterface.GetChild(kvp.Key).Find("item_count").GetComponent<Text>();
                if (kvp.Value != null)
                {
                    Item item = kvp.Value.getItem();
                    if(item.icon != null)
                        slotIcon.sprite = item.icon;
                    itemCount.text = kvp.Value.getItemCount().ToString();
                } else {
                    itemCount.text = "0";
                }
            }
        }

        int checkValidSlotNum()
        {
            foreach (KeyValuePair<int, InventorySlot> kvp in inventorySlots)
            {
                if (kvp.Value == null)
                {
                    return kvp.Key;
                }
                // 处理键值对
            }
            return 0;
        }
    }
}
