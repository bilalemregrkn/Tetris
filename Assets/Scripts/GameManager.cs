using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public MyBlockController Current { get; set; }


	private const int GridSizeX = 10;
	private const int GridSizeY = 20;

	public bool[,] Grid = new bool[GridSizeX, GridSizeY];

	public float GameSpeed => gameSpeed;
	[SerializeField, Range(.1f, 1f)] private float gameSpeed = 1;

	[SerializeField] private List<MyBlockController> listPrefabs;

	private List<MyBlockController> _listHistory = new List<MyBlockController>();

	#region Test

	public bool IsOpenTest;

	[SerializeField] private SpriteRenderer displayDataPrefabs;
	private SpriteRenderer[,] previewDisplay = new SpriteRenderer[GridSizeX, GridSizeY];

	private void UpdateDisplayPreview()
	{
		if (!IsOpenTest) return;

		for (int i = 0; i < GridSizeX; i++)
		{
			for (int j = 0; j < GridSizeY; j++)
			{
				var active = Grid[i, j];
				var sprite = previewDisplay[i, j];

				sprite.color = active ? Color.green : Color.red;

				var color = sprite.color;
				color.a = .5f;
				sprite.color = color;

			}
		}
	}

	#endregion

	private void Awake()
	{
		Instance = this;

		if (IsOpenTest)
		{
			for (int i = 0; i < GridSizeX; i++)
			{
				for (int j = 0; j < GridSizeY; j++)
				{
					var sprite = Instantiate(displayDataPrefabs, transform);
					sprite.transform.position = new Vector3(i, j, 0);
					previewDisplay[i, j] = sprite;
				}
			}
		}
	}

	private void Start()
	{
		Spawn();
	}

	public bool IsInside(List<Vector2> listCoordinate)
	{
		foreach (var coordinate in listCoordinate)
		{
			int x = Mathf.RoundToInt(coordinate.x);
			int y = Mathf.RoundToInt(coordinate.y);

			if (x < 0 || x >= GridSizeX)
			{
				//Horizontal out
				return false;
			}

			if (y < 0 || y >= GridSizeY)
			{
				//Vertical out
				return false;
			}


			if (Grid[x, y])
			{
				//Hit something
				return false;
			}
		}

		return true;
	}

	public void Spawn()
	{
		var index = Random.Range(0, listPrefabs.Count);
		var blockController = listPrefabs[index];
		var newBlock = Instantiate(blockController);
		Current = newBlock;
		_listHistory.Add(newBlock);

		UpdateDisplayPreview();
	}

	private bool IsFullRow(int index)
	{
		for (int i = 0; i < GridSizeX; i++)
		{
			if (!Grid[i, index])
				return false;
		}

		return true;
	}

	public void UpdateRemoveObjectController()
	{
		for (int i = 0; i < GridSizeY; i++)
		{
			var isFull = IsFullRow(i);
			if (isFull)
			{
				//Remove
				foreach (var myBlock in _listHistory)
				{
					var willDestroy = new List<Transform>(); 
					foreach (var piece in myBlock.ListPiece)
					{
						int y = Mathf.RoundToInt(piece.position.y);
						if (y == i)
						{
							//Add Remove
							willDestroy.Add(piece);
						}else if (y > i)
						{
							//Move Down
							var position = piece.position;
							position.y--;
							piece.position = position;
						}
					}
					
					//Remove
					foreach (var item in willDestroy)
					{
						myBlock.ListPiece.Remove(item);
						Destroy(item.gameObject);
					}
				}

				//ChangeData
				for (int j = 0; j < GridSizeX; j++)
					Grid[j, i] = false;

				for (int j = i+1; j < GridSizeY; j++)
				for (int k = 0; k < GridSizeX; k++)
					Grid[k, j - 1] = Grid[k, j];

				//Call Again
				UpdateRemoveObjectController();
				return;
			}
		}
	}
	
	public void OnGameOver()
	{
		Debug.Log("Game Over");
		Time.timeScale = 0;
	}
}