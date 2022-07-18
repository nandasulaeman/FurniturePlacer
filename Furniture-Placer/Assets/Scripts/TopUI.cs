using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TopUI : MonoBehaviour
{
    JsonProduct jsnProduct;
    public string url;
    public GameObject prefabProductOld;
    public RectTransform ParentItemsOld;
    public GameObject detail;
    public GameObject detailNotdestroyed;


    // Start is called before the first frame update
    void Start()
    {
            StartCoroutine(GetProductOld());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetProductOld()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://arfurni.000webhostapp.com/ProductNew");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnProduct = JsonUtility.FromJson<JsonProduct>(request.downloadHandler.text);
                StartCoroutine(GetIconesOld());
            }
        }
    }

    IEnumerator GetIconesOld()
    {
        for (int i = 0; i < jsnProduct.Result.Count; i++)
        {
            WWW w = new WWW("https://arfurni.000webhostapp.com/img/" + jsnProduct.Result[i].picture[1]);
            yield return w;

            if (w.error != null)
            {
                //error
            }
            else
            {
                if (w.isDone)
                {
                    Texture2D tx = w.texture;
                    jsnProduct.Result[i].pic = Sprite.Create(tx, new Rect(0f, 0f, tx.width, tx.height), Vector2.zero, 10f);
                }
            }
        }
        DrawContentOld();
    }

    void DrawContentOld()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabProductOld);
            goItems.transform.SetParent(ParentItemsOld, false);
            goItems.transform.GetChild(0).GetComponent<Image>().sprite = jsnProduct.Result[i].pic;
            goItems.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = "Rp" + jsnProduct.Result[i].price.ToString("#,###,###,###0");

            goItems.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }

    public void ItemClicked(int ItemIndex)
    {
        detail.SetActive(true);
        PlayerPrefs.SetString("IdProduct", jsnProduct.Result[ItemIndex].id_product);
        Debug.Log(jsnProduct.Result[ItemIndex].name);
        StartCoroutine(detailNotdestroyed.GetComponent<ControllerDetail>().GetProductbyID(jsnProduct.Result[ItemIndex].id_product));
    }

    public void goARScene()
    {
        PlayerPrefs.SetString("IdProduct", "");
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}
