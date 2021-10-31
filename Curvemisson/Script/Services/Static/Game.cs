using UnityEngine;
using System.Collections;

namespace Services.Static
{
    public static class Game
    {
        public static bool IsGameStatePlaying()
        {
            // TODO : 추후 다른 모드의 게임이 나오게 되면, 각 게임 별 처리 추가 요망
            return Services.Scene.SingleRacing.GameLogic.Instance.IsGameStatePlaying();
        }
    }
}