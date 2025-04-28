using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UI;

public class Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject _carPrefab; // Prefab of the car to spawn
    [SerializeField]
    private GameObject _coinPrefab; // Prefab of the coin to spawn
    private float _coinYOffset = 4.0f; // Y offset for coin spawning
    private float _carYOffset = 0.5f; // Y offset for car spawning
    private float speed = 5f;
    private List<GameObject> _spawnedObjects = new List<GameObject>();

    public List<GameObject> GetSpawnedObjects()
    {
        return _spawnedObjects;
    }

    public void SpawnCar(Vector3 position)
    {
        GameObject car = Instantiate(_carPrefab, position + new Vector3(0, _carYOffset, 0), Quaternion.identity);
        _spawnedObjects.Add(car);
    }

    public void SpawnCoin(Vector3 position)
    {
        GameObject coin = Instantiate(_coinPrefab, position + new Vector3(0, _coinYOffset, 0), Quaternion.identity);
        _spawnedObjects.Add(coin);
    }
    void Start()
    {
        speed = Random.Range(3f, 10f);
    }

    public void ResetSpawner()
    {
        speed = Random.Range(3f, 7f);
        foreach (GameObject obj in _spawnedObjects)
        {
            Destroy(obj);
        }
    } 

    // Update is called once per frame
    private float _nextSpawnTime = 0f;

    void Update()
    {
        // Move the spawned objects forward (from positive X to negative X direction) until they are at X = -14
        foreach (GameObject obj in _spawnedObjects)
        {
            if (obj != null)
            {
                obj.transform.position += Vector3.left * speed * Time.deltaTime;
                if (obj.transform.position.x < transform.position.x - 28f)
                {
                    Destroy(obj);
                }
            }
        }
        // Remove null references from the list
        _spawnedObjects.RemoveAll(item => item == null);

        // Check if it's time to spawn a new object
        if (Time.time >= _nextSpawnTime)
        {
            Vector3 spawnPosition = transform.position; // Use the current position of the spawner
            if (Random.Range(0, 2) == 0)
            {
                SpawnCar(spawnPosition);
            }
            else
            {
                SpawnCoin(spawnPosition);
            }

            // Set the next spawn time
            _nextSpawnTime = Time.time + Random.Range(2f, 5f);
        }
    }
}
