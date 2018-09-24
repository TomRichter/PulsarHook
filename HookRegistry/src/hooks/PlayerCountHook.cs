using System;
using System.Reflection;

namespace Hooks
{
	[RuntimeHook]
	class PlayerCountHook : Hook
	{
		public PlayerCountHook()
		{
			HookRegistry.Register(OnCall);
			isReentry = false;
		}

		public new static string[] GetExpectedMethods()
		{
			return new string[] { "PLServer::SetPlayerAsClassID", "PhotonNetwork::CreateRoom" };
		}

		private void PhotonNetwork_CreateRoom(object[] args, byte maxPlayers)
		{
			string roomName = (string)args[0];
			RoomOptions roomOptions = (RoomOptions)args[1];
			roomOptions.maxPlayers = maxPlayers;
			TypedLobby typedLobby = (TypedLobby)args[2];

			isReentry = true;
			PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);
		}

		private void PLServer_SetPlayerAsClassID(ref object serverObject, object[] args)
		{
			// Unpack arguments
			int playerID = (int)args[0];
			int classID = (int)args[1];

			// TODO: Make calling private methods easier
			MethodInfo initMethod = typeof(PLServer).GetMethod("GetPlayerAtID", BindingFlags.Instance | BindingFlags.NonPublic);
			PLPlayer playerAtID = (PLPlayer)initMethod.Invoke(serverObject, new object[] { playerID });

			if (playerAtID != null)
			{
				playerAtID.SetClassID(classID);

				initMethod = typeof(PLServer).GetMethod("ClassChangeMessage", BindingFlags.Instance | BindingFlags.NonPublic);
				initMethod.Invoke(serverObject, new object[] { playerAtID.GetPlayerName(false), classID });
			}
		}

		protected override object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			if (!IsExpectedMethod(GetExpectedMethods(), typeName, methodName))
			{
				return null;
			}

			if (isReentry)
			{
				// Prevent infinite recursion below, and allow original method to continue as normal.
				return null;
			}

			try
			{
				// Call the original method.
				// Let's capture the method call's return value instead of returning it directly.
				// This allows us to do more things after the original function runs its course.
				object returnValue = null;

				// This looks janky because we're handling 3 distinct classes in the same hook.
				// Most hooks will be simpler due to one hooking method in a single class or common superclass.
				if (typeName == "PhotonNetwork" && methodName == "CreateRoom")
				{
					PhotonNetwork_CreateRoom(args, maxPlayers: 99);
					return true;
				}
				else if (thisObj is PLServer)
				{
					// Completely replace the original function with our own behavior
					PLServer_SetPlayerAsClassID(ref thisObj, args);
					return true;
				}

				// Do stuff after the original method completes.
				// This allows the hook to run again on subsequent calls.
				isReentry = false;

				return returnValue;
			}
			catch (Exception e)
			{
				string message = String.Format(HOOK_FAILED, e);
				HookRegistry.Panic(message);
			}

			// Dummy value.  Returning anything other than null skips the original method.
			return true;
		}
	}
}
