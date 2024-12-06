using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Miscellaneous game elements
/// </summary>
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Setup money
        PersistentData.instance.SetMoney(PersistentData.PLAYER_STARTING_MONEY);
    }
}
