using System;
using UnityEngine;

namespace GamePlay.Core{

    [CreateAssetMenu(fileName = "Coin", menuName = "GamePlay/Coin", order = 4)]
    public class Coin : Item{
        String coinType;
    }
}