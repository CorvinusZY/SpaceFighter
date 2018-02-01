using UnityEngine;
using System.Collections;


public class csLaser2 : MonoBehaviour
{

    public float Width = 1.0f; //LineRenderer Width Value
    public float Offset = 1.0f; //LineRenderer MainTexture Offset Value 
    public float MaxLength = Mathf.Infinity; 
    public Color StartColor; 
    public Color EndColor;
    public float AlphaSpeed = 1f;
    public Transform LaserHitEffect; //For Laser Hit Effect.
    public Material _Material; //For LineRenderer Material
    
    private LineRenderer _LineRenderer; //LineRenderer Value
    private float NowLength; // if Raycast Hit Something, Save Length Information Between this transform , RacastHit's hit point.
    private GameObject _Effect;
    float AlphaValue = 1.0f;

    void Start()
    {
        _LineRenderer = GetComponent<LineRenderer>(); //LineRenderer Set

        
        
        _LineRenderer.material = _Material;
        _LineRenderer.SetWidth(Width, Width);
        _LineRenderer.SetColors(StartColor, EndColor);
        _LineRenderer.SetPosition(0, transform.position);

        RaycastHit hit; //Raycast Value Set

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxLength)) //Check the Raycast Hit Object
            NowLength = hit.distance;
        else
            NowLength = MaxLength;

        Vector3 NewPos = this.transform.position + new Vector3(transform.forward.x * (NowLength)
           , transform.forward.y * (NowLength), transform.forward.z * (NowLength)); //Set Next Position Use the NowLength

        _LineRenderer.SetPosition(1, NewPos); //LineRenderer 2 Position Set.
        Transform Obj = Instantiate(LaserHitEffect, transform.position, Quaternion.identity) as Transform; // Make Effect.
        Obj.transform.position = NewPos;
        //Obj.transform.rotation = hit.collider.transform.rotation;
        Obj.transform.parent = this.transform;
    }

    void Update()
    {
        _LineRenderer.material.SetTextureOffset("_MainTex",
            new Vector2(-Time.time * 30f * Offset, 0.0f)); //Because of Movement of Laser, Change x Offset throught Offset Value.

        AlphaValue -= Time.deltaTime * AlphaSpeed; // For disapperaing LineRenderer Texture, Alpha Value decreace.
        _LineRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(StartColor.r, StartColor.g, StartColor.b, AlphaValue)); // color or alpha value set.
    }
}
