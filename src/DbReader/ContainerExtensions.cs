//namespace DbReader
//{
//    using System;
//    using System.Linq;
//    using System.Reflection;
//    using LightInject;

//    public static class ContainerExtensions
//    {        
//        internal static void Validate(this ServiceContainer serviceContainer, Action<string> warnAction = null)
//        {
//            if (warnAction == null)
//            {
//                warnAction = s => { };
//            }
            

//            //// Try to resolve all services
//            //foreach (var serviceRegistration in serviceContainer.AvailableServices)
//            //{
//            //    serviceContainer.GetInstance(serviceRegistration.ServiceType, serviceRegistration.ServiceName);
//            //}


//            var serviceMap =
//                serviceContainer.AvailableServices.ToDictionary(sr => Tuple.Create(sr.ServiceType, sr.ServiceName));

//            foreach (var serviceRegistration in serviceContainer.AvailableServices)
//            {
//                if (serviceRegistration.ImplementingType != null)
//                {
//                    var constructor =
//                        serviceRegistration.ImplementingType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
//                    var parameters = constructor.GetParameters();
//                    foreach (var parameter in parameters)
//                    {
//                        ServiceRegistration dependency;
//                        if (serviceMap.TryGetValue(Tuple.Create(parameter.ParameterType, string.Empty), out dependency))
//                        {
//                            // Validate captive dependency
//                            if (serviceRegistration.Lifetime is PerContainerLifetime)
//                            {
//                                if (dependency.Lifetime == null || dependency.Lifetime is PerScopeLifetime || dependency.Lifetime is PerRequestLifeTime)
//                                {
//                                    warnAction(
//                                        $"The dependency {dependency} is being injected into {serviceRegistration} that has a longer lifetime");
//                                }
//                            }

//                            if (serviceRegistration.Lifetime is PerScopeLifetime)
//                            {
//                                if (dependency.Lifetime == null || dependency.Lifetime is PerRequestLifeTime)
//                                {
//                                    warnAction(
//                                        $"The dependency {dependency} is being injected into {serviceRegistration} that has a longer lifetime");
//                                }
//                            }
//                        };
//                    }
//                }    
//            }            

//        }
//    }
//}