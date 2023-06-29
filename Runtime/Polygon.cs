using System.Collections.Generic;
using System.Linq;
using CityJson;
using Godot;

public partial class Polygon : Node3D
{
    //https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html plane tutorial
    public void Draw(City city)
    {
        var cityObject = city.CityObjects[Name];
        var transform = Transform;
        transform.Origin = cityObject.Start - city.Metadata.Start;
        Transform = transform;

        foreach (var geometry in cityObject.Geometry)
        {
            foreach (List<List<int>>? polygon in geometry.Boundaries)
            {
                foreach (List<int>? listPoints in polygon)
                {
                    var s = new SurfaceTool();
                    var mesh = new ArrayMesh();
                    var mat = new OrmMaterial3D();
                    mat.AlbedoColor = new Color(1, 1, 1);

                    var vertices = new Vector3[listPoints.Count];
                    for (var i = 0; i < listPoints.Count; i++)
                        vertices[i] = city.Vertices[listPoints[i]] - cityObject.Start;

                    s.Begin(Mesh.PrimitiveType.Triangles);
                    s.SetMaterial(mat);
                    switch (vertices.Length)
                    {
                        case 3:
                        {
                            foreach (var vertex in vertices)
                                s.AddVertex(vertex);
                            break;
                        }
                        default:
                            foreach (var vertex in FromDelaunayTriangulation(vertices))
                                s.AddVertex(vertex);
                            break;
                    }

                    // s.End();

                    s.GenerateNormals();

                    var mi = new MeshInstance3D();
                    mi.Mesh = s.Commit(mesh);
                    AddChild(mi);
                }
            }
        }
    }

    private static IEnumerable<Vector3> FromDelaunayTriangulation(Vector3[] vertices)
    {
        int[]? tris = Geometry2D.TriangulateDelaunay(vertices.Select(vector3 => new Vector2(vector3.X + vector3.Z, vector3.Y)).ToArray());
        return tris.Select(i => vertices[i]);
    }

    private static IEnumerable<Vector3> IndexToVertex(IEnumerable<int> vertices, IReadOnlyList<Vector3> cityVertices)
    {
        return vertices.Select(i => cityVertices[i]);
    }

    private static IEnumerable<int> QuadToTris(List<int> ints)
    {
        var quad = 0;
        do
        {
            yield return 0;
            yield return quad + 1;
            yield return quad + 2;
            quad++;
        } while (ints.Count - quad > 2);
    }
}