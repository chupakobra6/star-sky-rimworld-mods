using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using HarmonyLib;
using Verse;

namespace Igor.RU_2969748433
{
    // Configure Lunar's existing translation supplier for these eight literal
    // operands. The original operations, other fields and dynamic specs stay native.
    [StaticConstructorOnStartup]
    internal static class BiomeVariantTranslations
    {
        internal const string Owner = "biomesteam.biomescaverns";
        internal const string GeologicalSHA = "29abc97081aff0e3c9cb7701c9c0a4972be327c7e8d6f23d47f6e106df768ba3";
        internal const string LunarSHA = "556c36c1111941cd032703a8e5e48be37568815fde40c6642290c52e8127bfda";
        internal const string LunarMvid = "39a8c288-d61b-4ee6-9073-71844c9d6f2f";
        internal const string LunarLoaderSHA = "3700ae08ed5bc8a24e11ac26eec71c718b03420ce3248aec81c16857906b99f3";
        internal sealed class Entry
        {
            internal readonly string DefName, Field, Operation, English, Key, Source, SHA;
            internal readonly bool Alpha;
            internal Entry(string name, string field, string operation, string english, string source, string sha, bool alpha = false)
            {
                DefName=name; Field=field; Operation=operation; English=english; Source=source; SHA=sha; Alpha=alpha;
                Key="chupakobra6_2969748433_BiomeVariant_"+name+"_"+field;
            }
        }
        internal static readonly Entry[] Entries = {
            new Entry("BMT_ShallowCave", "biomeLabel", "prependPar", "shallow cave", "1.6/Defs/BiomeVariants/BMT_ShallowCave.xml", "961a9a022f354544cc71bc061c9df666cb7903029351a5ecc99724e31242e30e"),
            new Entry("BMT_ShallowCave", "biomeDescription", "replace", "A shallow system of caves close to or connecting to the surface world. This proximity to the surface has led to a fascinating mix of life from both the surface and the UnderRim. While these caverns sometimes connect safely via tunnels, they are occasionally only accessible via sinkhole or crevasse.", "1.6/Defs/BiomeVariants/BMT_ShallowCave.xml", "961a9a022f354544cc71bc061c9df666cb7903029351a5ecc99724e31242e30e"),
            new Entry("BMT_GlacialHollows", "biomeLabel", "replace", "glacial hollows", "1.6/Defs/BiomeVariants/BMT_GlacialHollows.xml", "5e096239f3865731c3565cfcfb65eb6508b8bf0bc2101ede7f2f7e50b0375100"),
            new Entry("BMT_GlacialHollows", "biomeDescription", "replace", "Sitting within the sometimes kilometers thick ice of permanently frozen regions, these hollows have never seen warmth. Nearly every surface is made of solid ice, so dense and cold that they are impervious to heat, with any warmth quickly drawn away. Little life can survive here, but what does is unsurprisingly quite hardy.", "1.6/Defs/BiomeVariants/BMT_GlacialHollows.xml", "5e096239f3865731c3565cfcfb65eb6508b8bf0bc2101ede7f2f7e50b0375100"),
            new Entry("BMT_DesertShallows", "biomeLabel", "replace", "desert shallows", "1.6/Defs/BiomeVariants/BMT_DesertShallows.xml", "8a84dbca1560bb0aa6f5fec170a641836cf3e5d45521ace28950f929a8e6aa51"),
            new Entry("BMT_DesertShallows", "biomeDescription", "replace", "Water and weathering have carved out branching networks of caverns beneath the sand and stone of the desert. With the searing hot sun unable to reach, the subterranean nature of this environment lends itself to cooler living for anything within.", "1.6/Defs/BiomeVariants/BMT_DesertShallows.xml", "8a84dbca1560bb0aa6f5fec170a641836cf3e5d45521ace28950f929a8e6aa51"),
            new Entry("BMT_ScorchedPyroducts", "biomeLabel", "replace", "scorched pyroducts", "1.6/Mods/AlphaBiomes/Defs/BiomeVariants/BMT_ScorchedPyroducts.xml", "41c8acb52d24ed0f4439ecec532e50b8b820301b656460320ce15e55d17ab282", true),
            new Entry("BMT_ScorchedPyroducts", "biomeDescription", "replace", "A molten landscape unfolds on the surface, marked by liquid lava and active calderas. The dormant lava tubes leading to the depths unveil a hidden subterranean world.", "1.6/Mods/AlphaBiomes/Defs/BiomeVariants/BMT_ScorchedPyroducts.xml", "41c8acb52d24ed0f4439ecec532e50b8b820301b656460320ce15e55d17ab282", true)
        };
        private sealed class Applied
        {
            internal Entry Row;
            internal Def Def;
            internal FieldInfo Field;
            internal object Wrapper;
        }
        private static List<Applied> applied = new List<Applied>();
        internal static string InstallStatus = "not-applied: startup pending";
        static BiomeVariantTranslations() { LongEventHandler.ExecuteWhenFinished(Install); }
        private static void Install()
        {
            try
            {
                var owner = LoadedModManager.RunningModsListForReading.Single(m=>m.PackageId==Owner);
                var type = AccessTools.TypeByName("GeologicalLandforms.Defs.BiomeVariantDef");
                if(type==null) throw new TypeLoadException("Reviewed Geological BiomeVariantDef is unavailable");
                Apply(GenDefDatabase.GetAllDefsInDatabaseForDef(type).Cast<Def>(), owner,
                    LoadedModManager.RunningModsListForReading.Any(m=>m.PackageId=="sarg.alphabiomes"));
            }
            catch(Exception error) when(ExpectedContractFailure(error))
            {
                InstallStatus="not-applied: "+error.GetBaseException().Message;
                Log.Warning("[RU-2969748433] Biome variant translation "+InstallStatus+"; native wrappers retained");
            }
        }
        private static bool ExpectedContractFailure(Exception error) => error is InvalidOperationException || error is TypeLoadException
            || error is MissingMemberException || error is IOException || error is UnauthorizedAccessException || error is XmlException
            || error is TargetInvocationException && error.InnerException!=null && ExpectedContractFailure(error.InnerException);
        internal static void RequireHash(string path, string expected)
        {
            if(string.IsNullOrEmpty(path) || !Path.IsPathRooted(path))
                throw new InvalidOperationException("Reviewed Caverns translation input has no physical path");
            string actual;
            using(var stream=File.OpenRead(path))
            using(var hash=SHA256.Create()) actual=BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            if(actual!=expected) throw new InvalidOperationException("Reviewed Caverns translation input changed: "+path);
        }
        // Current LunarLoader loads these exact bytes through AppDomain.Load.
        // A blank Location is accepted only for its observed active owner and
        // actual Assembly reference, never by a matching assembly name alone.
        internal static void RequireLunarAssembly(Assembly assembly)
        {
            if(assembly==null || assembly.IsDynamic || assembly.FullName!="LunarFramework, Version=1.1.17.0, Culture=neutral, PublicKeyToken=null"
                || assembly.GetModules().Length!=1 || assembly.ManifestModule.ModuleVersionId.ToString("D")!=LunarMvid)
                throw new InvalidOperationException("Reviewed Lunar loaded module identity changed");
            if(!string.IsNullOrEmpty(assembly.Location)) { RequireHash(assembly.Location,LunarSHA); return; }
            var owners=LoadedModManager.RunningModsListForReading.Where(m=>m.assemblies!=null && m.assemblies.loadedAssemblies!=null
                && m.assemblies.loadedAssemblies.Any(a=>ReferenceEquals(a,assembly))).ToArray();
            if(owners.Length!=1 || owners[0].PackageId!="m00nl1ght.geologicallandforms")
                throw new InvalidOperationException("Reviewed byte-loaded Lunar has no unique active Geological owner");
            var owner=owners[0];
            var loaded=AppDomain.CurrentDomain.GetAssemblies();
            var loaders=owner.assemblies.loadedAssemblies.Where(a=>!a.IsDynamic && loaded.Any(b=>ReferenceEquals(a,b))
                && a.FullName=="LunarLoader, Version=1.1.17.0, Culture=neutral, PublicKeyToken=null").ToArray();
            if(loaders.Length!=1 || loaders[0].ManifestModule.ModuleVersionId.ToString("D")!="1c99627d-20c2-4a25-af1c-fe703af88106")
                throw new InvalidOperationException("Reviewed byte-loaded Lunar loader identity changed");
            RequireHash(loaders[0].Location,LunarLoaderSHA);
            RequireHash(Path.Combine(owner.RootDir,"1.6","Assemblies","LunarLoader.dll"),LunarLoaderSHA);
            RequireHash(Path.Combine(owner.RootDir,"1.6","Lunar","Components","LunarFramework.dll"),LunarSHA);
        }
        internal static XmlNode SourceNode(Entry row, string root)
            => SourceNode(row,root,new Dictionary<string,XmlDocument>());
        private static XmlNode SourceNode(Entry row, string root, Dictionary<string,XmlDocument> sources)
        {
            var path=Path.Combine(root,row.Source);
            XmlDocument xml;
            if(!sources.TryGetValue(path,out xml))
            {
                RequireHash(path,row.SHA);
                xml=new XmlDocument(); xml.Load(path); sources.Add(path,xml);
            }
            var nodes=xml.SelectNodes("/Defs/BiomeVariantDef[defName='"+row.DefName+"']/"+row.Field);
            if(nodes.Count!=1) throw new InvalidOperationException("Ambiguous/missing Caverns literal: "+row.DefName+"."+row.Field);
            var node=nodes[0];
            if(node.Attributes.Count!=1 || node.Attributes["operation"]?.Value!=row.Operation || node.InnerText!=row.English
                || node.ChildNodes.Cast<XmlNode>().Any(n=>n.NodeType!=XmlNodeType.Text && n.NodeType!=XmlNodeType.Whitespace))
                throw new InvalidOperationException("Caverns source is no longer the reviewed literal operand: "+row.Key);
            return node;
        }
        internal static object Parse(Type wrapperType, XmlNode node)
        {
            var wrapper=Activator.CreateInstance(wrapperType);
            var loader=wrapperType.GetMethod("LoadDataFromXmlCustom",new[]{typeof(XmlNode)});
            if(loader==null) throw new MissingMethodException("Lunar typed XML loader is unavailable");
            loader.Invoke(wrapper,new object[]{node});
            return wrapper;
        }
        // This is the exact SHA-bound Lunar DyadicModifierVerbose + FixedValue literal
        // shape, not a comparer for arbitrary expression/delegate graphs.
        internal static bool SameLiteral(object current, object expected)
        {
            if(current==null || expected==null || current.GetType()!=expected.GetType()) return false;
            var root=AccessTools.Field(current.GetType(),"_root");
            var actual=root?.GetValue(current) as Delegate;
            var wanted=root?.GetValue(expected) as Delegate;
            if(actual==null || wanted==null || actual.GetInvocationList().Length!=1 || wanted.GetInvocationList().Length!=1
                || actual.Method!=wanted.Method || actual.Target==null || wanted.Target==null || actual.Target.GetType()!=wanted.Target.GetType()) return false;
            var closure=wanted.Target.GetType();
            var fields=closure.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);
            if(fields.Length!=2 || !closure.Name.StartsWith("<>c__DisplayClass42_1",StringComparison.Ordinal)) return false;
            var supplier=closure.GetField("supplier");
            var parent=closure.GetField("CS$<>8__locals1");
            var outerA=parent?.GetValue(actual.Target); var outerB=parent?.GetValue(wanted.Target);
            if(supplier==null || outerA==null || outerB==null || outerA.GetType()!=outerB.GetType()) return false;
            var outer=outerB.GetType();
            if(!outer.Name.StartsWith("<>c__DisplayClass42_0",StringComparison.Ordinal)
                || outer.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly).Length!=5) return false;
            var operation=outer.GetField("dyadicOperation");
            if(operation==null || !Equals(operation.GetValue(outerA),operation.GetValue(outerB))) return false;
            var neutral=outer.GetField("neutralOperand");
            if(neutral==null || !Equals(neutral.GetValue(outerA),neutral.GetValue(outerB))) return false;
            foreach(var name in new[]{"operandSupplier","conditions","operandModifiers"})
            {
                var field=outer.GetField(name);
                if(field==null || field.GetValue(outerA)!=null || field.GetValue(outerB)!=null) return false;
            }
            var a=supplier.GetValue(actual.Target) as Delegate;
            var b=supplier.GetValue(wanted.Target) as Delegate;
            if(a==null || b==null || a.GetInvocationList().Length!=1 || b.GetInvocationList().Length!=1 || a.Method!=b.Method
                || a.Target==null || b.Target==null || a.Target.GetType()!=b.Target.GetType()) return false;
            var literal=b.Target.GetType();
            var values=literal.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);
            return literal.Name.StartsWith("<>c__DisplayClass30_0",StringComparison.Ordinal) && values.Length==1
                && values[0].Name=="value" && values[0].FieldType==typeof(string)
                && Equals(values[0].GetValue(a.Target),values[0].GetValue(b.Target));
        }
        internal static int Apply(IEnumerable<Def> definitions, ModContentPack owner, bool alphaActive)
        {
            if(owner==null || owner.PackageId!=Owner) throw new InvalidOperationException("Wrong Caverns translation owner");
            var type=AccessTools.TypeByName("GeologicalLandforms.Defs.BiomeVariantDef");
            if(type==null) throw new TypeLoadException("Reviewed Geological BiomeVariantDef is unavailable");
            RequireHash(type.Assembly.Location,GeologicalSHA);
            var verified=new HashSet<Assembly>{type.Assembly};
            var sources=new Dictionary<string,XmlDocument>();
            var defs=definitions.ToArray();
            var pending=new List<Applied>();
            foreach(var row in Entries)
            {
                var matches=defs.Where(d=>d.defName==row.DefName).ToArray();
                if(row.Alpha && !alphaActive)
                {
                    if(matches.Length!=0) throw new InvalidOperationException("Inactive Alpha Biomes branch unexpectedly supplied "+row.DefName);
                    continue;
                }
                if(matches.Length!=1 || matches[0].GetType()!=type || !ReferenceEquals(matches[0].modContentPack,owner))
                    throw new InvalidOperationException("Missing/ambiguous/foreign Caverns Def: "+row.DefName);
                var field=type.GetField(row.Field,BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
                var wrapperType=field?.FieldType;
                if(wrapperType==null || !wrapperType.IsGenericType || wrapperType.GetGenericTypeDefinition().FullName!="LunarFramework.XML.XmlDynamicValue`2"
                    || wrapperType.GetGenericArguments()[0]!=typeof(string) || wrapperType.GetGenericArguments()[1].FullName!="GeologicalLandforms.ICtxTile"
                    || wrapperType.GetGenericArguments()[1].Assembly!=type.Assembly)
                    throw new InvalidOperationException("Caverns wrapper CLR contract changed: "+row.Key);
                if(verified.Add(wrapperType.Assembly)) RequireLunarAssembly(wrapperType.Assembly);
                var source=SourceNode(row,owner.RootDir,sources);
                var expected=Parse(wrapperType,source);
                if(!SameLiteral(field.GetValue(matches[0]),expected)) throw new InvalidOperationException("Caverns effective wrapper is modified/dynamic: "+row.Key);
                var translated=source.CloneNode(true);
                var supplier=translated.OwnerDocument.CreateAttribute("supplier"); supplier.Value="translate";
                translated.Attributes.Append(supplier); translated.InnerText=row.Key;
                pending.Add(new Applied{Row=row,Def=matches[0],Field=field,Wrapper=Parse(wrapperType,translated)});
            }
            foreach(var item in pending) item.Field.SetValue(item.Def,item.Wrapper);
            applied=pending;
            InstallStatus="applied: "+pending.Count+" operands";
            return pending.Count;
        }
        // The existing Caverns native scenario uses these exact owned operands;
        // its accepted receipt proves application separately from source coverage.
        internal static string[][] AuditRows()
        {
            var verified=new HashSet<Assembly>();
            var sources=new Dictionary<string,XmlDocument>();
            foreach(var item in applied)
            {
                if(verified.Add(item.Def.GetType().Assembly)) RequireHash(item.Def.GetType().Assembly.Location,GeologicalSHA);
                if(verified.Add(item.Field.FieldType.Assembly)) RequireLunarAssembly(item.Field.FieldType.Assembly);
                SourceNode(item.Row,item.Def.modContentPack.RootDir,sources);
            }
            return applied.Select(item=>new[]{item.Def.defName,item.Row.Field,item.Row.Key,item.Row.English,item.Row.Operation,
                item.Row.SHA,ReferenceEquals(item.Field.GetValue(item.Def),item.Wrapper) ? "bound" : "changed"}).ToArray();
        }
    }
}
