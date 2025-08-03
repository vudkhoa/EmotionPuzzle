using UnityEngine;
using CustomUtils;
using System;

public class GameManager : SingletonMono<GameManager>
{
    public GameState State;

    private void Start()
    {
        GameModeContainer.Instance.InitGame();
        InitController();

        SlideController.Instance.SpawnLevel();
        
        State = GameState.Playing;
        //UIManager.Instance.OpenUI<GameplayUI>();

    }

    private void InitController()
    {
        CreateModule("DataManager", "DataManager");
        CreateModule("SlideController", "SlideController");
        CreateModule("GroundTileController", "GroundTileController");
        CreateModule("ItemTileController", "ItemTileController");
        CreateModule("ObstacleTileController", "ObstacleTileController");
        CreateModule("ElementController", "ElementController");
        CreateModule("BossController", "BossController");
        CreateModule("IceStarController", "IceStarController");
        CreateModule("RotateObjectController", "RotateObjectController");
    }

    private GameObject CreateObject(string module, string nameModule)
    {
        GameObject loginObject = GameObject.Instantiate(Resources.Load<GameObject>(module));
        loginObject.name = nameModule;

        return loginObject;
    }

    private GameObject CreateModule(string module, string nameModule)
    {
        GameObject loginObject = GameObject.Instantiate(Resources.Load<GameObject>(module), GameModeContainer.Instance.Manager.transform);
        loginObject.name = nameModule;

        return loginObject;
    }
}

[Serializable]
public enum GameState
{
    None = 0,
    Playing = 1,
    Pause = 2,
    GameOver = 3,
}
