using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public Flashlight flashlightController;
    public bool isRevealMode = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("La luz tocó a: " + other.gameObject.name);

        if (other.TryGetComponent<ChairEnemy>(out ChairEnemy enemy))
        {
            enemy.SetIlluminated(true);
        }

        if (other.TryGetComponent<FallingTrapEnemy>(out FallingTrapEnemy trapEnemy))
        {
            trapEnemy.SetIlluminated(true);
        }

        if (other.TryGetComponent<IllusionWall>(out IllusionWall wall))
        {
            wall.Reveal();
        }

        if (other.TryGetComponent<Mannequin>(out Mannequin mannequinEnemy))
        {
            mannequinEnemy.SetIlluminated(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<ChairEnemy>(out ChairEnemy enemy))
        {
            enemy.SetIlluminated(false);
        }

        if (other.TryGetComponent<FallingTrapEnemy>(out FallingTrapEnemy trapEnemy))
        {
            trapEnemy.SetIlluminated(false);
        }

        if(other.TryGetComponent<Mannequin>(out Mannequin mannequinEnemy))
        {
            mannequinEnemy.SetIlluminated(false);
        }
    }

}
