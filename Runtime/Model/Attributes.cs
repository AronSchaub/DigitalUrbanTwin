using System.Text.Json.Serialization;

namespace CityJson;

public class Attributes
{
    [JsonPropertyName("measuredHeight")]
    public double MeasuredHeight { get; set; }

    [JsonPropertyName("roofType")]
    public string RoofType { get; set; }

    [JsonPropertyName("storeysAboveGround")]
    public int StoreysAboveGround { get; set; }

    [JsonPropertyName("creationDate")]
    public string CreationDate { get; set; }

    [JsonPropertyName("function")]
    public string Function { get; set; }

    [JsonPropertyName("DatenquelleDachhoehe")]
    public int DatenquelleDachhoehe { get; set; }

    [JsonPropertyName("QualitaetDacherkennung")]
    public int QualitaetDacherkennung { get; set; }

    [JsonPropertyName("Gemeindeschluessel")]
    public int Gemeindeschluessel { get; set; }

    [JsonPropertyName("Stand_Basis-DLM")]
    public int StandBasisDlm { get; set; }

    [JsonPropertyName("DatenquelleLage")]
    public int DatenquelleLage { get; set; }

    [JsonPropertyName("DatenquelleBodenhoehe")]
    public int DatenquelleBodenhoehe { get; set; }

    [JsonPropertyName("Dachflaeche")]
    public double Dachflaeche { get; set; }

    [JsonPropertyName("Stand_Laserscan")]
    public int StandLaserscan { get; set; }

    [JsonPropertyName("MittlereTraufHoehe")]
    public double MittlereTraufHoehe { get; set; }

    [JsonPropertyName("ObjektHoehe")]
    public double ObjektHoehe { get; set; }

    [JsonPropertyName("Bodenhoehe")]
    public double Bodenhoehe { get; set; }

    [JsonPropertyName("Dachneigung")]
    public double Dachneigung { get; set; }

    [JsonPropertyName("Stand_DGM")]
    public int StandDgm { get; set; }

    [JsonPropertyName("Dachorientierung")]
    public double Dachorientierung { get; set; }

    [JsonPropertyName("Stand_3D-Stadtmodell")]
    public int Stand3DStadtmodell { get; set; }

    [JsonPropertyName("AbsoluteHoehe")]
    public double AbsoluteHoehe { get; set; }

    [JsonPropertyName("Geometrietyp2DReferenz")]
    public string Geometrietyp2DReferenz { get; set; }

    [JsonPropertyName("ObjektTeilHoehe")]
    public double? ObjektTeilHoehe { get; set; }

    [JsonPropertyName("BezugspunktDach")]
    public string BezugspunktDach { get; set; }
}