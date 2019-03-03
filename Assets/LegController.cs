using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{

    public Transform foot;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        // vector from leg to foot
        var dis = foot.transform.position - transform.position;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dis.y, dis.x) * Mathf.Rad2Deg);

        transform.localScale = new Vector3(dis.magnitude / spriteRenderer.bounds.size.x, transform.localScale.y, transform.localScale.z);

    }
}
