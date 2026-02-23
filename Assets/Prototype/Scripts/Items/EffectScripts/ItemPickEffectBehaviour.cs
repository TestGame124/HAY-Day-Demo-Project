using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;

public class ItemPickEffectBehaviour : MonoBehaviour
{
    public SpriteRenderer[] images;

    [SerializeField] UIParticle uiParticles;
    [SerializeField] UIParticleAttractor uiAttractor;

    
    
    public void Initialize(Sprite sprite)
    {
        transform.position += Vector3.up * 2;

        uiParticles.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        uiParticles.material.SetTexture("_BaseMap", sprite.texture);
        if (uiParticles.material.HasProperty("_Color")) uiParticles.material.SetColor("_Color", Color.white);
        if (uiParticles.material.HasProperty("_BaseColor")) uiParticles.material.SetColor("_BaseColor", Color.white);

        foreach (var item in uiParticles.particles)
        {
            ParticleSystemRenderer renderer = item.GetComponent<ParticleSystemRenderer>();

            Material mat = new Material(renderer.material);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", Color.white);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
            mat.SetTexture("_BaseMap", sprite.texture);

            renderer.material = mat;

        }
      
        uiParticles.transform.SetParent(UIController.instance.mainCanvas.transform, true);

        float destroyDelay = 4f;
        DOVirtual.DelayedCall(destroyDelay, () =>
        {

            Destroy(uiParticles.gameObject);
            Destroy(gameObject);
        });
    }

}
