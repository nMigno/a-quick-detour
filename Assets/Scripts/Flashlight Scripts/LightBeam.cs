using UnityEngine;
using System.Collections.Generic;

public class LightBeam : MonoBehaviour
{
    public Flashlight flashlightController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("La luz tocó a: " + other.gameObject.name);

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetIlluminated(true, flashlightController.stunDuration);
        }

        if (other.TryGetComponent<IllusionWall>(out IllusionWall wall))
        {
            wall.Reveal();
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
