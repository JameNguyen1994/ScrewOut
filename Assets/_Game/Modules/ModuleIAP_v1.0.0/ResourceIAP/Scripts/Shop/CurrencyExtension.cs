using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class CurrencyExtension
{
    public static string GetCurrencyFromPriceAtCurrentCulture(this decimal value, string isoCurrency)
    {
        Dictionary<string, string> data = new Dictionary<string, string>
        {
            {"USD", "en-US"}, // Đô la Mỹ
            {"VND", "vi-VN"}, // Đồng Việt Nam
            {"EUR", "fr-FR"}, // Euro - Pháp
            {"JPY", "ja-JP"}, // Yên Nhật
            {"GBP", "en-GB"}, // Bảng Anh
            {"AUD", "en-AU"}, // Đô la Úc
            {"CAD", "en-CA"}, // Đô la Canada
            {"CHF", "de-CH"}, // Franc Thụy Sĩ
            {"CNY", "zh-CN"}, // Nhân dân tệ Trung Quốc
            {"HKD", "zh-HK"}, // Đô la Hong Kong
            {"KRW", "ko-KR"}, // Won Hàn Quốc
            {"INR", "hi-IN"}, // Rupee Ấn Độ
            {"RUB", "ru-RU"}, // Rúp Nga
            {"BRL", "pt-BR"}, // Real Brazil
            {"MXN", "es-MX"}, // Peso Mexico
            {"SGD", "en-SG"}, // Đô la Singapore
            {"NZD", "en-NZ"}, // Đô la New Zealand
            {"SEK", "sv-SE"}, // Krona Thụy Điển
            {"NOK", "nb-NO"}, // Krone Na Uy
            {"DKK", "da-DK"}, // Krone Đan Mạch
            {"ZAR", "en-ZA"}, // Rand Nam Phi
            {"PLN", "pl-PL"}, // Zloty Ba Lan
            {"TRY", "tr-TR"}, // Lira Thổ Nhĩ Kỳ
            {"THB", "th-TH"}, // Baht Thái Lan
            {"MYR", "ms-MY"}, // Ringgit Malaysia
            {"IDR", "id-ID"}, // Rupiah Indonesia
            {"PHP", "en-PH"}, // Peso Philippines
            {"ARS", "es-AR"}, // Peso Argentina
            {"CLP", "es-CL"}, // Peso Chile
            {"COP", "es-CO"}, // Peso Colombia
            {"EGP", "ar-EG"}, // Bảng Ai Cập
            {"SAR", "ar-SA"}, // Riyal Ả Rập Saudi
            {"AED", "ar-AE"}, // Dirham Các tiểu vương quốc Ả Rập Thống nhất
            {"ILS", "he-IL"}, // Shekel Israel
            {"CZK", "cs-CZ"}, // Koruna Cộng hòa Séc
            {"HUF", "hu-HU"}, // Forint Hungary
            {"RON", "ro-RO"}, // Leu Romania
            {"BGN", "bg-BG"}, // Lev Bulgaria
            {"HRK", "hr-HR"}, // Kuna Croatia
            {"ISK", "is-IS"}, // Krona Iceland
            {"PKR", "ur-PK"}, // Rupee Pakistan
            {"KWD", "ar-KW"}, // Dinar Kuwait
            {"OMR", "ar-OM"}, // Rial Oman
            {"QAR", "ar-QA"}, // Rial Qatar
            {"BHD", "ar-BH"}, // Dinar Bahrain
            {"JOD", "ar-JO"}, // Dinar Jordan
            {"MAD", "ar-MA"}, // Dirham Ma-rốc
            {"TWD", "zh-TW"}, // Đô la Đài Loan
            {"UYU", "es-UY"}, // Peso Uruguay
            {"VUV", "fr-VU"}, // Vatu Vanuatu
        };
        
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        
        if (data.ContainsKey(isoCurrency))
        {
            cultureInfo = new CultureInfo(data[isoCurrency]);
        }

        if (value % 1 == 0)
        {
            return value.ToString("C0", cultureInfo);
        }

        if ((value * 10) % 1 == 0)
        {
            return value.ToString("C1", cultureInfo);
        }
        
        return value.ToString("C2", cultureInfo);
    }
}