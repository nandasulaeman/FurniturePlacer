using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControllerFeed : MonoBehaviour
{
    JsonProduct jsnProduct;
    JsonClassCat jsnCat;
    JsonClassByCat jsnClassByCat;
    public string url;
    public string urlCategory;
    public GameObject loadingScreen;
    public GameObject prefabCategory;
    public RectTransform ParentCategory;
    public GameObject prefabItemsbyCat;
    public RectTransform ParentItemsbyCat;
    public GameObject quitApp;
    public GameObject detail;
    public GameObject detailNotdestroyed;
    public TMPro.TMP_Text title;
    string imageUrl = "https://arcommerce.000webhostapp.com/img/";

    // Start is called before the first frame update
    void Start()
    {
        string idPro = PlayerPrefs.GetString("IdProduct");
        string subcat = PlayerPrefs.GetString("subCategory");
        if (idPro == "")
        {
            StartCoroutine(GetCategory());
            if (subcat == "")
            {
                StartCoroutine(GetProductbyCat("Sofa"));
                getTitle();
            }
            else
            {
                StartCoroutine(GetProductbyCat(subcat));
                getTitle();
            }

        }
        else
        {
            StartCoroutine(detail.GetComponent<ControllerDetail>().GetProductbyID(idPro));
            StartCoroutine(GetCategory());
            if (subcat == "")
            {
                StartCoroutine(GetProductbyCat("Sofa"));
                getTitle();
            }
            else
            {
                StartCoroutine(GetProductbyCat(subcat));
                getTitle();
            }
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

    public void getTitle()
    {
        string ttl = PlayerPrefs.GetString("subCategory");
        if (ttl == "")
        {
            title.text = "Sofa";
        }
        else
        {
            title.text = ttl;
        }
    }

    IEnumerator GetCategory()
    {
        loadingScreen.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get(urlCategory);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnCat = JsonUtility.FromJson<JsonClassCat>(request.downloadHandler.text);
                DrawCat();
            }
        }
    }

    void DrawCat()
    {
        int CatLength = jsnCat.Result.Count;
        for (int i = 0; i < CatLength; i++)
        {
            GameObject goCat1 = (GameObject)Instantiate(prefabCategory);
            goCat1.transform.SetParent(ParentCategory, false);
            goCat1.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnCat.Result[i].sub_category;
            goCat1.GetComponent<Button>().AddEventListener(i, CatClicked);
        }
    }
    public void ItemClicked(int ItemIndex)
    {
        Debug.Log(jsnClassByCat.Result[ItemIndex].name);
        PlayerPrefs.SetString("IdProduct", jsnClassByCat.Result[ItemIndex].id_product);
        Debug.Log(jsnClassByCat.Result[ItemIndex].name);
        StartCoroutine(detailNotdestroyed.GetComponent<ControllerDetail>().GetProductbyID(jsnClassByCat.Result[ItemIndex].id_product));
    }

    void CatClicked(int ItemIndex)
    {
        Debug.Log("Name = " + jsnCat.Result[ItemIndex].sub_category);
        Debug.Log("ID = " + jsnCat.Result[ItemIndex].id_category);
        PlayerPrefs.SetString("subCategory", jsnCat.Result[ItemIndex].sub_category);
        title.text = jsnCat.Result[ItemIndex].sub_category;

        StartCoroutine(GetProductbyCat(jsnCat.Result[ItemIndex].sub_category));
    }

    void PrefabDestroyed()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ItemCategory");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    IEnumerator GetProductbyCat(string catt)
    {
        PrefabDestroyed();
        UnityWebRequest request = UnityWebRequest.Get(urlCategory + "/" + catt);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnClassByCat = JsonUtility.FromJson<JsonClassByCat>(request.downloadHandler.text);
                DrawUIbyCat();
                loadingScreen.SetActive(false);
            }
        }
    }

    void DrawUIbyCat()
    {
        int Trow = jsnClassByCat.Result.Count;
        for (int j = 0; j < Trow; j++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabItemsbyCat);
            goItems.transform.SetParent(ParentItemsbyCat, false);
            Davinci.get().load(imageUrl + jsnClassByCat.Result[j].picture[0].picture1).setCached(true).into(goItems.transform.GetChild(0).GetComponent<Image>()).start();
            goItems.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnClassByCat.Result[j].name;
            goItems.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnClassByCat.Result[j].sub_category;
            goItems.transform.GetChild(1).GetChild(2).GetComponent<TMPro.TMP_Text>().text = "Rp" + jsnClassByCat.Result[j].price.ToString("#,###,###,###0");
            goItems.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnClassByCat.Result[j].rating.ToString();

            goItems.GetComponent<Button>().AddEventListener(j, ItemClicked);
        }
    }
}
