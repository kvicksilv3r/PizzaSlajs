using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMaker : MonoBehaviour
{
    public int widthCount;
    public int heightCount;

    public bool button;
    public bool genTexture;
    public bool assign;
    public bool reverse;

    public GameObject gridCell;

    public GameObject father;

    public GridLayoutGroup glg;

    public Texture2D sourceTexture;
    public List<Texture2D> gridTextures = new List<Texture2D>();

    public int cellCount = 0;

    public int cellWidth = 0;

    private void OnValidate()
    {
        if (button)
        {
            button = !button;
            GenerateGrid();
        }

        if (genTexture)
        {
            genTexture = false;
            SlizeImage(sourceTexture, cellWidth, widthCount, heightCount, (int)glg.spacing.x, glg.padding.right);
        }

        if (assign)
        {
            assign = false;
            AssignTextures();
        }

        if (reverse)
        {
            reverse = false;
            ReverseTextureOrder();
        }
    }

    private void ReverseTextureOrder()
    {
        gridTextures.Reverse();
    }

    private void AssignTextures()
    {
        for (int i = 0; i < father.transform.childCount; i++)
        {
            father.transform.GetChild(i).GetComponent<RawImage>().texture = gridTextures[i];
        }
    }

    private void SlizeImage(Texture2D srcTexture, int cellSide, int horizontalCells, int verticalCells, int spacing = 0, int padding = 0)
    {
        ClearTextureList();
        CopyPixels(srcTexture, cellSide, horizontalCells, verticalCells, spacing, padding);
    }

    private void CopyPixels(Texture2D srcTexture, int cellSide, int horizontalCells, int verticalCells, int spacing = 0, int padding = 0)
    {
        for (int y = 0; y < verticalCells; y++)
        {
            for (int x = 0; x < horizontalCells; x++)
            {
                int srcX = padding;
                srcX += x * spacing;
                srcX += x * cellSide;

                int srcY = padding;
                srcY += y * spacing;
                srcY += y * cellSide;

                var newTexture = CreateDestinationTexture(srcTexture, cellSide);
                Graphics.CopyTexture(srcTexture, 0, 0, srcX, srcY, cellSide, cellSide, newTexture, 0, 0, 0, 0);
                gridTextures.Add(newTexture);
            }
        }
    }

    private Texture2D CreateDestinationTexture(Texture2D srcTexture, int cellSide)
    {
        Texture2D newTexture = new Texture2D(cellSide, cellSide, srcTexture.format, false);
        return newTexture;
    }

    private void ClearTextureList()
    {
        gridTextures = new List<Texture2D>();
    }

    private void GenerateGrid()
    {
        foreach (Transform oldChild in father.transform)
        {
            Destroy(oldChild.gameObject);
        }

        var fatherBox = father.GetComponent<RectTransform>().rect;

        var useableWidth = fatherBox.width;
        useableWidth -= glg.padding.left;
        useableWidth -= glg.padding.right;
        useableWidth -= (widthCount - 1) * glg.spacing.x;

        cellWidth = Mathf.FloorToInt(useableWidth / widthCount);
        heightCount = Mathf.FloorToInt(fatherBox.height / cellWidth);

        print(heightCount);

        glg.cellSize = new Vector2(cellWidth, cellWidth);

        cellCount = widthCount * heightCount;

        for (int i = 0; i < cellCount; i++)
        {
            Instantiate(gridCell, father.transform);
        }
    }
}
