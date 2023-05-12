using System;

namespace ftrip.io.user_service.integration_tests.Seeding
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class InitializerForAttribute : Attribute
    {
        public Type InitializerFor { get; set; }

        public InitializerForAttribute(Type initializerFor)
        {
            InitializerFor = initializerFor;
        }
    }
}