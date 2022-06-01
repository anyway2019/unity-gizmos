using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScene : MonoBehaviour
{
    private Dictionary<string,Sprite> _textureCache = new Dictionary<string, Sprite>();

    private void Start()
    {
        Debug.Log("BaseScene Start");
    }

    protected void OnDestroy()
    {
        this.Dispose();
    }

    public void ShowImage(Image image, string url)
    {
        var cached = GetCache(url);
        if (cached != null)
        {
            image.sprite = cached;
            return;
        }

        Davinci.get()
            .load(url)
            .into(image)
            .start();
        image.sprite.texture.name = url;
        AddCache(url, image.sprite);
    }

    /// <summary>
    /// release resource like texture, audio, etc. example:image = null; then call Resources.UnloadUnusedAssets();
    /// </summary>
    protected virtual void Dispose()
    {
        ClearCache();
        _textureCache = null;
        Resources.UnloadUnusedAssets();
    }

    private Sprite GetCache(string url)
    {
        return _textureCache.TryGetValue(url,out var sprite) ? sprite : null;
    }

    private void AddCache(string key, Sprite texture)
    {
        _textureCache[key] = texture;
    }
    
    private void ClearCache()
    {
        _textureCache.Clear();
    }
}
