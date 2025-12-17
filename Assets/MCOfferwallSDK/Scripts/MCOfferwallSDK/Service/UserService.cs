using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.MCOfferwallSDK.Domain;

namespace Assets.Scripts.MCOfferwallSDK.Service
{
    class UserService : MonoBehaviour
    {
        private const string UserIdKey = "io.mychips.user_id";
        private const string IDFAKey = "io.mychips.idfa";
        private const string GAIDKey = "io.mychips.gaid"; 
        private const string AgeKey     = "io.mychips.age";     
        private const string GenderKey  = "io.mychips.gender";
        private const string Aff_sub1Key = "io.mychips.affsub1";
        private const string Aff_sub2Key = "io.mychips.affsub2";
        private const string Aff_sub3Key = "io.mychips.affsub3";
        private const string Aff_sub4Key = "io.mychips.affsub4";
        private const string Aff_sub5Key = "io.mychips.affsub5";
        

        /// <summary>
        /// Sets the persistent app-specific user id.
        /// </summary>
        /// <param name="userId">Your app's stable user id.</param>
        public void SetId(string userId)
        {
            PlayerPrefs.SetString(UserIdKey, userId);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }
        
        /// <summary>
        /// Sets the Apple Identifier for Advertisers (IDFA).
        /// </summary>
        /// <param name="idfa">Raw IDFA string.</param>
        public void SetIDFA(string idfa)
        {
            PlayerPrefs.SetString(IDFAKey, idfa);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Sets the Google Advertising ID (GAID).
        /// </summary>
        /// <param name="gaid">Raw GAID string.</param>
        public void SetGAID(string gaid) 
        {
            PlayerPrefs.SetString(GAIDKey, gaid);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Sets the age. Valid range is 0–100 (inclusive). Any other value clears the stored age.
        /// </summary>
        /// <param name="age">Age in years (0–100). Values outside this range are treated as invalid.</param>
        public void SetAge(int age)
        {
            if (age >= 0 && age <= 100)
                PlayerPrefs.SetInt(AgeKey, age);
            else
                PlayerPrefs.DeleteKey(AgeKey);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Sets the user gender to be sent to the offerwall backend.
        /// </summary>
        /// <param name="gender">One of the defined <see cref="MCGenderEnum"/> values.</param>
         public void SetGender(MCGenderEnum gender)
        {
            var wireValue = gender.ToWireValue();
            PlayerPrefs.SetString(GenderKey, wireValue);
            PlayerPrefs.Save();
        }

        /// <summary>Sets affiliate sub-parameter 1.</summary>
        public void SetAffSub1(string aff_sub1)
        {
            PlayerPrefs.SetString(Aff_sub1Key, aff_sub1);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }

        /// <summary>Sets affiliate sub-parameter 2.</summary>
        public void SetAffSub2(string aff_sub2)
        {
            PlayerPrefs.SetString(Aff_sub2Key, aff_sub2);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }

        /// <summary>Sets affiliate sub-parameter 3.</summary>
        public void SetAffSub3(string aff_sub3)
        {
            PlayerPrefs.SetString(Aff_sub3Key, aff_sub3);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }

        /// <summary>Sets affiliate sub-parameter 4.</summary>
        public void SetAffSub4(string aff_sub4)
        {
            PlayerPrefs.SetString(Aff_sub4Key, aff_sub4);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }
        
        /// <summary>Sets affiliate sub-parameter 5.</summary>
        public void SetAffSub5(string aff_sub5)
        {
            PlayerPrefs.SetString(Aff_sub5Key, aff_sub5);
            PlayerPrefs.Save(); // This will save the preferences immediately
        }
        
        /// <summary>
        /// Gets the stored IDFA or empty string if missing.
        /// </summary>
        public string GetIDFA()
        {
            return PlayerPrefs.GetString(IDFAKey, "");
        }
        
        /// <summary>
        /// Gets the stored GAID or empty string if missing.
        /// </summary>
        public string GetGAID()
        {
            return PlayerPrefs.GetString(GAIDKey, "");
        }
        
        /// <summary>
        /// Gets the stored age if present and valid (0–100); otherwise returns <c>null</c>.
        /// </summary>
        /// <returns>Age in [0,100], or <c>null</c> if not set/invalid.</returns>
        public int? GetAge()
        {
            if (PlayerPrefs.HasKey(AgeKey))
            {
                int v = PlayerPrefs.GetInt(AgeKey, 0);
                return (v >= 0 && v <= 100) ? v : (int?)null;
            }
            return null;
        }
        
        /// <summary>
        /// Gets the stored gender wire value or <c>null</c> if missing/empty.
        /// </summary>
        public string GetGender()
        {
            var v = PlayerPrefs.GetString(GenderKey, "");
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }

        /// <summary>Gets affiliate sub-parameter 1 (raw string, may be empty).</summary>
        public string GetAffSub1()
        {
            string affsub1= PlayerPrefs.GetString(Aff_sub1Key, "");
            return affsub1;
        }

        /// <summary>Gets affiliate sub-parameter 2 (raw string, may be empty).</summary>
        public string GetAffSub2()
        {
            string affsub2 = PlayerPrefs.GetString(Aff_sub2Key, "");
            return affsub2;
        }

        /// <summary>Gets affiliate sub-parameter 3 (raw string, may be empty).</summary>
        public string GetAffSub3()
        {
            string affsub3= PlayerPrefs.GetString(Aff_sub3Key, "");
             return affsub3;
        }

        /// <summary>Gets affiliate sub-parameter 4 (raw string, may be empty).</summary>
        public string GetAffSub4()
        {
            string affsub4 = PlayerPrefs.GetString(Aff_sub4Key, "");
            return affsub4;
        }

        /// <summary>Gets affiliate sub-parameter 5 (raw string, may be empty).</summary>
        public string GetAffSub5()
        {
            string affsub5= PlayerPrefs.GetString(Aff_sub5Key, "");
             return affsub5;
        }

        /// <summary>
        /// Gets an existing user id or generates and persists a new UUID v4 if missing.
        /// </summary>
        /// <returns>Stable user id string.</returns>
        public string GetOrCreateId()
        {
            string userId = PlayerPrefs.GetString(UserIdKey, "");

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = Guid.NewGuid().ToString();
                SetId(userId);
            }

            return userId;
        }
        
        /// <summary>
        /// Gets the gender as <see cref="MCGenderEnum"/> if the stored wire value is valid; otherwise <c>null</c>.
        /// </summary>
        public MCGenderEnum? GetGenderEnum()
        {
            var v = PlayerPrefs.GetString(GenderKey, "");
            return v switch
            {
                "m" => MCGenderEnum.Male,
                "f" => MCGenderEnum.Female,
                "o" => MCGenderEnum.Other,
                _ => null
            };
        }
    }
}
