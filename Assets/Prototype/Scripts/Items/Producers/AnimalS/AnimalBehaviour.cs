using UnityEngine;

public class AnimalBehaviour : MonoBehaviour
{
    public AnimalGoodsState State;
  

}

public enum AnimalGoodsState
{
    Idle,
    InProcess,
    Ready
}
