using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using OSGeo.OGR;
using OSGeo.OSR;

namespace Leipzig3DASP.Extensions;

///<author email="Sythelux Rikd">Sythelux Rikd</author>
public static class LayerExtensions
{
    public static IEnumerable<Feature> Features(this Layer layer)
    {
        layer.ResetReading();
        while (layer.GetNextFeature() is { } f)
        {
            yield return f;
        }
    }


    public static JsonObject ToJson(this Layer layer)
    {
        var def = layer.GetLayerDefn();
        var ext = new Envelope();
        layer.GetExtent(ext, 1);
        /* -------------------------------------------------------------------- */
        /*      Reading the spatial reference                                   */
        /* -------------------------------------------------------------------- */
        var sr = layer.GetSpatialRef();
        string srsWkt;
        if (sr != null)
            sr.ExportToPrettyWkt(out srsWkt, 1);
        else
            srsWkt = "(unknown)";

        var layerJson = new JsonObject
        {
            ["Name"] = def.GetName(),
            ["FeatureCount"] = layer.GetFeatureCount(1),
            ["Extent"] = new JsonArray(ext.MinX, ext.MaxX, ext.MinY, ext.MaxY),
            ["SRS_WKT"] = srsWkt,
        };

        /* -------------------------------------------------------------------- */
        /*      Reading the fields                                              */
        /* -------------------------------------------------------------------- */
        var fieldDefinitions = new JsonArray();
        for (var iAttr = 0; iAttr < def.GetFieldCount(); iAttr++)
        {
            var fDef = def.GetFieldDefn(iAttr);

            fieldDefinitions.Add(new JsonObject
            {
                ["Name"] = fDef.GetNameRef(),
                ["FieldTypeName"] = fDef.GetFieldTypeName(fDef.GetFieldType()),
                ["Width"] = fDef.GetWidth(),
                ["Precision"] = fDef.GetPrecision(),
            });
        }
        layerJson.Add("FieldDefinitions", fieldDefinitions);
        
        /* -------------------------------------------------------------------- */
        /*      Reading the shapes                                              */
        /* -------------------------------------------------------------------- */

        var fields = new JsonArray();
        layer.ResetReading();
        while (layer.GetNextFeature() is { } feat)
        {
            fields.Add(feat.ToJson(def));
            feat.Dispose();
        }
        layerJson.Add("Fields", fields);

        return layerJson;
    }

    public static JsonObject ToJson(this Feature feat, FeatureDefn def)
    {
        var obj = new JsonObject { { "Name", def.GetName() } };
        for (var iField = 0; iField < feat.GetFieldCount(); iField++)
        {
            var fDef = def.GetFieldDefn(iField);


            if (feat.IsFieldSet(iField))
            {
                if (fDef.GetFieldType() == FieldType.OFTStringList)
                {
                    var arr = new JsonArray();
                    foreach (string? s in feat.GetFieldAsStringList(iField)) arr.Add(s);
                    obj.Add(fDef.GetNameRef(), arr);
                }
                else if (fDef.GetFieldType() == FieldType.OFTIntegerList)
                {
                    var arr = new JsonArray();
                    int[]? iList = feat.GetFieldAsIntegerList(iField, out int count);
                    for (var i = 0; i < count; i++) arr.Add(iList[i]);
                    obj.Add(fDef.GetNameRef(), arr);
                }
                else if (fDef.GetFieldType() == FieldType.OFTRealList)
                {
                    var arr = new JsonArray();
                    double[]? iList = feat.GetFieldAsDoubleList(iField, out int count);
                    for (var i = 0; i < count; i++) arr.Add(iList[i]);
                    obj.Add(fDef.GetNameRef(), arr);
                }
                else
                    obj.Add(fDef.GetNameRef(), feat.GetFieldAsString(iField));
            }
            else
                obj.Add("unset",true);
        }

        if (feat.GetStyleString() != null)
            obj.Add("Style", feat.GetStyleString());

        var geom = feat.GetGeometryRef();
        if (geom != null)
        {
            // obj.Add(geom.GetGeometryName(), $"{geom.GetGeometryType()}");
            var arr = new JsonArray();
            for (var i = 0; i < geom.GetGeometryCount(); i++)
            {
                var subGeom = geom.GetGeometryRef(i);
                if (subGeom != null)
                {
                    arr.Add(new JsonObject
                    {
                        [subGeom.GetGeometryName()] = subGeom.GetGeometryType() + ""
                    });
                }
            }

            obj.Add(geom.GetGeometryName(), arr);

            var env = new Envelope();
            geom.GetEnvelope(env);
            obj.Add("Envelope", env.MinX + "," + env.MaxX + "," + env.MinY + "," + env.MaxY);

            geom.ExportToWkt(out string? geomWkt);
            obj.Add("Wkt", geomWkt);
        }

        return obj;
    }
}