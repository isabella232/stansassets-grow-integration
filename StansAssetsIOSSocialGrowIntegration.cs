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
	
	public class StansAssetsIOSSocialGrowIntegration : GameObjectGrowIntegration {
		
		private const string TAG = "GROW IOS Social StansAssetsIntegration";
		
		
		// Integration instance
		
		private static StansAssetsIOSSocialGrowIntegration instance;
		
		private StansAssetsIOSSocialGrowIntegration() {}
		
		private static StansAssetsIOSSocialGrowIntegration Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StansAssetsIOSSocialGrowIntegration();
				}
				return instance;
			}
		}
		
		
		// Integration specific implementation
		
		public override void Initialize() {
			IOSSocialManager.Initialize ();
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
		
		public class IOSSocialManager : IntegrationGameObject {
			
			private static bool initialized = false;
			
			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing...");
					GetSynchronousCodeGeneratedInstance<IOSSocialManager> ();
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



			private void OnTwitterPostFailed() {

				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFailed(SocialProvider.TWITTER, SocialActionType.UPDATE_STORY);
			}
			
			private void OnTwitterPostSuccess() {
				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFinished(SocialProvider.TWITTER, SocialActionType.UPDATE_STORY);
			}




			private void OnFacebookPostFailed() {
				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFailed(SocialProvider.FACEBOOK, SocialActionType.UPDATE_STORY);
			}
			
			private void OnFacebookPostSuccess() {
				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFinished(SocialProvider.FACEBOOK, SocialActionType.UPDATE_STORY);
			}
			
		


			
			
			private void OnInstaPostSuccess() {
				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFinished(SocialProvider.INSTAGRAM, SocialActionType.UPDATE_STORY);
			}
			
			
			private void OnInstaPostFailed(string data) {
				
				StansAssetsIOSSocialGrowIntegration.instance.OnSocialActionFailed(SocialProvider.INSTAGRAM, SocialActionType.UPDATE_STORY);
				
			}
			
		}
		
	}
}