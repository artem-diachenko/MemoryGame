using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes;
using UnityEngine;

internal class ImageSource
{
    // TODO: There can be provided some cache for already loaded images.

    private readonly List<Texture2D> _images = new List<Texture2D>();

    public bool IsLoaded { get; private set; }

    public List<Texture2D> Images
    {
        get { return _images; }
    }

    public IEnumerator LoadImages(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("Required images count must be positive ");
            yield break;
        }

        IsLoaded = false;

        var www = new WWW(string.Format("{0}/{1}", DataPathes.WebUrlString, DataPathes.ImgSourceFileName));
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("Error: " + www.error);
            yield break;
        }

        Images.Clear();

        var links = www.text.Split(new[] { "\r", "\n", " " }, StringSplitOptions.RemoveEmptyEntries).Take(count).ToList();
        yield return LoadImages(links);

        IsLoaded = true;

        Debug.LogWarning("Loaded images count: " + Images.Count);
    }

    private IEnumerator LoadImages(IList<string> links)
    {
        foreach (var link in links)
        {
            var www = new WWW(string.Format("{0}/{1}", DataPathes.WebUrlString, link));
            yield return www;

            if (www.error == null)
            {
                Images.Add(www.texture);
            }
        }
    }
}
