using Cysharp.Threading.Tasks;
using UnityEngine;

public class BubbleParticle : MonoBehaviour
{
    private void Awake()
    {
        Despawn();
    }

    private async void Despawn()
    {
        var randomTime = Random.Range(1f, 2f);
        await UniTask.WaitForSeconds(randomTime);
        if (!this) return;

        Destroy(gameObject);
    }

    private void Update()
    {
        transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
    }
}
