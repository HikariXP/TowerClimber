using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Addressables.CheckForCatalogUpdates();
        var aacc = Addressables.LoadAssetAsync<Texture>("123");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
