using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerFullAuth : MonoBehaviour
{
    #region Game Manager Instance
    public static GameManagerFullAuth Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private LevelManagerFullAuth _levelManager;

    public LevelManagerFullAuth LevelManager
    {
        get { return _levelManager; }
        set { _levelManager = value; }
    }

    public void SetManager(PlayerModel model)
    {
        model.SetManager = this;
    }
}
