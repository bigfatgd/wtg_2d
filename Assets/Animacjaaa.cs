using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    public float speed = 1f;         // how fast it moves up and down
    public float height = 0.5f;      // how far it moves up and down

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * height;
        transform.position = startPos + new Vector3(0, newY, 0);
    }
}
