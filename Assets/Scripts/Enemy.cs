using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public Transform target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (collision.gameObject.tag == "Player")
        {
            source.PlayOneShot(clip);
            sprite.enabled = false;
            Destroy(gameObject, 1.2f);
        }
        
    }
}
