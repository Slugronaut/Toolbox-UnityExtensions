/*
 Written by: Lucas Antunes (aka ItsaMeTuni), lucasba8@gmail.com
 In: 2/15/2018
 The only thing that you cannot do with this script is sell it by itself without substantially modifying it.
 */

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
public class EnumMaskPropertyDrawer : PropertyDrawer
{
    bool foldoutOpen = false;

    object theEnum;
    Array enumValues;
    Type enumUnderlyingType;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldoutOpen)
            return EditorGUIUtility.singleLineHeight * (Enum.GetValues(fieldInfo.FieldType).Length + 2);
        else
            return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        theEnum = fieldInfo.GetValue(property.serializedObject.targetObject);
        enumValues = Enum.GetValues(theEnum.GetType());
        enumUnderlyingType = Enum.GetUnderlyingType(theEnum.GetType());

        //We need to convert the enum to its underlying type, if we don't it will be boxed
        //into an object later and then we would need to unbox it like (UnderlyingType)(EnumType)theEnum.
        //If we do this here we can just do (UnderlyingType)theEnum later (plus we can visualize the value of theEnum in VS when debugging)
        theEnum = Convert.ChangeType(theEnum, enumUnderlyingType);

        EditorGUI.BeginProperty(position, label, property);

        foldoutOpen = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), foldoutOpen, label);

        if (foldoutOpen)
        {
            //Draw the All button
            if (GUI.Button(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1, 30, 15), "All"))
            {
                theEnum = DoNotOperator(Convert.ChangeType(0, enumUnderlyingType), enumUnderlyingType);
            }

            //Draw the None button
            if (GUI.Button(new Rect(position.x + 32, position.y + EditorGUIUtility.singleLineHeight * 1, 40, 15), "None"))
            {
                theEnum = Convert.ChangeType(0, enumUnderlyingType);
            }

            //Draw the list
            for (int i = 0; i < Enum.GetNames(fieldInfo.FieldType).Length; i++)
            {
                if (EditorGUI.Toggle(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * (2 + i), position.width, EditorGUIUtility.singleLineHeight), Enum.GetNames(fieldInfo.FieldType)[i], IsSet(i)))
                {
                    ToggleIndex(i, true);
                }
                else
                {
                    ToggleIndex(i, false);
                }
            }
        }

        fieldInfo.SetValue(property.serializedObject.targetObject, theEnum);
        property.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Get the value of an enum element at the specified index (i.e. at the index of the name of the element in the names array)
    /// </summary>
    object GetEnumValue(int _index)
    {
        return Convert.ChangeType(enumValues.GetValue(_index), enumUnderlyingType);
    }

    /// <summary>
    /// Sets or unsets a bit in theEnum based on the index of the enum element (i.e. the index of the element in the names array)
    /// </summary>
    /// <param name="_set">If true the flag will be set, if false the flag will be unset.</param>
    void ToggleIndex(int _index, bool _set)
    {
        if (_set)
        {
            if (IsNoneElement(_index))
            {
                theEnum = Convert.ChangeType(0, enumUnderlyingType);
            }

            //enum = enum | val
            theEnum = DoOrOperator(theEnum, GetEnumValue(_index), enumUnderlyingType);
        }
        else
        {
            if (IsNoneElement(_index) || IsAllElement(_index))
            {
                return;
            }

            object val = GetEnumValue(_index);
            object notVal = DoNotOperator(val, enumUnderlyingType);

            //enum = enum & ~val
            theEnum = DoAndOperator(theEnum, notVal, enumUnderlyingType);
        }

    }

    /// <summary>
    /// Checks if a bit flag is set at the provided index of the enum element (i.e. the index of the element in the names array)
    /// </summary>
    bool IsSet(int _index)
    {
        object val = DoAndOperator(theEnum, GetEnumValue(_index), enumUnderlyingType);

        //We handle All and None elements differently, since they're "special"
        if (IsAllElement(_index))
        {
            //If all other bits visible to the user (elements) are set, the "All" element checkbox has to be checked
            //We don't do a simple AND operation because there might be missing bits.
            //e.g. An enum with 6 elements including the "All" element. If we set all bits visible except the "All" bit,
            //two bits might be unset. Since we want the "All" element checkbox to be checked when all other elements are set
            //we have to make sure those two extra bits are also set.
            bool allSet = true;
            for (int i = 0; i < Enum.GetNames(fieldInfo.FieldType).Length; i++)
            {
                if (i != _index && !IsNoneElement(i) && !IsSet(i))
                {
                    allSet = false;
                    break;
                }
            }

            //Make sure all bits are set if all "visible bits" are set
            if (allSet)
            {
                theEnum = DoNotOperator(Convert.ChangeType(0, enumUnderlyingType), enumUnderlyingType);
            }

            return allSet;
        }
        else if (IsNoneElement(_index))
        {
            //Just check the "None" element checkbox our enum's value is 0
            return Convert.ChangeType(theEnum, enumUnderlyingType).Equals(Convert.ChangeType(0, enumUnderlyingType));
        }

        return !val.Equals(Convert.ChangeType(0, enumUnderlyingType));
    }

    /// <summary>
    /// Call the bitwise OR operator (|) on _lhs and _rhs given their types.
    /// Will basically return _lhs | _rhs
    /// </summary>
    /// <param name="_lhs">Left-hand side of the operation.</param>
    /// <param name="_rhs">Right-hand side of the operation.</param>
    /// <param name="_type">Type of the objects.</param>
    /// <returns>Result of the operation</returns>
    static object DoOrOperator(object _lhs, object _rhs, Type _type)
    {
        if (_type == typeof(int))
        {
            return ((int)_lhs) | ((int)_rhs);
        }
        else if (_type == typeof(uint))
        {
            return ((uint)_lhs) | ((uint)_rhs);
        }
        else if (_type == typeof(short))
        {
            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((short)((short)_lhs | (short)_rhs));
        }
        else if (_type == typeof(ushort))
        {
            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((ushort)((ushort)_lhs | (ushort)_rhs));
        }
        else if (_type == typeof(long))
        {
            return ((long)_lhs) | ((long)_rhs);
        }
        else if (_type == typeof(ulong))
        {
            return ((ulong)_lhs) | ((ulong)_rhs);
        }
        else if (_type == typeof(byte))
        {
            //byte and sbyte don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((byte)((byte)_lhs | (byte)_rhs));
        }
        else if (_type == typeof(sbyte))
        {
            //byte and sbyte don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((sbyte)((sbyte)_lhs | (sbyte)_rhs));
        }
        else
        {
            throw new System.ArgumentException("Type " + _type.FullName + " not supported.");
        }
    }

    /// <summary>
    /// Call the bitwise AND operator (&) on _lhs and _rhs given their types.
    /// Will basically return _lhs & _rhs
    /// </summary>
    /// <param name="_lhs">Left-hand side of the operation.</param>
    /// <param name="_rhs">Right-hand side of the operation.</param>
    /// <param name="_type">Type of the objects.</param>
    /// <returns>Result of the operation</returns>
    static object DoAndOperator(object _lhs, object _rhs, Type _type)
    {
        if (_type == typeof(int))
        {
            return ((int)_lhs) & ((int)_rhs);
        }
        else if (_type == typeof(uint))
        {
            return ((uint)_lhs) & ((uint)_rhs);
        }
        else if (_type == typeof(short))
        {
            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((short)((short)_lhs & (short)_rhs));
        }
        else if (_type == typeof(ushort))
        {
            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((ushort)((ushort)_lhs & (ushort)_rhs));
        }
        else if (_type == typeof(long))
        {
            return ((long)_lhs) & ((long)_rhs);
        }
        else if (_type == typeof(ulong))
        {
            return ((ulong)_lhs) & ((ulong)_rhs);
        }
        else if (_type == typeof(byte))
        {
            return unchecked((byte)((byte)_lhs & (byte)_rhs));
        }
        else if (_type == typeof(sbyte))
        {
            //byte and sbyte don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((sbyte)((sbyte)_lhs & (sbyte)_rhs));
        }
        else
        {
            throw new System.ArgumentException("Type " + _type.FullName + " not supported.");
        }
    }

    /// <summary>
    /// Call the bitwise NOT operator (~) on _lhs given its type.
    /// Will basically return ~_lhs
    /// </summary>
    /// <param name="_lhs">Left-hand side of the operation.</param>
    /// <param name="_type">Type of the object.</param>
    /// <returns>Result of the operation</returns>
    static object DoNotOperator(object _lhs, Type _type)
    {
        if (_type == typeof(int))
        {
            return ~(int)_lhs;
        }
        else if (_type == typeof(uint))
        {
            return ~(uint)_lhs;
        }
        else if (_type == typeof(short))
        {
            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((short)~(short)_lhs);
        }
        else if (_type == typeof(ushort))
        {

            //ushort and short don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((ushort)~(ushort)_lhs);
        }
        else if (_type == typeof(long))
        {
            return ~(long)_lhs;
        }
        else if (_type == typeof(ulong))
        {
            return ~(ulong)_lhs;
        }
        else if (_type == typeof(byte))
        {
            //byte and sbyte don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return (byte)~(byte)_lhs;
        }
        else if (_type == typeof(sbyte))
        {
            //byte and sbyte don't have bitwise operators, it is automatically converted to an int, so we convert it back
            return unchecked((sbyte)~(sbyte)_lhs);
        }
        else
        {
            throw new System.ArgumentException("Type " + _type.FullName + " not supported.");
        }
    }

    /// <summary>
    /// Check if the element of specified index is a "None" element (all bits unset, value = 0).
    /// </summary>
    /// <param name="_index">Index of the element.</param>
    /// <returns>If the element has all bits unset or not.</returns>
    bool IsNoneElement(int _index)
    {
        return GetEnumValue(_index).Equals(Convert.ChangeType(0, enumUnderlyingType));
    }

    /// <summary>
    /// Check if the element of specified index is an "All" element (all bits set, value = ~0).
    /// </summary>
    /// <param name="_index">Index of the element.</param>
    /// <returns>If the element has all bits set or not.</returns>
    bool IsAllElement(int _index)
    {
        object elemVal = GetEnumValue(_index);
        return elemVal.Equals(DoNotOperator(Convert.ChangeType(0, enumUnderlyingType), enumUnderlyingType));
    }
}


/// <summary>
/// Special thanks to chat-gippity for saving me the time writing this bullshit.
/// </summary>
[CustomPropertyDrawer(typeof(UlongBitmaskEnumAttribute))]
public class UlongBitmaskEnumDrawer : PropertyDrawer
{
    private static bool showFoldout = false;
    private bool[] bitValues;
    string[] _SortedEnumNames;

    Type _FieldType;
    Type FieldType
    {
        get
        {
            if(_FieldType == null)
            {
                UlongBitmaskEnumAttribute bitmaskAttribute = (UlongBitmaskEnumAttribute)attribute;
                _FieldType = bitmaskAttribute.EnumType;
            }

            return _FieldType;
        }
    }

    /*
    string[] EnumNames
    {
        get
        {
            if (_SortedEnumNames == null)
            {
                UlongBitmaskEnumAttribute bitmaskAttribute = (UlongBitmaskEnumAttribute)attribute;
                _SortedEnumNames = SortEnumNames(bitmaskAttribute.enumNames);
            }
            return _SortedEnumNames;
        }
    }

    private string[] SortEnumNames(string[] enumNames)
    {
        if (!FieldType.IsEnum)
        {
            Debug.LogError("Type provided must be an enum.");
            return enumNames;
        }

        ulong[] enumValues = new ulong[enumNames.Length];
        for (int i = 0; i < enumNames.Length; i++)
        {
            enumValues[i] = Convert.ToUInt64(Enum.Parse(FieldType, enumNames[i]));
        }

        Array.Sort(enumValues, enumNames);
        return enumNames;
    }
    */

    string[] EnumNames
    {
        get
        {
            _SortedEnumNames = null;
            if (_SortedEnumNames == null)
            {
                UlongBitmaskEnumAttribute bitmaskAttribute = (UlongBitmaskEnumAttribute)attribute;
                _SortedEnumNames = new string[bitmaskAttribute.enumNames.Length];
                bitmaskAttribute.enumNames.CopyTo(_SortedEnumNames, 0);
            }
            return _SortedEnumNames;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (bitValues == null)
        {
            bitValues = new bool[EnumNames.Length];
            UpdateBitValues((ulong)property.longValue);
        }

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        showFoldout = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), showFoldout, label);
        position.y += EditorGUIUtility.singleLineHeight;

        if (showFoldout)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect checkboxPosition = new(position.x, position.y, position.width, lineHeight);

            for (int i = 0; i < EnumNames.Length; i++)
            {
                bitValues[i] = EditorGUI.Toggle(checkboxPosition, EnumNames[i], bitValues[i]);
                checkboxPosition.y += lineHeight;

                if ((i + 1) % 10 == 0)
                {
                    DrawSeparator(ref checkboxPosition);
                }
            }

            long newBitmask = UpdateBitmaskValue((ulong)property.longValue);
            property.longValue = (long)newBitmask;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (showFoldout)
        {
            int lineCount = EnumNames.Length;
            int separatorCount = Mathf.CeilToInt(lineCount / 10f) - 1;
            height += EditorGUIUtility.singleLineHeight * (lineCount + separatorCount);
        }

        return height;
    }

    private void UpdateBitValues(ulong bitmask)
    {
        for (int i = 0; i < EnumNames.Length; i++)
        {
            ulong bitValue = 1UL << i;
            bitValues[i] = (bitmask & bitValue) == bitValue;
        }
    }

    private long UpdateBitmaskValue(ulong originalBitmask)
    {
        long newBitmask = 0;

        for (int i = 0; i < EnumNames.Length; i++)
        {
            if (bitValues[i])
            {
                newBitmask |= 1L << i;
            }
        }

        return newBitmask;
    }

    private void DrawSeparator(ref Rect position)
    {
        float separatorHeight = 1f;
        position.y += separatorHeight;
        EditorGUI.DrawRect(position, Color.gray);
        position.y += separatorHeight;
    }
}


/// <summary>
/// 
/// </summary>
[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class BitMaskPropertyDrawer : PropertyDrawer
{
    private const int CheckboxWidth = 15;
    private const int CheckboxHeight = 15;
    private const int Spacing = 2;
    private const int HighlightInterval = 10;
    private readonly Color HighlightColor = new (0.8f, 0.8f, 1f); // Custom highlight color (light blue)

    BitMaskAttribute _BitMaskAttr;
    BitMaskAttribute BitMaskAttr
    {
        get
        {
            if (_BitMaskAttr == null)
                _BitMaskAttr = (BitMaskAttribute)attribute;
            return _BitMaskAttr;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Ensure that the property is of type ulong
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            ulong bitmask = (ulong)property.longValue;

            EditorGUI.BeginChangeCheck();

            Rect checkboxPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, CheckboxWidth, CheckboxHeight);
            int headerIndex = 0;
            // Draw checkboxes for each bit of the bitmask
            for (int i = 0; i < 64; i++)
            {
                bool value = (bitmask & (1UL << i)) != 0;

                // Check if the current toggle should be highlighted
                bool isHighlighted = i % HighlightInterval == 0 || i == 0;

                // Store the current GUI color to reset it later
                Color originalColor = GUI.backgroundColor;

                // Apply the highlight color if necessary
                if (isHighlighted)
                {
                    //GUI.backgroundColor = HighlightColor;
                    GUI.color = HighlightColor;
                    GUI.contentColor = HighlightColor;
                    EditorGUI.LabelField(checkboxPosition, "");
                    checkboxPosition.y += CheckboxHeight + Spacing;

                    var headerRect = checkboxPosition;
                    headerRect.width = 300;
                    EditorGUI.LabelField(headerRect, BitMaskAttr.GetHeader(headerIndex++));
                    checkboxPosition.y += CheckboxHeight + Spacing;
                    GUI.color = originalColor;
                    GUI.contentColor = originalColor;
                    continue;
                }

                EditorGUI.BeginChangeCheck();
                value = EditorGUI.Toggle(checkboxPosition, new GUIContent(BitMaskAttr.GetLabel(i)), value);
                if (EditorGUI.EndChangeCheck())
                {
                    bitmask = value ? bitmask | (1UL << i) : bitmask & ~(1UL << i);
                }

                // Reset the GUI color back to the original color
                //GUI.backgroundColor = originalColor;
                GUI.color = originalColor;
                GUI.contentColor = originalColor;
                checkboxPosition.y += CheckboxHeight + Spacing;
            }

            if (EditorGUI.EndChangeCheck())
            {
                // Update the property value if any checkboxes are changed
                property.longValue = (long)bitmask;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use BitMask attribute with ulong.");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int ExtraLines = ((64 / 10) + 1) * 2;
        return base.GetPropertyHeight(property, label) + ((ExtraLines + 64) * (CheckboxHeight + Spacing));
    }
}

#endif



[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class EnumMaskAttribute : PropertyAttribute
{

}


[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class UlongBitmaskEnumAttribute : PropertyAttribute
{
    public string[] enumNames;
    public Type EnumType;

    public UlongBitmaskEnumAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            Debug.LogError("BitmaskEnumAttribute only supports Enum types.");
            return;
        }

        EnumType = enumType;
        enumNames = Enum.GetNames(enumType);
    }
}


[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class BitMaskAttribute : PropertyAttribute
{
    string[] Labels;
    string[] Headers;

    public BitMaskAttribute(string[] labels, string[] headers)
    {
        Labels = labels;
        Headers = headers;
    }

    public string GetLabel(int i)
    {
        if (i < 0 || i >= Labels.Length)
            return string.Empty;

        return Labels[i];
    }

    public string GetHeader(int i)
    {
        if (i < 0 || i >= Headers.Length)
            return string.Empty;

        return Headers[i];
    }
}
