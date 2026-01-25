using UnityEngine;
using UnityEngine.UI;

public class ConfrimationWorldUIBehaviour : MonoBehaviour
{
    [SerializeField] Button confirmBtn, cancelBtn, rotateBtn;
    [SerializeField] Button sellBtn;
    

    public void Initialize(Transform itemPos,System.Action onConfirm, System.Action onCancel, System.Action onRotate, System.Action optionalAction = null)
    {
        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
        rotateBtn.onClick.RemoveAllListeners();

        confirmBtn.onClick.AddListener(() => onConfirm?.Invoke());
        cancelBtn.onClick.AddListener(() => onCancel?.Invoke());
        rotateBtn.onClick.AddListener(() => onRotate?.Invoke());

        if (sellBtn)
        {
            sellBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.AddListener(() => optionalAction?.Invoke());
        }

        transform.position = itemPos.position + new Vector3(0,2,0);
    }

}
