using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridMaker : MonoBehaviour
{
	public int widthCount;
	public int heightCount;

	public bool button;
	public bool genTexture;
	public bool assign;
	public bool reverse;
	public bool everything;

	public GameObject gridCell;

	public GameObject father;

	public GridLayoutGroup glg;

	public Texture2D sourceTexture;
	public List<Texture2D> gridTextures = new List<Texture2D>();

	public int cellCount = 0;

	public int cellWidth = 0;

	private string timeToGen = "";

	public TextMeshProUGUI timeToGenTMP;

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

		if (everything)
		{
			everything = false;
			DoEverything();
		}
	}

	public void DoEverything()
	{
		Stopwatch s = new Stopwatch();
		s.Start();

		GenerateGrid();
		SlizeImage(sourceTexture, cellWidth, widthCount, heightCount, (int)glg.spacing.x, glg.padding.right);
		ReverseTextureOrder();
		AssignTextures();

		s.Stop();

		timeToGen = $"Took {s.ElapsedMilliseconds.ToString()} ms to gen";
		print(timeToGen);

		if (Application.isPlaying)
		{
			timeToGenTMP.text = timeToGen;
		}
	}

	public void SetCellAmount(int amount)
	{
		widthCount = amount;
	}

	private void ReverseTextureOrder()
	{
		gridTextures.Reverse();
	}

	private void AssignTextures()
	{
		for (int i = 0; i < gridTextures.Count; i++)
		{
			father.transform.GetChild(i).GetComponent<RawImage>().texture = gridTextures[i];
		}
	}

	public void KillChildren()
	{
		foreach (Transform oldChild in father.transform)
		{
			DestroyImmediate(oldChild.gameObject);
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
			DestroyImmediate(oldChild.gameObject);
		}

		var fatherBox = father.GetComponent<RectTransform>().rect;

		var useableWidth = fatherBox.width;
		useableWidth -= glg.padding.left;
		useableWidth -= glg.padding.right;
		useableWidth -= (widthCount - 1) * glg.spacing.x;

		cellWidth = Mathf.FloorToInt(useableWidth / widthCount);
		heightCount = Mathf.FloorToInt(fatherBox.height / (cellWidth + glg.spacing.x));

		print(heightCount);

		glg.cellSize = new Vector2(cellWidth, cellWidth);

		cellCount = widthCount * heightCount;

		for (int i = 0; i < cellCount; i++)
		{
			Instantiate(gridCell, father.transform);
		}
	}
}
