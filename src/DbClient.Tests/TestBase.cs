using System;
using System.Reflection;
using DbClient.LightInject;

namespace DbClient.Tests
{
    public class ContainerFixture : IDisposable
    {
        public ContainerFixture()
        {
            var container = CreateContainer();
            container.RegisterFrom<CompositionRoot>();
            Configure(container);
            ServiceFactory = container.BeginScope();
            InjectPrivateFields();
        }

        private void InjectPrivateFields()
        {
            var privateInstanceFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var privateInstanceField in privateInstanceFields)
            {
                privateInstanceField.SetValue(this, GetInstance(ServiceFactory, privateInstanceField));
            }
        }

        internal Scope ServiceFactory { get; }

        public void Dispose() => ServiceFactory.Dispose();

        public TService GetInstance<TService>(string name = "")
            => ServiceFactory.GetInstance<TService>(name);

        private object GetInstance(IServiceFactory factory, FieldInfo field)
            => ServiceFactory.TryGetInstance(field.FieldType) ?? ServiceFactory.GetInstance(field.FieldType, field.Name);

        internal virtual IServiceContainer CreateContainer() => new ServiceContainer();

        internal virtual void Configure(IServiceRegistry serviceRegistry) {}
    }
}