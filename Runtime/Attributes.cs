
using System;
using UnityEngine;


namespace Peg
{

    /// <summary>
    /// Apply to strings to make their inspector display
    /// a scene asset sceletion control. The property
    /// drawer for this requires that their also be
    /// a serialized backing field with the same name as
    /// the field this is applied to and a leading underscore.
    /// 
    /// Ex: [SceneName]
    ///     string Scene;
    ///     
    ///     [SerializeField]
    ///     SceneAsset _Scene;
    /// </summary>
    public class SceneNameAttribute : PropertyAttribute
    {
        public SceneNameAttribute() { }
    }

    /// <summary>
    /// Apply to types of Vector3 to make their inspector display much smaller.
    /// </summary>
    public class CompactAttribute : PropertyAttribute
    {
        public CompactAttribute() { }
    }


    /// <summary>
    /// Tells the inspector to draw the enum as a mask field rather than an enum dropdown.
    /// </summary>
    public class MaskedEnumAttribute : PropertyAttribute
    {
        public string EnumName;

        public MaskedEnumAttribute() { }

        public MaskedEnumAttribute(string name)
        {
            EnumName = name;
        }

    }
}

