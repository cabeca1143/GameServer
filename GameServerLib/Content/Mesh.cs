using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
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
    public string Name; //char[128]
    public int NumVertices;
    public int NumFaces;
    public uint Flags;
}

struct BoundingBox
{
    internal Vector3 Min;
    internal Vector3 Max;
}

internal class Mesh
{
    internal string Name;
    internal string FileName;
    internal int Flags;
    internal bool HasPivot;
    internal Vector3 CentralPoint;
    internal Vector3 Pivot;
    internal Vector3 PivotExport;
    internal Vector3 Rotation;
    internal Vector3 Move;
    internal Vector3 Scale;
    internal int UseWorldMatrix;
    internal Vector4[] RotateMatrix;
    internal Vector4[] WorldMatrix;
    internal uint VertexCountInVB;
    internal int NumVertices;
    internal Vector3 WorldVertexList;
    //r3dColor* VertexColorList;
    internal int NumFaces;
    //Riot::Face* FaceList;
    internal string[] MatNamesArray;
    internal VertexNormalsState eVertexNormalsState;
    internal Vector3 VertexNormals;
    internal Vector3 U;
    internal Vector3 V;
    ///Riot::Mesh_0* m_pTransformedObj;
    internal Matrix4x4 m_CachedTransform;
    internal VertexType eVertexType;
    internal BoundingBox BBox;

    private const int MESH_ID = 1752393037;

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

        if (header.ID != MESH_ID)
        {
            //Log error
            return false;
        }

        short v1;
        short v2;

        //TODO: Find a safe way to do this in C#?
        unsafe
        {
            v1 = *(short*)&header.Version;
            v2 = *((short*)&header.Version + 1);
        }


        if (v1 > 2)
        {
            //Log Invalid Version
            return false;
        }

        //if (header.Version >= 807683271) //Uh??
        //{
        //    //Log
        //    return false;
        //}



        return true;
    }

    internal enum VertexNormalsState
    {
        kNoVertexNormals = 0x0,
        kHasAnyVertexNormalsMask = 0x1,
        kSmoothVertexNormals = 0x2,
        kFlatVertexNormals = 0x3,
    }

    internal enum VertexType
    {
        World = 0x0,
        SimpleWorld = 0x1,
    }
}
