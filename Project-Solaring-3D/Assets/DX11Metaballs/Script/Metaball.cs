using UnityEngine;

[AddComponentMenu("DX11Metaballs/Metaball")]
public class Metaball : MonoBehaviour {

    public float radius = 1.0f;

    public Vector4 pack()
    {
        return new Vector4(transform.position.x, transform.position.y, transform.position.z, radius);
    }

}
