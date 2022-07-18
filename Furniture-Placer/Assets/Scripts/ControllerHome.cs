using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> onClick)
    {
        button.onClick.AddListener(delegate ()
        {
            onClick(param);
        });
    }
}
public class ControllerHome : MonoBehaviour
{
    JsonProduct jsnProduct;
    JsonProductOLD jsnProductOLD;
    public string url;
    public GameObject loadingScreen;
    public GameObject prefabProductNew;
    public GameObject prefabProductOld;
    public RectTransform ParentItemsOld;
    public RectTransform ParentItemsNews;
    public GameObject quitApp;
    public GameObject detail;
    public GameObject detailNotdestroyed;


    // Start is called before the first frame update
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
                detail.SetActive(false);
                PlayerPrefs.SetString("IdProduct", "");
            }
            else
            {
                quitApp.SetActive(true);
            }
        }
    }

    IEnumerator GetProductOld()
    {
        loadingScreen.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get("https://arcommerce.000webhostapp.com/ProductNew");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnProductOLD = JsonUtility.FromJson<JsonProductOLD>(request.downloadHandler.text);
                StartCoroutine(GetProductNew());
                DrawContentOld();
            }
        }
    }

    string imageUrl = "https://arcommerce.000webhostapp.com/img/";
   
    void DrawContentOld()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabProductOld);
            goItems.transform.SetParent(ParentItemsOld, false);
            Davinci.get().load(imageUrl + jsnProductOLD.Result[i].picture[1]).setCached(true).into(goItems.transform.GetChild(0).GetComponent<Image>()).start();
            goItems.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnProductOLD.Result[i].name;
            goItems.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = "Rp"+jsnProductOLD.Result[i].price.ToString("#,###,###,###0");
            goItems.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>().text = "30%";
            goItems.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().text = "<s>Rp" + jsnProductOLD.Result[i].price.ToString("#,###,###,###0");
            goItems.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnProductOLD.Result[i].rating.ToString();
            
            goItems.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }

    IEnumerator GetProductNew()
    {
        loadingScreen.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get("https://arcommerce.000webhostapp.com/ProductNew");
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
            Davinci.get().load(imageUrl + jsnProduct.Result[i].picture[1]).setCached(true).into(goItems.transform.GetChild(0).GetComponent<Image>()).start();
            goItems.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnProduct.Result[i].name;
            goItems.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnProduct.Result[i].sub_category;
            goItems.transform.GetChild(1).GetChild(2).GetComponent<TMPro.TMP_Text>().text = "Rp" + jsnProduct.Result[i].price.ToString("#,###,###,###0");
            goItems.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnProduct.Result[i].rating.ToString();

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
