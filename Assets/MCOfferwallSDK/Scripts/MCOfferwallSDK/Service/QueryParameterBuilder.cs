using System;
using System.Collections.Generic;

public class QueryParameterBuilder
{
    public string AdUnitId { get; set; }  // Ad unit identifier; appears at the very beginning.
    public string UserId { get; set; }    // User identifier.
    public string IDFA { get; set; }      // Apple Identifier for Advertisers
    public string GAID { get; set; }    // Google Advertising ID
    public int? Age { get; set; }         // Age
    public string Gender { get; set; }    // Gender
    public string AffSub1 { get; set; }   // Affiliate subparameter 1.
    public string AffSub2 { get; set; }   // Affiliate subparameter 2.
    public string AffSub3 { get; set; }   // Affiliate subparameter 3.
    public string AffSub4 { get; set; }   // Affiliate subparameter 4.
    public string AffSub5 { get; set; }   // Affiliate subparameter 5.

    // Dictionary to hold additional custom parameters.
    private readonly Dictionary<string, string> customParameters = new Dictionary<string, string>();

    /// <summary>
    /// Adds an additional custom parameter.
    /// Use this method for parameters other than AdUnitId, UserId, and AffSub1�AffSub5.
    /// For example: SetParam("age", "33");
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    public void SetParam(string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            customParameters[key] = value;
        }
    }

    /// <summary>
    /// Provides a read-only view of the custom parameters.
    /// </summary>
    /// <returns>A read-only dictionary of custom parameters.</returns>
    public IReadOnlyDictionary<string, string> GetCustomParameters()
    {
        return customParameters;
    }

    /// <summary>
    /// Builds the complete query string starting with a '?'.
    /// The order of parameters is as follows:
    /// 1. adunit_id (if provided)
    /// 2. user_id (if provided)
    /// 3. idfa (if provided)
    /// 4. gaid (if provided)
    /// 5. age / gender (if provided)  
    /// 6. aff_sub1 (if provided)
    /// 7. aff_sub2 (if provided)
    /// 8. aff_sub3 (if provided)
    /// 9. aff_sub4 (if provided)
    /// 10. aff_sub5 (if provided)
    /// 11. All additional custom parameters (in an unspecified order)
    /// 12. The fixed parameter sdk=android
    /// </summary>
    /// <returns>The complete query string.</returns>
    public string BuildQueryString()
    {
        List<string> parameters = new List<string>();

        // Place AdUnitId at the very beginning if provided.
        if (!string.IsNullOrWhiteSpace(AdUnitId))
            parameters.Add($"adunit_id={Uri.EscapeDataString(AdUnitId)}");
        
        // Add the fixed properties.
        if (!string.IsNullOrWhiteSpace(UserId))
            parameters.Add($"user_id={Uri.EscapeDataString(UserId)}");
        if (!string.IsNullOrWhiteSpace(IDFA))
            parameters.Add($"idfa={Uri.EscapeDataString(IDFA)}");
        if (!string.IsNullOrWhiteSpace(GAID))
            parameters.Add($"gaid={Uri.EscapeDataString(GAID)}");
        if (Age.HasValue && Age.Value >= 0 && Age.Value <= 100)
            parameters.Add($"age={Age.Value}");
        if (!string.IsNullOrWhiteSpace(Gender))
            parameters.Add($"gender={Uri.EscapeDataString(Gender)}");
        if (!string.IsNullOrWhiteSpace(AffSub1))
            parameters.Add($"aff_sub1={Uri.EscapeDataString(AffSub1)}");
        if (!string.IsNullOrWhiteSpace(AffSub2))
            parameters.Add($"aff_sub2={Uri.EscapeDataString(AffSub2)}");
        if (!string.IsNullOrWhiteSpace(AffSub3))
            parameters.Add($"aff_sub3={Uri.EscapeDataString(AffSub3)}");
        if (!string.IsNullOrWhiteSpace(AffSub4))
            parameters.Add($"aff_sub4={Uri.EscapeDataString(AffSub4)}");
        if (!string.IsNullOrWhiteSpace(AffSub5))
            parameters.Add($"aff_sub5={Uri.EscapeDataString(AffSub5)}");

        // Add any additional custom parameters.
        foreach (var kv in customParameters)
        {
            parameters.Add($"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}");
        }

        // Append the fixed parameter for SDK.
        parameters.Add("sdk=android");

        return "?" + string.Join("&", parameters);
    }
}

