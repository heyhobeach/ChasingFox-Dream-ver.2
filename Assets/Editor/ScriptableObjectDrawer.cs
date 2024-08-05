// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// [CustomPropertyDrawer(typeof(ScriptableObject), true)]
// public class ScriptableObjectDrawer : PropertyDrawer
// {
//     // Dictionary to cache editors for each object
//     private Dictionary<Object, Editor> editorCache = new Dictionary<Object, Editor>();

//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         float totalHeight = EditorGUI.GetPropertyHeight(property, label, true);

//         if (property.isExpanded && property.objectReferenceValue != null)
//         {
//             SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
//             SerializedProperty prop = serializedObject.GetIterator();
//             if (prop.NextVisible(true))
//             {
//                 do
//                 {
//                     totalHeight += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
//                 } while (prop.NextVisible(false));
//             }
//         }

//         return totalHeight;
//     }

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         // Draw the main property field
//         EditorGUI.PropertyField(position, property, label, true);
//         Local_DrawTreeNode(property, 0);

//         // Draw expanded properties if foldout is open
//         void Local_DrawTreeNode(SerializedProperty node, int depth)
//         {
//             EditorGUI.indentLevel = depth;
//             SerializedObject serializedObject = new SerializedObject(node.objectReferenceValue);
//             SerializedProperty prop = serializedObject.GetIterator();

//             Rect newPosition = position;
//             newPosition.y += EditorGUI.GetPropertyHeight(property, label, true);
//             newPosition.height = EditorGUIUtility.singleLineHeight;
//             if (prop.NextVisible(true))
//             {
//                 do
//                 {
//                     if(prop.arrayElementType.Equals("BehaviorNode") && prop.isArray) Local_DrawTreeNode(prop, depth + 1);
//                     float propertyHeight = EditorGUI.GetPropertyHeight(prop, true);
//                     newPosition.height = propertyHeight;
//                     EditorGUI.PropertyField(newPosition, prop, true);
//                     newPosition.y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
//                 }while(prop.NextVisible(false));
//             }
//             if (GUI.changed)
//                 serializedObject.ApplyModifiedProperties();
//             // foreach (var child in prop)
//             // {
//             //     Local_DrawTreeNode(child, depth + 1);
//             // }
//         }
//         // if (property.isExpanded && property.objectReferenceValue != null)
//         // {
//         //     // Indent child fields
//         //     EditorGUI.indentLevel++;

//         //     // Get the SerializedObject
//         //     SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
//         //     SerializedProperty prop = serializedObject.GetIterator();

//         //     // Calculate position for the nested properties
//         //     Rect newPosition = position;
//         //     newPosition.y += EditorGUI.GetPropertyHeight(property, label, true);
//         //     newPosition.height = EditorGUIUtility.singleLineHeight;
            
            

//         //     // Move to the first child property
//         //     if (prop.NextVisible(true))
//         //     {
//         //         do
//         //         {
//         //             float propertyHeight = EditorGUI.GetPropertyHeight(prop, true);
//         //             newPosition.height = propertyHeight;
//         //             EditorGUI.PropertyField(newPosition, prop, true);
//         //             newPosition.y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
//         //         } while (prop.NextVisible(false));
//         //     }

//         //     // Apply modified properties
//         //     if (GUI.changed)
//         //         serializedObject.ApplyModifiedProperties();

//         //     // Reset indent level
//         //     EditorGUI.indentLevel--;
//         // }
//     }
// }