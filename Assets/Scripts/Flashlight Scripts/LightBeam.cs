using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public Flashlight flashlightController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetIlluminated(true, flashlightController.stunDuration);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetIlluminated(false, 0f);
        }
    }
}
