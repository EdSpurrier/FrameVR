#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FrameVR.Editor
{
    public static class FrameVRLayerSetup
    {
        private static readonly string[] RequiredLayers =
        {
            "FrameVR_Player",
            "FrameVR_Hands",
            "FrameVR_Interactable",
            "FrameVR_Grabbable",
            "FrameVR_IgnoreHands",
            "FrameVR_XRRaycast"
        };

        [MenuItem("FrameVR/Setup/Create Layers")]
        public static void CreateLayers()
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty layers = tagManager.FindProperty("layers");

            foreach (string layerName in RequiredLayers)
                AddLayer(layers, layerName);

            tagManager.ApplyModifiedProperties();

            Debug.Log("FrameVR >> Layers setup complete.");
        }

        private static void AddLayer(SerializedProperty layers, string layerName)
        {
            if (LayerExists(layers, layerName))
                return;

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);

                if (!string.IsNullOrEmpty(layer.stringValue))
                    continue;

                layer.stringValue = layerName;
                Debug.Log($"FrameVR >> Added layer: {layerName}");
                return;
            }

            Debug.LogWarning($"FrameVR >> Could not add layer '{layerName}'. No free user layer slots.");
        }

        private static bool LayerExists(SerializedProperty layers, string layerName)
        {
            for (int i = 0; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                    return true;
            }

            return false;
        }
    }
}
#endif