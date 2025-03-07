using System;
using System.Collections.Generic;
using GamePlay.NonCombat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GamePlay.Interface
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Sprite defaultSlotIcon;
        String slotItemName;
        int itemCount;
        Item item;
        Image slotIcon;
        Text itemCountText;
        bool needUpdateSlot = false;
        private Canvas canvas;
        private RectTransform draggingRT;
        Vector2 slotPosition;
        Transform characterTransform;


        void Start()
        {
            //获取画布
            canvas = GetComponentInParent<Canvas>();
            slotIcon = transform.Find("item_icon").GetComponent<Image>();
            itemCountText = transform.Find("item_count").GetComponent<Text>();
        }

        void Update()
        {
            if(!needUpdateSlot) return;
            if(item != null && item.icon != null) 
                slotIcon.sprite = item.icon;
            else 
                slotIcon.sprite = defaultSlotIcon;
            itemCountText.text = itemCount.ToString();
            needUpdateSlot = false;
        }

        /* ********************************************处理物品标签拖动逻辑*************************************************** */
        public void OnBeginDrag(PointerEventData eventData)
        {
            //存储组件初始位置
            draggingRT = GetComponent<RectTransform>();
            slotPosition = new Vector2(draggingRT.anchoredPosition.x, draggingRT.anchoredPosition.y);
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            if (draggingRT != null)
            {
                draggingRT.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {
            //从缓存中获取点击ui的碰撞结果
            List<RaycastResult> results = InterfacePointCache.results;
            if(results == null) return;
            //flag，用于判断是否拖出背包或者装备栏区域
            bool stillInUI = false;
            foreach (RaycastResult result in results)
            {
                if (gameObject != result.gameObject && result.gameObject.tag == "item_slot")
                {
                    //交换格子位置
                    draggingRT.anchoredPosition = result.gameObject.GetComponent<RectTransform>().anchoredPosition;
                    result.gameObject.GetComponent<RectTransform>().anchoredPosition = slotPosition;
                    return;
                }
                if(result.gameObject.tag == "item_relative_ui"){
                    stillInUI = true;
                }
            }
            //拖动离开ui范围，实行丢弃方案
            if(!stillInUI && item != null){
                //丢弃到世界
                GameObject gnd = Instantiate(item.gnd,characterTransform.position,Quaternion.identity);
                PickTarget pickTarget = gnd.GetComponent<PickTarget>();
                pickTarget.singleItemCount = itemCount;
                //物品栏中去除
                cleanInventorySlot();
                
            }
            draggingRT.anchoredPosition = slotPosition;
        }

        /* ********************************************处理物品标签拖动逻辑*************************************************** */

        public void updateInventorySlot(String slotItemName,Item item, int itemCount){
            this.slotItemName = slotItemName;
            this.item = item;
            this.itemCount = this.itemCount == 0?itemCount : this.itemCount + itemCount;
            needUpdateSlot = true;
        }

        public void cleanInventorySlot(){
            //更新数据结构
            slotItemName = null;
            item = null;
            itemCount = 0;
            needUpdateSlot = true;
        }

        public void setItemName(String itemName){
            this.slotItemName = itemName;
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

        public void setItem(Item item){
            this.item = item;
        }

        public void setTransform(Transform transform){
            this.characterTransform = transform;
        }
    }
}
