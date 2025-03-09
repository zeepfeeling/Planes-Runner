using System;
using System.Collections.Generic;
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
        List<InventorySlot> inventorySlots = new List<InventorySlot>();
        Transform inventorySlotInterface;

        //初始化背包格子以及对应数据结构
        void Awake()
        {
            if(inventoryInterface != null){
                inventorySlotInterface = inventoryInterface.transform.Find("items/Slots");
                for(int index = 0; index < slotCopunt; index ++){
                    GameObject slotIcon = Instantiate(slotIconSample);
                    slotIcon.transform.SetParent(inventorySlotInterface, false);
                    InventorySlot inventorySlot = slotIcon.GetComponent<InventorySlot>();
                    inventorySlot.setTransform(transform);
                    inventorySlots.Add(inventorySlot);
                }
            }
        }

        public bool addItemsToInventory(Item item,int singleItemCount){
            //货币类处理
            if(item.GetType() == typeof(Coin)){
                coins += singleItemCount;
                return true;
            }
            InventorySlot inventorySlot = getValidSlot(item);
            //当没有可用格子，无法添加
            if(inventorySlot == null){
                return false;
            }
            //将物品设置到格子中
            inventorySlot.updateInventorySlot(item.itemName, item, singleItemCount);
            Debug.Log("拾取了 " + item.itemName + " " + singleItemCount + "个");
            return true;
        }

        InventorySlot getValidSlot(Item item)
        {
            //装备类型或者新物品需要分配一个新槽位
            if(item.GetType() == typeof(Weapon)) {
                foreach (InventorySlot inventorySlot in inventorySlots)
                {
                    if (inventorySlot.getItem() == null)
                    {
                        return inventorySlot;
                    }
                }
                return null;
            } else {
                foreach (InventorySlot inventorySlot in inventorySlots)
                {
                    //同物品堆叠
                    if (inventorySlot.getItem() != null && inventorySlot.getItem().itemName == item.itemName)
                    {
                        return inventorySlot;
                    }
                }
                //新物品分配新槽位
                foreach (InventorySlot inventorySlot in inventorySlots)
                {
                    if (inventorySlot.getItem() == null)
                    {
                        return inventorySlot;
                    }
                }
                return null;
            }
        }
    }
}
