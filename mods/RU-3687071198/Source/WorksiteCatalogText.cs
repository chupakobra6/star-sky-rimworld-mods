using System;
using System.Collections.Generic;
using MiningOutpost.Orbital.Services;

namespace Igor.WorksitesRussian
{
    public static class WorksiteCatalogText
    {
        public static int Apply(IEnumerable<OppJob> jobs,Func<string,string> translate)
        {
            int count=0;
            foreach(var job in jobs)
            {
                string prefix="chupakobra6_3687071198_job."+job.Id;
                Set(ref job.Name,prefix+".Name",translate,ref count);
                Set(ref job.Subtitle,prefix+".Subtitle",translate,ref count);
                foreach(var node in job.Nodes)
                {
                    // This local fix node already uses its own four Keyed strings.
                    if(job.Id=="ransom"&&node.Id=="igor-ransom-payment")continue;
                    string nodePrefix=prefix+".node."+node.Id;
                    Set(ref node.Title,nodePrefix+".Title",translate,ref count);
                    Set(ref node.Text,nodePrefix+".Text",translate,ref count);
                }
                foreach(var edge in job.Edges)
                {
                    if(job.Id=="ransom"&&edge.From=="igor-ransom-payment")continue;
                    Set(ref edge.Label,prefix+".edge."+edge.From+"."+edge.To+"."+edge.Kind+".Label",translate,ref count);
                }
            }
            return count;
        }
        private static void Set(ref string field,string key,Func<string,string> translate,ref int count)
        {
            if(string.IsNullOrEmpty(field))return;
            string value=translate(key);
            if(string.IsNullOrEmpty(value))throw new InvalidOperationException("Missing reviewed Worksites catalog text: "+key);
            field=value;
            count++;
        }
    }
}
