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

	public class StansAssetsIOSIAPGrowIntegration : GameObjectGrowIntegration {

		private const string TAG = "GROW IOS IAP StansAssetsIntegration";
		public const char DATA_SPLITTER = '|';

		public static Dictionary<string, IOSProductTemplate> products = new Dictionary<string, IOSProductTemplate>();

		// Integration instance

		private static StansAssetsIOSIAPGrowIntegration instance;

		private StansAssetsIOSIAPGrowIntegration() {}

		private static StansAssetsIOSIAPGrowIntegration Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StansAssetsIOSIAPGrowIntegration();
				}
				return instance;
			}
		}


		// Integration specific implementation

		public override void Initialize() {
			IOSInAppPurchaseManager.Initialize ();
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

		public class IOSInAppPurchaseManager : IntegrationGameObject {

			private static bool initialized = false;

			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing...");
					GetSynchronousCodeGeneratedInstance<IOSInAppPurchaseManager> ();
					initialized = true;
				}
			}



			public void OnStoreKitInitFailed(string message) {
				StansAssetsIOSIAPGrowIntegration.instance.OnBillingNotSupported();

				DelegateMessage (message);

			}
			
			public void onStoreDataReceived(string message) {

				if(message.Equals(string.Empty)) {
					return;
				}
				
				
				string[] storeData = message.Split(DATA_SPLITTER);
				
				for(int i = 0; i < storeData.Length; i+=7) {
					string prodcutId = storeData[i];
					IOSProductTemplate tpl =  new IOSProductTemplate();
					
					
					tpl.DisplayName 	= storeData[i + 1];
					tpl.Description 	= storeData[i + 2];
					tpl.LocalizedPrice 	= storeData[i + 3];
					tpl.Price 			= System.Convert.ToSingle(storeData[i + 4]);
					tpl.CurrencyCode 	= storeData[i + 5];
					tpl.CurrencySymbol 	= storeData[i + 6];
					tpl.IsAvaliable = true;

					products.Add(prodcutId, tpl);
					
				}


				StansAssetsIOSIAPGrowIntegration.instance.OnBillingSupported();
				DelegateMessage (message);
			}




			public void onTransactionFailed(string message) {
				
				string[] data;
				data = message.Split("|" [0]);
				
				string prodcutId = data [0];
				int e = System.Convert.ToInt32(data [2]);
				IOSTransactionErrorCode erroCode = (IOSTransactionErrorCode) e;

				if(erroCode == IOSTransactionErrorCode.SKErrorPaymentCanceled) {
					StansAssetsIOSIAPGrowIntegration.instance.OnMarketPurchaseCancelled(prodcutId);
				} else {
					StansAssetsIOSIAPGrowIntegration.instance.OnMarketPurchaseFailed();
				}

			}


			public void onProductBought(string array) {
				
				string[] data;
				data = array.Split("|" [0]);

				
				string productId = data [0];

				if(products.ContainsKey(productId)) {
					IOSProductTemplate tpl = products[productId];
					StansAssetsIOSIAPGrowIntegration.instance.OnMarketPurchaseFinished(productId, tpl.PriceInMicros, tpl.CurrencyCode);
				}else  {
					StansAssetsIOSIAPGrowIntegration.instance.OnMarketPurchaseFinished(productId, 0, "USD");
				}
			

			//	StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(productId)
				
				
			}


			public void onRestoreTransactionFailed(string array) {
				
				StansAssetsIOSIAPGrowIntegration.instance.OnRestoreTransactionsFinished(false);
			}
			
			public void onRestoreTransactionComplete(string array) {
				StansAssetsIOSIAPGrowIntegration.instance.OnRestoreTransactionsFinished(true);
			}


			public void onVerificationResult(string array) {
				
				string[] data;
				data = array.Split("|" [0]);
				
				int status = System.Convert.ToInt32(data[0]);

				
				if(status != 0) {
					StansAssetsIOSIAPGrowIntegration.instance.OnVerificationFailed();
				}


			}

		}

		public enum IOSTransactionErrorCode  {
			
			SKErrorUnknown = 0,
			SKErrorClientInvalid = 1,               // client is not allowed to issue the request, etc.
			SKErrorPaymentCanceled = 2,            // user canceled the request, etc.
			SKErrorPaymentInvalid = 3,              // purchase identifier was invalid, etc.
			SKErrorPaymentNotAllowed = 4,           // this device is not allowed to make the payment
			SKErrorStoreProductNotAvailable = 5,    // Product is not available in the current storefront
			SKErrorPaymentNoPurchasesToRestore = 6,  // No purchases to restore"
			SKErrorPaymentServiceNotInitialized = 7,  //StoreKit initialization required
			SKErrorNone = 8 //No error occurred
		}

		public class IOSProductTemplate  {
			
			//Editor Only
			public bool IsOpen = true;
			
			
	
			private bool _IsAvaliable = false;
			
	
			private string _Id = string.Empty;
			
		
			private string _DisplayName =  "New Product";
			
	
			private string _Description;
			
	
			private float _Price = 0.99f;
			
		
			private string _LocalizedPrice = string.Empty;
			

			private string _CurrencySymbol = "$";
		
			private string _CurrencyCode = "USD";
		
			private Texture2D _Texture;
	
			
			
			

			
			public string Id {
				get {
					return _Id;
				}
				
				set {
					_Id = value;
				}
			}
			
		
			
			public string DisplayName {
				get {
					return _DisplayName;
				}
				
				set {
					_DisplayName = value;
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
			
		
			public float Price {
				get {
					return _Price;
				} 
				
				set {
					_Price = value;
				}
			}
			
			public int PriceInMicros {
				get {
					return System.Convert.ToInt32(_Price * 1000000f);
				} 
				
			}
			
			public string LocalizedPrice {
				get {
					if(_LocalizedPrice.Equals(string.Empty)) {
						return Price + " " + _CurrencySymbol;
					} else {
						return _LocalizedPrice;
					}
					
				}
				
				set {
					_LocalizedPrice = value;
				}
			}
			
			public string CurrencySymbol {
				get {
					return _CurrencySymbol;
				} 
				
				set {
					_CurrencySymbol = value;
				}
			}
			
			public string CurrencyCode {
				get {
					return _CurrencyCode;
				}
				
				set {
					_CurrencyCode = value;
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
			

			public bool IsAvaliable {
				get {
					return _IsAvaliable;
				}
				
				set {
					_IsAvaliable = value;
				}
			}
		}


	

    }
}
