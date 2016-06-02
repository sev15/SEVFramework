﻿using Microsoft.Practices.ServiceLocation;

namespace SEV.DI
{
    public sealed class DIConfiguration
    {
        private readonly IDIContainerFactory m_containerFactory;

        public static DIConfiguration Create(IDIContainerFactory factory)
        {
            return new DIConfiguration(factory);
        }

        private DIConfiguration(IDIContainerFactory factory)
        {
            m_containerFactory = factory;
        }

        public IDIContainer CreateDIContainer()
        {
            return m_containerFactory.CreateContainer();
        }

        public IServiceLocator CreateServiceLocator(IDIContainer container)
        {
            return m_containerFactory.CreateServiceLocator(container);
        }
    }
}