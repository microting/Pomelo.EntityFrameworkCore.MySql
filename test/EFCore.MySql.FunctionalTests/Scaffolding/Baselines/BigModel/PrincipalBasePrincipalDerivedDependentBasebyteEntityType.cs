// <auto-generated />
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

#pragma warning disable 219, 612, 618
#nullable disable

namespace TestNamespace
{
    [EntityFrameworkInternal]
    public partial class PrincipalBasePrincipalDerivedDependentBasebyteEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "PrincipalBasePrincipalDerived<DependentBase<byte?>>",
                typeof(Dictionary<string, object>),
                baseEntityType,
                sharedClrType: true,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(Dictionary<string, object>)),
                propertyBag: true,
                propertyCount: 5,
                foreignKeyCount: 2,
                unnamedIndexCount: 1,
                keyCount: 1);

            var derivedsId = runtimeEntityType.AddProperty(
                "DerivedsId",
                typeof(long),
                propertyInfo: runtimeEntityType.FindIndexerPropertyInfo(),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            derivedsId.SetGetter(
                long (Dictionary<string, object> entity) => ((((IDictionary<string, object>)entity).ContainsKey("DerivedsId") ? entity["DerivedsId"] : null) == null ? 0L : ((long)((((IDictionary<string, object>)entity).ContainsKey("DerivedsId") ? entity["DerivedsId"] : null)))),
                bool (Dictionary<string, object> entity) => (((IDictionary<string, object>)entity).ContainsKey("DerivedsId") ? entity["DerivedsId"] : null) == null,
                long (Dictionary<string, object> instance) => ((((IDictionary<string, object>)instance).ContainsKey("DerivedsId") ? instance["DerivedsId"] : null) == null ? 0L : ((long)((((IDictionary<string, object>)instance).ContainsKey("DerivedsId") ? instance["DerivedsId"] : null)))),
                bool (Dictionary<string, object> instance) => (((IDictionary<string, object>)instance).ContainsKey("DerivedsId") ? instance["DerivedsId"] : null) == null);
            derivedsId.SetSetter(
                (Dictionary<string, object> entity, long value) => entity["DerivedsId"] = ((object)(value)));
            derivedsId.SetMaterializationSetter(
                (Dictionary<string, object> entity, long value) => entity["DerivedsId"] = ((object)(value)));
            derivedsId.SetAccessors(
                long (InternalEntityEntry entry) =>
                {
                    if (entry.FlaggedAsStoreGenerated(0))
                    {
                        return entry.ReadStoreGeneratedValue<long>(0);
                    }
                    else
                    {
                        {
                            if (entry.FlaggedAsTemporary(0) && (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsId"] : null) == null)
                            {
                                return entry.ReadTemporaryValue<long>(0);
                            }
                            else
                            {
                                var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsId"] : null);
                                return (nullableValue == null ? default(long) : ((long)(nullableValue)));
                            }
                        }
                    }
                },
                long (InternalEntityEntry entry) =>
                {
                    var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsId"] : null);
                    return (nullableValue == null ? default(long) : ((long)(nullableValue)));
                },
                long (InternalEntityEntry entry) => entry.ReadOriginalValue<long>(derivedsId, 0),
                long (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<long>(derivedsId, 0),
                object (ValueBuffer valueBuffer) => valueBuffer[0]);
            derivedsId.SetPropertyIndexes(
                index: 0,
                originalValueIndex: 0,
                shadowIndex: -1,
                relationshipIndex: 0,
                storeGenerationIndex: 0);
            derivedsId.TypeMapping = MySqlLongTypeMapping.Default.Clone(
                comparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v),
                keyComparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v),
                providerValueComparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v));
            derivedsId.SetCurrentValueComparer(new EntryCurrentValueComparer<long>(derivedsId));
            derivedsId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var derivedsAlternateId = runtimeEntityType.AddProperty(
                "DerivedsAlternateId",
                typeof(Guid),
                propertyInfo: runtimeEntityType.FindIndexerPropertyInfo(),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            derivedsAlternateId.SetGetter(
                Guid (Dictionary<string, object> entity) => ((((IDictionary<string, object>)entity).ContainsKey("DerivedsAlternateId") ? entity["DerivedsAlternateId"] : null) == null ? new Guid("00000000-0000-0000-0000-000000000000") : ((Guid)((((IDictionary<string, object>)entity).ContainsKey("DerivedsAlternateId") ? entity["DerivedsAlternateId"] : null)))),
                bool (Dictionary<string, object> entity) => (((IDictionary<string, object>)entity).ContainsKey("DerivedsAlternateId") ? entity["DerivedsAlternateId"] : null) == null,
                Guid (Dictionary<string, object> instance) => ((((IDictionary<string, object>)instance).ContainsKey("DerivedsAlternateId") ? instance["DerivedsAlternateId"] : null) == null ? new Guid("00000000-0000-0000-0000-000000000000") : ((Guid)((((IDictionary<string, object>)instance).ContainsKey("DerivedsAlternateId") ? instance["DerivedsAlternateId"] : null)))),
                bool (Dictionary<string, object> instance) => (((IDictionary<string, object>)instance).ContainsKey("DerivedsAlternateId") ? instance["DerivedsAlternateId"] : null) == null);
            derivedsAlternateId.SetSetter(
                (Dictionary<string, object> entity, Guid value) => entity["DerivedsAlternateId"] = ((object)(value)));
            derivedsAlternateId.SetMaterializationSetter(
                (Dictionary<string, object> entity, Guid value) => entity["DerivedsAlternateId"] = ((object)(value)));
            derivedsAlternateId.SetAccessors(
                Guid (InternalEntityEntry entry) =>
                {
                    if (entry.FlaggedAsStoreGenerated(1))
                    {
                        return entry.ReadStoreGeneratedValue<Guid>(1);
                    }
                    else
                    {
                        {
                            if (entry.FlaggedAsTemporary(1) && (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsAlternateId"] : null) == null)
                            {
                                return entry.ReadTemporaryValue<Guid>(1);
                            }
                            else
                            {
                                var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsAlternateId"] : null);
                                return (nullableValue == null ? default(Guid) : ((Guid)(nullableValue)));
                            }
                        }
                    }
                },
                Guid (InternalEntityEntry entry) =>
                {
                    var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("DerivedsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["DerivedsAlternateId"] : null);
                    return (nullableValue == null ? default(Guid) : ((Guid)(nullableValue)));
                },
                Guid (InternalEntityEntry entry) => entry.ReadOriginalValue<Guid>(derivedsAlternateId, 1),
                Guid (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<Guid>(derivedsAlternateId, 1),
                object (ValueBuffer valueBuffer) => valueBuffer[1]);
            derivedsAlternateId.SetPropertyIndexes(
                index: 1,
                originalValueIndex: 1,
                shadowIndex: -1,
                relationshipIndex: 1,
                storeGenerationIndex: 1);
            derivedsAlternateId.TypeMapping = MySqlGuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v),
                keyComparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v),
                providerValueComparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v));
            derivedsAlternateId.SetCurrentValueComparer(new EntryCurrentValueComparer<Guid>(derivedsAlternateId));
            derivedsAlternateId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var principalsId = runtimeEntityType.AddProperty(
                "PrincipalsId",
                typeof(long),
                propertyInfo: runtimeEntityType.FindIndexerPropertyInfo(),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            principalsId.SetGetter(
                long (Dictionary<string, object> entity) => ((((IDictionary<string, object>)entity).ContainsKey("PrincipalsId") ? entity["PrincipalsId"] : null) == null ? 0L : ((long)((((IDictionary<string, object>)entity).ContainsKey("PrincipalsId") ? entity["PrincipalsId"] : null)))),
                bool (Dictionary<string, object> entity) => (((IDictionary<string, object>)entity).ContainsKey("PrincipalsId") ? entity["PrincipalsId"] : null) == null,
                long (Dictionary<string, object> instance) => ((((IDictionary<string, object>)instance).ContainsKey("PrincipalsId") ? instance["PrincipalsId"] : null) == null ? 0L : ((long)((((IDictionary<string, object>)instance).ContainsKey("PrincipalsId") ? instance["PrincipalsId"] : null)))),
                bool (Dictionary<string, object> instance) => (((IDictionary<string, object>)instance).ContainsKey("PrincipalsId") ? instance["PrincipalsId"] : null) == null);
            principalsId.SetSetter(
                (Dictionary<string, object> entity, long value) => entity["PrincipalsId"] = ((object)(value)));
            principalsId.SetMaterializationSetter(
                (Dictionary<string, object> entity, long value) => entity["PrincipalsId"] = ((object)(value)));
            principalsId.SetAccessors(
                long (InternalEntityEntry entry) =>
                {
                    if (entry.FlaggedAsStoreGenerated(2))
                    {
                        return entry.ReadStoreGeneratedValue<long>(2);
                    }
                    else
                    {
                        {
                            if (entry.FlaggedAsTemporary(2) && (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsId"] : null) == null)
                            {
                                return entry.ReadTemporaryValue<long>(2);
                            }
                            else
                            {
                                var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsId"] : null);
                                return (nullableValue == null ? default(long) : ((long)(nullableValue)));
                            }
                        }
                    }
                },
                long (InternalEntityEntry entry) =>
                {
                    var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsId"] : null);
                    return (nullableValue == null ? default(long) : ((long)(nullableValue)));
                },
                long (InternalEntityEntry entry) => entry.ReadOriginalValue<long>(principalsId, 2),
                long (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<long>(principalsId, 2),
                object (ValueBuffer valueBuffer) => valueBuffer[2]);
            principalsId.SetPropertyIndexes(
                index: 2,
                originalValueIndex: 2,
                shadowIndex: -1,
                relationshipIndex: 2,
                storeGenerationIndex: 2);
            principalsId.TypeMapping = MySqlLongTypeMapping.Default.Clone(
                comparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v),
                keyComparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v),
                providerValueComparer: new ValueComparer<long>(
                    bool (long v1, long v2) => v1 == v2,
                    int (long v) => ((object)v).GetHashCode(),
                    long (long v) => v));
            principalsId.SetCurrentValueComparer(new EntryCurrentValueComparer<long>(principalsId));
            principalsId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var principalsAlternateId = runtimeEntityType.AddProperty(
                "PrincipalsAlternateId",
                typeof(Guid),
                propertyInfo: runtimeEntityType.FindIndexerPropertyInfo(),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            principalsAlternateId.SetGetter(
                Guid (Dictionary<string, object> entity) => ((((IDictionary<string, object>)entity).ContainsKey("PrincipalsAlternateId") ? entity["PrincipalsAlternateId"] : null) == null ? new Guid("00000000-0000-0000-0000-000000000000") : ((Guid)((((IDictionary<string, object>)entity).ContainsKey("PrincipalsAlternateId") ? entity["PrincipalsAlternateId"] : null)))),
                bool (Dictionary<string, object> entity) => (((IDictionary<string, object>)entity).ContainsKey("PrincipalsAlternateId") ? entity["PrincipalsAlternateId"] : null) == null,
                Guid (Dictionary<string, object> instance) => ((((IDictionary<string, object>)instance).ContainsKey("PrincipalsAlternateId") ? instance["PrincipalsAlternateId"] : null) == null ? new Guid("00000000-0000-0000-0000-000000000000") : ((Guid)((((IDictionary<string, object>)instance).ContainsKey("PrincipalsAlternateId") ? instance["PrincipalsAlternateId"] : null)))),
                bool (Dictionary<string, object> instance) => (((IDictionary<string, object>)instance).ContainsKey("PrincipalsAlternateId") ? instance["PrincipalsAlternateId"] : null) == null);
            principalsAlternateId.SetSetter(
                (Dictionary<string, object> entity, Guid value) => entity["PrincipalsAlternateId"] = ((object)(value)));
            principalsAlternateId.SetMaterializationSetter(
                (Dictionary<string, object> entity, Guid value) => entity["PrincipalsAlternateId"] = ((object)(value)));
            principalsAlternateId.SetAccessors(
                Guid (InternalEntityEntry entry) =>
                {
                    if (entry.FlaggedAsStoreGenerated(3))
                    {
                        return entry.ReadStoreGeneratedValue<Guid>(3);
                    }
                    else
                    {
                        {
                            if (entry.FlaggedAsTemporary(3) && (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsAlternateId"] : null) == null)
                            {
                                return entry.ReadTemporaryValue<Guid>(3);
                            }
                            else
                            {
                                var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsAlternateId"] : null);
                                return (nullableValue == null ? default(Guid) : ((Guid)(nullableValue)));
                            }
                        }
                    }
                },
                Guid (InternalEntityEntry entry) =>
                {
                    var nullableValue = (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("PrincipalsAlternateId") ? ((Dictionary<string, object>)(entry.Entity))["PrincipalsAlternateId"] : null);
                    return (nullableValue == null ? default(Guid) : ((Guid)(nullableValue)));
                },
                Guid (InternalEntityEntry entry) => entry.ReadOriginalValue<Guid>(principalsAlternateId, 3),
                Guid (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<Guid>(principalsAlternateId, 3),
                object (ValueBuffer valueBuffer) => valueBuffer[3]);
            principalsAlternateId.SetPropertyIndexes(
                index: 3,
                originalValueIndex: 3,
                shadowIndex: -1,
                relationshipIndex: 3,
                storeGenerationIndex: 3);
            principalsAlternateId.TypeMapping = MySqlGuidTypeMapping.Default.Clone(
                comparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v),
                keyComparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v),
                providerValueComparer: new ValueComparer<Guid>(
                    bool (Guid v1, Guid v2) => v1 == v2,
                    int (Guid v) => ((object)v).GetHashCode(),
                    Guid (Guid v) => v));
            principalsAlternateId.SetCurrentValueComparer(new EntryCurrentValueComparer<Guid>(principalsAlternateId));
            principalsAlternateId.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var rowid = runtimeEntityType.AddProperty(
                "rowid",
                typeof(byte[]),
                propertyInfo: runtimeEntityType.FindIndexerPropertyInfo(),
                nullable: true,
                concurrencyToken: true,
                valueGenerated: ValueGenerated.OnAddOrUpdate,
                beforeSaveBehavior: PropertySaveBehavior.Ignore,
                afterSaveBehavior: PropertySaveBehavior.Ignore);
            rowid.SetGetter(
                byte[] (Dictionary<string, object> entity) => ((((IDictionary<string, object>)entity).ContainsKey("rowid") ? entity["rowid"] : null) == null ? null : ((byte[])((((IDictionary<string, object>)entity).ContainsKey("rowid") ? entity["rowid"] : null)))),
                bool (Dictionary<string, object> entity) => (((IDictionary<string, object>)entity).ContainsKey("rowid") ? entity["rowid"] : null) == null,
                byte[] (Dictionary<string, object> instance) => ((((IDictionary<string, object>)instance).ContainsKey("rowid") ? instance["rowid"] : null) == null ? null : ((byte[])((((IDictionary<string, object>)instance).ContainsKey("rowid") ? instance["rowid"] : null)))),
                bool (Dictionary<string, object> instance) => (((IDictionary<string, object>)instance).ContainsKey("rowid") ? instance["rowid"] : null) == null);
            rowid.SetSetter(
                (Dictionary<string, object> entity, byte[] value) => entity["rowid"] = ((object)(value)));
            rowid.SetMaterializationSetter(
                (Dictionary<string, object> entity, byte[] value) => entity["rowid"] = ((object)(value)));
            rowid.SetAccessors(
                byte[] (InternalEntityEntry entry) => (entry.FlaggedAsStoreGenerated(4) ? entry.ReadStoreGeneratedValue<byte[]>(4) : (entry.FlaggedAsTemporary(4) && (((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("rowid") ? ((Dictionary<string, object>)(entry.Entity))["rowid"] : null) == null ? entry.ReadTemporaryValue<byte[]>(4) : ((byte[])((((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("rowid") ? ((Dictionary<string, object>)(entry.Entity))["rowid"] : null))))),
                byte[] (InternalEntityEntry entry) => ((byte[])((((IDictionary<string, object>)((Dictionary<string, object>)(entry.Entity))).ContainsKey("rowid") ? ((Dictionary<string, object>)(entry.Entity))["rowid"] : null))),
                byte[] (InternalEntityEntry entry) => entry.ReadOriginalValue<byte[]>(rowid, 4),
                byte[] (InternalEntityEntry entry) => entry.GetCurrentValue<byte[]>(rowid),
                object (ValueBuffer valueBuffer) => valueBuffer[4]);
            rowid.SetPropertyIndexes(
                index: 4,
                originalValueIndex: 4,
                shadowIndex: -1,
                relationshipIndex: -1,
                storeGenerationIndex: 4);
            rowid.TypeMapping = MySqlDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<byte[]>(
                    bool (byte[] v1, byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals(v1, v2),
                    int (byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(v),
                    byte[] (byte[] v) => (v == null ? null : v.ToArray())),
                keyComparer: new ValueComparer<byte[]>(
                    bool (byte[] v1, byte[] v2) => StructuralComparisons.StructuralEqualityComparer.Equals(((object)(v1)), ((object)(v2))),
                    int (byte[] v) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(((object)(v))),
                    byte[] (byte[] source) => source.ToArray()),
                providerValueComparer: new ValueComparer<DateTime>(
                    bool (DateTime v1, DateTime v2) => v1.Equals(v2),
                    int (DateTime v) => ((object)v).GetHashCode(),
                    DateTime (DateTime v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "timestamp(6)",
                    precision: 6),
                converter: new ValueConverter<byte[], DateTime>(
                    DateTime (byte[] v) => BytesToDateTimeConverter.FromBytes(v),
                    byte[] (DateTime v) => BytesToDateTimeConverter.ToBytes(v)));
            rowid.AddAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { derivedsId, derivedsAlternateId, principalsId, principalsAlternateId });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { principalsId, principalsAlternateId });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("DerivedsId"), declaringEntityType.FindProperty("DerivedsAlternateId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id"), principalEntityType.FindProperty("AlternateId") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PrincipalsId"), declaringEntityType.FindProperty("PrincipalsAlternateId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id"), principalEntityType.FindProperty("AlternateId") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PrincipalBasePrincipalDerived<DependentBase<byte?>>_Princip~1");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            var derivedsId = runtimeEntityType.FindProperty("DerivedsId");
            var derivedsAlternateId = runtimeEntityType.FindProperty("DerivedsAlternateId");
            var principalsId = runtimeEntityType.FindProperty("PrincipalsId");
            var principalsAlternateId = runtimeEntityType.FindProperty("PrincipalsAlternateId");
            var rowid = runtimeEntityType.FindProperty("rowid");
            var key = runtimeEntityType.FindKey(new[] { derivedsId, derivedsAlternateId, principalsId, principalsAlternateId });
            key.SetPrincipalKeyValueFactory(KeyValueFactoryFactory.CreateCompositeFactory(key));
            key.SetIdentityMapFactory(IdentityMapFactoryFactory.CreateFactory<IReadOnlyList<object>>(key));
            runtimeEntityType.SetOriginalValuesFactory(
                ISnapshot (InternalEntityEntry source) =>
                {
                    var entity8 = ((Dictionary<string, object>)(source.Entity));
                    return ((ISnapshot)(new Snapshot<long, Guid, long, Guid, byte[]>(((ValueComparer<long>)(((IProperty)derivedsId).GetValueComparer())).Snapshot(source.GetCurrentValue<long>(derivedsId)), ((ValueComparer<Guid>)(((IProperty)derivedsAlternateId).GetValueComparer())).Snapshot(source.GetCurrentValue<Guid>(derivedsAlternateId)), ((ValueComparer<long>)(((IProperty)principalsId).GetValueComparer())).Snapshot(source.GetCurrentValue<long>(principalsId)), ((ValueComparer<Guid>)(((IProperty)principalsAlternateId).GetValueComparer())).Snapshot(source.GetCurrentValue<Guid>(principalsAlternateId)), (source.GetCurrentValue<byte[]>(rowid) == null ? null : ((ValueComparer<byte[]>)(((IProperty)rowid).GetValueComparer())).Snapshot(source.GetCurrentValue<byte[]>(rowid))))));
                });
            runtimeEntityType.SetStoreGeneratedValuesFactory(
                ISnapshot () => ((ISnapshot)(new Snapshot<long, Guid, long, Guid, byte[]>(((ValueComparer<long>)(((IProperty)derivedsId).GetValueComparer())).Snapshot(default(long)), ((ValueComparer<Guid>)(((IProperty)derivedsAlternateId).GetValueComparer())).Snapshot(default(Guid)), ((ValueComparer<long>)(((IProperty)principalsId).GetValueComparer())).Snapshot(default(long)), ((ValueComparer<Guid>)(((IProperty)principalsAlternateId).GetValueComparer())).Snapshot(default(Guid)), (default(byte[]) == null ? null : ((ValueComparer<byte[]>)(((IProperty)rowid).GetValueComparer())).Snapshot(default(byte[])))))));
            runtimeEntityType.SetTemporaryValuesFactory(
                ISnapshot (InternalEntityEntry source) => ((ISnapshot)(new Snapshot<long, Guid, long, Guid, byte[]>(default(long), default(Guid), default(long), default(Guid), default(byte[])))));
            runtimeEntityType.SetShadowValuesFactory(
                ISnapshot (IDictionary<string, object> source) => Snapshot.Empty);
            runtimeEntityType.SetEmptyShadowValuesFactory(
                ISnapshot () => Snapshot.Empty);
            runtimeEntityType.SetRelationshipSnapshotFactory(
                ISnapshot (InternalEntityEntry source) =>
                {
                    var entity8 = ((Dictionary<string, object>)(source.Entity));
                    return ((ISnapshot)(new Snapshot<long, Guid, long, Guid>(((ValueComparer<long>)(((IProperty)derivedsId).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<long>(derivedsId)), ((ValueComparer<Guid>)(((IProperty)derivedsAlternateId).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<Guid>(derivedsAlternateId)), ((ValueComparer<long>)(((IProperty)principalsId).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<long>(principalsId)), ((ValueComparer<Guid>)(((IProperty)principalsAlternateId).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<Guid>(principalsAlternateId)))));
                });
            runtimeEntityType.Counts = new PropertyCounts(
                propertyCount: 5,
                navigationCount: 0,
                complexPropertyCount: 0,
                originalValueCount: 5,
                shadowCount: 0,
                relationshipCount: 4,
                storeGeneratedCount: 5);
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "PrincipalBasePrincipalDerived<DependentBase<byte?>>");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
