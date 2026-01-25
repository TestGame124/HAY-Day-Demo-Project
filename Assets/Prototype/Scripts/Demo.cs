using UnityEngine;

[CreateAssetMenu(fileName = "Demo", menuName = "Prototype/Demo")]
public class Demo : ScriptableObject
{
    public void OnButtonClick()
    {
        Debug.Log("Button Clicked!");
    }
}
