/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */

namespace Grow.Integrations
{

	public class StansAssetsGrowIntegration : GrowIntegration {

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
			// Add to all listeners here
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

		// Delegate methods implementation and calls to GrowIntegration methods here

    }
}
