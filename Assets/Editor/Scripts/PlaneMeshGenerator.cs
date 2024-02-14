using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class PlaneMeshGenerator : OdinEditorWindow
{
    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [SerializeField]
    private float _widthUnitSize;

    [SerializeField]
    private float _heightUnitSize;

    [SerializeField]
    private string _exportMeshName;

    [SerializeField]
    private DefaultAsset _exportDirectory;
    private Editor _gameObjectEditor;

    [SerializeField]
    private bool _isHorizontal;

    [MenuItem("Window/Plane Mesh Generator")]
    private static void Open()
    {
        GetWindow<PlaneMeshGenerator>().Show();
    }

    [Sirenix.OdinInspector.Button]
    void Export()
    {
        if (_exportDirectory == null || string.IsNullOrEmpty(_exportMeshName))
            return;

        var exportDirectoryPath = AssetDatabase.GetAssetPath(_exportDirectory);
        if (Path.GetExtension(_exportMeshName) != ".asset")
        {
            _exportMeshName += ".asset";
        }

        var newMesh = MeshUtils.CreateQuadMesh(_width, _height, _widthUnitSize, _heightUnitSize, _isHorizontal);
        var exportPath = Path.Combine(exportDirectoryPath, _exportMeshName);
        AssetDatabase.CreateAsset(newMesh, exportPath);
    }

    protected override void OnImGUI()
    {
        base.OnImGUI();

        if (_gameObjectEditor != null)
        {
            _gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), new GUIStyle());
        }
    }


    [Sirenix.OdinInspector.Button]
    void RefreshPreview()
    {
        var newMesh = MeshUtils.CreateQuadMesh(_width, _height, _widthUnitSize, _heightUnitSize, _isHorizontal);
        _gameObjectEditor = OdinEditor.CreateEditor(newMesh);

        if (_gameObjectEditor != null)
        {
            _gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), new GUIStyle());
        }
    }
}
