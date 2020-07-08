using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRockEvent : MonoBehaviour
{
    [SerializeField] private GameObject[] rockPool;
    [SerializeField] private Vector2 spawnAreaSize;
    [SerializeField] private GameObject rockPrefab;
    private Vector3 spawnArea;
    public bool started = false;
    [SerializeField] private float spawnInterval = 2f;
    private float currentTime = 0f;

    private void Awake() 
    {
        for (int i = 0; i < rockPool.Length; i++)
        {
            Vector2 randomPos = new Vector2(UnityEngine.Random.Range(this.transform.position.x + spawnAreaSize.x, this.transform.position.x - spawnAreaSize.x),
                                        UnityEngine.Random.Range(this.transform.position.y + spawnAreaSize.y, this.transform.position.y - spawnAreaSize.y));
            GameObject gO = Instantiate(rockPrefab, randomPos, Quaternion.identity);
            gO.transform.SetParent(this.transform);
            FallingRock rock = gO.GetComponent<FallingRock>();
            rock.Setup(randomPos);
            rock.Disable();
            rockPool[i] = gO;       
        }
    }

    private void Update() 
    {
        if (!started)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= spawnInterval)
            Spawn();
    }

    public void Spawn()
    {
        currentTime = 0f;

        for (int i = 0; i < rockPool.Length; i++)
        {
            if (!rockPool[i].gameObject.activeSelf)
            {
                Vector2 randomPos = new Vector2(UnityEngine.Random.Range(this.transform.position.x + spawnAreaSize.x, this.transform.position.x - spawnAreaSize.x),
                                        UnityEngine.Random.Range(this.transform.position.y + spawnAreaSize.y, this.transform.position.y - spawnAreaSize.y));
                rockPool[i].GetComponent<FallingRock>().Setup(randomPos);
                rockPool[i].GetComponent<FallingRock>().Enable();
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(this.transform.position, spawnAreaSize);
    }
}
