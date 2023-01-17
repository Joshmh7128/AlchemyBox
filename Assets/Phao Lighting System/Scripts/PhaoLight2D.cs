using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class PhaoLight2D : MonoBehaviour
{
    // control our sprite renderer
    SpriteRenderer lightRenderer;
    [HideInInspector] [SerializeField] List<Sprite> lightTypes = new List<Sprite>(); // all the types of lights
    enum PhaoLightTypes
    {
        Point, SpotHard, SpotSoft, ParabolicRect, ParabolicCircle, SoftPoint, Custom
    }
    [SerializeField] PhaoLightTypes lightType = new PhaoLightTypes();
    // light controls
    [Range(0f, 1f)]
    [SerializeField] float intensity = 1;
    [Range(0f, 4f)]
    [SerializeField] float range = 5;
    [Range(0f, 1f)]
    [SerializeField] float falloff = 1;
    Color internalColor; float hue, sat, val;
    [SerializeField] bool lockScale = false;

    void Reset()
    {
        intensity = 1;
        range = 5;
        falloff = 1;
    }

    void OnEnable()
    {
        // get our sprite renderer
        lightRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // clamp values
        intensity = Mathf.Clamp01(intensity);
        range = Mathf.Clamp(range, 0.01f, 64);
        falloff = Mathf.Clamp01(falloff);
        // get color values
        Color.RGBToHSV(lightRenderer.color, out hue, out sat, out val);
        // apply values to internal color
        internalColor = Color.HSVToRGB(hue, sat, falloff);
        internalColor.a = intensity;
        // set those values to the renderers color
        lightRenderer.color = internalColor;
        // set our range
        if (lockScale)
        transform.localScale = new Vector3(range, range, range);
        // upate the sprite
        try { lightRenderer.sprite = lightTypes[(int)lightType]; } catch { }
    }
}

