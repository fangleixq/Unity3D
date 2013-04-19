﻿using UnityEngine;
using UnityEditor;
using System;
using klock.geometry;

public class kPolyGUI
{
    //--------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                      //
    //    SELECTION HELPER                                                                                                                  //
    //                                                                                                                                      //
    //--------------------------------------------------------------------------------------------------------------------------------------//
    #region SELECTION HELPER
    public static GameObject S_OBJECT
    {
        get
        {
            return Selection.activeGameObject;
        }
    }
    public static MeshFilter S_MESHFILTER
    {
        get
        {
            GameObject selection = S_OBJECT;
            return (selection != null && selection.GetComponent<MeshFilter>() != null ? selection.GetComponent<MeshFilter>() : null);
        }
    }
    public static Mesh S_MESH
    {
        get
        {
            MeshFilter meshFilter = S_MESHFILTER;
            return (meshFilter != null ? meshFilter.sharedMesh : null);
        }
    }
    #endregion
    //--------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                      //
    //     MAIN TOOL BAR                                                                                                                    //
    //                                                                                                                                      //
    //--------------------------------------------------------------------------------------------------------------------------------------//
    #region  MAIN TOOL BAR
    /*private static Texture2D[] TOOL_BAR_ICONS = new Texture2D[] {   klock.kLibary.LoadBitmap("create", 20,20),
                                                                    klock.kLibary.LoadBitmap("modify", 20,20),
                                                                    klock.kLibary.LoadBitmap("utility", 20,20),
                                                                    klock.kLibary.LoadBitmap("utility", 20,20)};*/
    private static string[] S_MAIN_TOOLBAR = new string[] { "CREATE", "EDIT", "INFO", "MAT" };
    public static void MAIN_TOOLBAR()
    {
        kPoly2Tool k2p = kPoly2Tool.instance;
        k2p.MAIN_MENU_ID = GUILayout.Toolbar(k2p.MAIN_MENU_ID, S_MAIN_TOOLBAR);

    }
    #endregion
    //--------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                      //
    //     PANEL CREATE                                                                                                                     //
    //                                                                                                                                      //
    //--------------------------------------------------------------------------------------------------------------------------------------// 
    #region PANEL CREATE
    private static bool FOLD_para = true;
    private static bool FOLD_name = true;
    private static bool FOLD_object = true;
    private static bool FOLD_create = true;
    private static int MESH_PRIM_INDEX = 0;
    private static string[] MESH_PRIM = new string[2] { "Standard Primitive", "Unity Primitive" };
    private static int MESH_TYPE_INDEX = 1;
    private static string[] MESH_TYPE_a = new string[6] { "Cube", "Plane", "Cone", "Cylinder", "Sphere", "Box" };
    private static string[] MESH_TYPE_b = new string[5] { "Cube", "Sphere", "Capsule", "Cylinder", "Plane" };

    private static string _meshName = "kPoly";
    private static float _width = 1;
    private static float _height = 1;
    private static float _depth = 1;
    private static int _uSegments = 1;
    private static int _vSegments = 1;
    private static int _zSegments = 1;
    private static int _pivotIndex = 0;
    private static string[] _pivotLabels = { "UpperLeft", "UpperCenter", "UpperRight", "MiddleLeft", "MiddleCenter", "MiddleRight", "LowerLeft", "LowerCenter", "LowerRight" };
    private static int _faceIndex = 0;
    private static string[] _windinLabels = { "TopLeft", "TopRight", "ButtomLeft", "ButtomRight" };
    private static int _windinIndex = 2;
    private static string[] _colliderLabels = { "none", "MeshCollider", "BoxCollider" };
    private static int _colliderIndex = 1;

    private static float openingAngle = 0f;
    private static bool outside = true;
    private static bool inside = false;

    private static GUILayoutOption[] E_WIDTH = new GUILayoutOption[]
    { 
        GUILayout.MaxWidth( 80 )
       
    };


    public static void CREATE_objectSelect()
    {
        bool GUI_TEMP = GUI.enabled;

        EditorGUILayout.BeginVertical();
        GUILayout.Space(2);
        // OBJECT CART 
        MESH_PRIM_INDEX = EditorGUILayout.Popup(MESH_PRIM_INDEX, MESH_PRIM);//, folderSkin());

        // OBJECT TYPE 
        FOLD_object = EditorGUILayout.Foldout(FOLD_object, "Object Types");//, folderSkin());
        if (FOLD_object)
        {
            int objectTemp = MESH_TYPE_INDEX;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            MESH_TYPE_INDEX = GUILayout.SelectionGrid(MESH_TYPE_INDEX, (MESH_PRIM_INDEX == 0 ? MESH_TYPE_a : MESH_TYPE_b), 2);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            if (objectTemp != MESH_TYPE_INDEX)
            {
                ResetEditorValues();
            }
        }
        // OBJECT NAME
        FOLD_name = EditorGUILayout.Foldout(FOLD_name, "Object Name");
        if (FOLD_name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.enabled = MESH_TYPE_INDEX != -1;
            _meshName = EditorGUILayout.TextField(_meshName, labelCSkin());
            GUI.enabled = GUI_TEMP;
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }
        // OBJECT PARAMETERS
        FOLD_para = EditorGUILayout.Foldout(FOLD_para, "Parameters");
        if (FOLD_para)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.enabled = MESH_TYPE_INDEX != -1;
            if (MESH_PRIM_INDEX == 0)
            {
                switch (MESH_TYPE_INDEX)
                {
                    case 0:
                        CREATE_cube();
                        break;
                    case 1:
                        CREATE_plane();
                        break;
                    case 2:
                        CREATE_cone();
                        break;
                }
            }
            else
            {
                EditorGUILayout.LabelField("No editabled properties.", EditorStyles.boldLabel);
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }
        // OBJECT CREATION TYPE 
        FOLD_create = EditorGUILayout.Foldout(FOLD_create, "Creation Types");//, folderSkin());
        if (FOLD_create)
        {
            // Editor Button for start mesh creation
            if (GUILayout.Button(new GUIContent("GameObject [ Scene ]")))
            {
                switch (MESH_PRIM_INDEX)
                {
                    case 0:
                        // "Cube", "Plane", "Cone", "Cylinder", "Sphere", "Box"
                        switch (MESH_TYPE_INDEX)
                        {
                            case 0:
                                kPoly.Create_Cube_Object(_meshName, _uSegments, _vSegments, _zSegments, _width, _height, _depth);
                                break;
                            case 1:
                                kPoly.Create_Plane_Object(_meshName, _uSegments, _vSegments, _width, _height, _faceIndex, _windinIndex, _pivotIndex, _colliderIndex);
                                break;
                            case 2:
                                kPoly.Create_Cone_Object(_meshName, _uSegments, _width, _depth, _height, openingAngle, outside, inside);
                                break;
                        }
                        break;
                    case 1:
                        //"Cube", "Sphere", "Capsule", "Cylinder", "Plane"
                        switch (MESH_TYPE_INDEX)
                        {
                            case 0:
                                GameObject.CreatePrimitive(PrimitiveType.Cube);
                                break;
                            case 1:
                                GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                break;
                            case 2:
                                GameObject.CreatePrimitive(PrimitiveType.Capsule);
                                break;
                            case 3:
                                GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                                break;
                            case 4:
                                GameObject.CreatePrimitive(PrimitiveType.Plane);
                                break;
                        }
                        break;
                }
            }
            GUILayout.Button(new GUIContent("Mesh Asset [ DataBase ]"));
            GUILayout.BeginHorizontal();
            GUILayout.Button(new GUIContent("Export File [ Folder ]"));
            GUILayout.Button(new GUIContent(".."), GUILayout.Width(18));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        GUI.enabled = GUI_TEMP;
        GUI.skin = null;
    }
    static void CREATE_cone()
    {
        EditorGUILayout.BeginVertical();
        // Editor value reset button
        if (GUILayout.Button(new GUIContent("Reset Editor"), EditorStyles.miniButton))
        {
            ResetEditorValues();
        }
        // if openingAngle>0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;
        EditorGUILayout.Space();
        _uSegments = EditorGUILayout.IntField("Segments", _uSegments); // numVertices
        EditorGUILayout.Space();
        _width = EditorGUILayout.FloatField("Radius Top", _width); //radiusTop
        _depth = EditorGUILayout.FloatField("Radius Bottom", _depth);//radiusBottom
        _height = EditorGUILayout.FloatField("Height", _height);//length
        EditorGUILayout.Space();
        openingAngle = EditorGUILayout.FloatField("Open Angle", openingAngle);
        EditorGUILayout.Space();
        outside = EditorGUILayout.Toggle("Outside", outside);
        inside = EditorGUILayout.Toggle("Inside", inside);
        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }
    static void CREATE_cube()
    {
        EditorGUILayout.BeginVertical();
        // Editor value reset button
        if (GUILayout.Button(new GUIContent("Reset Editor"), EditorStyles.miniButton))
        {
            ResetEditorValues();
        }
        EditorGUILayout.Space();
        // Editor value for width and height of the created mesh [ float ]
        _width = EditorGUILayout.FloatField("Width", _width);
        _height = EditorGUILayout.FloatField("Height", _height);
        _depth = EditorGUILayout.FloatField("Depth", _depth);
        EditorGUILayout.Space();
        // Editor value for width and height segments of the created mesh [ int ]
        _uSegments = EditorGUILayout.IntField("uSegments", _uSegments);
        _vSegments = EditorGUILayout.IntField("vSegments", _vSegments);
        _zSegments = EditorGUILayout.IntField("zSegments", _zSegments);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
    static void CREATE_plane()
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        // Editor value reset button
        if (GUILayout.Button(new GUIContent("Reset Editor"), EditorStyles.miniButton))
        {
            ResetEditorValues();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        // Editor value for width and height of the created mesh [ float ]
        _width = EditorGUILayout.FloatField("Width", _width);
        _height = EditorGUILayout.FloatField("Height", _height);
        EditorGUILayout.Space();
        // Editor value for width and height segments of the created mesh [ int ]
        _uSegments = EditorGUILayout.IntField("uSegments", _uSegments);
        _vSegments = EditorGUILayout.IntField("vSegments", _vSegments);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        // Editor value for the pivot point of the created mesh Unity.TextAnchor
        GUILayout.Label("Pivot ");
        GUILayout.Space(18);
        _pivotIndex = EditorGUILayout.Popup(_pivotIndex, _pivotLabels);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        // Editor value for the mesh face direction FACING.XZ
        GUILayout.Label("Facing ");
        GUILayout.Space(10);
        _faceIndex = EditorGUILayout.Popup(_faceIndex, klock.geometry.kPoly.FACING);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        // Editor value for triangle winding order
        GUILayout.Label("Winding");
        GUILayout.Space(2);
        _windinIndex = EditorGUILayout.Popup(_windinIndex, _windinLabels);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        // Editor value for collider export
        GUILayout.Label("Collider ");
        GUILayout.Space(3);
        _colliderIndex = EditorGUILayout.Popup(_colliderIndex, _colliderLabels);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        // Starting GUI changes check
        if (EditorGUI.EndChangeCheck())
        {
            _width = Mathf.Clamp(_width, 0, int.MaxValue);
            _height = Mathf.Clamp(_height, 0, int.MaxValue);
            _uSegments = Mathf.Clamp(_uSegments, 1, int.MaxValue);
            _vSegments = Mathf.Clamp(_vSegments, 1, int.MaxValue);
            Debug.Log("Change Editor");
        }
        EditorGUILayout.EndVertical();
    }
    /*public static GUIStyle folderSkin()
    {
        GUIStyle cs = EditorStyles.foldout;
        //cs.normal.background = GUI.skin.label.onActive.background;
        // cs.fontStyle = FontStyle.Bold;
        // cs.fontSize = 11;
        return cs;
    }*/
    public static GUIStyle labelCSkin()
    {
        GUIStyle cs = new GUIStyle(EditorStyles.textField);
        cs.alignment = TextAnchor.MiddleCenter;

        return cs;
    }
    /** Reset the editor values to default.*/
    private static void ResetEditorValues()
    {
        _width = 1;
        _height = 1;
        _depth = 1;

        _uSegments = 1;
        _vSegments = 1;
        _zSegments = 1;

        _pivotIndex = 0;
        _faceIndex = 0;
        _colliderIndex = 1;
        _windinIndex = 2;

        openingAngle = 0f;
        outside = true;
        inside = false;
        _meshName = "kPoly";

        if (MESH_TYPE_INDEX == 2)
        {
            _uSegments = 10;
            _width = 0;
        }
    }
    #endregion
    //--------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                      //
    //     PANEL INFO                                                                                                                       //
    //                                                                                                                                      //
    //--------------------------------------------------------------------------------------------------------------------------------------//
    #region INFO
    private static bool _SHOW_TRIAS = true;
    private static bool _SHOW_NEIBS = false;
    private static bool _SHOW_DHANS = false;

    public static void INFO_main()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(5);

        GameObject _selection = S_OBJECT;
        Mesh _selectMesh = S_MESH;
        // current selection idendifier
        GUI.enabled = (_selection != null);
        EditorGUILayout.ObjectField("Selection ", _selection, typeof(GameObject), true);
        EditorGUILayout.LabelField("Mesh Name : " + (_selection != null && _selectMesh != null ? _selectMesh.name : "none"));
        EditorGUI.indentLevel = 1;
        EditorGUILayout.LabelField("Vertecies : " + (_selection != null && _selectMesh != null ? _selectMesh.vertexCount + " " : "0"));
        EditorGUILayout.LabelField("Triangles : " + (_selection != null && _selectMesh != null ? (_selectMesh.vertexCount / 3) + " " : "0"));
        EditorGUILayout.LabelField("Faces : " + (_selection != null && _selectMesh != null ? (_selectMesh.vertexCount / 6) + " " : "0"));
        EditorGUILayout.LabelField("SubMeshes : " + (_selection != null && _selectMesh != null ? _selectMesh.subMeshCount : 0));
        EditorGUILayout.Space();
        EditorGUI.indentLevel = 0;
        _SHOW_TRIAS = EditorGUILayout.Toggle("Triangles", _SHOW_TRIAS);
        _SHOW_NEIBS = EditorGUILayout.Toggle("Neigbours", _SHOW_NEIBS);
        _SHOW_DHANS = EditorGUILayout.Toggle("Default Handles", _SHOW_DHANS);
        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();

    }
    #endregion
    //--------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                      //
    //     PANEL INFO MATERIAL                                                                                                                      //
    //                                                                                                                                      //
    //--------------------------------------------------------------------------------------------------------------------------------------//
    #region MATERIAL PANEL
    private static int MAT_CART_INDEX = 0;
    private static string[] MAT_CART = new string[2] { "Standard Materials", "Extra" };
    private static int MAT_TYPE_INDEX = 0;
    private static string[] MAT_TYPE_a = new string[3] { "Diffuse", "Diffuse Transparent", "Diffuse Bumped" };
    private static string[] MAT_TYPE_b = new string[1] { "Checker" };
    private static Color Color_a = Color.white;
    private static Color Color_b = Color.black;
    private static int Checker_Size = 1;
    private static Texture2D Mtexture_a = null;
    private static Texture2D Mtexture_b = null;
    private static Texture2D Mtexture_c = null;
    private static Shader Mshader = null;

    private static Shader[] shaders = null;

    public static void MAT_main()
    {
        // Fetch all shader assets
        if (shaders == null)
        {
            shaders = (Shader[])UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Shader));
            int n = shaders.Length;
            System.Collections.Generic.List<Shader> ls = new System.Collections.Generic.List<Shader>();

            foreach( Shader s in shaders )
            {
                if (s != null && s.name != "" && !s.name.StartsWith("__") && !s.name.StartsWith("Hidden/"))
                {
                    ls.Add(s);
                }
            }
            MAT_TYPE_a = new string[ls.Count];
            for (int i = 0; i < n; i++)
            {
                MAT_TYPE_a[i] = ls[i].name;
            }
        }



        bool GUI_TEMP = GUI.enabled;
        GameObject _selection = S_OBJECT;
        Material _selectMat = (S_MESHFILTER != null) ? S_MESHFILTER.renderer.sharedMaterial : null;
        _meshName = (_selectMat != null) ? _selectMat.name : "kMaterial";
        GUI.enabled = (_selection != null);

        // EditorGUILayout.BeginHorizontal();
        // GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(2);

        // OBJECT CART 
        MAT_CART_INDEX = EditorGUILayout.Popup(MAT_CART_INDEX, MAT_CART);//, folderSkin());

        // OBJECT TYPE 
        FOLD_object = EditorGUILayout.Foldout(FOLD_object, "Shader Types");//, folderSkin());
        if (FOLD_object)
        {
            int objectTemp = MAT_TYPE_INDEX;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            MAT_TYPE_INDEX = GUILayout.SelectionGrid(MAT_TYPE_INDEX, (MAT_CART_INDEX == 0 ? MAT_TYPE_a : MAT_TYPE_b), 2);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            if (objectTemp != MAT_TYPE_INDEX)
            {
                ResetEditorValues();
            }
        }
        // OBJECT NAME
        FOLD_name = EditorGUILayout.Foldout(FOLD_name, "Material Name");
        if (FOLD_name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.enabled = MESH_TYPE_INDEX != -1;
            _meshName = EditorGUILayout.TextField(_meshName, labelCSkin());
            GUI.enabled = GUI_TEMP;
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        /* (MAT_TYPE_INDEX)
        {
            case 1:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(80));
                Color_a = EditorGUILayout.ColorField(Color_a);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Texture", GUILayout.MaxWidth(80));
                Mtexture_a = ((Texture2D)EditorGUILayout.ObjectField(Mtexture_a, typeof(Texture2D), true));
                EditorGUILayout.EndHorizontal();
                break;
            case 2:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(80));
                Color_a = EditorGUILayout.ColorField(Color_a);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Texture", GUILayout.MaxWidth(80));
                Mtexture_a = ((Texture2D)EditorGUILayout.ObjectField(Mtexture_a, typeof(Texture2D), true));
                EditorGUILayout.EndHorizontal();
                break;
            case 3:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color A", GUILayout.MaxWidth(80));
                Color_a = EditorGUILayout.ColorField(Color_a);
                EditorGUILayout.EndHorizontal();
                //
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color B", GUILayout.MaxWidth(80));
                Color_b = EditorGUILayout.ColorField(Color_b);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Checker Size");
                Checker_Size = EditorGUILayout.IntField(Checker_Size);
                EditorGUILayout.EndHorizontal();
                break;
        }
         *  "Diffuse", "Diffuse Transparent", "Diffuse Bumped"
         */
        switch (MAT_CART_INDEX)
        {
            case 0:
                switch (MAT_TYPE_INDEX)
                {
                    case 0:
                    case 1:
                        //diffuse 
                        //Diffuse Transparent
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(80));
                        Color_a = EditorGUILayout.ColorField(Color_a);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Texture", GUILayout.MaxWidth(80));
                        Mtexture_a = ((Texture2D)EditorGUILayout.ObjectField(Mtexture_a, typeof(Texture2D), true));
                        EditorGUILayout.EndHorizontal();

                        break;
                    case 2:
                        //Diffuse Bumped
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(80));
                        Color_a = EditorGUILayout.ColorField(Color_a);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Main Texture", GUILayout.MaxWidth(80));
                        Mtexture_a = ((Texture2D)EditorGUILayout.ObjectField(Mtexture_a, typeof(Texture2D), true));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Bump Texture", GUILayout.MaxWidth(80));
                        Mtexture_b = ((Texture2D)EditorGUILayout.ObjectField(Mtexture_b, typeof(Texture2D), true));
                        EditorGUILayout.EndHorizontal();
                        break;
                }
                break;
            case 1:
                switch (MAT_TYPE_INDEX)
                {
                    case 0:
                        /*EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Shader", GUILayout.MaxWidth(80));
                         Mshader = EditorGUILayout.ObjectField(Mshader, typeof(Shader), true) as Shader;
                        EditorGUILayout.EndHorizontal();
                        */
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Color A", GUILayout.MaxWidth(80));
                        Color_a = EditorGUILayout.ColorField(Color_a);
                        EditorGUILayout.EndHorizontal();
                        //
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Color B", GUILayout.MaxWidth(80));
                        Color_b = EditorGUILayout.ColorField(Color_b);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Checker Size");
                        Checker_Size = EditorGUILayout.IntField(Checker_Size);
                        EditorGUILayout.EndHorizontal();
                        break;
                }
                break;
        }
        GUILayout.Space(10);

        // MAT CREATION TYPE 
        FOLD_create = EditorGUILayout.Foldout(FOLD_create, "Creation Types");//, folderSkin());
        if (FOLD_create)
        {
            // Editor Button for start material creation
            if (GUILayout.Button(new GUIContent("Replace Selected [ Material ]")))
            {
                switch (MAT_CART_INDEX)
                {
                    case 0: break;
                    case 1: break;
                }
            }
            GUILayout.Button(new GUIContent("Material Asset [ DataBase ]"));
            GUILayout.BeginHorizontal();
            GUILayout.Button(new GUIContent("Export File [ Folder ]"));
            GUILayout.Button(new GUIContent(".."), GUILayout.Width(18));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        //GUILayout.Space(10);
        //EditorGUILayout.EndHorizontal();
    }
    public static Material MAT_creator(int index, Color colorA)
    {
        Material m = null;
        switch (index)
        {
            case 1: m = new Material(Shader.Find("Diffuse")); m.color = colorA; m.mainTexture = Mtexture_a; break;
            case 2: m = new Material(Shader.Find("Transparent/Diffuse")); m.color = colorA; m.mainTexture = Mtexture_a; break;
            case 3: m = MAT_checker(); break;
        }
        return m;
    }

    public static Material MAT_checker()
    {
        Material m = new Material(Shader.Find("Transparent/Diffuse"));
        Texture2D t = new Texture2D(Checker_Size * 2, Checker_Size * 2, TextureFormat.ARGB32, false);
        int size = Checker_Size;
        Color ca = Color_a;
        Color cb = Color_b;

        for (int i = 0; i < size; i++)
        {
            t.SetPixel(i + 0, i + 0, ca);
            t.SetPixel(i + size, i + 0, cb);
            t.SetPixel(i + 0, i + size, cb);
            t.SetPixel(i + size, i + size, ca);
        }
        t.Apply();
        t.wrapMode = TextureWrapMode.Repeat;
        t.filterMode = FilterMode.Point;
        m.mainTexture = t;
        return m;
    }
    #endregion

    #region EXTRA GUI
    public static Enum EnumToolbar(Enum selected)
    {
        string[] toolbar = System.Enum.GetNames(selected.GetType());
        Array values = System.Enum.GetValues(selected.GetType());

        for (int i = 0; i < toolbar.Length; i++)
        {
            string toolname = toolbar[i];
            toolname = toolname.Replace("_", " ");
            toolbar[i] = toolname;
        }

        int selected_index = 0;
        while (selected_index < values.Length)
        {
            if (selected.ToString() == values.GetValue(selected_index).ToString())
            {
                break;
            }
            selected_index++;
        }
        selected_index = GUILayout.Toolbar(selected_index, toolbar);
        return (Enum)values.GetValue(selected_index);
    }
    #endregion
}

