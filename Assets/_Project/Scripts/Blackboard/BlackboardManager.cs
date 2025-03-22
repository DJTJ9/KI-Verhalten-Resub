﻿using BlackboardSystem;
using UnityEngine;

public class BlackboardManager : MonoBehaviour
{
    public static Blackboard SharedBlackboard { get; private set; } = new Blackboard();

    [SerializeField] private BlackboardData blackboardData;

    void Awake()
    {
        if (blackboardData)
        {
            blackboardData.SetValuesOnBlackboard(SharedBlackboard);
        }
    }
}