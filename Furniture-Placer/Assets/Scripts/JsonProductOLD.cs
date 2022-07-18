using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JsonProductOLD
{
    public List<productListOLD> Result;
    public List<jmlProductOLD> Jumlah;
}

[Serializable]
public class productListOLD
{
    public Sprite pic;
    public string id_product;
    public string name;
    public float rating;
    public int price;
    public string description;
    public List<string> picture;
    public List<string> colors;
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
public class jmlProductOLD
{
    public int JmlProduct;
}