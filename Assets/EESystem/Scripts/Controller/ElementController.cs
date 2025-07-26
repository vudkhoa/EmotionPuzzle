using UnityEngine;
using CustomUtils;
using System.Collections.Generic;

public class ElementController : SingletonMono<ElementController>
{
    public List<Element> ElementList;

    public void SpawnElement(List<ElementDetail> elementDetails)
    {
        foreach (ElementDetail elementDetail in elementDetails)
        {
            Vector3Int gridPos = new Vector3Int(elementDetail.ElementPos.x, elementDetail.ElementPos.y, 0);
            Vector3 pos = SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos);
            Element eGO = Instantiate(elementDetail.Element, pos, Quaternion.identity);
            eGO.Setup(elementDetail.EmotionType, elementDetail.ElementPos);

            this.ElementList.Add(eGO);
        }
    }

    public void MoveElement(List<Vector2Int> cellsToSlide, Direction direction)
    {
        if (SlideController.Instance.elementId == 0)
        {
            return;
        }

        int count = cellsToSlide.Count;
        for (int i = 0; i < count; i++) 
        {
            foreach (Element e in this.ElementList)
            {
                if (e.CurrentPos == cellsToSlide[i])
                {
                    Vector2Int toGrid = new Vector2Int(0, 0);
                    if (i == 0)
                    {
                        toGrid = cellsToSlide[count - 1];
                    }
                    else
                    {
                        toGrid = cellsToSlide[i - 1];
                    }
                    Vector3 worldPos = SlideController.Instance.elementTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

                    e.MoveTo(toGrid, worldPos);

                    break;
                }
            }
        }
    }
}
