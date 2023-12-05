using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TEST_SCRIPT_001 : MonoBehaviour
{
    [SerializeField]
    [Range(0.05f, 30f)]
    float SpawnRate = 0.2f;
    [SerializeField]
    [Range(0, 30)]
    int SpawnCount = 2;
    [Range(1, 5)]
    int LineCount = 2;
    //   [SerializeField]
    //  [Range(1f, 30f)]
    //  float DestroyTime = 5f;
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
            SpawnCount = 5;
            if (SpawnCount > 2)
            {
                if (SpawnCount%2==0)
                {
                    int _the_line = SpawnCount %2;
                }
                else
                {

                }
                for (int i = SpawnCount; i <= SpawnCount; i++)
                {

                    StartCoroutine(_SpawnPrefab(i, SpawnCount));
                }

            }
            else
            {
                for (int i = -SpawnCount; i <= SpawnCount; i++)
                {

                    StartCoroutine(_SpawnPrefab(i, SpawnCount));
                }
            }
            /////////////---
            int _sediment_ = SpawnCount % LineCount;
            int _Line_Y_ = _sediment_==0?SpawnCount/LineCount:(SpawnCount / LineCount)+1;
            /////////////---
            yield return new WaitForSecondsRealtime(SpawnRate);
            Debug.Log("UN");
            yield return null;
        }
        while (true);
    }
    public IEnumerator _SpawnPrefab(int i,float s)
    {
        float Y =0.01f*((s * s *2)-((i*i)+(s*s)));
        Debug.Log((i)+"=i;y="+Y.ToString());
        GameObject _bullet = Instantiate(BulletPrefab);
        _bullet.transform.SetParent(transform);
        _bullet.transform.position = transform.position + (transform.right * 0.03f * (i))+(transform.up*Y)+(new Vector3(Random.Range(-0.005f, 0.005f), Random.Range((0.01f), Y*0.02f), Random.Range(-0.005f, 0.005f)));
        _bullet.transform.rotation = transform.rotation;
        yield return null;
    }
}
