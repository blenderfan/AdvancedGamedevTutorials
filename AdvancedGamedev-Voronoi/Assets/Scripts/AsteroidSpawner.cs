using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    #region Public Fields

    public GameObject asteroidPrefab;

    public int numberOfAsteroids;

    public Vector2 sizeRange;

    public World world;

    #endregion

    #region Private Fields

    private List<Vector3> asteroidPositions = new List<Vector3>();

    #endregion

    public List<Vector3> GetAsteroidPositions() => this.asteroidPositions;


    void Start()
    {
        for(int i = 0; i < this.numberOfAsteroids; i++)
        {
            var asteroid = GameObject.Instantiate(this.asteroidPrefab);

            var pos = this.transform.position;
            var radius = Mathf.Sqrt(Random.value) * (this.world.radius - this.sizeRange.y);
            var angle = Random.Range(0.0f, 360.0f);

            var offset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

            pos = pos + offset * radius;

            asteroid.transform.position = pos;

            var localEuler = asteroid.transform.localEulerAngles;
            localEuler.z = Random.Range(0.0f, 360.0f);
            asteroid.transform.localEulerAngles = localEuler;
            asteroid.transform.localScale = Vector3.one * Random.Range(this.sizeRange.x, this.sizeRange.y);

            asteroid.transform.parent = this.transform;

            this.asteroidPositions.Add(pos);
        }
    }

}
