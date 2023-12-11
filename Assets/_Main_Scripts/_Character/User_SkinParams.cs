using System.Collections;
using UnityEngine;

public class User_SkinParams : MonoBehaviour
{
    public GameObject DashPrefab;
    public GameObject BasicAttackPrefab;
    public GameObject Attack1Prefab;
    public GameObject Attack2Prefab;
    public GameObject Attack3Prefab;
    public GameObject TauntPrefab;
}
namespace WaitRealTime
{
    public static class WaitToRealTime
    {
        public static IEnumerator WaitRealTime(float _Time)
        {
            yield return new WaitForSecondsRealtime(_Time);

            yield return true;
        }
    }
}