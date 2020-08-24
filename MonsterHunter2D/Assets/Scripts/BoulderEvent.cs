using UnityEngine;

/// <summary>
/// an event that spawns boulders once triggered. Joachim
/// </summary>
public class BoulderEvent : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxSpawnIntervalTime;
    [SerializeField] private GameObject[] boulders = new GameObject[10];
    [SerializeField] private Transform[] spawnPoints;

    private float currentTime = 0f;
    private float currentInterval;

    private bool activated;
 
    // Start is called before the first frame update
    void Start()
    {
        currentInterval = GetNextIntervalTime();
        currentTime = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated)
            return;

        if (Waited())
            SpawnBoulder();
    }

    private void SpawnBoulder()
    {
        for (int i = 0; i < boulders.Length; i++)
        {
            if (boulders[i].activeSelf == false)
            {
                boulders[i].transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length - 1)].position;
                boulders[i].SetActive(true);

                currentTime = 0f;
                currentInterval = GetNextIntervalTime();
                return;
            }
        }

        for (int i = 0; i < boulders.Length; i++)
        {
            if (boulders[i].activeSelf == true)
            {
                boulders[i].GetComponent<DestructableObject>().ActivateDestruction();
            }
        }
    }

    private float GetNextIntervalTime()
    {
        return UnityEngine.Random.Range(minMaxSpawnIntervalTime.x, minMaxSpawnIntervalTime.y);
    }

    private bool Waited()
    {
        currentTime += Time.deltaTime;

        if (currentTime < currentInterval)
            return false;
        else
            return true;
    }

    public void Activate()
    {
        activated = true;
    }

    public void DeActivate()
    {
        activated = false;

        for (int i = 0; i < boulders.Length; i++)
        {
            if (boulders[i].activeSelf == true)
            {
                boulders[i].GetComponent<DestructableObject>().ActivateDestruction();
            }
        }
    }

    


}
