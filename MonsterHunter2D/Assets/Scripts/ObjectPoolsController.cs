using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the object pool controller. It stores pools of objects and returns the next one which can be set to true by the receiver. 
/// In case a bigger pool is needed it instantiates new objects.
/// </summary>
public class ObjectPoolsController : MonoBehaviour
{

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    Dictionary<string, GameObject> prefabDictionary;

    [SerializeField] int poolGrowRate;

    // Weapon related preafabs.
    [Header("Weapon related prefabs:")]
    [Header("Prefabs the objectpools are made of:")]
    [SerializeField] GameObject arrowNormalPrefab;
    [SerializeField] GameObject arrowRopePrefab;
    [SerializeField] GameObject spearNormalPrefab;
    [SerializeField] GameObject spearPlatformPrefab;
    [SerializeField] GameObject bombNormalPrefab;
    [SerializeField] GameObject bombStickyPrefab;
    [SerializeField] GameObject bombMegaPrefab;

    // Enemy projectiles
    [SerializeField] GameObject snakeVenomPrefab;

    // Enviorment related prefabs.
    [Header("Enviorment related prefabs:")]
    [SerializeField] GameObject smallRockPrefab;

    // Pools for every poolable object in game.
    // Weapon related pools.
    Queue<GameObject> arrowNormalPool = new Queue<GameObject>();
    Queue<GameObject> arrowRopePool = new Queue<GameObject>();
    Queue<GameObject> spearNormalPool = new Queue<GameObject>();
    Queue<GameObject> spearPlatformPool = new Queue<GameObject>();
    Queue<GameObject> bombNormalPool = new Queue<GameObject>();
    Queue<GameObject> bombStickyPool = new Queue<GameObject>();
    Queue<GameObject> bombMegaPool = new Queue<GameObject>();

    // Enviorment related pools.
    Queue<GameObject> snakeVenomPool = new Queue<GameObject>();


    // Enviorment related pools.
    Queue<GameObject> smallRocksPool = new Queue<GameObject>();

    #region Singleton
    // Singleton.
    static public ObjectPoolsController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    /// <summary>
    /// Initialize the pool dictionary and add all object pools to it.
    /// </summary>
    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();
        // Add all weapon related pools to the pool dictionary. And add the corresponding prefab to the prefab dictionary.
        poolDictionary.Add(ActiveWeaponType.ArrowNormal.ToString(), arrowNormalPool);
        prefabDictionary.Add(ActiveWeaponType.ArrowNormal.ToString(), arrowNormalPrefab);
        poolDictionary.Add(ActiveWeaponType.ArrowRope.ToString(), arrowRopePool);
        prefabDictionary.Add(ActiveWeaponType.ArrowRope.ToString(), arrowRopePrefab);
        poolDictionary.Add(ActiveWeaponType.SpearNormal.ToString(), spearNormalPool);
        prefabDictionary.Add(ActiveWeaponType.SpearNormal.ToString(), spearNormalPrefab);
        poolDictionary.Add(ActiveWeaponType.SpearPlatform.ToString(), spearPlatformPool);
        prefabDictionary.Add(ActiveWeaponType.SpearPlatform.ToString(), spearPlatformPrefab);
        poolDictionary.Add(ActiveWeaponType.BombNormal.ToString(), bombNormalPool);
        prefabDictionary.Add(ActiveWeaponType.BombNormal.ToString(), bombNormalPrefab);
        poolDictionary.Add(ActiveWeaponType.BombSticky.ToString(), bombStickyPool);
        prefabDictionary.Add(ActiveWeaponType.BombSticky.ToString(), bombStickyPrefab);
        poolDictionary.Add(ActiveWeaponType.BombMega.ToString(), bombMegaPool);
        prefabDictionary.Add(ActiveWeaponType.BombMega.ToString(), bombMegaPrefab);

        // Enemy related pools
        poolDictionary.Add("snakeVenomPool", snakeVenomPool);
        prefabDictionary.Add("snakeVenomPool", snakeVenomPrefab);


        // Add all other pools to the dictionary.
        poolDictionary.Add("smallRocksPool", smallRocksPool);
        prefabDictionary.Add("smallRocksPool", smallRockPrefab);
    }

    /// <summary>
    /// If another method needs an object to "instantiate", get the object from the pool instead.
    /// </summary>
    /// <param name="type">The name of the pool you want an object from.</param>
    /// <returns>The GameObject the receiver can work with.</returns>
    public GameObject GetFromPool(string poolName)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            if (poolDictionary[poolName].Count == 0)
                GrowPool(poolName);
            return poolDictionary[poolName].Dequeue();
        }
        else
        {
            Debug.Log("Pool with tag: " + poolName + " doesnt exist.");
            return null;
        }
    }

    /// <summary>
    /// Instantiates more from the pool specific prefabs if needed.
    /// </summary>
    /// <param name="poolToGrow">The pool to instantiate more prefabs for.</param>
    private void GrowPool(string poolToGrow)
    {
        if (prefabDictionary.ContainsKey(poolToGrow))
        {
            for (int i = 0; i < poolGrowRate; i++)
            {
                var instanceToAdd = Instantiate(prefabDictionary[poolToGrow]);
                instanceToAdd.transform.SetParent(transform);
                AddToPool(instanceToAdd, poolToGrow);
            }
        }
    }

    /// <summary>
    /// Deactivates the already instantiated gameobject first. And then adds it to its corresponding pool.
    /// </summary>
    /// <param name="instance">The gameobject to add to the pool.</param>
    /// <param name="poolToAddTo">The pool to add the gameobject to.</param>
    public void AddToPool(GameObject instance, string poolToAddTo)
    {
        instance.SetActive(false);
        instance.transform.SetParent(transform);
        poolDictionary[poolToAddTo].Enqueue(instance);
    }
}
