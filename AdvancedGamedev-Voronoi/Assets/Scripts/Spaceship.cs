using Unity.Profiling;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    #region Public Fields

    public float asteroidInfluence = 1.5f;

    public float spaceshipSize;

    public World world;

    public Vector2 velocityRange;

    #endregion

    #region Private Fields

    private AsteroidSpawner asteroidSpawner;

    private Vector3 velocity;

    #endregion


    void Start()
    {
        this.asteroidSpawner = GameObject.FindObjectOfType<AsteroidSpawner>();

        float vel = Random.Range(this.velocityRange.x, this.velocityRange.y);

        var rndCircle = Random.insideUnitCircle;
        if (Mathf.Abs(rndCircle.x) < 0.1f) rndCircle.x = Mathf.Sign(rndCircle.x) * 0.1f;
        if (Mathf.Abs(rndCircle.y) < 0.1f) rndCircle.y = Mathf.Sign(rndCircle.y) * 0.1f;

        this.velocity = new Vector3(rndCircle.x, rndCircle.y, 0.0f) * vel;
    }

    public void UpdatePositionAndVelocity(ProfilerMarker marker, VoronoiLookupTable table = null)
    {
        var pos = this.transform.position;

        var nextPos = pos + this.velocity * Time.deltaTime;
        var dir = nextPos.normalized;
        if (nextPos.magnitude > this.world.radius - this.spaceshipSize * 0.5f)
        {
            this.velocity = Vector3.Reflect(this.velocity, - dir);
        }


        float velocityLength = this.velocity.magnitude;

        //Without Lookup Table
        var asteroidPositions = this.asteroidSpawner.GetAsteroidPositions();

        int closestAsteroid = -1;
        float closestDistSq = float.PositiveInfinity;

        if (table == null)
        {
            marker.Begin();
            for (int i = 0; i < asteroidPositions.Count; i++)
            {
                var asteroidPos = asteroidPositions[i];
                var asteroidDir = asteroidPos - nextPos;
                float distSq = Vector3.Dot(asteroidDir, asteroidDir);
                if (distSq < closestDistSq)
                {
                    closestAsteroid = i;
                    closestDistSq = distSq;
                }
            }
            marker.End();
        } else
        {
            marker.Begin();
            int index = table.GetTableIndex(new Vector2(nextPos.x, nextPos.y));
            if(index > 0)
            {
                closestAsteroid = table.table[index];
            }
            marker.End();
        }

        if (closestAsteroid >= 0)
        {
            var closestAsteroidPos = asteroidPositions[closestAsteroid];
            var closestAsteroidDir = closestAsteroidPos - nextPos;
            this.velocity = Vector3.Normalize(Vector3.Lerp(this.velocity.normalized, closestAsteroidDir.normalized, Time.deltaTime * this.asteroidInfluence)) * velocityLength;

            float angle = Vector3.SignedAngle(Vector3.right, this.velocity.normalized, Vector3.forward);
            if (angle < 0.0f) angle += 360.0f;

            this.transform.position = nextPos;
            this.transform.rotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
        }

    }
}
