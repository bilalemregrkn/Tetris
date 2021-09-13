using System.Collections.Generic;
using UnityEngine;

public class MyPlayerInput : MonoBehaviour
{
	public bool IsPressLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
	public bool IsPressRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
	public bool IsPressUp => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

	private void Update()
	{
		if (IsPressLeft || IsPressRight)
		{
			var value = IsPressLeft ? -1 : 1;
			var isMovable = GameManager.Instance.IsInside(GetPreviewHorizontalPosition(value));
			if (isMovable)
				MoveHorizontal(value);
		}
		else if (IsPressUp)
		{
			//Rotate
			var isRotatable = GameManager.Instance.IsInside(GetPreviewPosition());
			if(isRotatable)
				Rotate();
		}
	}

	private List<Vector2> GetPreviewPosition()
	{
		var result = new List<Vector2>();
		var listPiece = GameManager.Instance.Current.ListPiece;
		var pivot = GameManager.Instance.Current.transform.position;
		foreach (var piece in listPiece)
		{
			var position = piece.position;

			position -= pivot;
			position = new Vector3(position.y, -position.x, 0);
			position += pivot;
			
			result.Add(position);
		}
		return result;
	}

	private List<Vector2> GetPreviewHorizontalPosition(int value)
	{
		var result = new List<Vector2>();
		var listPiece = GameManager.Instance.Current.ListPiece;
		foreach (var piece in listPiece)
		{
			var position = piece.position;
			position.x += value;
			result.Add(position);
		}

		return result;
	}

	private void MoveHorizontal(int value)
	{
		var current = GameManager.Instance.Current.transform;
		var position = current.position;
		position.x += value;
		current.position = position;
	}

	private void Rotate()
	{
		var current = GameManager.Instance.Current.transform;
		var angles = current.eulerAngles;
		angles.z += -90;
		current.eulerAngles = angles;
	}
}