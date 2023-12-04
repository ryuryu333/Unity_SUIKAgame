using UnityEngine;

namespace MainSceneEnumList
{
    public enum GameObjectTag
    {
        FallingObject,
        FallingObjectIgnoreGameover,
        FallingObjectBeforeDrop,
        GameoverLine
    }

    public enum MainSceneSituation
    {
        Initializing,
        Playing,
        Pause,
        Gameover
    }
}
