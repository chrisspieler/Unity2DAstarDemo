using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Vector2> CurrentPath;
    public float Speed = 4.5f;
    public float ArriveRadius = 0.1f;

    private void Start()
    {
        NavigationManager nav = GameController.Current.Navigation;
        nav.OnPathfinderUpdated += (s, e) =>
        {
            nav.UpdateEnemyPath(this);
        };
    }

    void Update()
    {
        if (!(CurrentPath?.Count > 0))
        {
            return;
        }
        var nextSegment = CurrentPath[0];
        if (Vector2.Distance(nextSegment, transform.position) <= ArriveRadius)
        {
            CurrentPath.RemoveAt(0);
        }
        else
        {
            var direction = nextSegment - (Vector2)transform.position;
            transform.Translate(Speed * Time.deltaTime * direction);
        }
    }
}
