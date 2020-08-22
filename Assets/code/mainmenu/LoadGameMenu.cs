﻿using System;
using System.Collections.Generic;
using data.game;
using ui.elements;
using UnityEngine;

namespace mainmenu {
public class LoadGameMenu : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private GameStateSo gameState;
	[SerializeField] private ListedGameSaveElement listElementPrefab;
	[SerializeField] private Transform listParent;
#pragma warning restore 0649

	private readonly List<ListedGameSaveElement> listElementPool = new List<ListedGameSaveElement>();

	private void OnEnable() {
		var saveFiles = gameState.ListSaves();

		if (listElementPool.Count > saveFiles.Count) {
			for (var i = saveFiles.Count; i < listElementPool.Count; i++) {
				listElementPool[i].Button.onClick.RemoveAllListeners();
				listElementPool[i].gameObject.SetActive(false);
			}
		}
		else if (listElementPool.Count < saveFiles.Count) {
			for (var i = listElementPool.Count; i < saveFiles.Count; i++) {
				listElementPool.Add(Instantiate(listElementPrefab, listParent));
			}
		}

		for (var i = 0; i < saveFiles.Count; i++) {
			var saveFile = saveFiles[i];
			listElementPool[i].NameDisplay.text = saveFile.name;
			listElementPool[i].DateDisplay.text = $"{saveFile.timestamp:MMMM dd, yyyy 'at' HH:mm:ss}";
			listElementPool[i].Button.onClick.AddListener(
				async () => {
					await gameState.LoadGame(saveFile.location);
					gameState.NextScene();
				}
			);
		}
	}
}
}
