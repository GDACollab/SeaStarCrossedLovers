using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockData))]
public class BlockDataEditor : Editor {

    // SOURCE: https://unitycodemonkey.com/video.php?v=E91NYvDqsy8

    private const float CELL_SIZE = 100;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        BlockData blockData = (BlockData)target;

        Texture texture = null;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("blockPrefab"),
            GUIContent.none, true, GUILayout.Width(CELL_SIZE));

        EditorGUILayout.Space();

        GUILayout.Label("Blocklet Grid", new GUIStyle { fontStyle = FontStyle.Bold });

        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_03 != null)
                {
                    texture = blockData.cell_03.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_03"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_13 != null)
                {
                    texture = blockData.cell_13.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_13"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_23 != null)
                {
                    texture = blockData.cell_23.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_23"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_33 != null)
                {
                    texture = blockData.cell_33.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_33"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_02 != null) {
                    texture = blockData.cell_02.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_02"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_12 != null) {
                    texture = blockData.cell_12.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_12"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_22 != null) {
                    texture = blockData.cell_22.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_22"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_32 != null)
                {
                    texture = blockData.cell_32.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_32"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_01 != null) {
                    texture = blockData.cell_01.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_01"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_11 != null) {
                    texture = blockData.cell_11.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_11"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_21 != null) {
                    texture = blockData.cell_21.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_21"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_31 != null)
                {
                    texture = blockData.cell_31.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_31"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_00 != null) {
                    texture = blockData.cell_00.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_00"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_10 != null) {
                    texture = blockData.cell_10.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_10"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_20 != null) {
                    texture = blockData.cell_20.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_20"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
                texture = null;
                if (blockData.cell_30 != null)
                {
                    texture = blockData.cell_30.sprite.texture;
                }
                GUILayout.Box(texture, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cell_30"),
                    GUIContent.none, true, GUILayout.Width(CELL_SIZE));
            EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}