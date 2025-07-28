using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementCoordinateSO", fileName = "ElementCoordinateData")]
public class ElementCoordinateSO : ScriptableObject
{
    public List<ElementCoordinationDetail> ElementCoordinationDetail;

    public Element GetResult(ElementType elementType1, ElementType elementType2)
    {
        foreach (var item in ElementCoordinationDetail)
        {
            if (item.ElementType1 == elementType1 && item.ElementType2 == elementType2)
            {
                return item.ElementResult;
            }
        }

        return null;
    }
}

[Serializable]
public class ElementCoordinationDetail
{
    public ElementType ElementType1;
    public ElementType ElementType2;
    public Element ElementResult;
}
