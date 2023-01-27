using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Toolbox;

namespace Toolbox.ToolboxEditor
{
    /*
    /// <summary>
    /// PropertyDrawer for <see cref="BoundProperty"/> datatype.
    /// </summary>
    #if FULL_INSPECTOR
    //[FullInspector.CustomAttributePropertyEditor(typeof(BoundProperty))]
    [CustomPropertyDrawer(typeof(BoundProperty))]
    public class BoundPropertyDrawer : PropertyDrawer, FullInspector.IAttributePropertyEditor
    #else
    [CustomPropertyDrawer(typeof(BoundProperty))]
    public class BoundPropertyDrawer : PropertyDrawer
    #endif
    {
        #if FULL_INSPECTOR
        public Attribute Attribute { get; set; }

        public PropertyEditorChain EditorChain { get; set; }

        public bool CanEdit(Type dataType)
        {
            return dataType == typeof(BoundProperty);
        }
        #endif

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 3 + 10;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BoundProperty bp = null;
            var target = property.serializedObject.targetObject;
            var field = target.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public);
            bp = field.GetValue(target) as BoundProperty;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.DrawRect(position, new Color(0.6f, 0.6f, 0.6f, 0.6f));

            float lineHeight = GetPropertyHeight(property, label) / 3;

            Rect p = position;
            p.width *= 0.12f;

            Rect p0 = position;
            p0.height = lineHeight;

            Rect p1 = p0;
            p1.y += lineHeight;

            Rect p2 = p1;
            p2.y += lineHeight;

            p0.x += p.width;
            p0.width = position.width - p.width;

            var at = this.attribute;
            var attrs = field.GetCustomAttributes(typeof(BoundTypeAttribute), false);
            BoundTypeAttribute boundAttr = attrs.Length > 0 ? attrs[0] as BoundTypeAttribute : null;
            Type filter = boundAttr == null ? null : boundAttr.BoundType;
            if (filter == null) filter = bp.Filter;

            EditorGUI.PrefixLabel(p, new GUIContent("Purpose", "Used to identify purpose at a glance.\n\nWarning: This is stripped from the final build."));
            bp.Notes = EditorGUI.TextField(p0, bp.Notes, GUI.skin.FindStyle("ToolbarTextField"));

            var bindRes = DisplayBindingControl(p1, "Source", bp.SourceContext, bp.SourceIndex, filter, MemberTypes.Property, false, true);
            bp.DataSourcePath = bindRes.Path;
            bp.SourceContext = bindRes.Context;
            bp.SourceIndex = bindRes.PopupIndex;

            bindRes = DisplayBindingControl(p2, "Dest", bp.DestContext, bp.DestIndex, filter, MemberTypes.Field | MemberTypes.Property, true, false);
            bp.DataDestPath = bindRes.Path;
            bp.DestContext = bindRes.Context;
            bp.DestIndex = bindRes.PopupIndex;

            if (EditorGUI.EndChangeCheck() || GUI.changed)
            {
                //TODO: Push updates here?
                EditorGUI.EndProperty();
            }
        }

        /// <summary>
        /// Returned by <see cref="DisplayBindingControl"/>
        /// </summary>
        public struct BindingResult
        {
            public UnityEngine.Object Context;
            public string Path;
            public int PopupIndex;

            public BindingResult(UnityEngine.Object context, string path, int popupIndex)
            {
                Context = context;
                Path = path;
                PopupIndex = popupIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        public static BindingResult DisplayBindingControl(Rect position, string name, UnityEngine.Object context, int sourceIndex, Type typeFilter = null, MemberTypes bindType = MemberTypes.Field | MemberTypes.Property, bool requireWrite = true, bool requireRead = true)
        {
            Rect p0 = position;
            p0.width *= 0.2f;

            Rect p1 = position;
            p1.x += p0.width;
            p1.width = position.width - p0.width;

            EditorGUI.PrefixLabel(p0, new GUIContent(name));
            return DisplayBindingControl(p1, context, sourceIndex, typeFilter, bindType, requireWrite, requireRead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        public static BindingResult DisplayBindingControl(Rect position, UnityEngine.Object context, int sourceIndex, Type typeFilter = null, MemberTypes bindType = MemberTypes.Field | MemberTypes.Property, bool requireWrite = true, bool requireRead = true)
        {
            Rect p1 = position;
            p1.width *= 0.4f;

            Rect p2 = position;
            p2.x += p1.width;
            p2.width -= p1.width;

            
            context = EditorGUI.ObjectField(p1, context, typeof(UnityEngine.Object), true);
            if (!TypeHelper.IsReferenceNull(context))
            {
                var mems = context.GetType().GetMembers(BindingHelper.DefaultFlags);
                if(typeFilter != null) mems = mems.Where(x => x.DataType() == typeFilter).ToArray();
                string[] nameList = null;

                if ((bindType & (MemberTypes.Field | MemberTypes.Property)) != 0)
                    nameList = (from x in mems
                                where x is FieldInfo || (x is PropertyInfo && (!requireRead || (x as PropertyInfo).CanRead) && (!requireWrite || (x as PropertyInfo).CanWrite))
                                select x.Name).ToArray();
                else if ((bindType & MemberTypes.Field) != 0)
                    nameList = (from x in mems where x is FieldInfo select x.Name).ToArray();
                else if ((bindType & MemberTypes.Property) != 0)
                    nameList = (from x in mems
                                where x is PropertyInfo && (!requireRead || (x as PropertyInfo).CanRead) && (!requireWrite || (x as PropertyInfo).CanWrite)
                                select x.Name).ToArray();

                if (sourceIndex >= nameList.Length) sourceIndex = 0;
                if (nameList.Length < 1)
                    return new BindingResult(context, null, sourceIndex);

                sourceIndex = EditorGUI.Popup(p2, sourceIndex, nameList);
                return new BindingResult(context, nameList[sourceIndex], sourceIndex);
            }
            return new BindingResult(context, null, sourceIndex);
        }

    }


    /// <summary>
    /// Draws a string as a Unity SceneAsset selection control. Requires a backing field
    /// with the same name as the string variable and an underscore in front.
    /// Ex: string Scene -> SceneAsset _Scene;
    /// </summary>
    #if FULL_INSPECTOR
    [FullInspector.CustomAttributePropertyEditor(typeof(Toolbox.Common.SceneNameAttribute))]
    [CustomPropertyDrawer(typeof(Toolbox.Common.SceneNameAttribute))]
    public class SceneDrawer : PropertyDrawer, FullInspector.IAttributePropertyEditor
    #else
    [CustomPropertyDrawer(typeof(Toolbox.Common.SceneNameAttribute))]
    public class SceneDrawer : PropertyDrawer
    #endif
    {
        #if FULL_INSPECTOR
        public Attribute Attribute { get; set; }

        public PropertyEditorChain EditorChain { get; set; }

        public bool CanEdit(Type dataType)
        {
            return dataType == typeof(string);
        }
        #endif

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var subProp = prop.serializedObject.FindProperty("_" + prop.name);
            if (subProp == null)
            {
                Debug.LogError(prop.name + " requires a serialized backing field of type SceneAsset with the identifier _" + prop.name);
                return;
            }
            EditorGUI.BeginProperty(position, label, prop);
            EditorGUI.BeginChangeCheck();

            SceneAsset temp = subProp.objectReferenceValue as SceneAsset;
            temp = EditorGUI.ObjectField(position, label, temp, typeof(SceneAsset), false) as SceneAsset;
            subProp.objectReferenceValue = temp;
            if(subProp.objectReferenceValue != null) prop.stringValue = temp.name;

            if(EditorGUI.EndChangeCheck() || GUI.changed)
            EditorGUI.EndProperty();
            
        }
    }


    /// <summary>
    /// Property drawer for displaying dropdown inspectors controls with a list of clsses in them.
    /// </summary>
    [CustomPropertyDrawer(typeof(Toolbox.Common.ClassListAttribute))]
    public class ClassListDrawer : PropertyDrawer
    {
        List<string> Names = new List<string>();
        Dictionary<string, int> Indexer = new Dictionary<string, int>();
        ClassListAttribute ClassListAttr { get { return ((ClassListAttribute)attribute); } }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, prop);
            //EditorGUI.BeginChangeCheck();

            //We need to convert our string property to an index
            //that is used by the dropdown list and then back again.
            //Not the fastest, or the cleverest, but it gets the job done.
            string selected = prop.stringValue;
            Names.Clear();
            Indexer.Clear();
            int i = 1;
            Names.Add(ClassListAttr.DefaultChoice);
            Indexer.Add(ClassListAttr.DefaultChoice, 0);
            foreach (Type t in TypeHelper.FindSubClasses(ClassListAttr.InheritsFrom))
            {
                Names.Add(t.FullName);
                Indexer.Add(t.FullName, i);
                i++;
            }
            if (string.IsNullOrEmpty(selected)) selected = ClassListAttr.DefaultChoice;
            Indexer[selected] = EditorGUI.Popup(position, ClassListAttr.Label, Indexer[selected], Names.ToArray());
            prop.stringValue = Names[Indexer[selected]];
            EditorGUI.EndProperty();
        }

    }


    /// <summary>
    ///  Property drawer for displaying dropdown inspectors controls with a list of classes that expose a certain interface in them.
    /// </summary>
    [CustomPropertyDrawer(typeof(Toolbox.Common.InterfaceListAttribute))]
    public class InterfaceListDrawer : PropertyDrawer
    {
        List<string> Names = new List<string>();
        Dictionary<string, int> Indexer = new Dictionary<string, int>();
        InterfaceListAttribute ClassListAttr { get { return ((InterfaceListAttribute)attribute); } }

        //cached results for faster editing
        static Dictionary<Type, Type[]> Lookup = new Dictionary<Type, Type[]>(20);
        

#pragma warning disable CS0169 // The field 'InterfaceListDrawer.scrollPos' is never used
        Vector2 scrollPos;
#pragma warning restore CS0169 // The field 'InterfaceListDrawer.scrollPos' is never used
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);
            //EditorGUI.BeginChangeCheck();

            //We need to convert our string property to an index
            //that is used by the dropdown list and then back again.
            //Not the fastest, or the cleverest, but it gets the job done.
            string selected = prop.stringValue as string;
            Names.Clear();
            Indexer.Clear();
            int i = 1;
            Names.Add(ClassListAttr.DefaultChoice);
            Indexer.Add(ClassListAttr.DefaultChoice, 0);
            
            Type[] types = null;
            if (!Lookup.TryGetValue(ClassListAttr.InheritsFrom, out types))
            {
                if (ClassListAttr.DefaultConstructorOnly)
                    types = TypeHelper.FindInterfaceImplementationsWithDefaultConstructors(ClassListAttr.InheritsFrom);
                else types = TypeHelper.FindInterfaceImplementations(ClassListAttr.InheritsFrom);

                types = types.OrderBy(t => t.FullName).ToArray();
                Lookup[ClassListAttr.InheritsFrom] = types;
            }

            foreach (Type t in types)
            {
                Names.Add(t.FullName);
                Indexer.Add(t.FullName, i);
                i++;
            }
            if (string.IsNullOrEmpty(selected)) selected = ClassListAttr.DefaultChoice;
            try
            {
                //using TRY-CATCH here to avoid kill the whole inspector if a datatype goes missing
                Indexer[selected] = EditorGUI.Popup(position, ClassListAttr.Label, Indexer[selected], Names.ToArray());
                prop.stringValue = Names[Indexer[selected]];
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                prop.stringValue = null;
            }

            
            EditorGUI.EndProperty();
        }
    }


    [CustomPropertyDrawer(typeof(Toolbox.Common.CompactAttribute))]
    public class CompactDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {

            //EditorGUIUtility.LookLikeControls();
            position.xMin += 4;
            position.xMax -= 4;

            EditorGUI.BeginProperty(position, label, prop);
            EditorGUI.BeginChangeCheck();

            switch (prop.type)
            {
                case "Vector3":
                    var newV3 = EditorGUI.Vector3Field(position, label.text, prop.vector3Value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        prop.vector3Value = newV3;
                    }
                    break;
                case "Vector2":
                    var newV2 = EditorGUI.Vector2Field(position, label.text, prop.vector2Value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        prop.vector2Value = newV2;
                    }
                    break;
                case "Quaternion":
                    var newV4 = EditorGUI.Vector4Field(position, label.text, QuaternionToVector4(prop.quaternionValue));
                    if (EditorGUI.EndChangeCheck())
                    {
                        prop.quaternionValue = ConvertToQuaternion(newV4);
                    }
                    break;
                default:

                    EditorGUI.HelpBox(position, "[Compact] doesn't work with type '" + prop.type + "' (Supported: Vector2, Vector3, Quaternion)", MessageType.Error);
                    break;
            }

            EditorGUI.EndProperty();

        }

        private Quaternion ConvertToQuaternion(Vector4 v4)
        {
            return new Quaternion(v4.x, v4.y, v4.z, v4.w);
        }
        private Vector4 QuaternionToVector4(Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float extraHeight = 1f;
            return base.GetPropertyHeight(property, label) + extraHeight;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(MaskedEnumAttribute))]
    public class CustomMaskPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //parse the property for the sweet, sweet info inside.
            MaskedEnumAttribute settings = (MaskedEnumAttribute)attribute;
            var mask = Toolbox.ToolboxEditor.ToolboxEditorUtility.GetBackingField<Enum>(property);

            string propName = settings.EnumName;
            if (string.IsNullOrEmpty(propName)) propName = label.text;

            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            mask = EditorGUI.EnumFlagsField(position, propName, mask);
            property.intValue = (int)Convert.ChangeType(mask, mask.GetType());
            EditorGUI.EndProperty();
        }
    }
    

    /// <summary>
    /// Base drawer for HashMap<>. Because Unity doesn't support property drawers for
    /// generics you must derive a concrete, non-generic hash map from HashMap<>
    /// and then also derive a concrete, non-generic property drawer from this class that is applied
    /// to the aforementioned dictionary.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public abstract class HashMapDrawer<TK, TV> : PropertyDrawer
    {
        private HashMap<TK, TV> _Dictionary;
        private bool _Foldout;
        private const float kButtonWidth = 18f;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //TODO: If we are going to support custom classes as values, we'll need to
            //somehow determine how big a space they will require here
            CheckInitialize(property, label);
            if (_Foldout)
                return (_Dictionary.Count + 1) * 17f;
            return 17f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CheckInitialize(property, label);

            position.height = 17f;

            var foldoutRect = position;
            foldoutRect.width -= 2 * kButtonWidth;
            EditorGUI.BeginChangeCheck();
            _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool(label.text, _Foldout);

            var buttonRect = position;
            buttonRect.x = position.width - kButtonWidth + position.x;
            buttonRect.width = kButtonWidth + 2;

            if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
            {
                EditorGUI.FocusTextInControl("");
                AddNewItem();
            }

            buttonRect.x -= kButtonWidth;

            //if (EditorGUI.actionKey)
            {
                if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
                {
                    ClearDictionary();
                }
            }

            if (!_Foldout)
                return;
            //bool openGroup = false;

            foreach (var item in _Dictionary)
            {
                //EditorGUILayout.BeginHorizontal();
                //openGroup = true;

                var key = item.Key;
                var value = item.Value;

                position.y += 17f;

                var keyRect = position;
                keyRect.width /= 2;
                keyRect.width -= 4;
                EditorGUI.BeginChangeCheck();


                var newKey = DoField(keyRect, typeof(TK), key);
                //var newKey = DoField(typeof(TK), key, property, GUILayout.MaxWidth(200));
                if (EditorGUI.EndChangeCheck())
                {
                    if (_Dictionary.ContainsKey(newKey))
                        Debug.Log("<color=red>Key already in use: \"" + newKey + "\"</color>");
                    else
                    {
                        try
                        {
                            _Dictionary.Remove(key);
                            _Dictionary.Add(newKey, value);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("<color=red>" + e.Message + "</color>");
                        }
                        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        break;
                    }
                }

                var valueRect = position;
                valueRect.x = position.width / 2 + 15;
                valueRect.width = keyRect.width - kButtonWidth;
                EditorGUI.BeginChangeCheck();
                value = DoField(valueRect, typeof(TV), value);
                //value = DoField(typeof(TV), value, property);
                if (EditorGUI.EndChangeCheck())
                {
                    _Dictionary[key] = value;
                    if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    break;
                }

                //if (EditorGUI.actionKey)
                {
                    var removeRect = valueRect;
                    removeRect.x = valueRect.xMax + 2;
                    removeRect.width = kButtonWidth;

                    //GUILayout.Space(8);
                    //if (GUILayout.Button(new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight, GUILayout.Width(20), GUILayout.ExpandWidth(false)))
                    if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
                    {
                        //TODO: need to de-focus textboxes when removing keys!
                        EditorGUI.FocusTextInControl("");
                        RemoveItem(key);
                        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        break;
                    }
                }

                //EditorGUILayout.EndHorizontal();
                //openGroup = false;
            }

            //if (openGroup) EditorGUILayout.EndHorizontal();
        }

        private void RemoveItem(TK key)
        {
            _Dictionary.Remove(key);
        }

        private void CheckInitialize(SerializedProperty property, GUIContent label)
        {
            if (_Dictionary == null)
            {
                //BIG BUG HERE: If we are accessing our dictionary from within a nested class and not directly from our
                //component, then this won't work at all!
                var target = property.serializedObject.targetObject;
                _Dictionary = fieldInfo.GetValue(target) as HashMap<TK, TV>;
                if (_Dictionary == null)
                {
                    _Dictionary = new HashMap<TK, TV>();
                    fieldInfo.SetValue(target, _Dictionary);
                }

                _Foldout = EditorPrefs.GetBool(label.text);
            }
        }


        private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
            new Dictionary<Type, Func<Rect, object, object>>()
            {
                { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
                { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
                { typeof(string), (rect, value) => EditorGUI.TextField(rect,  (string)value) },
                { typeof(bool), (rect, value) => EditorGUI.Toggle(rect,  (bool)value) },
                { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
                { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
                { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
                { typeof(Rect), (rect, value) => EditorGUI.RectField(rect,   (Rect)value) },
            };

        private static readonly Dictionary<Type, Func<GUILayoutOption[], object, object>> _LayoutFields =
            new Dictionary<Type, Func<GUILayoutOption[], object, object>>()
            {
                { typeof(int), (option, value) => EditorGUILayout.IntField((int)value, option) },
                { typeof(float), (option, value) => EditorGUILayout.FloatField((float)value, option) },
                { typeof(string), (option, value) => EditorGUILayout.TextField((string)value, option) },
                { typeof(bool), (option, value) => EditorGUILayout.Toggle((bool)value, option) },
                { typeof(Vector2), (option, value) => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value, option) },
                { typeof(Vector3), (option, value) => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value, option) },
                { typeof(Bounds), (option, value) => EditorGUILayout.BoundsField((Bounds)value, option) },
                { typeof(Rect), (option, value) => EditorGUILayout.RectField((Rect)value, option) },
            };

        private static T DoField<T>(Rect rect, Type type, T value)
        {
            Func<Rect, object, object> field;
            if (_Fields.TryGetValue(type, out field))
            {
                return (T)field(rect, value);
            }

            if (type.IsEnum)
                return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

            if (typeof(UnityObject).IsAssignableFrom(type))
                return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

            //TODO: obtain custom PropertyDrawers and apply them here
            AbstractSuperEditor.ComplexField(rect, new GUIContent(type.Name), value);

            //Debug.Log("Type is not supported: " + type);
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="prop"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static T DoField<T>(Type type, T value, SerializedProperty prop, params GUILayoutOption[] options)
        {
            Func<GUILayoutOption[], object, object> field;
            if (_LayoutFields.TryGetValue(type, out field))
            {
                return (T)field(options, value);
            }

            if (type.IsEnum)
                return (T)(object)EditorGUILayout.EnumPopup((Enum)(object)value);

            if (typeof(UnityObject).IsAssignableFrom(type))
                return (T)(object)EditorGUILayout.ObjectField((UnityObject)(object)value, type, true);

            AbstractSuperEditor.ComplexField(new GUIContent(type.Name), value);

            //Debug.Log("Type is not supported: " + type);
            return value;
        }

        private void ClearDictionary()
        {
            _Dictionary.Clear();
        }

        private void AddNewItem()
        {
            TK key;
            if (typeof(TK) == typeof(string))
                key = (TK)(object)Guid.NewGuid().ToString();
            else key = default(TK);

            var value = default(TV);
            try
            {
                _Dictionary.Add(key, value);
            }
            catch (Exception e)
            {
                Debug.Log("<color=red>" + e.Message + "</color>");
            }
        }
    }


    [CustomPropertyDrawer(typeof(StringIntHashMap))]
    public class StringIntHashMapDrawer : HashMapDrawer<string, int> { }

    [CustomPropertyDrawer(typeof(StringFloatHashMap))]
    public class StringFloatHashMapDrawer : HashMapDrawer<string, float> { }

    [CustomPropertyDrawer(typeof(StringStringHashMap))]
    public class StringStringHashMapDrawer : HashMapDrawer<string, string> { }

    [CustomPropertyDrawer(typeof(StringBoolHashMap))]
    public class StringBoolHashMapDrawer : HashMapDrawer<string, bool> { }


    */
}