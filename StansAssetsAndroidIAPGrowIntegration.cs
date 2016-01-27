/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */

using UnityEngine;
using System.Collections.Generic;

namespace Grow.Integrations
{
	
	public class StansAssetsAndroidIAPGrowIntegration : GameObjectGrowIntegration {
		
		private const string TAG = "GROW Android IAP StansAssetsIntegration";

		public const string DATA_SPLITTER = "|";
		public const int BILLING_RESPONSE_RESULT_OK = 0;
		public static Dictionary<string, GoogleProductTemplate> products = new Dictionary<string, GoogleProductTemplate>();

		
		
		// Integration instance
		
		private static StansAssetsAndroidIAPGrowIntegration instance;
		
		private StansAssetsAndroidIAPGrowIntegration() {}
		
		private static StansAssetsAndroidIAPGrowIntegration Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StansAssetsAndroidIAPGrowIntegration();
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

			public void OnProducttDetailsRecive(string data) {
				if(data.Equals(string.Empty)) {
					return;
				}
				
				string[] storeData;
				storeData = data.Split(DATA_SPLITTER [0]);
				
				
				for(int i = 0; i < storeData.Length; i+=7) {

					GoogleProductTemplate tpl =  new GoogleProductTemplate();
					tpl.SKU 						= storeData[i];
					tpl.LocalizedPrice 		  		= storeData[i + 1];
					tpl.Title 	      				= storeData[i + 2];
					tpl.Description   				= storeData[i + 3];
					tpl.PriceAmountMicros 	      	= System.Convert.ToInt64(storeData[i + 4]);
					tpl.PriceCurrencyCode   		= storeData[i + 5];
					tpl.OriginalJson   				= storeData[i + 6];

					products.Add(tpl.SKU, tpl);

				}
				

			}


			public void OnPurchaseFinishedCallback(string data) {
				Debug.Log(data);
				string[] storeData;
				storeData = data.Split(DATA_SPLITTER [0]);
				
				int resp = System.Convert.ToInt32 (storeData[0]);

				
				if(resp == BILLING_RESPONSE_RESULT_OK) {
					
					string SKU 						= storeData[2];
				

					if(products.ContainsKey(SKU)) {
						GoogleProductTemplate tpl = products[SKU];
						StansAssetsAndroidIAPGrowIntegration.instance.OnMarketPurchaseFinished(SKU, tpl.PriceAmountMicros, tpl.PriceCurrencyCode);
					}else  {
						StansAssetsAndroidIAPGrowIntegration.instance.OnMarketPurchaseFinished(SKU, 0, "USD");
					}

				}  else {
					StansAssetsAndroidIAPGrowIntegration.instance.OnMarketPurchaseFailed();
				}
				

			}

			public void OnBillingSetupFinishedCallback(string data) {

				string[] storeData;
				storeData = data.Split(DATA_SPLITTER [0]);
				
				int resp = System.Convert.ToInt32 (storeData[0]);
				if(resp == 0) {
					StansAssetsAndroidIAPGrowIntegration.instance.OnBillingSupported();
				} else {
					StansAssetsAndroidIAPGrowIntegration.instance.OnBillingNotSupported();
				}
			}






			
		}



		public class GoogleProductTemplate  {
			
			//Editor Only
			public bool IsOpen = true;
			
			public string SKU = string.Empty;
			
			
			private string _OriginalJson = string.Empty;
			
			
			[SerializeField]
			private string _LocalizedPrice = "0.99 $";
			
			[SerializeField]
			private long   _PriceAmountMicros = 990000;
			
			[SerializeField]
			private string _PriceCurrencyCode = "USD";
			
			
			[SerializeField]
			private string _Description = string.Empty;
			
			[SerializeField]
			private string _Title =  "New Product";
			
			[SerializeField]
			private Texture2D _Texture;
			
				
			public string OriginalJson {
				get {
					return _OriginalJson;
				} 
				
				set {
					_OriginalJson = value;
				}
			}
		
			
			public float Price {
				get {
					return _PriceAmountMicros / 1000000f;
				} 
				
				
			}
			
		
			public long PriceAmountMicros  {
				get {
					return _PriceAmountMicros;
				}
				
				set {
					_PriceAmountMicros = value;
				}
			}
			
			
			
		
			
			
			public string PriceCurrencyCode  {
				get {
					return _PriceCurrencyCode;
				}
				
				set {
					_PriceCurrencyCode = value;
				}
			}
			
			public string LocalizedPrice {
				get {
					return _LocalizedPrice;
				}
				
				set {
					_LocalizedPrice = value;
				}
			}
			
			
		
			
			public string Description {
				get {
					return _Description;
				}
				
				set {
					_Description = value;
				}
			}
			

			public string Title {
				get {
					return _Title;
				}
				
				set {
					_Title = value;
				}
			}
			
			public Texture2D Texture {
				get {
					return _Texture;
				}
				
				set {
					_Texture = value;
				}
			}
			
		
		}









		
	}
}