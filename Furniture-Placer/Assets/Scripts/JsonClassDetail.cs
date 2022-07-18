using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class JsonClassDetail
{
    public Sprite pic;
    public string id_product;
    public string name;
    public int price;
    public string description;
    public List<PictureDetail> picture;
    public List<string> colors;
    public float rating;
    public string tokopedia;
    public string bukalapak;
    public string shopee;
    public string lazada;
    public string jdID;
    public string blibli;
    public string id_category;
    public string name_category;
    public string sub_category;
}

[Serializable]
public class PictureDetail
{
    public string picture;
}

// [Serializable]
// public class Colors
// {
//     public string color;
// }