using GamePlay.Core;
using UnityEngine;

public class Wears : MonoBehaviour
{
    public GameObject equipInterface;
    //头部装备
    Equipment head_headBand, head_nose, head_ears, head_mask, head_helmet, head_glass;
    //上身装备
    Equipment upper_underwear, upper_short, upper_long, upper_arm, upper_shoulderR, upper_shoulderL;
    //下身装备
    Equipment lower_underwear, lower_short, lower_long, lower_leg, lower_kneeR, lower_kneeL;
    //手部装备
    Equipment handR, handL;
    //脚部装备
    Equipment footR, footL, sockR, sockL;
    //左右指环
    Equipment[] ringsL = new Equipment[5];
    Equipment[] ringsR = new Equipment[5];
    //项链
    Equipment[]  upper_neck = new Equipment[3];
    //主副武器位
    Weapon mainWeaponR, mainWeaponL;
    Weapon secWeaponR, secWeaponL;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
