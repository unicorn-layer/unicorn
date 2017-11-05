﻿
using System;
using Unicorn.IO;
using Unicorn.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unicorn.Entities {
	[DisallowMultipleComponent]
	public class NetworkSceneManager : GlobalEntityModule<NetworkSceneManager> {
		private static SubSet<Connection> _clients;

		private enum ServerMessage : byte { SceneLoaded }
		private enum ClientMessage : byte { LoadScene }

		protected override void Awake() {
			base.Awake();

			DontDestroyOnLoad(gameObject);
			_clients = new SubSet<Connection>(Group);

			SceneManager.sceneLoaded += SceneLoaded;
			UntilDestroy.Add(() => SceneManager.sceneLoaded -= SceneLoaded);

			if (IsServer) {
				Group.Added(UntilDestroy, conn => {
					Send(conn, payload => {
						payload.Write((byte)ClientMessage.LoadScene);
						payload.Write(SceneManager.GetActiveScene().name);
					});
				});
			}
		}

		private void SceneLoaded(Scene scene, LoadSceneMode mode) {
			if (IsServer) {
				Send(Group, payload => {
					payload.Write((byte)ClientMessage.LoadScene);
					payload.Write(scene.name);
				});
			} else {
				Send(payload => {
					payload.Write((byte)ServerMessage.SceneLoaded);
					payload.Write(scene.name);
				});
			}
		}

		protected virtual void LoadScene(string name) {
			SceneManager.LoadSceneAsync(name);
		}

		protected override void Receive(Connection sender, DataReader payload) {
			if (IsServer) {
				switch((ServerMessage)payload.ReadByte()) {
					case ServerMessage.SceneLoaded:
						var sceneName = payload.ReadString();
						if (sceneName == SceneManager.GetActiveScene().name) {
							_clients.Add(sender);
						} else {
							_clients.Remove(sender);
						}
						break;
				}
			} else {
				switch((ClientMessage)payload.ReadByte()) {
					case ClientMessage.LoadScene:
						LoadScene(payload.ReadString());
						break;
				}
			}
		}
	}
}
