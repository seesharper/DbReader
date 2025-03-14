namespace DbClient
{
    using System;
    using System.Data;

    /// <summary>
    /// Specifies the convert function to be used when passing an argument of type <typeparamref name="TArgument"/>.
    /// </summary>
    /// <typeparam name="TArgument">The type of argument to be converted.</typeparam>
    public class PassDelegate<TArgument>
    {
        /// <summary>
        /// Specifies the <paramref name="convertFunction"/> to be used when passing an argument of type <typeparamref name="TArgument"/>.
        /// </summary>
        /// <param name="convertFunction">The convert function to be used when passimg the argument.</param>
        public void Use(Action<IDataParameter, TArgument> convertFunction)
        {            
            ArgumentProcessor.RegisterProcessDelegate(convertFunction);
        }
    }
}