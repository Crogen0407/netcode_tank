using UnityEngine;

public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule _particleMain;

    private void Awake()
    {
        _particleMain = GetComponent<ParticleSystem>().main; //메인모듈만 뽑아온다.
    }

    private void Update()
    {
        _particleMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
