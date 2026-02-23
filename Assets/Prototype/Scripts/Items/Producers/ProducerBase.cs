using System.Collections;
using UnityEngine;

public abstract class ProducerBase : MonoBehaviour, ITapeable
{
    [SerializeField] protected ItemData[] items;
    protected Coroutine growthCoroutine;
    [Header("VFX")]
    [SerializeField] protected GameObject effectOnMature;

    [Space]
    public ProducerState ProducerState = ProducerState.Empty;
    
    
    [Space]
    public float growthRate = 1.0f;
    public float maxTimeToGetReady = 10.0f;


    
    public abstract void OnTap();
    public abstract void Gather();

    public bool IsEmpty() => ProducerState == ProducerState.Empty;

    protected abstract IEnumerator InProcess();

}

public enum ProducerState
{
    Empty,
    InProgress,
    ReadyToHarvest
}