using DG.Tweening;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public ItemPickEffectBehaviour itemPickEffectPrefab;

    public void SpawnItemPickerEffect(Sprite itemSprite, Vector3 itemPos)
    {
        ItemPickEffectBehaviour itemPickEffect = Instantiate(itemPickEffectPrefab, transform.position, Quaternion.identity);
        itemPickEffect.transform.position = itemPos;
        itemPickEffect.Initialize(itemSprite);
        itemPickEffect.gameObject.SetActive(true);
        DOVirtual.DelayedCall(5.0f, () =>
        {
            Destroy(itemPickEffect.gameObject);
        });
    }
}
