using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour {

    [SerializeField] Texture2D mColorSwapTex;
    [SerializeField] Color[] mSpriteColors;

    [SerializeField] SpriteRenderer mSpriteRenderer;
    [SerializeField] Color previousColor;    

    public const float cHitEffectTime = 0.2f;

    private void Start() {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        previousColor = GetComponent<SpriteRenderer>().color;
        InitColorSwapTex();
    }

    public void InitColorSwapTex() {
        Texture2D colorSwapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
        colorSwapTex.filterMode = FilterMode.Point;
    
        for (int i = 0; i < colorSwapTex.width; i++)
            colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
    
        colorSwapTex.Apply();
    
        mSpriteRenderer.material.SetTexture("_SwapTex", colorSwapTex);
    
        mSpriteColors = new Color[colorSwapTex.width];
        mColorSwapTex = colorSwapTex;
    }

    public void SwapAllSpritesColorsTemporarily(Color color) {
        GetComponent<SpriteRenderer>().color = Color.white;
        for (int i = 0; i < mColorSwapTex.width; ++i)
            mColorSwapTex.SetPixel(i, 0, color);
        mColorSwapTex.Apply();
    }

    public void ResetAllSpritesColors() {
        GetComponent<SpriteRenderer>().color = previousColor;
        for (int i = 0; i < mColorSwapTex.width; ++i)
            mColorSwapTex.SetPixel(i, 0, mSpriteColors[i]);
        mColorSwapTex.Apply();
    }

    public void StartHitEffect() {
        SwapAllSpritesColorsTemporarily(Color.red);
    }

    public IEnumerator HurtEffect() {
        StartHitEffect();
        yield return new WaitForSeconds(cHitEffectTime);
        ResetAllSpritesColors();
    }

}
