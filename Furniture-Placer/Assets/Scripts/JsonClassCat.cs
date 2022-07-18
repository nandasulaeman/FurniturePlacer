using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Result
{
    public Sprite pic;
    public string id_category;
    public string name_category;
    public string sub_category;
}

[Serializable]
public class JsonClassCat
{
    public List<Result> Result;
}