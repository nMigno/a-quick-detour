using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectButtonFix : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    { 
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
