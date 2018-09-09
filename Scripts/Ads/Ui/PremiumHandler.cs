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
#if UNITY_PURCHASING
    [RequireComponent(typeof(IAPListener))]
#endif
    public class PremiumHandler : Controller<PremiumHandler> 
#if UNITY_PURCHASING 
        , IStoreListener 
#endif
    {
        
#if UNITY_PURCHASING
        public IAPButton Button;
        [CanBeNull] public Func<bool> CanShowPremium;

        [Bind] private IAPListener _listener;

        private Product _product;

        private void Start()
        {
            this.Bind();
            Button.gameObject.SetActive(false);      
            _listener.onPurchaseComplete.AddListener(OnProduct);
            _listener.onPurchaseFailed.AddListener(OnProductFail);   
        }

        public void OnProductFail(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError($"Could not buy {product} because {reason}");
        }

        public void OnProduct(Product product)
        {
            RegisterPurchase(product);
        }

        private bool RegisterPurchase(Product product)
        {
            if (product.definition.id != Button.productId || !product.hasReceipt) return false;
            StencilPremium.Purchase();
            Button.gameObject.SetActive(false);
            return true;
        }

        private bool _logged;
        private void Update()
        {
            if (StencilPremium.HasPremium)
            {
                Debug.Log("Premium history detected");
                enabled = false;
                return;
            }

            if (!StencilIap.IsReady()) return;

            if (_product == null)
                _product = Button.GetProduct();
            
            if (_product == null)
            {
                if (_logged) return;
                _logged = true;
                Debug.LogWarning("Can't find premium product");
                return;
            }
            if (!_product.hasReceipt)
            {
                Debug.Log("No premium receipt, activating.");
                Button.gameObject.SetActive(CanShowPremium?.Invoke() ?? true);
                enabled = false;
            }
            else
            {
                Debug.Log("Receipt found for premium.");
                StencilPremium.Purchase();
                enabled = false;
            }
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