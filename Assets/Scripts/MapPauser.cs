using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPauser : MonoBehaviour, IPausable
{
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;
    [SerializeField] private Map _map;
    [SerializeField] private PlatformMover _platfowmMover;
    [SerializeField] private InGameMenu _inGameMenu;

    public bool IsPaused => _map.IsPaused;

    public bool CanPause => _map.State == Map.GameState.Playing;
    public bool CanContinue => IsPaused;

    public void Continue()
    {
        _inGameMenu.HideAll();
        _map.Continue();
        _platfowmMover.Continue();
    }

    public void Pause()
    {
        _map.Pause();
        _platfowmMover.Pause();
    }

    private void Update()
    {
        if(Input.GetKeyDown(_pauseKey))
        {
            if(IsPaused)
            {
                Continue();
            }
            else
            {
                Pause();
                _inGameMenu.ShowPauseMenu();
            }
        }
    }
}
