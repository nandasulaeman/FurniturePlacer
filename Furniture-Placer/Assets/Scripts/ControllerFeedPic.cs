using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControllerFeedPic : MonoBehaviour
{
    // Start is called before the first frame update
    JsonProduct jsnProduct;
    public string url;
    public GameObject loadingScreen;
    public GameObject prefabProductNew;
    public RectTransform ParentItemsNews;
    public GameObject quitApp;
    public GameObject detail;
    public GameObject detailNotdestroyed;
    string imageUrl = "https://arcommerce.000webhostapp.com/img/";


    void Start()
    {
        string idPro = PlayerPrefs.GetString("IdProduct");
        if (idPro == "")
        {
            StartCoroutine(GetProductNew());

        }
        else
        {
            StartCoroutine(detail.GetComponent<ControllerDetail>().GetProductbyID(idPro));
            StartCoroutine(GetProductNew());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (detail.activeInHierarchy == true)
            {
                PlayerPrefs.SetString("IdProduct", "");
                detail.SetActive(false);
            }
            else
            {
                quitApp.SetActive(true);
            }
        }
    }

    IEnumerator GetProductNew()
    {
        loadingScreen.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get(url);
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
                DrawContentNew();
                loadingScreen.SetActive(false);
            }
        }
    }
    void DrawContentNew()
    {
        for (int i = 0; i < jsnProduct.Result.Count; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabProductNew);
            goItems.transform.SetParent(ParentItemsNews, false);
            Davinci.get().load(imageUrl + jsnProduct.Result[i].picture[1]).setCached(true).into(goItems.GetComponent<Image>()).start(); 
            goItems.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }

    public void ItemClicked(int ItemIndex)
    {
        Debug.Log(jsnProduct.Result[ItemIndex].name);
        PlayerPrefs.SetString("IdProduct", jsnProduct.Result[ItemIndex].id_product);
        Debug.Log(jsnProduct.Result[ItemIndex].name);
        StartCoroutine(detailNotdestroyed.GetComponent<ControllerDetail>().GetProductbyID(jsnProduct.Result[ItemIndex].id_product));
    }

}
