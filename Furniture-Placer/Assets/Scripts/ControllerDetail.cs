using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ControllerDetail : MonoBehaviour
{
    JsonClassDetail jsnDetail;
    public GameObject prefabDetailPic;
    public RectTransform ParentDetailPic;
    public TMPro.TMP_Text judul;
    public TMPro.TMP_Text category;
    public TMPro.TMP_Text price;
    public TMPro.TMP_Text desc;
    public TMPro.TMP_Text rating;
    public GameObject detail;
    public GameObject tokopedia;
    public GameObject shopee;
    public GameObject bukalapak;
    public GameObject lazada;
    public GameObject jdID;
    public GameObject blibli;
    public GameObject productStore;
    public GameObject quitApp;
    public GameObject loadingScreen;
    string imageUrl = "https://arcommerce.000webhostapp.com/img/";

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(GetProductbyID("pr00001"));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (productStore.activeInHierarchy == true)
            {
                RemoveDetail();
                ImageDestroy();
                productStore.SetActive(false);
            }
        }
    }

    public IEnumerator GetProductbyID(string id)
    {
        RemoveDetail();
        ImageDestroy();
        loadingScreen.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get("https://arcommerce.000webhostapp.com/ApiProduct/" + id);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnDetail = JsonUtility.FromJson<JsonClassDetail>(request.downloadHandler.text);
                RemoveDetail();
                ImageDestroy();
                detail.SetActive(true);
                DrawDetailPic();
            }

            DrawDetail();
            if (jsnDetail.bukalapak == "" || jsnDetail.bukalapak == null)
            {
                bukalapak.SetActive(false);
            }
            if (jsnDetail.tokopedia == "" || jsnDetail.tokopedia == null)
            {
                tokopedia.SetActive(false);
            }
            if (jsnDetail.shopee == "" || jsnDetail.shopee == null)
            {
                shopee.SetActive(false);
            }
            if (jsnDetail.lazada == "" || jsnDetail.lazada == null)
            {
                lazada.SetActive(false);
            }
            if (jsnDetail.jdID == "" || jsnDetail.jdID == null)
            {
                jdID.SetActive(false);
            }
            if (jsnDetail.blibli == "" || jsnDetail.blibli == null)
            {
                blibli.SetActive(false);
            }

            shopee.GetComponent<Button>().AddEventListener(jsnDetail.shopee, linkShopee);
            tokopedia.GetComponent<Button>().AddEventListener(jsnDetail.tokopedia, linkTokopedia);
            bukalapak.GetComponent<Button>().AddEventListener(jsnDetail.bukalapak, linkBukalapak);
            lazada.GetComponent<Button>().AddEventListener(jsnDetail.lazada, linklazada);
            jdID.GetComponent<Button>().AddEventListener(jsnDetail.jdID, linkjdID);
            blibli.GetComponent<Button>().AddEventListener(jsnDetail.blibli, linkblibli);
            loadingScreen.SetActive(false);
        }
    }
    public void RemoveDetail()
    {
        judul.text = "";
        category.text = "";
        price.text = "";
        desc.text = "";
        rating.text = "";
        bukalapak.SetActive(true);
        shopee.SetActive(true);
        tokopedia.SetActive(true);
        lazada.SetActive(true);
        jdID.SetActive(true);
        blibli.SetActive(true);
    }

    public void ImageDestroy()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Imgs");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    void DrawDetailPic()
    {
        for (int i = 0; i < jsnDetail.picture.Count; i++)
        {
            Debug.Log(jsnDetail.picture.Count);
            GameObject goCat = (GameObject)Instantiate(prefabDetailPic);
            goCat.transform.SetParent(ParentDetailPic, false);
            Davinci.get().load(imageUrl + jsnDetail.picture[i].picture).setCached(true).into(goCat.GetComponent<Image>()).start();
        }

    }
    void DrawDetail()
    {
        judul.text = jsnDetail.name;
        category.text = jsnDetail.name_category + " > " + jsnDetail.sub_category;
        price.text = "Rp" + jsnDetail.price.ToString("#,###,###,###0");
        desc.text = jsnDetail.description;
        rating.text = jsnDetail.rating.ToString();
    }

    public void linkBukalapak(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }
    public void linkShopee(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }
    public void linkTokopedia(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }
    public void linklazada(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }
    public void linkjdID(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }
    public void linkblibli(string link)
    {
        Application.OpenURL("http://www.bit.ly/" + link);
    }

    public void openStore()
    {
        productStore.SetActive(true);
    }

    public void hideStore()
    {
        productStore.SetActive(false);
    }
    public void goToAR()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

}
