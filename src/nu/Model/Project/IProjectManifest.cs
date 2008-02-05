namespace nu.Model.Project
{
    using System;
    using System.Collections.Generic;
    using nu.Model.Template;

    public interface IProjectManifest
    {
        IEnumerable<String> Directories { get;}
        IEnumerable<TransformationElement> Files { get;}
    }
}