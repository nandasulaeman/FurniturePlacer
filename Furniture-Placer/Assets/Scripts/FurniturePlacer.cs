using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FurniturePlacer : MonoBehaviour
{
    public Transform placementIndicator;
    public GameObject selectionUI;

    public GameObject parentpanel;
    public GameObject prefabdatapreview;
    public RectTransform parentdata;
    public string url;

    private List<GameObject> furniture = new List<GameObject>();
    private GameObject curSelected;
    private Camera cam;
    JsonProduct jsnData;
    JsonClassDetail jsnDetail;
    public GameObject player;
    public GameObject master;
    public GameObject ButtonAnim1;
    public Animator anim;
    string imageUrl = "https://arcommerce.000webhostapp.com/img/";



    private void Start()
    {
        player = GameObject.FindWithTag("notdestroy");
        master = GameObject.FindWithTag("Master");

        master.SetActive(false);

        string idPro = PlayerPrefs.GetString("IdProduct");
        if (idPro == "")
        {
            StartCoroutine(GetDataPreview());
        }
        else
        {
            StartCoroutine(GetChildData(idPro));
            StartCoroutine(GetDataPreview());
        }

        cam = Camera.main;
        selectionUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
        {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject != null && furniture.Contains(hit.collider.gameObject))
                {
                    if (curSelected != null && hit.collider.gameObject != curSelected)
                    {
                        Select(hit.collider.gameObject);
                    }
                    else if (curSelected == null)
                    {
                        Select(hit.collider.gameObject);
                    }
                }
                else
                {
                    Deselect();
                }
            }

        }

        if (curSelected != null && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
        {
            MoveSelected();
        }

        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     goBack();
        // }
    }
    void MoveSelected()
    {
        Vector3 curPos = cam.ScreenToViewportPoint(Input.touches[0].position);
        Vector3 lastPos = cam.ScreenToViewportPoint(Input.touches[0].position - Input.touches[0].deltaPosition);

        Vector3 touchDir = curPos - lastPos;

        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        curSelected.transform.position += (camRight * touchDir.x + camForward * touchDir.y);
    }
    void Select(GameObject selected)
    {
        if (curSelected != null)
        {
            ToggleSelectionVisul(curSelected, false);
        }
        curSelected = selected;
        ToggleSelectionVisul(curSelected, true);
        openinfo();
        selectionUI.SetActive(true);
        anim = curSelected.GetComponent<Animator>();
        if (anim != null)
        {
            ButtonAnim1.SetActive(true);
        }
        else
        {
            ButtonAnim1.SetActive(false);
        }
    }
    void Deselect()
    {
        if (curSelected != null)
        {
            ToggleSelectionVisul(curSelected, false);
        }
        curSelected = null;
        selectionUI.SetActive(false);
    }
    void ToggleSelectionVisul(GameObject obj, bool toggle)
    {
        obj.transform.Find("Selected").gameObject.SetActive(toggle);
    }
    public void PlaceFurniture(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, placementIndicator.position, Quaternion.identity);
        furniture.Add(obj);
        Select(obj);
    }

    public void ScaleSelected(float rate)
    {
        curSelected.transform.localScale += Vector3.one * rate;
    }

    public void RotateSelected(float rate)
    {
        curSelected.transform.eulerAngles += Vector3.up * rate;
    }

    public void SetColor(Image buttonImage)
    {
        MeshRenderer[] meshRenderers = curSelected.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr.gameObject.name == "Selected")
                continue;
            mr.material.color = buttonImage.color;
        }
    }

    public void DeleteSelected()
    {
        furniture.Remove(curSelected);
        Destroy(curSelected);
        Deselect();
    }

    public void openinfo()
    {
        string idsplit = curSelected.name;
        string[] splitArray = idsplit.Split(char.Parse("-"));
        PlayerPrefs.SetString("IdProduct", splitArray[0]);
        StartCoroutine(GetChildData(splitArray[0]));
    }
    private int nums = 1;

    public void Open1()
    {
        Animator anim = curSelected.GetComponent<Animator>();
        anim.SetInteger("OpenInt", nums);
        nums++;
        if (nums > 4)
        {
            nums = 0;
        }
    }
    public GameObject loadingScreen;

    public string armode;
    IEnumerator GetDataPreview()
    {
        loadingScreen.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Get(url + armode);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            //show message "no internet "
        }
        else
        {
            if (request.isDone)
            {
                jsnData = JsonUtility.FromJson<JsonProduct>(request.downloadHandler.text);
                loadingScreen.SetActive(false);
                DrawUI();
            }
        }
    }

    void DrawUI()
    {
        int Trow = jsnData.Result.Count;
        for (int i = 0; i < Trow; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabdatapreview);
            goItems.transform.SetParent(parentdata, false);
            Davinci.get().load(imageUrl + jsnData.Result[i].picture[0]).setCached(true).into(goItems.transform.GetChild(0).GetComponent<Image>()).start();
            goItems.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
    }
    void ItemClicked(int ItemIndex)
    {
        PlayerPrefs.SetString("IdProduct", jsnData.Result[ItemIndex].id_product);
        StartCoroutine(GetChildData(jsnData.Result[ItemIndex].id_product));
    }
    public GameObject childPanel;
    IEnumerator GetChildData(string id)
    {
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
                loadingScreen.SetActive(false);
                parentpanel.SetActive(false);
                childPanel.SetActive(true);
                RemoveDetail();
                PrefabDestroy();
                DrawChildPic();
                DrawDetail();
            }
        }
    }
    public GameObject prefabchildData;
    public RectTransform parentchildData;

    void DrawChildPic()
    {
        int Trow = jsnDetail.colors.Count;
        for (int i = 0; i < Trow; i++)
        {
            GameObject goItems = (GameObject)Instantiate(prefabchildData);
            goItems.transform.SetParent(parentchildData, false);
            goItems.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = jsnDetail.colors[i];
            goItems.GetComponent<Button>().AddEventListener(i, ChildClicked);
        }
    }
    private GameObject placedPrefab;

    void ChildClicked(int ItemIndex)
    {
        placedPrefab = Resources.Load<GameObject>($"Prefabs/{jsnDetail.id_product}-{jsnDetail.colors[ItemIndex]}");
        PlaceFurniture(placedPrefab);
    }

    public TMPro.TMP_Text judulHeader;
    public TMPro.TMP_Text judul;
    public TMPro.TMP_Text price;
    public TMPro.TMP_Text desc;
    public TMPro.TMP_Text rating;

    void DrawDetail()
    {
        judulHeader.text = jsnDetail.name;
        judul.text = jsnDetail.name;
        rating.text = jsnDetail.rating.ToString();
        price.text = "Rp" + jsnDetail.price.ToString("#,###,###,###0");
        desc.text = jsnDetail.description;
    }

    public int scenesnow;
    public void goBack()
    {
        master.SetActive(true);
        objekDestroy();
        SceneManager.UnloadSceneAsync(scenesnow);
    }

    public void buy()
    {
        string idPro = PlayerPrefs.GetString("IdProduct");
        player.GetComponent<ControllerDetail>().RemoveDetail();
        player.GetComponent<ControllerDetail>().ImageDestroy();
        StartCoroutine(player.GetComponent<ControllerDetail>().GetProductbyID(idPro));
        objekDestroy();
        StartCoroutine(unloadscene());
        // SceneManager.UnloadSceneAsync(1);
    }

    IEnumerator unloadscene()
    {
        loadingScreen.SetActive(true);
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        master.SetActive(true);
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(2f);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        SceneManager.UnloadSceneAsync(1);
    }

    void RemoveDetail()
    {
        judul.text = "";
        price.text = "";
        desc.text = "";
    }

    void PrefabDestroy()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("3Dproduct");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    void objekDestroy()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("prefabs");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
}
