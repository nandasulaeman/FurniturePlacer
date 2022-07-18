using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JsonClassByCat
{
    public List<productData> Result;
}

[Serializable]
public class productData
{
    public Sprite pic;
    public string id_product;
    public string name;
    public int price;
    public float rating;
    public string description;
    public List<Pictures> picture;
    public string tokopedia;
    public string bukalapak;
    public string shopee;
    public string lazada;
    public string jdID;
    public string blibli;
    public string name_category;
    public string sub_category;
}

[Serializable]
public class Pictures
{
    public string picture1;
    public string picture2;
    public string picture3;
}

