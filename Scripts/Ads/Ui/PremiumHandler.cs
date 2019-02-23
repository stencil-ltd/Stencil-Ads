using System;
using Binding;
using JetBrains.Annotations;
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

        public static event EventHandler<IStoreController> OnStoreInit; 
        public event EventHandler<Product> OnPurchasePremium;

        private static IAPListener _listener;

        private Product _product;

        public override void Register()
        {
            base.Register();
            this.Bind();
            Button.gameObject.SetActive(false);
            if (!_listener)
            {
                // IAPListener will call DontDestroyOnLoad
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
            if (RegisterPurchase(product))
                OnPurchasePremium?.Invoke(this, product);
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
            Debug.Log("Show Premium: Check");
            if (StencilPremium.IgnorePremium)
            {
                var ret = CanShowPremium?.Invoke() != false;
                Debug.Log($"Show Premium: {ret} [ignore + custom predicate or null]");
                return ret;
            }
            
            if (StencilPremium.HasPremium)
            {
                Debug.Log("Show Premium: false [already bought]");
                return false;
            }

            if (!StencilIap.IsReady())
            {
                Debug.Log("Show Premium: false [not ready]");
                return false;
            }
            
            if (_product == null)
                _product = Button.GetProduct();

            if (_product == null)
            {
                Debug.Log("Show Premium: false [product error]");
                return false;
            }

            if (!_product.hasReceipt)
            {
                var ret = CanShowPremium?.Invoke() != false;
                Debug.Log($"Show Premium: {ret} [custom predicate or null]");
                return ret;
            }

            if (!_purchased)
            {
                _purchased = true;
                Debug.Log("Show Premium: Receipt found for premium.");
                StencilPremium.Purchase();
            }
            
            Debug.Log("Show Premium: false [fallthrough]");
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
            OnStoreInit?.Invoke(this, controller);
        }
#endif
    }
}