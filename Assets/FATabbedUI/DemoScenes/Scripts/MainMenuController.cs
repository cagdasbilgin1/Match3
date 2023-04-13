using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	public FAScrollSnapBehaviour scrollSnapBehaviour;
	int currentTabState;

	private void Start()
	{
		currentTabState = scrollSnapBehaviour.startingPage;
		scrollSnapBehaviour.onScrollToPage?.AddListener(OnScrollToPage);
	}
	private void OnDisable()
	{
		scrollSnapBehaviour.onScrollToPage?.RemoveListener(OnScrollToPage);
	}
	void OnScrollToPage(int page)
	{
		currentTabState = page;
	}
	public void ChangePage(int to)
	{
		if (to == currentTabState)
			return;
		currentTabState = to;
		scrollSnapBehaviour.LerpToPage(to);
	}
}
