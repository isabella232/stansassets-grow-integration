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
	
	public class StansAssetsGrowIntegration : GameObjectGrowIntegration {
		
		private const string TAG = "GROW StansAssetsIntegration";

		public const string DATA_SPLITTER = "|";
		public const int BILLING_RESPONSE_RESULT_OK = 0;
		public static Dictionary<string, GoogleProductTemplate> androidProducts = new Dictionary<string, GoogleProductTemplate>();
		public static Dictionary<string, IOSProductTemplate> iosProducts = new Dictionary<string, IOSProductTemplate>();
		
		
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
			AndroidInAppPurchaseManager.Initialize ();
			IOSInAppPurchaseManager.Initialize ();
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


		public class AndroidInAppPurchaseManager : IntegrationGameObject {
			
			private static bool initialized = false;
			
			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing AndroidInAppPurchaseManager...");
					GetSynchronousCodeGeneratedInstance<AndroidInAppPurchaseManager> ();
					initialized = true;
				}
			}

			public void OnQueryInventoryFinishedCallBack(string data) {
				DelegateMessage (data);
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

					androidProducts.Add(tpl.SKU, tpl);

				}

				DelegateMessage (data);
			}


			public void OnPurchaseFinishedCallback(string data) {
				
				string[] storeData;
				storeData = data.Split(DATA_SPLITTER [0]);
				
				int resp = System.Convert.ToInt32 (storeData[0]);

				
				if(resp == BILLING_RESPONSE_RESULT_OK) {
					
					string SKU 						= storeData[2];
				

					if(androidProducts.ContainsKey(SKU)) {
						GoogleProductTemplate tpl = androidProducts[SKU];
						StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(SKU, tpl.PriceAmountMicros, tpl.PriceCurrencyCode);
					}else  {
						StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(SKU, 0, "USD");
					}

				}  else {
					StansAssetsGrowIntegration.instance.OnMarketPurchaseFailed();
				}
				
				DelegateMessage (data);
			}

			public void OnBillingSetupFinishedCallback(string data) {

				string[] storeData;
				storeData = data.Split(DATA_SPLITTER [0]);
				
				int resp = System.Convert.ToInt32 (storeData[0]);
				if(resp == 0) {
					StansAssetsGrowIntegration.instance.OnBillingSupported();
				} else {
					StansAssetsGrowIntegration.instance.OnBillingNotSupported();
				}

				DelegateMessage(data);
			}
				
		}

		public class IOSInAppPurchaseManager : IntegrationGameObject {

			private static bool initialized = false;

			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing IOSInAppPurchaseManager...");
					GetSynchronousCodeGeneratedInstance<IOSInAppPurchaseManager> ();
					initialized = true;
				}
			}



			public void OnStoreKitInitFailed(string message) {
				StansAssetsGrowIntegration.instance.OnBillingNotSupported();

				DelegateMessage (message);

			}

			public void onStoreDataReceived(string message) {

				if(message.Equals(string.Empty)) {
					return;
				}


				string[] storeData = message.Split(DATA_SPLITTER[0]);

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

					iosProducts.Add(prodcutId, tpl);

				}


				StansAssetsGrowIntegration.instance.OnBillingSupported();
				DelegateMessage (message);
			}




			public void onTransactionFailed(string message) {

				string[] data;
				data = message.Split("|" [0]);

				string prodcutId = data [0];
				int e = System.Convert.ToInt32(data [2]);
				IOSTransactionErrorCode erroCode = (IOSTransactionErrorCode) e;

				if(erroCode == IOSTransactionErrorCode.SKErrorPaymentCanceled) {
					StansAssetsGrowIntegration.instance.OnMarketPurchaseCancelled(prodcutId);
				} else {
					StansAssetsGrowIntegration.instance.OnMarketPurchaseFailed();
				}

				DelegateMessage(message);
			}


			public void onProductBought(string array) {

				string[] data;
				data = array.Split("|" [0]);


				string productId = data [0];

				if(iosProducts.ContainsKey(productId)) {
					IOSProductTemplate tpl = iosProducts[productId];
					StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(productId, tpl.PriceInMicros, tpl.CurrencyCode);
				}else  {
					StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(productId, 0, "USD");
				}


				//	StansAssetsGrowIntegration.instance.OnMarketPurchaseFinished(productId)
				DelegateMessage(array);
			}


			public void onRestoreTransactionFailed(string array) {

				StansAssetsGrowIntegration.instance.OnRestoreTransactionsFinished(false);
				DelegateMessage(array);
			}

			public void onRestoreTransactionComplete(string array) {
				StansAssetsGrowIntegration.instance.OnRestoreTransactionsFinished(true);
				DelegateMessage(array);
			}


			public void onVerificationResult(string array) {

				string[] data;
				data = array.Split("|" [0]);

				int status = System.Convert.ToInt32(data[0]);


				if(status != 0) {
					StansAssetsGrowIntegration.instance.OnVerificationFailed();
				}

				DelegateMessage(array);
			}

		}

		public class IOSSocialManager : IntegrationGameObject {

			private static bool initialized = false;

			public static void Initialize() {
				if (!initialized) {
					Debug.Log (TAG + " Initializing IOSSocialManager...");
					GetSynchronousCodeGeneratedInstance<IOSSocialManager> ();
					initialized = true;
				}
			}

			private void OnTwitterPostFailed() {
				StansAssetsGrowIntegration.instance.OnSocialActionFailed(SocialProvider.TWITTER, SocialActionType.UPDATE_STORY);
				DelegateMessage();
			}

			private void OnTwitterPostSuccess() {
				StansAssetsGrowIntegration.instance.OnSocialActionFinished(SocialProvider.TWITTER, SocialActionType.UPDATE_STORY);
				DelegateMessage();
			}

			private void OnFacebookPostFailed() {
				StansAssetsGrowIntegration.instance.OnSocialActionFailed(SocialProvider.FACEBOOK, SocialActionType.UPDATE_STORY);
				DelegateMessage();
			}

			private void OnFacebookPostSuccess() {
				StansAssetsGrowIntegration.instance.OnSocialActionFinished(SocialProvider.FACEBOOK, SocialActionType.UPDATE_STORY);
				DelegateMessage();
			}
				
			private void OnInstaPostSuccess() {
				StansAssetsGrowIntegration.instance.OnSocialActionFinished(SocialProvider.INSTAGRAM, SocialActionType.UPDATE_STORY);
				DelegateMessage();
			}
				
			private void OnInstaPostFailed(string data) {
				StansAssetsGrowIntegration.instance.OnSocialActionFailed(SocialProvider.INSTAGRAM, SocialActionType.UPDATE_STORY);
				DelegateMessage();
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