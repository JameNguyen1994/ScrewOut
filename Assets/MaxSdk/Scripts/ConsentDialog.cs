using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Ad.Consent
{
    public sealed class ConsentDialog
    {
        private ConsentRequestParameters request;
        public bool IsInitialized { get; private set; }
        private UnityAction<bool> OnChoice;

        public ConsentDialog(bool tagForUnderAgeOfConsent, UnityAction<bool> onChoice, List<string> deviceIds = null, DebugGeography geography = DebugGeography.Disabled)
        {
            OnChoice = onChoice;
            request = new ConsentRequestParameters()
            {
                TagForUnderAgeOfConsent = tagForUnderAgeOfConsent,
                ConsentDebugSettings = new ConsentDebugSettings()
                {
                    DebugGeography = geography,
                    TestDeviceHashedIds = deviceIds
                }
            };

            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        private void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                Debug.Log($"Consent error: {error.Message}");
                OnChoice?.Invoke(false);
                return;
            }
            
            Debug.Log($"Consent status is: {ConsentInformation.ConsentStatus}");

            if (ConsentInformation.IsConsentFormAvailable())
            {
                LoadConsentForm();
            }
            else
            {
                OnChoice?.Invoke(false);
                Debug.Log("Consent is not available");
            }
        }

        private void LoadConsentForm()
        {
            ConsentForm.Load(OnLoadConsentForm);
        }

        private void OnLoadConsentForm(ConsentForm consentForm, FormError error)
        {
            if (error != null)
            {
                Debug.Log($"Consent form load error: {error.Message}");
                OnChoice?.Invoke(false);
                return;
            }
            
            Debug.Log($"Consent status: {ConsentInformation.ConsentStatus}");
            

            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
            {
                consentForm.Show(OnShowForm);
            }
            else
            {
                if (ConsentInformation.ConsentStatus == ConsentStatus.NotRequired || 
                    ConsentInformation.ConsentStatus == ConsentStatus.Unknown)
                {
                    OnChoice?.Invoke(false);
                    return;
                }
                
                OnChoice?.Invoke(true);
            }
        }

        private void OnShowForm(FormError error)
        {
            if (error != null)
            {
                Debug.Log($"Show consent form error: {error.Message}");
                OnChoice?.Invoke(false);
                return;
            }
            
            Debug.Log("On Show Form Callback!");
            
            LoadConsentForm();
        }

        public void ResetConsent()
        {
            ConsentInformation.Reset();
        }
    }
}
