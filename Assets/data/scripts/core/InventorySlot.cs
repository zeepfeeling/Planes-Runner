using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace GamePlay.Core
{
    public class InventorySlot
    {
        String slotItemName;
        int itemCount;
        Item item;

        public InventorySlot(String slotItemName,Item item, int itemCount){
            this.slotItemName = slotItemName;
            this.item = item;
            this.itemCount = itemCount;
        }
        public int getItemCount(){
            return itemCount;
        }

        public void setItemCount(int itemCount){
            this.itemCount = itemCount;
        }

        public Item getItem(){
            return item;
        }
    }
}
