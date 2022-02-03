using System.Reflection;

namespace BuildingBlocks.Messaging.Scheduling.Helpers
{
    public static class Extensions
    {
        public static System.Type GetPayloadType(this MessageSerializedObject messageSerializedObject)
        {
            if (messageSerializedObject?.AssemblyName == null)
                return null;

            var assembly = Assembly.Load(messageSerializedObject.AssemblyName);

            var type = assembly
                .GetTypes()
                .Where(t => t.FullName == messageSerializedObject.FullTypeName)
                .ToList().FirstOrDefault();
            return type;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetExecutingAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }
                }
            } while (stack.Count > 0);
        }
    }
}