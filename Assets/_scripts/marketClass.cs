using UnityEngine;
using System.Collections;
using UnionAssets.FLE;

public class marketClass : MonoBehaviour {
	public static marketClass instance = null;
	//private static bool _isInited = false;
	
	//replace with your consumable item
	public const string coins1 = "coins1";
	public const string coins2 = "coins2";

	//replace with your non-consumable item
	public const string complect = "complect";

	public UILabel[] pricesReal = new UILabel[4];
	public UILabel[] currenciesReal = new UILabel[4];
	public UILabel coinsLabel;
	public UILabel energyLabel;
	public UILabel hintsLabel;

	public GameObject marketMainMenu;
	public GameObject specialMenu;
	public GameObject coinsMenu;
	public GameObject hintsMenu;
	public GameObject customizationMenu;
	public GameObject notCoinsMenu;
	public GameObject thanksMenu;

	
	private static bool ListnersAdded = false;
	public GameObject item;

	// Use this for initialization
	void Start () {
		if (initClass.progress.Count == 0) {
			initClass.getProgress();
		}
		Debug.Log("init market");
		if(instance != null){
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
		
		if(ListnersAdded) {
			return;
		}

		//Filling product list
		//You can skip this if you alredy did this in Editor settings menu
		AndroidInAppPurchaseManager.instance.addProduct(coins1);
		AndroidInAppPurchaseManager.instance.addProduct(coins2);
		AndroidInAppPurchaseManager.instance.addProduct(complect);
		
		
		//listening for purchase and consume events
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_PRODUCT_PURCHASED, OnProductPurchased);
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_PRODUCT_CONSUMED,  OnProductConsumed);
		
		//initilaizing store
		AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, OnBillingConnected);
		
		//you may use loadStore function without parametr if you have filled base64EncodedPublicKey in plugin settings
		AndroidInAppPurchaseManager.instance.loadStore();
		
		ListnersAdded = true;
		gameObject.SetActive(false);
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnEnable () {
		if (initClass.progress.Count == 0) {
			initClass.getProgress();
		}
		coinsLabel.text = initClass.progress["coins"].ToString();
		energyLabel.text = initClass.progress["energy"].ToString();
		hintsLabel.text = initClass.progress["hints"].ToString();
	}

	public void purchaseForCoins() {
		Debug.Log("purchaseForCoins");
		if (item == null) item = ActiveAnimation.current.gameObject;
		Debug.Log(item);
		int amount = int.Parse(item.transform.GetChild(1).GetComponent<UILabel>().text);
		int cost = int.Parse(item.transform.GetChild(0).GetComponent<UILabel>().text);
		bool bought = false;

		//если скин
		if (item.name.Substring(0,4) == "skin") {
			//покупаем скин
			if (initClass.progress[item.name] == 0) {
				if (cost > initClass.progress["coins"]) {
					notCoinsMenu.SetActive(true);
				} else {
					initClass.progress["coins"] -= cost;
					coinsLabel.text = initClass.progress["coins"].ToString();
					thanksMenu.SetActive(true);
					item.transform.GetChild(3).GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);
					//убираем label price и icon price [0] и [2]
					item.transform.GetChild(0).gameObject.SetActive(false);
					item.transform.GetChild(2).gameObject.SetActive(false);
					//добавляем accept [3]
					item.transform.GetChild(3).gameObject.SetActive(true);
					bought = true;
				}
			}
			if (bought || initClass.progress[item.name] != 0) {
				bought = true;
				//если купили, делаем скин активным
				//делаем accept [3] везде неактивным
				for (int j = 1; j <= 2; j++) {
					item.transform.parent.GetChild(j - 1).GetChild(3).GetComponent<UISprite>().color = new Color32(100, 100, 100, 100);
					if (initClass.progress["skin" + j] == 2) initClass.progress["skin" + j] = 1;
				}
				item.transform.GetChild(3).GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);
				initClass.progress[item.name] = 2;
			}
			
			
		//hints		
		} else if (item.name.Substring(0,4) == "hint") {
			if (cost > initClass.progress["coins"]) {
					notCoinsMenu.SetActive(true);
			} else {
				bought = true;
				initClass.progress["coins"] -= cost;
				initClass.progress["hints"] += amount;
				initClass.saveProgress();
				coinsLabel.text = initClass.progress["coins"].ToString();
				hintsLabel.text = initClass.progress["hints"].ToString();
				thanksMenu.SetActive(true);

			}
		//energy		
		} else if (item.name == "energy item") {
			if (cost > initClass.progress["coins"]) {
				GameObject ncm = GameObject.Instantiate(notCoinsMenu);
				ncm.SetActive(true);
				ncm.transform.parent = GameObject.Find("root/Camera/UI Root").transform;
				ncm.transform.localPosition = new Vector3(0, 0, -100F);
				ncm.transform.localScale = new Vector3(1F, 1F, 0);
				ncm.name = "not coins menu";
				UIPlayAnimation anim = ncm.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UIPlayAnimation>();
				GameObject backTr = GameObject.Find("root/Camera/UI Root/back transition");
				EventDelegate.Add(anim.onFinished, backTr.GetComponent<iClickClass>().backTransitionExit);
			} else {
				bought = true;
				initClass.progress["coins"] -= cost;
				Debug.Log(cost);
				initClass.progress["energyTime"] -= amount * lsEnergyClass.costEnergy;
				initClass.saveProgress();
				GameObject.Find("energy").GetComponent<lsEnergyClass>().OnApplicationPause(false);
				initLevelMenuClass.coinsLabel.text = initClass.progress["coins"].ToString();

				GameObject thm = GameObject.Instantiate(thanksMenu);
				thm.SetActive(true);
				thm.transform.parent = GameObject.Find("root/Camera/UI Root").transform;
				thm.transform.localPosition = new Vector3(0, 0, -0.01F);
				thm.transform.localScale = new Vector3(1F, 1F, 0);
				thm.name = "thanks menu";
				
			}
		}
		item = null;
		if (bought) initClass.saveProgress();
	}

	public void purchase(string SKU) {
		//ActiveAnimation.current.name must = SKU
		//AndroidInAppPurchaseManager.instance.purchase (ActiveAnimation.current.name);
	}
	
	public static void consume(string SKU) {
		AndroidInAppPurchaseManager.instance.consume (SKU);
	}
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private static void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase) {
		//some stuff for processing product purchse. Add coins, unlock track, etc
		
		switch(purchase.SKU) {
		case coins1:
			consume(coins1);
			break;
		case coins2:
			consume(coins2);
			break;
		case complect:
			consume(complect);
			//GameDataExample.EnableCoinsBoost();
			break;
		}
	}
	
	private static void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase) {
		switch(purchase.SKU) {
		case coins1:
			initClass.progress["coins"] += 250;
			marketClass.instance.coinsLabel.text = initClass.progress["coins"].ToString();
			Debug.Log("added 250 coins");
			initClass.saveProgress();
			break;
		case coins2:
			initClass.progress["coins"] += 500;
			marketClass.instance.coinsLabel.text = initClass.progress["coins"].ToString();
			Debug.Log("added 500 coins");
			initClass.saveProgress();
			break;
		//если комплект
		case complect:
			//купили комплект
			instance.thanksMenu.SetActive(true);
			Transform item = instance.specialMenu.transform.GetChild(0).GetChild(0);
			//убираем label price и label currency [0] и [1]
			item.GetChild(0).gameObject.SetActive(false);
			item.GetChild(1).gameObject.SetActive(false);
			//добавляем accept [2]
			item.GetChild(2).gameObject.SetActive(true);
			initClass.progress["complect"] = 1;
			initClass.saveProgress();
			break;

	}
}
	
	private static void OnProductPurchased(CEvent e) {
		BillingResult result = e.data as BillingResult;
		
		//this flag will tell you if purchase is available
		//result.isSuccess
		
		
		//infomation about purchase stored here
		//result.purchase
		
		//here is how for example you can get product SKU
		//result.purchase.SKU
		
		
		if(result.isSuccess) {
			OnProcessingPurchasedProduct (result.purchase);
		} else {
			AndroidMessage.Create("Product Purchase Failed", result.response.ToString() + " " + result.message);
		}
		
		Debug.Log ("Purchased Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	private static void OnProductConsumed(CEvent e) {
		BillingResult result = e.data as BillingResult;
		
		if(result.isSuccess) {
			OnProcessingConsumeProduct (result.purchase);
		} else {
			AndroidMessage.Create("Product Cousume Failed", result.response.ToString() + " " + result.message);
		}
		
		Debug.Log ("Cousume Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	private static void OnBillingConnected(CEvent e) {
		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_BILLING_SETUP_FINISHED, OnBillingConnected);
		
		
		if(result.isSuccess) {
			//Store connection is Successful. Next we loading product and customer purchasing details
			AndroidInAppPurchaseManager.instance.addEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetrieveProductsFinised);
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();
			
		} 
		
		AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		Debug.Log ("Connection Responce: " + result.response.ToString() + " " + result.message);
	}
	
	
	
	
	private static void OnRetrieveProductsFinised(CEvent e) {
		BillingResult result = e.data as BillingResult;
		AndroidInAppPurchaseManager.instance.removeEventListener (AndroidInAppPurchaseManager.ON_RETRIEVE_PRODUC_FINISHED, OnRetrieveProductsFinised);
		
		if(result.isSuccess) {
			
			UpdateStoreData();
			//_isInited = true;
			
			
		} else {
			AndroidMessage.Create("Connection Responce", result.response.ToString() + " " + result.message);
		}
		
	}
	
	
	
	private static void UpdateStoreData() {
		//marketClass.instance.
		foreach(GoogleProductTemplate p in AndroidInAppPurchaseManager.instance.inventory.products) {
			int y = 0;
			foreach (UILabel price in marketClass.instance.pricesReal) {
				if (price.transform.parent.name == p.SKU) {
					price.text = (int.Parse(p.priceAmountMicros)/ 1000000).ToString();
					marketClass.instance.currenciesReal[y].text = p.priceCurrencyCode;
				}
				y++;
			}
			Debug.Log("Loaded product title: " + p.title);
			Debug.Log("Loaded product price: " + p.price);
			Debug.Log("Loaded product priceCurrencyCode: " + p.priceCurrencyCode);
			Debug.Log("Loaded product SKU: " + p.SKU);
			Debug.Log("Loaded product originalJson: " + p.originalJson);

		}
		
		//chisking if we already own some consuamble product but forget to consume those
		if(AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(coins1)) {
			consume(coins1);
		}
		//chisking if we already own some consuamble product but forget to consume those
		if(AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(coins2)) {
			consume(coins2);
		}

		//Check if non-consumable rpduct was purchased, but we do not have local data for it.
		//It can heppens if game was reinstalled or download on oher device
		//This is replacment for restore purchase fnunctionality on IOS
		
		

		if(AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(complect)) {
			GameDataExample.EnableCoinsBoost();
		}
		
		
	}


}
