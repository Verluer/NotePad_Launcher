using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// Время жизни сервиса. По умолчанию Transient.
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Регистрировать ли как сам класс (без интерфейса (View, VM).
        /// </summary>
        public bool AsSelf { get; }

        /// <summary>
        /// Конкретный тип интерфейса для регистрации.
        /// Если не указан, то сканер возьмёт первый интерфейс или все (если RegisterAllInterfaces = true).
        /// </summary>
        public Type? ServiceType { get; }

        /// <summary>
        /// Регистрировать все интерфейсы, реализованные классом.
        /// Игнорируется, если указан ServiceType.
        /// </summary>
        public bool RegisterAllInterfaces { get; }

        public RegisterServiceAttribute(
            ServiceLifetime lifetime = ServiceLifetime.Transient,
            bool asSelf = false,
            Type? serviceType = null,
            bool registerAllInterfaces = false)
        {
            Lifetime = lifetime;
            AsSelf = asSelf;
            ServiceType = serviceType;
            RegisterAllInterfaces = registerAllInterfaces;
        }
    }
}
