using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TEST_SCRIPT_001 : MonoBehaviour
{
    [SerializeField]
    [Range(0.05f, 30f)]
    float SpawnRate = 0.2f;
    [SerializeField]
    [Range(2, 30)]
    int SpawnCount = 2;
    [SerializeField]
    [Range(3f, 30f)]
    float DestroyTime = 5f;
    [SerializeField]
    private GameObject BulletPrefab;
    void Start()
    {
        StartCoroutine(_SpawnPrefabs());
    }
    public IEnumerator _SpawnPrefabs()
    {
        do
        {
            for (int i= -SpawnCount; i<SpawnCount;i++)
            {
                GameObject _bullet = Instantiate(BulletPrefab, transform);
                _bullet.transform.position = transform.position+ new Vector3(i,0,0);
                _bullet.transform.rotation = transform.rotation;
                _bullet.GetComponent<_RedBulletScript>().DestroyTime = DestroyTime;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(SpawnRate);
            yield return null;
        }
        while (true);
    }
}
