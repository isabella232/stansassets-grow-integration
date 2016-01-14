/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */

using UnityEngine;

namespace Grow.Integrations
{

	public class StansAssetsGrowIntegration : GameObjectGrowIntegration {

		private const string TAG = "GROW StansAssetsIntegration";


		// Integration instance

		private static StansAssetsGrowIntegration instance;

		private StansAssetsGrowIntegration() {}

		private static StansAssetsGrowIntegration Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StansAssetsGrowIntegration();
				}
				return instance;
			}
		}


		// Integration specific implementation

		public override void Initialize() {
			YOUR_GAME_OBJECT_NAME.Initialize ();
		}

		public override string GetIntegrationName() {
			return "stansassets";
		}

		public override string GetIntegrationDisplayName() {
			return "Stan's Assets";
		}

		public override string GetIntegrationVersion() {
			return "1.0.0";
		}

		public override string[] GetDependencies() {
			return new string[]{ };
		}

		public class YOUR_GAME_OBJECT_NAME : IntegrationGameObject {

			private static bool initialized = false;

			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing...");
					GetSynchronousCodeGeneratedInstance<YOUR_GAME_OBJECT_NAME> ();
					initialized = true;
				}
			}

			// All messages of your game objects go here

			public void MessageExample(string message) {
				// Code to parse message (if it's a json you can use:
				// JSONObject messageJson = new JSONObject(message);
				// JSONObject is included in the GrowIntegration.dll

				// Send the event to GrowIntegration. Example:
				// StansAssetsGrowIntegration.Instance.OnLoginFinished (provider, user_id);

				// Call DelegateMessage to continue the message if needed
				DelegateMessage (message);
			}

		}

    }
}
