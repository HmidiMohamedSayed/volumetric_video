using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class PLYReader
{
    public static Mesh Read(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("PLY file not found: " + filePath);
            return null;
        }

        Mesh mesh = new Mesh();

        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Color> colors = new List<Color>();
                List<int> triangles = new List<int>();

                string line;
                bool isHeader = true;
                int vertexCount = 0, faceCount = 0;
                bool isLittleEndian = true;

                // Read the header
                while (isHeader && (line = ReadAsciiLine(reader)) != null)
                {
                    if (line.StartsWith("format"))
                    {
                        if (line.Contains("binary_little_endian"))
                        {
                            isLittleEndian = true;
                        }
                        else if (line.Contains("binary_big_endian"))
                        {
                            isLittleEndian = false;
                        }
                        else
                        {
                            Debug.LogError("Unsupported PLY format: " + line);
                            return null;
                        }
                    }
                    else if (line.StartsWith("element vertex"))
                    {
                        vertexCount = int.Parse(line.Split(' ')[2]);
                    }
                    else if (line.StartsWith("element face"))
                    {
                        faceCount = int.Parse(line.Split(' ')[2]);
                    }
                    else if (line.StartsWith("end_header"))
                    {
                        isHeader = false;
                    }
                }

                // Parse vertices
                for (int i = 0; i < vertexCount; i++)
                {
                    float x = ReadFloat(reader, isLittleEndian);
                    float y = ReadFloat(reader, isLittleEndian);
                    float z = ReadFloat(reader, isLittleEndian);
                    vertices.Add(new Vector3(x, y, z));

                    // Optionally read normals
                    if (reader.BaseStream.Position + 12 <= reader.BaseStream.Length)
                    {
                        float nx = ReadFloat(reader, isLittleEndian);
                        float ny = ReadFloat(reader, isLittleEndian);
                        float nz = ReadFloat(reader, isLittleEndian);
                        normals.Add(new Vector3(nx, ny, nz));
                    }

                    // Optionally read colors
                    if (reader.BaseStream.Position + 3 <= reader.BaseStream.Length)
                    {
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();
                        colors.Add(new Color(r / 255f, g / 255f, b / 255f));
                    }
                }

                // Parse faces
                for (int i = 0; i < faceCount; i++)
                {
                    byte vertexCountInFace = reader.ReadByte(); // e.g., 3 for triangles
                    if (vertexCountInFace == 3)
                    {
                        int v1 = ReadInt(reader, isLittleEndian);
                        int v2 = ReadInt(reader, isLittleEndian);
                        int v3 = ReadInt(reader, isLittleEndian);
                        triangles.Add(v1);
                        triangles.Add(v2);
                        triangles.Add(v3);
                    }
                    else
                    {
                        Debug.LogWarning("Non-triangular face detected, skipping.");
                        reader.BaseStream.Seek(vertexCountInFace * 4, SeekOrigin.Current);
                    }
                }

                // Assign data to the mesh
                mesh.SetVertices(vertices);
                if (normals.Count == vertices.Count)
                {
                    mesh.SetNormals(normals);
                }
                if (colors.Count == vertices.Count)
                {
                    mesh.SetColors(colors);
                }
                mesh.SetTriangles(triangles, 0);
                mesh.RecalculateBounds();
                if (normals.Count == 0)
                {
                    mesh.RecalculateNormals();
                }
            }
        }
        catch (System.Exception ex)
        {
            //Debug.LogError("Error reading PLY file: " + ex.Message);
            return null;
        }

        return mesh;
    }

    private static string ReadAsciiLine(BinaryReader reader)
    {
        List<byte> bytes = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != '\n')
        {
            bytes.Add(b);
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static float ReadFloat(BinaryReader reader, bool isLittleEndian)
    {
        byte[] bytes = reader.ReadBytes(4);
        if (!isLittleEndian)
        {
            System.Array.Reverse(bytes);
        }
        return System.BitConverter.ToSingle(bytes, 0);
    }

    private static int ReadInt(BinaryReader reader, bool isLittleEndian)
    {
        byte[] bytes = reader.ReadBytes(4);
        if (!isLittleEndian)
        {
            System.Array.Reverse(bytes);
        }
        return System.BitConverter.ToInt32(bytes, 0);
    }
}
