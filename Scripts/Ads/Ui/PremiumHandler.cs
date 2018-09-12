using System;
using Binding;
using JetBrains.Annotations;
using Plugins.UI;
using Purchasing;
using UI;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace Ads.Ui
{
    public class PremiumHandler : Controller<PremiumHandler> 
#if UNITY_PURCHASING 
        , IStoreListener 
#endif
    {
#if UNITY_PURCHASING
        public IAPButton Button;
        [CanBeNull] public Func<bool> CanShowPremium;

        private static IAPListener _listener;

        private Product _product;

        public override void Register()
        {
            base.Register();
            this.Bind();
            Button.gameObject.SetActive(false);
            if (!_listener)
            {
                _listener = new GameObject("Premium Listener").AddComponent<IAPListener>();
                _listener.consumePurchase = false;
                _listener.onPurchaseComplete = new IAPListener.OnPurchaseCompletedEvent();
                _listener.onPurchaseFailed = new IAPListener.OnPurchaseFailedEvent();
            }   
            _listener.onPurchaseComplete.AddListener(OnProduct);
            _listener.onPurchaseFailed.AddListener(OnProductFail);
        }

        public override void WillUnregister()
        {
            base.WillUnregister();
            _listener.onPurchaseComplete.RemoveListener(OnProduct);
            _listener.onPurchaseFailed.RemoveListener(OnProductFail);
        }

        public void OnProductFail(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError($"Could not buy {product} because {reason}");
            CheckPremium();
        }

        public void OnProduct(Product product)
        {
            RegisterPurchase(product);
        }

        private bool RegisterPurchase(Product product)
        {
            if (product.definition.id != Button.productId || !product.hasReceipt) return false;
            StencilPremium.Purchase();
            CheckPremium();
            return true;
        }
        
        private void Update()
        {
            if (_product != null) return;
            if (!Button.HasProduct()) return;
            _product = Button.GetProduct();
            CheckPremium();
        }

        private bool _purchased;
        private bool _CanShowPremium()
        {
            if (StencilPremium.HasPremium)
                return false;
            
            if (!StencilIap.IsReady()) 
                return false;
            
            if (_product == null)
                _product = Button.GetProduct();
            
            if (_product == null)
                return false;

            if (!_product.hasReceipt)
                return CanShowPremium?.Invoke() != false;

            if (!_purchased)
            {
                _purchased = true;
                Debug.Log("Receipt found for premium.");
                StencilPremium.Purchase();
            }
            return false;
        }

        public void CheckPremium()
        {
            Button.gameObject.SetActive(_CanShowPremium());
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (RegisterPurchase(e.purchasedProduct))
                return PurchaseProcessingResult.Complete;
            return PurchaseProcessingResult.Pending;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            
        }
#endif
    }
}