using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServerLib.Content;

struct MeshHeader
{
    public uint r3dID;
    public uint ID;
    public uint Version;
    public string Name;
    public int NumVertices;
    public int NumFaces;
    public uint Flags;
}

internal class Mesh
{
    string Name;
    string FileName;
    int Flags;
    bool bHasPivot;
    Vector3 CentralPoint;
    Vector3 vPivot;
    Vector3 vPivotExport;
    Vector3 vRotation;
    Vector3 vMove;
    Vector3 vScale;
    int bUseWorldMatrix;
    //r3dMatrix4x4 RotateMatrix;
    //r3dMatrix4x4 WorldMatrix;
    //DWORD m_VertexCountInVB;
    int NumVertices;
    //r3dPoint3D* WorldVertexList;
    //r3dColor* VertexColorList;
    int NumFaces;
    //Riot::Face* FaceList;
    //const char* MatNamesArray;
    //Riot::Mesh::VertexNormalsState vertexNormalsState;
    Vector3 VertexNormals;
    Vector3 vU;
    Vector3 vV;
    ///Riot::Mesh_0* m_pTransformedObj;
    //r3dMatrix4x4 m_CachedTransform;
    //Riot::Mesh::VertexType m_VertexType;
    //r3dBox3D BBox;

    internal Mesh(string path)
    {
        BinaryReader br = new(File.OpenRead(path));
        LoadShared(br);
    }

    bool LoadShared(BinaryReader br)
    {
        MeshHeader header = new()
        {
            r3dID = br.ReadUInt32(),
            ID = br.ReadUInt32(),
            Version = br.ReadUInt32(),
            Name = Encoding.ASCII.GetString(br.ReadBytes(128)),
            NumVertices = br.ReadInt32(),
            NumFaces = br.ReadInt32(),
            Flags = br.ReadUInt32()
        };

        if (header.ID != 1752393037)
        {
            //Log error
            return false;
        }

        if (header.Version >= 3) //Double-check
        {
            //Log Invalid Version
            return false;
        }

        if (header.Version >= 807683271) //?
        {
            
        }

        return true;
    }
}
