using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ControllerSearch : MonoBehaviour
{
    JsonClassByCat jsnDataSearch;
    public string urlsearch;

    public GameObject loadingScreen;
    public GameObject SearchBox;
    public GameObject prefabProductNew;
    public GameObject Self;
    public GameObject detail;
    public GameObject detailNotdestroyed;

    public RectTransform ParentItemsNews;
    public TMPro.TMP_InputField inputSearching;
    string imageUrl = "https://arcommerce.000webhostapp.com/img/";

    // Start is called before the first frame update
    void Start()
    {
        string idPro = PlayerPrefs.GetString("IdProduct");
        if (idPro != "")
        {
            StartCoroutine(detail.GetComponent<ControllerDetail>().GetProductbyID(idPro));
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
            }
        }

    }

    public void SendSearch()
    {
        SearchBox.SetActive(true);
        Debug.Log("Name = " + inputSearching.text);
        PrefabDestroyed();
        StartCoroutine(GetItemSearch(inputSearching.text));
    }

    void PrefabDestroyed()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("search");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    IEnumerator GetItemSearch(string searchs)
    {
        loadingScreen.SetActive(true);
        UnityWebRequest request = UnityWebRequest.Get(urlsearch + searchs);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            loadingScreen.SetActive(false);
        }
        else
        {
            if (request.isDone)
            {
                jsnDataSearch = JsonUtility.FromJson<JsonClassByCat>(request.downloadHandler.text);
                DrawUISearch();
                loadingScreen.SetActive(false);
            }
        }
    }
    void DrawUISearch()
    {
        int Trow = jsnDataSearch.Result.Count;
        for (int i = 0; i < Trow; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabProductNew);
            goItems.transform.SetParent(ParentItemsNews, false);
            Davinci.get().load(imageUrl + jsnDataSearch.Result[i].picture[0].picture1).setCached(true).into(goItems.transform.GetChild(0).GetComponent<Image>()).start();
            goItems.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnDataSearch.Result[i].name;
            goItems.transform.GetChild(1).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnDataSearch.Result[i].sub_category;
            goItems.transform.GetChild(1).GetChild(2).GetComponent<TMPro.TMP_Text>().text = "Rp" + jsnDataSearch.Result[i].price.ToString("#,###,###,###0");
            goItems.transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<TMPro.TMP_Text>().text = jsnDataSearch.Result[i].rating.ToString();

            goItems.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }
    public void ItemClicked(int ItemIndex)
    {
        Debug.Log(jsnDataSearch.Result[ItemIndex].name);
        PlayerPrefs.SetString("IdProduct", jsnDataSearch.Result[ItemIndex].id_product);
        Debug.Log(jsnDataSearch.Result[ItemIndex].name);
        StartCoroutine(detailNotdestroyed.GetComponent<ControllerDetail>().GetProductbyID(jsnDataSearch.Result[ItemIndex].id_product));
    }

}
