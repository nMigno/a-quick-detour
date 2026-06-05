using UnityEngine;
using System.Collections.Generic;

public class LightBeam : MonoBehaviour
{
    public Flashlight flashlightController;
    public bool isRevealMode = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("La luz tocó a: " + other.gameObject.name);

        if (!isRevealMode && other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetIlluminated(true);
        }

        if (!isRevealMode && other.TryGetComponent<CupEnemy>(out CupEnemy cupEnemy))
        {
            cupEnemy.SetIlluminated(true);
        }

        if (isRevealMode && other.TryGetComponent<IllusionWall>(out IllusionWall wall))
        {
            wall.Reveal();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!isRevealMode && other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.SetIlluminated(false);
        }

        if (!isRevealMode && other.TryGetComponent<CupEnemy>(out CupEnemy cupEnemy))
        {
            cupEnemy.SetIlluminated(false);
        }
    }
}
