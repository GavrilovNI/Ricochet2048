using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPauser : MonoBehaviour, IPausable
{
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;
    [SerializeField] private Map _map;
    [SerializeField] private InGameMenu _inGameMenu;

    public bool IsPaused => _map.IsPaused;

    public bool CanPause => _map.State == Map.GameState.Playing;
    public bool CanContinue => IsPaused;

    public void Continue()
    {
        if(CanContinue == false)
            throw new System.InvalidOperationException("Can't continue. Game is not paused.");
        _inGameMenu.HideAll();
        _map.Continue();
    }

    public void Pause()
    {
        if(CanPause == false)
            throw new System.InvalidOperationException("Can't pause. Game is not playing.");
        _map.Pause();
        _inGameMenu.ShowPauseMenu();
    }

    private void Update()
    {
        if(Input.GetKeyDown(_pauseKey))
        {
            if(CanPause)
                Pause();
            else if(CanContinue)
                Continue();
        }
    }
}
