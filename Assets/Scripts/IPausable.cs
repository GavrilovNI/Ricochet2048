using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    bool IsPaused { get; }
    void Pause();
    void Continue();
}
