using DG.Tweening;
using UnityEngine;

public class ItemPickEffectBehaviour : MonoBehaviour
{
    public SpriteRenderer[] images;
    
    public void Initialize(Sprite sprite)
    {
        transform.position += Vector3.up * 2;
        foreach (SpriteRenderer img in images)
        {
            img.transform.position = transform.position + (Vector3.up * 10);

            img.sprite = sprite;

            float randomZRotation = Random.Range(-40f, 40f);

            Vector3 randomOffset = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20, 20f));
            img.transform.DOJump(transform.position + randomOffset, 30,1,.7f).SetEase(Ease.OutBounce);
            //img.transform.rotation = Quaternion.Euler(30, 45, randomZRotation);
        }

    }

}
