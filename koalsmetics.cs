using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;
using BepInEx.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jotunn.Utils;
using R3DCore.Menu;
using UnityEngine.Rendering;

namespace KWC
{
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("ROUNDS 3D.exe")]
    [HarmonyPatch]
    public class KWC : BaseUnityPlugin
    {
        private const string ModId = "koala.wonderous.cosmetics";
        private const string ModName = "KWC";
        public const string Version = "0.0.1";
        public static GameObject[] eyes;
        public static GameObject[] mouths;

        public static KWC instance { get; private set; }
        public static Mesh squareMesh = new Mesh();
        public static Color[] skinColors = new[] {
            new Color(0.6392157f, 0.2862745f, 0.1686275f, 1f), // Orange
            new Color(0.1647059f, 0.3098039f, 0.5843138f, 1f), // Blue
            new Color(0.6313726f, 0.2705882f, 0.2705882f, 1f), // Red
            new Color(0.2627451f, 0.5372549f, 0.3254902f, 1f), // Green
            new Color(0.6235294f, 0.6392157f, 0.172549f, 1f), // Yellow
            new Color(0.3607843f, 0.172549f, 0.6392157f, 1f), // Purple
            new Color(0.6392157f, 0.172549f, 0.3960784f, 1f), // Magenta
            new Color(0.172549f, 0.6392157f, 0.6117647f, 1f), // Cyan
            new Color(0.6392157f, 0.3607843f, 0.2705882f, 1f), // Tangerine
            new Color(0.254902f, 0.3686275f, 0.5843138f, 1f), // Light blue
            new Color(0.6313726f, 0.3686275f, 0.3686275f, 1f), // Peach
            new Color(0.345098f, 0.5372549f, 0.3882353f, 1f), // Lime
            new Color(0.627451f, 0.6392157f, 0.3764706f, 1f), // Light Yellow
            new Color(0.4196078f, 0.2745098f, 0.6392157f, 1f), // Orchid
            new Color(0.6392157f, 0.2745098f, 0.4470588f, 1f), // Pink
            new Color(0.2745098f, 0.6392157f, 0.6156863f, 1f), // Aquamarine
            new Color(0.4392157f, 0.1960784f, 0.1137255f, 1f), // Dark Orange
            new Color(0.1058824f, 0.2f, 0.3843137f, 1f), // Dark Blue
            new Color(0.4313726f, 0.1843137f, 0.1843137f, 1f), // Dark Red
            new Color(0.1647059f, 0.3372549f, 0.2039216f, 1f), // Dark Green
            new Color(0.427451f, 0.4392157f, 0.1176471f, 1f), // Dark Yellow
            new Color(0.2470588f, 0.1176471f, 0.4392157f, 1f), // Indigo
            new Color(0.4392157f, 0.1176471f, 0.2705882f, 1f), // Cerise
            new Color(0.1176471f, 0.4392157f, 0.4196078f, 1f), // Teal
            new Color(0.4392157f, 0.2470588f, 0.1843137f, 1f), // Burnt Orange
            new Color(0.1647059f, 0.2392157f, 0.3843137f, 1f), // Midnight Blue
            new Color(0.4313726f, 0.2509804f, 0.2509804f, 1f), // Maroon
            new Color(0.2156863f, 0.3372549f, 0.2431373f, 1f), // Evergreen
            new Color(0.427451f, 0.4392157f, 0.254902f, 1f), // Gold
            new Color(0.2862745f, 0.1882353f, 0.4392157f, 1f), // Violot
            new Color(0.4392157f, 0.1882353f, 0.3058824f, 1f), // Ruby
            new Color(0.1882353f, 0.4392157f, 0.4196078f, 1f) // Dark Cyan
        };
        public static void GenerateSquare()
        {
            squareMesh.name = "KoalasCube";
            float size = 0.5f;
            Vector3[] vertices = {
                new Vector3(-size, -size, -size),
                new Vector3( size, -size, -size),
                new Vector3( size,  size, -size),
                new Vector3(-size,  size, -size),
                new Vector3(-size,  size,  size),
                new Vector3( size,  size,  size),
                new Vector3( size, -size,  size),
                new Vector3(-size, -size,  size),
            };
            int[] triangles = {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5 ,6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6
            };
            Vector2[] uvs = {
                new Vector2(0, 0.66f),
                new Vector2(0.25f, 0.66f),
                new Vector2(0, 0.33f),
                new Vector2(0.25f, 0.33f),

                new Vector2(0.5f, 0.66f),
                new Vector2(0.5f, 0.33f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
            };
            squareMesh.vertices = vertices;
            squareMesh.triangles = triangles;
            squareMesh.uv = uvs;
            squareMesh.RecalculateNormals();
        }
        private void Awake()
        {
            instance = this;
            new Harmony(ModId).PatchAll();
            GenerateSquare();
        }
        /**
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Connection), "OnJoinedRoom")]
        public static void PatchOnJoinedRoom()
        {
            foreach (var v in FindObjectsOfType<PhotonView>().Where((pv) => pv.gameObject.GetComponent<Player>()).ToArray())
            {
                var ___player = v.GetComponent<Player>();
                var kd = ___player.transform.GetComponent<KosmeticData>();
                int index = ___player.refs.view.ControllerActorNr - 1 % 32;
                bool isBox = false;
                switch (___player.refs.view.Controller.NickName)
                {
                    case "AncientKoala": index = 6; isBox = true; break;
                    case "Anarkey": index = 31; isBox = true; break;
                }
                if (isBox)
                {
                    var cm = ___player.transform.Find("Collider_Map");
                    cm.GetComponent<MeshFilter>().mesh = squareMesh;
                    var ch = ___player.transform.Find("Collider_Hitbox");
                    ch.GetComponent<MeshFilter>().mesh = squareMesh;
                    if (!ch.GetComponent<BoxCollider>())
                    {
                        ch.gameObject.AddComponent<BoxCollider>().material = ch.GetComponent<SphereCollider>().material;
                        Destroy(ch.GetComponent<SphereCollider>());
                    }
                }
                var col = skinColors[index];
                col.r *= 1.25f;
                col.g *= 1.25f;
                col.b *= 1.25f;
                var colObj = ___player.transform.Find("Collider_Hitbox");
                var meshRen = colObj.GetComponent<MeshRenderer>();
                var mat = meshRen.material;
                var newMat = new Material(mat);
                newMat.name = "koalaschangedcolor";
                newMat.color = col;
                meshRen.material = newMat;
                foreach (var ren in ___player.transform.Find("Limbs").GetComponentsInChildren<MeshRenderer>())
                {
                    ren.material = newMat;
                }
                ___player.transform.Find("Collider_Map").GetComponent<MeshRenderer>().material = newMat;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Connection), "OnPlayerEnteredRoom")]
        public static void PatchOnPlayerEntered()
        {
            foreach (var v in FindObjectsOfType<PhotonView>().Where((pv) => pv.gameObject.GetComponent<Player>()).ToArray())
            {
                var ___player = v.GetComponent<Player>();
                var kd = ___player.transform.GetComponent<KosmeticData>();
                int index = ___player.refs.view.ControllerActorNr - 1 % 32;
                bool isBox = false;
                switch (___player.refs.view.Controller.NickName)
                {
                    case "AncientKoala": index = 6; isBox = true; break;
                    case "Anarkey": index = 31; isBox = true; break;
                }
                if (isBox)
                {
                    var cm = ___player.transform.Find("Collider_Map");
                    cm.GetComponent<MeshFilter>().mesh = squareMesh;
                    var ch = ___player.transform.Find("Collider_Hitbox");
                    ch.GetComponent<MeshFilter>().mesh = squareMesh;
                    if (!ch.GetComponent<BoxCollider>())
                    {
                        ch.gameObject.AddComponent<BoxCollider>().material = ch.GetComponent<SphereCollider>().material;
                        Destroy(ch.GetComponent<SphereCollider>());
                    }
                }
                var col = skinColors[index];
                col.r *= 1.25f;
                col.g *= 1.25f;
                col.b *= 1.25f;
                var colObj = ___player.transform.Find("Collider_Hitbox");
                var meshRen = colObj.GetComponent<MeshRenderer>();
                var mat = meshRen.material;
                var newMat = new Material(mat);
                newMat.name = "koalaschangedcolor";
                newMat.color = col;
                meshRen.material = newMat;
                foreach (var ren in ___player.transform.Find("Limbs").GetComponentsInChildren<MeshRenderer>())
                {
                    ren.material = newMat;
                }
                ___player.transform.Find("Collider_Map").GetComponent<MeshRenderer>().material = newMat;
            }
        }**/
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "Start")]
        public static void PatchAwake(Player __instance)
        {
            var a = __instance.gameObject.AddComponent<KosmeticData>();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "Start")]
        public static void PatchStart2(Player __instance)
        {
            foreach (var v in FindObjectsOfType<PhotonView>().Where((pv) => pv.gameObject.GetComponent<Player>()).ToArray())
            {
                var ___player = v.GetComponent<Player>();
                var kd = ___player.transform.GetComponent<KosmeticData>();
                int index = ___player.refs.view.ControllerActorNr - 1 % 32;
                bool isBox = false;
                switch (___player.refs.view.Controller.NickName)
                {
                    case "AncientKoala": index = 6; isBox = true; break;
                    case "Anarkey": index = 31; isBox = true; break;
                }
                if (isBox)
                {
                    var cm = ___player.transform.Find("Collider_Map");
                    cm.GetComponent<MeshFilter>().mesh = squareMesh;
                    var ch = ___player.transform.Find("Collider_Hitbox");
                    ch.GetComponent<MeshFilter>().mesh = squareMesh;
                    if (!ch.GetComponent<BoxCollider>())
                    {
                        ch.gameObject.AddComponent<BoxCollider>().material = ch.GetComponent<SphereCollider>().material;
                        Destroy(ch.GetComponent<SphereCollider>());
                    }
                }
                var col = skinColors[index];
                col.r *= 1.25f;
                col.g *= 1.25f;
                col.b *= 1.25f;
                var colObj = ___player.transform.Find("Collider_Hitbox");
                var meshRen = colObj.GetComponent<MeshRenderer>();
                var mat = meshRen.material;
                var newMat = new Material(mat);
                newMat.name = "koalaschangedcolor";
                newMat.color = col;
                meshRen.material = newMat;
                foreach (var ren in ___player.transform.Find("Limbs").GetComponentsInChildren<MeshRenderer>())
                {
                    ren.material = newMat;
                }
                ___player.transform.Find("Collider_Map").GetComponent<MeshRenderer>().material = newMat;
            }
        }
    }

    public class KosmeticData : MonoBehaviour
    {
        public int colorID = -1;
        public int preferedID;
        public int[] cosmeticIDS = new int[] { 0, 0, 0, 0 };
        public bool isBox = false;

        [PunRPC]
        public static void RPCPushFaceData(int actorId, int[] cosIds, bool boxin)
        {
            FindObjectsOfType<PhotonView>().Where((p) => p.ControllerActorNr == actorId).ToArray()[0].gameObject.GetComponent<KosmeticData>().cosmeticIDS = cosIds;
            FindObjectsOfType<PhotonView>().Where((p) => p.ControllerActorNr == actorId).ToArray()[0].gameObject.GetComponent<KosmeticData>().isBox = boxin;
        }
    }
}
