using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectButtonFix : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    { 
        if (gameObject.activeInHierarchy && EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayUIHoverSound();
        }
    }
}
