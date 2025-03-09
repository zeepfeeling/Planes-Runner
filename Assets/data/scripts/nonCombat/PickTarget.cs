using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.NonCombat
{
    public class PickTarget : MonoBehaviour
    {
        public bool equipable = true;
        public bool castable = false;
        public bool pickable = true;
        public Core.Item item = null;
        public int singleItemCount = 1;
        public GameObject nameBar = null;
        GameObject nameBarInstance;

        public float pickRange = 1f;

        void Start()
        {
            if(item != null && nameBar != null){
                GameObject canvas = GameObject.Find("Canvas");
                if(canvas == null) return;
                nameBarInstance = Instantiate(nameBar,canvas.transform);
                Text itmeName = nameBarInstance.GetComponentInChildren<Text>();
                itmeName.text = item.itemName;
            }
        }

        void LateUpdate()
        {
            if (nameBarInstance != null)
            {
                // 获取Player物体的世界位置
                Vector3 worldPosition = transform.position;
                // 将世界位置转换为Canvas的屏幕空间位置
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
                // 设置UI元素的位置
                nameBarInstance.transform.position = screenPosition;
            }
        }

        public bool isEquipable(){
            return equipable;
        }

        public bool isCastable(){
            return castable;
        }

        public bool isPickable(){
            return pickable;
        }

        public Core.Item getItem(){
            return item;
        }

        public float getPickRange(){
            return pickRange;
        }

        public void vanish(){
            Destroy(gameObject);
            Destroy(nameBarInstance);
        }
    }

}