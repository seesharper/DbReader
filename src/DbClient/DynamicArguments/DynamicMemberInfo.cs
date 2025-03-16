using System;

namespace DbClient.DynamicArguments
{
    /// <summary>
    /// Represents metadata about a dynamic member.
    /// </summary>
    public class DynamicMemberInfo
    {
        /// <summary>
        /// The name of the member
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The type of the member.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMemberInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member</param>
        public DynamicMemberInfo(string name, Type type)
        {
            this.Name = name;
            this.Type = type; ;
        }

        ///<inheritdoc/>
        public bool Equals(DynamicMemberInfo other)
        {
            return (Name, Type).Equals((other.Name, other.Type));
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            return (Name, Type).GetHashCode();
        }

        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as DynamicMemberInfo);
        }
    }

    /// <summary>
    /// Represents the metadata and the value of a dynamic member.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicMemberInfo<T> : DynamicMemberInfo
    {
        /// <summary>
        /// The value of the dynamic member.
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMemberInfo{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the member.</param>
        /// <param name="value">The value of the dynamic member.</param>
        /// <returns></returns>
        public DynamicMemberInfo(string name, T value) : base(name, typeof(T))
        {
            this.Value = value;
        }
    }
}
