using ftrip.io.framework.Domain;
using ftrip.io.framework.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ftrip.io.user_service.integration_tests.Seeding
{
    public class Seeder
    {
        private readonly List<object> _seededObjects = new List<object>();
        private readonly Type _type;
        private readonly DbContext _context;

        public Seeder(Type type, DbContext context)
        {
            _context = context;
            _type = type;
        }

        public async Task SeedAsync()
        {
            var infoOfObjects = GetInfoOfObjectsForSeed();

            while (infoOfObjects.Keys.Any())
            {
                foreach (var typeOfObjects in infoOfObjects.Keys)
                {
                    try
                    {
                        TryToInitializeObjects(typeOfObjects);

                        var seededObjectOfSpecificType = infoOfObjects[typeOfObjects]
                            .Select(fieldinfoOfObject => fieldinfoOfObject.GetValue(null));

                        await _context.AddRangeAsync(seededObjectOfSpecificType);
                        await _context.SaveChangesAsync();

                        _seededObjects.AddRange(seededObjectOfSpecificType);
                        infoOfObjects.Remove(typeOfObjects);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private DefaultDictionary<Type, List<FieldInfo>> GetInfoOfObjectsForSeed()
        {
            var infoOfObjects = new DefaultDictionary<Type, List<FieldInfo>>();

            _type.GetFields()
                .Where(fieldInfoOfObject =>
                {
                    var isGuidIdEntity = typeof(Entity<Guid>).IsAssignableFrom(fieldInfoOfObject.FieldType);
                    var isStringIdEntity = typeof(Entity<string>).IsAssignableFrom(fieldInfoOfObject.FieldType);
                    return isGuidIdEntity || isStringIdEntity;
                })
                .ToList()
                .ForEach(fieldInfoOfObject =>
                {
                    infoOfObjects[fieldInfoOfObject.FieldType].Add(fieldInfoOfObject);
                });

            return infoOfObjects;
        }

        private void TryToInitializeObjects(Type typeOfObjects)
        {
            TryToInitializeObjectsUsingMethod(typeOfObjects);
            TryToInitializeObjectsUsingAttribute(typeOfObjects);
        }

        private void TryToInitializeObjectsUsingMethod(Type typeOfObjects)
        {
            var initializeMethod = _type.GetMethod($"Initialize{typeOfObjects.Name}s");
            initializeMethod?.Invoke(null, null);
        }

        private void TryToInitializeObjectsUsingAttribute(Type typeOfObjects)
        {
            foreach (var method in _type.GetMethods())
            {
                var initializeForAttribute = method.GetCustomAttribute(typeof(InitializerForAttribute)) as InitializerForAttribute;
                if (initializeForAttribute == null || initializeForAttribute.InitializerFor != typeOfObjects)
                {
                    continue;
                }

                method.Invoke(null, null);
            }
        }

        public async Task UnseedAsync()
        {
            _context.RemoveRange(_seededObjects);
            await _context.SaveChangesAsync();
        }
    }
}