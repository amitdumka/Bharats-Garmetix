namespace Garmetix.CoreServices
{
    internal class HSNService
    {
    }

    public class HsnCodeDetail
    {
        /// <summary>
        /// The HSN (Harmonized System of Nomenclature) code.
        /// E.g., "6105", "6204", "5007"
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Description of the goods associated with the HSN code.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional: A common or default TaxRateId associated with this HSN code.
        /// Note: Actual tax rate might depend on other factors (e.g., sale value thresholds for some items).
        /// It's often better to link TaxRate directly on the Product, but this can provide a default.
        /// </summary>
        public string DefaultTaxRateId { get; set; }

        /// <summary>
        /// Optional: Illustrative GST rate percentage (e.g., 5, 12, 18).
        /// This is for informational purposes; the actual TaxRate object should be used for calculations.
        /// </summary>
        public decimal? IllustrativeGstRate { get; set; }

        public HsnCodeDetail()
        {
            Code = string.Empty;
            Description = string.Empty;
            DefaultTaxRateId = string.Empty;
            IllustrativeGstRate = null;

        }

        public HsnCodeDetail(string code, string description, string defaultTaxRateId = null, decimal? illustrativeGstRate = null)
        {
            Code = code;
            Description = description;
            DefaultTaxRateId = defaultTaxRateId;
            IllustrativeGstRate = illustrativeGstRate;
        }
    }
}

//_hsnCodeDetails.AddRange(new List<HsnCodeDetail>
//            {
//                new HsnCodeDetail("5007", "Woven fabrics of silk or of silk waste.", gst5.TaxRateId, 5),
//                new HsnCodeDetail("5208", "Woven fabrics of cotton, containing 85% or more by weight of cotton, weighing not more than 200 g/m².", gst5.TaxRateId, 5),
//                new HsnCodeDetail("5209", "Woven fabrics of cotton, containing 85% or more by weight of cotton, weighing more than 200 g/m².", gst5.TaxRateId, 5),
//                new HsnCodeDetail("5407", "Woven fabrics of synthetic filament yarn, including woven fabrics obtained from materials of heading 5404.", gst12.TaxRateId, 12), // Rate can vary, check latest notifications
//                new HsnCodeDetail("5408", "Woven fabrics of artificial filament yarn, including woven fabrics obtained from materials of heading 5405.", gst12.TaxRateId, 12), // Rate can vary
//                new HsnCodeDetail("6101", "Men's or boys' overcoats, car-coats, capes, cloaks, anoraks (including ski-jackets), wind-cheaters, wind-jackets and similar articles, knitted or crocheted, other than those of heading 6103.", gst12.TaxRateId, 12), // Apparel often 5% or 12% based on value
//                new HsnCodeDetail("6103", "Men's or boys' suits, ensembles, jackets, blazers, trousers, bib and brace overalls, breeches and shorts (other than swimwear), knitted or crocheted.", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6104", "Women's or girls' suits, ensembles, jackets, blazers, dresses, skirts, divided skirts, trousers, bib and brace overalls, breeches and shorts (other than swimwear), knitted or crocheted.", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6105", "Men's or boys' shirts, knitted or crocheted.", gst12.TaxRateId, 12), // If sale price < Rs. 1000, rate is 5%, otherwise 12%. This kind of logic needs to be handled during tax calculation, not just HSN.
//                new HsnCodeDetail("6106", "Women's or girls' blouses, shirts and shirt-blouses, knitted or crocheted.", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6109", "T-shirts, singlets and other vests, knitted or crocheted.", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6201", "Men's or boys' overcoats, car-coats, capes, cloaks, anoraks (including ski-jackets), wind-cheaters, wind-jackets and similar articles, other than those of heading 6203 (woven).", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6203", "Men's or boys' suits, ensembles, jackets, blazers, trousers, bib and brace overalls, breeches and shorts (other than swimwear) (woven).", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6204", "Women's or girls' suits, ensembles, jackets, blazers, dresses, skirts, divided skirts, trousers, bib and brace overalls, breeches and shorts (other than swimwear) (woven).", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6205", "Men's or boys' shirts (woven).", gst12.TaxRateId, 12), // Similar value-based rate logic may apply
//                new HsnCodeDetail("6211", "Track suits, ski suits and swimwear; other garments.", gst12.TaxRateId, 12),
//                new HsnCodeDetail("6302", "Bed linen, table linen, toilet linen and kitchen linen.", gst5.TaxRateId, 5) // Or 12% depending on material/value
//            });
// public async Task<IEnumerable<HsnCodeDetail>> SearchHsnCodesAsync(string searchTerm)
//{
//    await Task.Delay(50);
//    if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2) // Require at least 2 chars to search
//    {
//        return Enumerable.Empty<HsnCodeDetail>();
//    }

//    string lowerSearchTerm = searchTerm.ToLowerInvariant().Trim();
//    return _hsnCodeDetails
//        .Where(h => h.Code.Contains(lowerSearchTerm, StringComparison.OrdinalIgnoreCase) ||
//                    (h.Description != null && h.Description.ToLowerInvariant().Contains(lowerSearchTerm)))
//        .Take(20) // Limit results for performance in a UI lookup
//        .ToList();
//}
// --- HSN Code Operations ---
//public async Task<HsnCodeDetail> GetHsnCodeDetailsAsync(string hsnCode)
//{
//    await Task.Delay(50); // Simulate async
//    if (string.IsNullOrWhiteSpace(hsnCode)) return null;

//    var detail = _hsnCodeDetails.FirstOrDefault(h => h.Code.Equals(hsnCode.Trim(), StringComparison.OrdinalIgnoreCase));

//    // In a real app, if not found locally, you might query an external API here.
//    // Example (conceptual):
//    // if (detail == null && IsInternetAvailable()) {
//    //     try {
//    //         // var httpClient = new HttpClient();
//    //         // var response = await httpClient.GetStringAsync($"https://api.examplegst.com/hsn/{hsnCode}");
//    //         // detail = JsonConvert.DeserializeObject<HsnCodeDetail>(response);
//    //         // if (detail != null) { _hsnCodeDetails.Add(detail); } // Cache it
//    //     } catch (Exception ex) {
//    //         System.Diagnostics.Debug.WriteLine($"API call for HSN {hsnCode} failed: {ex.Message}");
//    //     }
//    // }
//    return detail;
//}