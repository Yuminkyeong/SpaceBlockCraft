using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using CWEnum;


//#if UNITY_ANDROID && !UNITY_EDITOR
#if UNITY_ANDROID

using UnityEngine.Purchasing;
using UnityEngine.Analytics;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;



public class CWCashManager : CWManager<CWCashManager>, IStoreListener
{

    public delegate void CBResult(bool bResult);

    string m_szProductID;
    CBResult CBResultFunc;

    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    void SuccessEndTask()
    {
        CBResultFunc(true);
    }
    void FailEndTask()
    {
        CBResultFunc(false);
    }


    public override void Create()
    {
       // InitializePurchasing();
        base.Create();
    }

    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }
    
    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var module = StandardPurchasingModule.Instance();

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);


      
        
        foreach(var v in CWArrayManager.Instance.m_kCoinData)
        {

            builder.AddProduct(v.Key, ProductType.Consumable, new IDs { { v.Key, AppleAppStore.Name }, { v.Key, GooglePlay.Name }, });
        }


      



        UnityPurchasing.Initialize(this, builder);
    }


    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            CWDebugManager.Instance.Log("RestorePurchases FAIL. Not initialized.");
            //NoticeMessage.Instance.Show

            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            CWDebugManager.Instance.Log("RestorePurchases started ...");

            var apple = extensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { CWDebugManager.Instance.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            CWDebugManager.Instance.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

     void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        CWDebugManager.Instance.Log("OnInitialized : PASS");

        storeController = sc;
        extensionProvider = ep;
    }

    void OnInitializeFailed(InitializationFailureReason reason)
    {
        CWDebugManager.Instance.Log("OnInitializeFailed InitializationFailureReason:" + reason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        CWDebugManager.Instance.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

        SuccessEndTask();
        //CBResultFunc(true);
        CWSocketManager.Instance.UseShop(string.Format("{0}", args.purchasedProduct.definition.id));
        return PurchaseProcessingResult.Complete;
    }

    void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        FailEndTask();
        CWDebugManager.Instance.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }


#region 외부함수
 
    void _BuyProductID()
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(m_szProductID);

                if (p != null && p.availableToPurchase)
                {
                    CWDebugManager.Instance.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    CWDebugManager.Instance.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                CWDebugManager.Instance.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            CWDebugManager.Instance.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    public void BuyProductID(string productId, CBResult ResultFunc)
    {
        if (CWHeroManager.Instance.m_bBlockUser)
        {
            // 차단 유저면 통과
            return;
        }
        m_szProductID = productId;
        CBResultFunc = ResultFunc;

        if (CWMainGame.Instance.m_bCheatVersion)
        {
            CBResultFunc(true);
            return;
        }

        //CWSocketManager.Instance.Close(_BuyProductID);
        _BuyProductID();

    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {

    }

    void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        FailEndTask();
        CWDebugManager.Instance.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        CWDebugManager.Instance.Log("OnInitialized : PASS");

        storeController = controller;
        extensionProvider = extensions;

    }
#endregion

}
#else
public class CWCashManager : CWManager<CWCashManager>
{
    public delegate void CBResult(bool bResult);
    public void BuyProductID(string productId, CBResult ResultFunc)
    {
        ResultFunc(true);
    }

    public void InitializePurchasing()
    {
    }
}
#endif

