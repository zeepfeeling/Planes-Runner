using UnityEngine;
namespace GamePlay.Core
{
    public class PointBar : MonoBehaviour
    {

        public GameObject pointBarPrefab;
        GameObject uiCanvas;
        Character attributes;
        GameObject pointBar;
        GameObject pointFill;
        float maxValue = 100;
        float currentValue = 0;
        Vector2 barSize;



        private void Start()
        {
            //寻找画布组件
            uiCanvas = GameObject.FindWithTag("UICanvas");
            //实例化点数条
            pointBar = Instantiate(pointBarPrefab, uiCanvas.transform);
            pointFill = pointBar.transform.Find("hitPointBar_float").gameObject;
            attributes = GetComponent<Character>();
            barSize = pointBar.GetComponent<RectTransform>().sizeDelta;
        }
        void LateUpdate()
        {
            if (pointBar == null) return;
            barPositionUpdate();
            barLengthUpdate();
        }

        //点数条长度更新
        private void barLengthUpdate()
        {
            if (pointFill == null) return;
            currentValue = attributes.getHitPoint();
            maxValue = attributes.getMaxHitPoint();
            float valueRate = currentValue / maxValue;
            pointFill.GetComponent<RectTransform>().sizeDelta = new Vector2(barSize.x * valueRate, barSize.y);
        }

        // 点数条位置更新
        private void barPositionUpdate()
        {
            Vector3 cScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 uiPosition = new Vector2(cScreenPosition.x, cScreenPosition.y + 150);
            RectTransform uiTranform = pointBar.GetComponent<RectTransform>();
            uiTranform.position = uiPosition;
        }
    }
}